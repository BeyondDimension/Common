#if !NETFRAMEWORK && !PROJ_SETUP
using BD.Common8.Http.ClientFactory.Models;
using Microsoft.Extensions.Logging;
using Polly;
using System.Extensions;
using System.Formats;
using System.Net.Http.Client;
using System.Runtime.CompilerServices;
using System.Security.Cryptography;

namespace BD.Common8.Http.ClientFactory.Services.Implementation;

/// <summary>
/// <see cref="IImageHttpClientService"/> 的默认实现类
/// </summary>
/// <param name="loggerFactory"></param>
/// <param name="httpPlatformHelper"></param>
/// <param name="clientFactory"></param>
public sealed partial class ImageHttpClientServiceImpl(
    ILoggerFactory loggerFactory,
    IHttpPlatformHelperService httpPlatformHelper,
    IClientHttpClientFactory clientFactory) : IImageHttpClientService
{
    const string TAG = "ImageHttpClient";
    const string SchemeFile = "file:///";

    readonly ILogger logger = loggerFactory.CreateLogger(TAG);
    readonly IHttpPlatformHelperService httpPlatformHelper = httpPlatformHelper;
    readonly IClientHttpClientFactory clientFactory = clientFactory;

    public async Task<T?> GetImageAsync<T>(GetImageArgs args, Func<string, T?> filePathConvert, Func<HttpResponseImageContent, T?> responseConvert, CancellationToken cancellationToken) where T : notnull
    {
        (string requestUri, int isPollyNum, bool cache, bool cacheFirst, HttpHandlerCategory category) = args;

        if (!String2.IsHttpUrl(requestUri, httpsOnly: false))
        {
            if (requestUri.StartsWith(SchemeFile, StringComparison.OrdinalIgnoreCase))
            {
                var filePath = requestUri[SchemeFile.Length..];
                try
                {
                    return filePathConvert(filePath);
                }
                catch (DirectoryNotFoundException)
                {
                    return default;
                }
                catch (FileNotFoundException)
                {
                    return default;
                }
            }
            // 仅支持 HTTPS 协议
            return default;
        }

        HttpResponseImageContent? response = default;
        if (cacheFirst && category != HttpHandlerCategory.Offline && cache)
        {
            // 如果缓存优先，则先从缓存中取
            response = await SendAsync(requestUri, args.HashValue, HttpHandlerCategory.Offline, cancellationToken);
            var result = responseConvert(response);
            if (result != null)
            {
                return result;
            }
        }

        try
        {
            if (isPollyNum > 0 && category != HttpHandlerCategory.Offline)
            {
                // 使用 Polly 尝试 numRetries 次进行网络请求，如果强行指定离线缓存，则不进行多次尝试
                response = await Policy.HandleResult<HttpResponseImageContent>(
                        static x => (x.Stream == null && x.FilePath == null) && !x.IsStopped) // 流为空时并且没有取消请求的情况下重试
                    .WaitAndRetryAsync(isPollyNum, i => PollyRetryAttempt(i, isPollyNum))
                    .ExecuteAsync(cancellationToken => SendAsync(requestUri, args.HashValue, category, cancellationToken), cancellationToken);
            }
            else
            {
                // 不进行多次尝试，仅一次获取
                response = await SendAsync(requestUri, args.HashValue, category, cancellationToken);
            }
        }
        catch (Exception e)
        {
            if (e.GetKnownType().IsCanceledException())
            {
                return default;
            }
            const string logMsg = $"{nameof(GetImageAsync)} fail, category: {{category}}";
            logger.LogWarning(e, logMsg, category);
        }

        if (response != null)
        {
            var result = responseConvert(response);
            if (result != null)
            {
                return result;
            }
        }

        if (!cacheFirst && category != HttpHandlerCategory.Offline && cache)
        {
            // 非缓存优先的情况下，从网络中加载失败，再去缓存中尝试加载
            response = await SendAsync(requestUri, args.HashValue, HttpHandlerCategory.Offline, cancellationToken);
        }

        if (response != null)
        {
            var result = responseConvert(response);
            if (result != null)
            {
                return result;
            }
        }

        return default;
    }

    static FileStream? GetStream(string filePath)
    {
        try
        {
            var fileStream = new FileStream(filePath, FileMode.Open, FileAccess.Read, FileShare.ReadWrite | FileShare.Delete);
            return fileStream;
        }
        catch (DirectoryNotFoundException)
        {
            return null;
        }
        catch (FileNotFoundException)
        {
            return null;
        }
    }

    /// <inheritdoc/>
    public async Task<Stream?> GetImageMemoryStreamAsync(GetImageArgs args,
        CancellationToken cancellationToken)
    {
        var result = await GetImageAsync<Stream>(args,
            static filePath => GetStream(filePath),
            static response =>
            {
                if (response.Stream != null)
                {
                    return response.Stream;
                }
                else if (response.FilePath != null)
                {
                    return GetStream(response.FilePath);
                }
                return default;
            },
            cancellationToken);
        return result;
    }

    /// <inheritdoc/>
    public async Task<string?> GetImageFilePathAsync(GetImageArgs args,
        CancellationToken cancellationToken = default)
    {
        var result = await GetImageAsync<string>(args,
            static filePath => filePath,
            static response =>
            {
                if (response.Stream is FileStream fileStream)
                {
                    return fileStream.Name;
                }
                else if (response.FilePath != null)
                {
                    return response.FilePath;
                }
                return default;
            },
            cancellationToken);
        return result;
    }

    HttpRequestMessage GetRequestMessage(string requestUri, string? hashValue, HttpHandlerCategory category)
    {
        var originalRequestUri = requestUri;
        var request = new HttpRequestMessage(HttpMethod.Get, requestUri);
        if (category == HttpHandlerCategory.Offline)
        {
            if (!string.IsNullOrWhiteSpace(hashValue))
            {
                const string hashsBaseUrl = "https://local.steampp.net/bd.common8.http.clientfactory/hashs/";
                originalRequestUri = hashValue.Length switch
                {
                    Hashs.String.Lengths.MD5 => $"{hashsBaseUrl}md5/{hashValue}",
                    Hashs.String.Lengths.SHA1 => $"{hashsBaseUrl}sha1/{hashValue}",
                    Hashs.String.Lengths.SHA256 => $"{hashsBaseUrl}sha256/{hashValue}",
                    Hashs.String.Lengths.SHA384 => $"{hashsBaseUrl}sha384/{hashValue}",
                    Hashs.String.Lengths.SHA512 => $"{hashsBaseUrl}sha512/{hashValue}",
                    _ => $"{hashsBaseUrl}{hashValue.Length}/{hashValue}",
                };
            }
        }
        SetOriginalRequestUri(request, originalRequestUri);
        return request;
    }

    async Task<HttpResponseImageContent> SendAsync(string requestUri, string? hashValue, HttpHandlerCategory category, CancellationToken cancellationToken)
    {
        try
        {
            using var req = GetRequestMessage(requestUri, hashValue, category);
            var rsp = await SendAsync(req, category, cancellationToken);
            return rsp;
        }
        catch (Exception e)
        {
            if (e.GetKnownType().IsCanceledException())
            {
                return true;
            }
            const string logMsg = $"{nameof(SendAsync)} fail, category: {{category}}";
            logger.LogWarning(e, logMsg, category);
            return false; // 可重试
        }
    }

    /// <summary>
    /// 判断流中的数据是否为图片流
    /// </summary>
    /// <param name="s"></param>
    /// <returns></returns>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    static bool IsImageStream(Stream? s)
    {
        if (s == default) return false;

        try
        {
            if (s.Length <= 4) return false;
        }
        catch
        {
        }

        try
        {
            if (s.CanSeek)
            {
                if (!FileFormat.IsImage(s, out var _))
                    return false;
            }
        }
        catch
        {
        }

        return true;
    }

    async Task<HttpResponseImageContent> SendAsync(
        HttpRequestMessage request,
        HttpHandlerCategory category,
        CancellationToken cancellationToken = default)
    {
        request.Headers.Accept.ParseAdd(httpPlatformHelper.AcceptImages);
        request.Headers.UserAgent.ParseAdd(httpPlatformHelper.UserAgent);

        var client = clientFactory.CreateClient(TAG, category);

        using var response = await client.SendAsync(request,
            HttpCompletionOption.ResponseHeadersRead,
            cancellationToken)
            .ConfigureAwait(false);

        if (response.IsSuccessStatusCode)
        {
            if (request.Options.TryGetValue(IImageHttpClientService.KeyFilePath, out var filePath))
            {
                try
                {
                    if (!string.IsNullOrWhiteSpace(filePath))
                    {
                        using var fileStream = GetStream(filePath);
                        if (IsImageStream(fileStream))
                        {
                            return filePath;
                        }
                    }
                }
                catch (DirectoryNotFoundException)
                {
                    return false;
                }
                catch (FileNotFoundException)
                {
                    return false;
                }
            }
            //else if (request.Options.TryGetValue(IImageHttpClientService.KeyByteArray, out var bytes))
            //{
            //    var stream = new MemoryStream(bytes, false);
            //    if (IsImageStream(stream))
            //    {
            //        return stream;
            //    }
            //}
            else
            {
                var content = await response.Content.ReadAsStreamAsync(cancellationToken);
                var memoryStream = FusilladeClientHttpClientFactory.MemoryStreamManager.GetStream();
                await content.CopyToAsync(memoryStream, cancellationToken);
                if (IsImageStream(memoryStream))
                {
                    return memoryStream;
                }
            }
        }
        else
        {
            return true; // 请求结束，状态码不为 2xx 则判定失败且不进行重试
        }
        return false;
    }
}

partial class ImageHttpClientServiceImpl // OriginalRequestUri
{
    /// <summary>
    /// 默认请求地址
    /// </summary>
    public const string DefaultRequestUri = "/";

    /// <summary>
    /// 从请求消息中获取原始请求地址
    /// </summary>
    /// <param name="request"></param>
    /// <param name="defaultRequestUri"></param>
    /// <returns></returns>
    public static string GetOriginalRequestUri(
        HttpRequestMessage request,
        string defaultRequestUri = DefaultRequestUri)
    {
        string? originalRequestUri;
        if (request.Options.TryGetValue(IImageHttpClientService.KeyOriginalRequestUri, out var originalRequestUri_))
        {
            originalRequestUri = originalRequestUri_;
        }
        else
        {
            originalRequestUri = request.RequestUri?.ToString()!;
        }
        if (string.IsNullOrEmpty(originalRequestUri))
        {
            originalRequestUri = defaultRequestUri;
        }
        return originalRequestUri;
    }

    static void SetOriginalRequestUri(HttpRequestMessage request, string? originalRequestUri = null)
    {
        originalRequestUri ??= request.RequestUri?.ToString();
        if (!string.IsNullOrWhiteSpace(originalRequestUri))
        {
            request.Options.Set(IImageHttpClientService.KeyOriginalRequestUri, originalRequestUri);
        }
    }
}

partial class ImageHttpClientServiceImpl // Polly
{
    static TimeSpan PollyRetryAttempt(int attemptNumber, int numRetries)
    {
        var powY = attemptNumber % numRetries;
        var timeSpan = TimeSpan.FromMilliseconds(Math.Pow(2, powY));
        int addS = attemptNumber / numRetries;
        if (addS > 0) timeSpan = timeSpan.Add(TimeSpan.FromSeconds(addS));
        return timeSpan;
    }
}

public sealed class HttpResponseImageContent
{
    HttpResponseImageContent()
    {
    }

    /// <summary>
    /// 图片的内存流
    /// </summary>
    public Stream? Stream { get; init; }

    /// <summary>
    /// 图片保存的本地路径
    /// </summary>
    public string? FilePath { get; init; }

    /// <summary>
    /// 请求是否中止重试，比如取消，停止重试等
    /// </summary>
    internal bool IsStopped { get; init; }

    public static implicit operator HttpResponseImageContent(bool isStopped) => new()
    {
        IsStopped = isStopped,
    };

    public static implicit operator HttpResponseImageContent(Stream stream) => new()
    {
        IsStopped = true,
        Stream = stream,
    };

    public static implicit operator HttpResponseImageContent(string filePath) => new()
    {
        IsStopped = true,
        FilePath = filePath,
    };
}
#endif
using Microsoft.Extensions.Logging;
using Nito.AsyncEx;
using Polly;
using System.Collections.Concurrent;
using System.IO.FileFormats;
using static System.Net.Http.IImageHttpClientService;

namespace System.Net.Http;

partial class ImageHttpClientService
{
    public async Task<Stream?> GetImageStreamAsync(string requestUri, string channelType, CancellationToken cancellationToken = default)
    {
        var file = await GetImageLocalFilePathByPollyAsync(requestUri, channelType, cancellationToken);
        return string.IsNullOrEmpty(file) ? null : File.OpenRead(file);
    }

    #region Polly

    const int numRetries = 5;

    static TimeSpan PollyRetryAttempt(int attemptNumber)
    {
        var powY = attemptNumber % numRetries;
        var timeSpan = TimeSpan.FromMilliseconds(Math.Pow(2, powY));
        int addS = attemptNumber / numRetries;
        if (addS > 0) timeSpan = timeSpan.Add(TimeSpan.FromSeconds(addS));
        return timeSpan;
    }

    #endregion

    Task<string?> IImageHttpClientService.GetImageAsync(string requestUri, string channelType, CancellationToken cancellationToken)
    {
        return GetImageLocalFilePathByPollyAsync(requestUri, channelType, cancellationToken);
    }

    public async Task<string?> GetImageLocalFilePathByPollyAsync(string requestUri, string channelType, CancellationToken cancellationToken)
    {
        var r = await Policy.HandleResult<string?>(string.IsNullOrWhiteSpace)
          .WaitAndRetryAsync(numRetries, PollyRetryAttempt)
          .ExecuteAsync(ct => GetImageLocalFilePathAsync(requestUri, channelType, ct), cancellationToken);
        return r;
    }

    readonly AsyncLock lockGetImageLocalFilePathAsync = new();

    public async Task<string?> GetImageLocalFilePathAsync(string requestUri, string channelType, CancellationToken cancellationToken)
    {
        if (!String2.IsHttpUrl(requestUri)) return null;

        Task<string?> task;
        ConcurrentDictionary<string, Task<string?>>? pairs = null;

        using (await lockGetImageLocalFilePathAsync.LockAsync(cancellationToken))
        {
            if (get_image_pipeline.ContainsKey(channelType))
            {
                pairs = get_image_pipeline[channelType];
                if (pairs.TryGetValue(requestUri, out var value))
                {
                    var findResult = await value;
                    return findResult;
                }
            }
            else
            {
                get_image_pipeline.TryAdd(channelType, new ConcurrentDictionary<string, Task<string?>>());
            }

            var dirPath = GetImagesCacheDirectory(channelType);
            var fileName = Hashs.String.SHA256(requestUri) + FileEx.ImageSource;
            var localCacheFilePath = Path.Combine(dirPath, fileName);

            pairs ??= get_image_pipeline[channelType];
            task = GetImageAsync_(requestUri, localCacheFilePath, cancellationToken);
            pairs.TryAdd(requestUri, task);
        }

        var result = await task;
        pairs.TryRemove(requestUri, out var _);
        return result;
    }

    async Task<string?> GetImageAsync_(
        string requestUri,
        string localCacheFilePath,
        CancellationToken cancellationToken)
    {
        var localCacheFilePathExists = File.Exists(localCacheFilePath);

        try
        {
            if (localCacheFilePathExists) // 存在缓存文件
            {
                using var fileStream = IOPath.OpenRead(localCacheFilePath);
                if (fileStream == null)
                    return null;
                if (FileFormat.IsImage(fileStream, out var format))
                {
                    if (http_helper.SupportedImageFormats.Contains(format)) // 读取缓存并且格式符合要求
                    {
                        fileStream.Close();
                        return localCacheFilePath;
                    }
                    else // 格式不准确
                    {
                        logger.LogWarning("GetImageAsync not supported imageFormat: {format}.", format);
                    }
                }
                else // 未知的图片格式
                {
                    logger.LogWarning("GetImageAsync unknown imageFormat.");
                }
            }
        }
        catch (Exception e)
        {
            logger.LogWarning(e, "GetImageAsync_ load localFile fail.");
        }

        if (localCacheFilePathExists)
        {
            File.Delete(localCacheFilePath); // 必须删除文件，重新下载覆盖
        }

        try
        {
            var request = new HttpRequestMessage(HttpMethod.Get, requestUri);
            request.Headers.Accept.ParseAdd(http_helper.AcceptImages);
            request.Headers.UserAgent.ParseAdd(http_helper.UserAgent);
            var client = CreateClient();
            var response = await client.SendAsync(request,
                HttpCompletionOption.ResponseHeadersRead,
                cancellationToken)
                .ConfigureAwait(false);
            if (response.IsSuccessStatusCode)
            {
                var stream = await response.Content.ReadAsStreamAsync(cancellationToken);
                var localCacheFilePath2 = localCacheFilePath + ".download-cache";
                var fileStream = File.Create(localCacheFilePath2);
                cancellationToken.Register(() =>
                {
                    fileStream.Close();
                    IOPath.FileTryDelete(localCacheFilePath2);
                });
                await stream.CopyToAsync(fileStream, cancellationToken);
                await fileStream.FlushAsync(cancellationToken);
                bool isSupportedImageFormat;
                if (FileFormat.IsImage(fileStream, out var format))
                {
                    if (http_helper.SupportedImageFormats.Contains(format)) // 读取缓存并且格式符合要求
                    {
                        isSupportedImageFormat = true;
                    }
                    else // 格式不准确
                    {
                        logger.LogWarning("GetImageAsync download not supported imageFormat: {format}.", format);
                        isSupportedImageFormat = false;
                    }
                }
                else // 未知的图片格式
                {
                    logger.LogWarning("GetImageAsync download unknown imageFormat.");
                    isSupportedImageFormat = false;
                }
                fileStream.Close();
                if (isSupportedImageFormat)
                {
                    File.Move(localCacheFilePath2, localCacheFilePath);
                    return localCacheFilePath;
                }
                else
                {
                    IOPath.FileTryDelete(localCacheFilePath2);
                }
            }
        }
        catch (Exception e)
        {
#if !DEBUG
            if (e is SocketException se && se.SocketErrorCode == SocketError.ConnectionReset)
            {
                // https://docs.microsoft.com/en-us/windows/win32/winsock/windows-sockets-error-codes-2
                // 10054 WSAECONNRESET
                return default;
            }
#endif
            logger.LogWarning(e, "GetImageAsync_ fail.");
        }

        return default;
    }

    readonly ConcurrentDictionary<string, ConcurrentDictionary<string, Task<string?>>> get_image_pipeline = new();
}

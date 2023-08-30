using Polly;
using static System.Net.Http.IImageHttpClientService;

// ReSharper disable once CheckNamespace
namespace System.Net.Http;

public sealed partial class ImageHttpClientService : GeneralHttpClientFactory, IImageHttpClientService
{
    public ImageHttpClientService(
        ILoggerFactory loggerFactory,
        IHttpPlatformHelperService http_helper,
        IHttpClientFactory clientFactory)
        : base(loggerFactory.CreateLogger(TAG), http_helper, clientFactory)
    {

    }

    public async Task<MemoryStream?> GetImageMemoryStreamAsync(
        string requestUri,
        bool isPolly = true,
        bool cache = false,
        bool cacheFirst = false,
        HttpHandlerCategory category = DefaultHttpHandlerCategory,
        CancellationToken cancellationToken = default)
    {
        if (!String2.IsHttpUrl(requestUri))
            return default;

        if (!cache)
        {
            category = HttpHandlerCategory.Default;
        }
        else
        {
            isPolly = false;
        }

        MemoryStream? response = null;

        if (cacheFirst && category != HttpHandlerCategory.Offline && cache)
        {
            // 如果缓存优先，则先从缓存中取
            response = await _GetImageMemoryStreamCoreByOfflineAsync(cancellationToken);
            if (response != null) return response;
        }

        try
        {
            if (isPolly && category != HttpHandlerCategory.Offline)
            {
                // 使用 Polly 尝试 numRetries 次进行网络请求，如果强行指定离线缓存，则不进行多次尝试
                response = await Policy.HandleResult<MemoryStream?>(x => x == null)
                    .WaitAndRetryAsync(numRetries, PollyRetryAttempt)
                    .ExecuteAsync(_GetImageMemoryStreamCoreAsync, cancellationToken);
            }
            else
            {
                // 不进行多次尝试，仅一次获取
                response = await _GetImageMemoryStreamCoreAsync(cancellationToken);
            }
        }
        catch (Exception e)
        {
            logger.LogWarning(e,
                $"{nameof(GetImageMemoryStreamAsync)} fail, category: {{category}}.", category);
        }

        if (!cacheFirst && response == null && category != HttpHandlerCategory.Offline && cache)
        {
            // 非缓存优先的情况下，从网络中加载失败，再去缓存中尝试加载
            response = await _GetImageMemoryStreamCoreByOfflineAsync(cancellationToken);
        }

        return response;

        async Task<MemoryStream?> _GetImageMemoryStreamCoreAsync(CancellationToken cancellationToken)
        {
            try
            {
                var r = await GetImageMemoryStreamCoreAsync(
                    requestUri,
                    category,
                    cancellationToken);
                return r;
            }
            catch (Exception e)
            {
                logger.LogWarning(e,
                    $"{nameof(_GetImageMemoryStreamCoreAsync)} fail, category: {{category}}.", category);
                return default;
            }
        }

        async Task<MemoryStream?> _GetImageMemoryStreamCoreByOfflineAsync(CancellationToken cancellationToken)
        {
            try
            {
                var r = await GetImageMemoryStreamCoreAsync(
                    requestUri,
                    HttpHandlerCategory.Offline,
                    cancellationToken);
                return r;
            }
            catch (Exception e)
            {
                logger.LogWarning(e,
                    $"{nameof(_GetImageMemoryStreamCoreByOfflineAsync)} fail, category: {{category}}(Offline).", category);
                return default;
            }
        }
    }

    async Task<MemoryStream?> GetImageMemoryStreamCoreAsync(
        string requestUri,
        HttpHandlerCategory category = DefaultHttpHandlerCategory,
        CancellationToken cancellationToken = default)
    {
        var request = new HttpRequestMessage(HttpMethod.Get, requestUri);
        request.Headers.Accept.ParseAdd(http_helper.AcceptImages);
        request.Headers.UserAgent.ParseAdd(http_helper.UserAgent);
        var client = CreateClient(TAG, category);
        var response = await client.SendAsync(request,
            HttpCompletionOption.ResponseHeadersRead,
            cancellationToken)
            .ConfigureAwait(false);
        if (response.IsSuccessStatusCode)
        {
            var imageBytes = await response.Content.ReadAsByteArrayAsync(cancellationToken);
            return new MemoryStream(imageBytes, false);
        }

        return default;
    }

    #region Polly

    const int numRetries = 2; // 尝试次数

    static TimeSpan PollyRetryAttempt(int attemptNumber)
    {
        var powY = attemptNumber % numRetries;
        var timeSpan = TimeSpan.FromMilliseconds(Math.Pow(2, powY));
        int addS = attemptNumber / numRetries;
        if (addS > 0) timeSpan = timeSpan.Add(TimeSpan.FromSeconds(addS));
        return timeSpan;
    }

    #endregion
}

namespace BD.Common8.Http.ClientFactory.Services.Implementation;

/// <summary>
/// <see cref="IImageHttpClientService"/> 的默认实现类
/// </summary>
/// <param name="loggerFactory"></param>
/// <param name="httpPlatformHelper"></param>
/// <param name="clientFactory"></param>
public sealed class ImageHttpClientServiceImpl(
    ILoggerFactory loggerFactory,
    IHttpPlatformHelperService httpPlatformHelper,
    IClientHttpClientFactory clientFactory) : IImageHttpClientService
{
    const string TAG = "ImageHttpClient";

    readonly ILogger logger = loggerFactory.CreateLogger(TAG);
    readonly IHttpPlatformHelperService httpPlatformHelper = httpPlatformHelper;
    readonly IClientHttpClientFactory clientFactory = clientFactory;

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

    /// <inheritdoc/>
    public async Task<MemoryStream?> GetImageMemoryStreamAsync(
        string requestUri,
        bool isPolly,
        bool cache,
        bool cacheFirst,
        HttpHandlerCategory category,
        CancellationToken cancellationToken)
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

        ImageMemoryStreamWrapper response = default;

        if (cacheFirst && category != HttpHandlerCategory.Offline && cache)
        {
            // 如果缓存优先，则先从缓存中取
            response = await _GetImageMemoryStreamCoreByOfflineAsync(cancellationToken);
            if (IsImageStream(response.Stream))
                return response.Stream;

            try
            {
                response.Stream?.Dispose();
            }
            catch
            {
            }
        }

        try
        {
            if (isPolly && category != HttpHandlerCategory.Offline)
            {
                // 使用 Polly 尝试 numRetries 次进行网络请求，如果强行指定离线缓存，则不进行多次尝试
                response = await Policy.HandleResult<ImageMemoryStreamWrapper>(
                        static x => x.Stream == null && !x.IsStopped) // 流为空时并且没有取消请求的情况下重试
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
            if (e.GetKnownType().IsCanceledException())
                return null;
            logger.LogWarning(e,
                $"{nameof(GetImageMemoryStreamAsync)} fail, category: {{category}}.", category);
        }

        if (!cacheFirst && response.Stream == null && category != HttpHandlerCategory.Offline && cache)
        {
            // 非缓存优先的情况下，从网络中加载失败，再去缓存中尝试加载
            response = await _GetImageMemoryStreamCoreByOfflineAsync(cancellationToken);
        }

        if (IsImageStream(response.Stream))
            return response.Stream;

        try
        {
            response.Stream?.Dispose();
        }
        catch
        {
        }

        return null;

        async Task<ImageMemoryStreamWrapper> _GetImageMemoryStreamCoreAsync(CancellationToken cancellationToken)
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
                if (e.GetKnownType().IsCanceledException())
                    return true;
                logger.LogWarning(e,
                    $"{nameof(_GetImageMemoryStreamCoreAsync)} fail, category: {{category}}.", category);
                return false; // 可重试
            }
        }

        async Task<ImageMemoryStreamWrapper> _GetImageMemoryStreamCoreByOfflineAsync(CancellationToken cancellationToken)
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
                if (e.GetKnownType().IsCanceledException())
                    return true;
                logger.LogWarning(e,
                    $"{nameof(_GetImageMemoryStreamCoreByOfflineAsync)} fail, category: {{category}}(Offline).", category);
                return false; // 可重试
            }
        }
    }

    /// <summary>
    /// 图片内存流包装类型
    /// </summary>
    readonly record struct ImageMemoryStreamWrapper
    {
        /// <summary>
        /// 图片内存流
        /// </summary>
        public MemoryStream? Stream { get; init; }

        /// <summary>
        /// 请求是否中止，比如取消，停止重试等
        /// </summary>
        public required bool IsStopped { get; init; }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static implicit operator ImageMemoryStreamWrapper(bool isStopped) => isStopped ? new()
        {
            IsStopped = true,
        } : default;

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static implicit operator ImageMemoryStreamWrapper(MemoryStream? stream) => new()
        {
            IsStopped = true,
            Stream = stream,
        };
    }

    /// <summary>
    /// 用于 Http 图片客户端的请求
    /// </summary>
    /// <param name="method"></param>
    /// <param name="requestUri"></param>
    public sealed class ImageHttpRequestMessage(HttpMethod method, [StringSyntax(StringSyntaxAttribute.Uri)] string requestUri) : HttpRequestMessage(method, requestUri)
    {
        /// <summary>
        /// 原始请求地址，因某些请求 301/302 跳转会改变地址
        /// </summary>
        public string OriginalRequestUri { get; } = requestUri;
    }

    async Task<ImageMemoryStreamWrapper> GetImageMemoryStreamCoreAsync(
        string requestUri,
        HttpHandlerCategory category,
        CancellationToken cancellationToken = default)
    {
        var request = new ImageHttpRequestMessage(HttpMethod.Get, requestUri);
        request.Headers.Accept.ParseAdd(httpPlatformHelper.AcceptImages);
        request.Headers.UserAgent.ParseAdd(httpPlatformHelper.UserAgent);
        var client = clientFactory.CreateClient(TAG, category);
        var response = await client.SendAsync(request,
            HttpCompletionOption.ResponseHeadersRead,
            cancellationToken)
            .ConfigureAwait(false);
        if (response.IsSuccessStatusCode)
        {
            var imageBytes = await response.Content.ReadAsByteArrayAsync(cancellationToken);
            var memoryStream = new MemoryStream(imageBytes, false);
            return memoryStream;
        }

        return true; // 请求结束，状态码不为 2xx 则判定失败且不进行重试
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

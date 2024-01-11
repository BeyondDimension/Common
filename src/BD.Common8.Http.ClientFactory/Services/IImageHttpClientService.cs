namespace BD.Common8.Http.ClientFactory.Services;

/// <summary>
/// Http 图片客户端请求服务
/// </summary>
public interface IImageHttpClientService
{
    /// <summary>
    /// <see cref="HttpHandlerCategory"/> 的默认值
    /// </summary>
    protected const HttpHandlerCategory DefaultHttpHandlerCategory = GetImageArgs.DefaultHttpHandlerCategory;

    /// <summary>
    /// 通过 Get 请求 Image <see cref="MemoryStream"/>
    /// </summary>
    /// <param name="requestUri">请求地址</param>
    /// <param name="isPolly">是否使用 <see cref="Polly"/> 重试</param>
    /// <param name="cache">是否使用缓存</param>
    /// <param name="cacheFirst">是否优先使用缓存，否则将优先请求网络</param>
    /// <param name="category">使用的调度器种类</param>
    /// <param name="cancellationToken">取消操作标记</param>
    /// <returns></returns>
    Task<MemoryStream?> GetImageMemoryStreamAsync(
        string requestUri,
        bool isPolly = true,
        bool cache = false,
        bool cacheFirst = false,
        HttpHandlerCategory category = DefaultHttpHandlerCategory,
        CancellationToken cancellationToken = default) => GetImageMemoryStreamAsync(new()
        {
            RequestUri = requestUri,
            IsPolly = isPolly,
            UseCache = cache,
            CacheFirst = cacheFirst,
            Category = category,
        }, cancellationToken);

    /// <summary>
    /// 通过 Get 请求 Image <see cref="MemoryStream"/>
    /// </summary>
    /// <param name="args"></param>
    /// <param name="cancellationToken">取消操作标记</param>
    /// <returns></returns>
    Task<MemoryStream?> GetImageMemoryStreamAsync(GetImageArgs args,
        CancellationToken cancellationToken = default);
}

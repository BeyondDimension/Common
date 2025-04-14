#if !NETFRAMEWORK && !PROJ_SETUP
namespace BD.Common8.Http.ClientFactory.Services;

/// <summary>
/// 图片的 HTTP 请求服务
/// </summary>
public interface IImageHttpClientService
{
    // 从网络中加载图片至 View 层控件
    // 由文件路径直接传递 Skia 构建 Bitmap 对象
    // ----- 网络请求 -----
    // Http 请求响应内容流读取 RateLimitedHttpMessageHandler2.SendAsync
    // 复制到内存流中 RecyclableMemoryStream
    // 由 cacheResult RequestCacheRepository.Save 方法保存到本地文件中
    // 通过 FusilladeClientHttpClientFactory.ReadAsStreamAsync 与类型 RecyclableMemoryStreamContent 直接获取内存流对象实例，避免复制流产生开销
    // 通过 KeyFilePath 设置文件路径
    // 响应内容替换为 BD.Common8.Http.ClientFactory.Services.Implementation.FileContent 由路径提供内容
    // ----- 本地缓存 -----
    // 由 retrieveBody RequestCacheRepository.Fetch 方法读取响应内容并设置 KeyFilePath 值
    // 通过 KeyFilePath 值直接返回文件路径或文件流

    // ImageValueConverter.GetDecodeBitmap
    // 使用 Bitmap(string fileName) OR Bitmap(Stream stream) OR Bitmap.DecodeToWidth
    // https://github.com/AvaloniaUtils/AsyncImageLoader.Avalonia/blob/master/AsyncImageLoader.Avalonia/Loaders/DiskCachedWebImageLoader.cs#L32
    // https://github.com/AvaloniaUtils/AsyncImageLoader.Avalonia/blob/master/AsyncImageLoader.Avalonia/Loaders/BaseWebImageLoader.cs#L68

    /// <summary>
    /// <see cref="HttpHandlerCategory"/> 的默认值
    /// </summary>
    public const HttpHandlerCategory DefaultHttpHandlerCategory = HttpHandlerCategory.UserInitiated;

    /// <summary>
    /// 图片本地缓存路径的 HTTP 请求键
    /// </summary>
    public static readonly HttpRequestOptionsKey<string> KeyFilePath = new("ResponseContentFilePath");

    ///// <summary>
    ///// 自定义返回流的 HTTP 请求键
    ///// </summary>
    //public static readonly HttpRequestOptionsKey<Stream> KeyStream = new("ResponseContentStream");

    ///// <summary>
    ///// 自定义返回字节数组的 HTTP 请求键
    ///// </summary>
    //public static readonly HttpRequestOptionsKey<byte[]> KeyByteArray = new("ResponseContentByteArray");

    /// <summary>
    /// 原始请求地址的 HTTP 请求键，因某些请求 301/302 跳转会改变地址
    /// </summary>
    protected static readonly HttpRequestOptionsKey<string> KeyOriginalRequestUri = new("OriginalRequestUri");

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
    [Obsolete("use GetImageFilePathAsync OR GetImageAsync")]
    Task<Stream?> GetImageMemoryStreamAsync(
        string requestUri,
        bool isPolly = true,
        bool cache = false,
        bool cacheFirst = false,
        HttpHandlerCategory category = DefaultHttpHandlerCategory,
        CancellationToken cancellationToken = default) => GetImageMemoryStreamAsync(new()
        {
            RequestUri = requestUri,
            IsPollyNum = isPolly ? 2 : 0,
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
    [Obsolete("use GetImageFilePathAsync OR GetImageAsync")]
    Task<Stream?> GetImageMemoryStreamAsync(GetImageArgs args,
        CancellationToken cancellationToken = default);

    /// <summary>
    /// 通过 Get 请求获取网络图片并保存到本地返回路径
    /// </summary>
    /// <param name="args"></param>
    /// <param name="cancellationToken"></param>
    /// <returns></returns>
    Task<string?> GetImageFilePathAsync(GetImageArgs args,
        CancellationToken cancellationToken = default);

    /// <summary>
    /// 通过 Get 请求获取网络图片并保存到本地返回路径或流泛型自定义类型转换返回
    /// </summary>
    /// <typeparam name="T"></typeparam>
    /// <param name="args"></param>
    /// <param name="filePathConvert"></param>
    /// <param name="responseConvert"></param>
    /// <param name="cancellationToken"></param>
    /// <returns></returns>
    Task<T?> GetImageAsync<T>(GetImageArgs args, Func<string, T?> filePathConvert, Func<HttpResponseImageContent, T?> responseConvert, CancellationToken cancellationToken) where T : notnull;
}
#endif
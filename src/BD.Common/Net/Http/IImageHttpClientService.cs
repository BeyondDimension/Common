// ReSharper disable once CheckNamespace
namespace System.Net.Http;

public interface IImageHttpClientService
{
    protected const string TAG = "ImageHttpClient";

    protected const HttpHandlerCategory DefaultHttpHandlerCategory = HttpHandlerCategory.UserInitiated;

    /// <summary>
    /// 通过 Get 请求 Image <see cref="MemoryStream"/>
    /// </summary>
    /// <param name="requestUri"></param>
    /// <param name="isPolly">是否使用 <see cref="Polly"/> 重试</param>
    /// <param name="cache">是否使用缓存</param>
    /// <param name="category">使用的调度器种类</param>
    /// <param name="cancellationToken">取消操作</param>
    /// <returns></returns>
    Task<MemoryStream?> GetImageMemoryStreamAsync(
        string requestUri,
        bool isPolly = true,
        bool cache = false,
        HttpHandlerCategory category = DefaultHttpHandlerCategory,
        CancellationToken cancellationToken = default);
}

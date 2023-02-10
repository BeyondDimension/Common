namespace System.Net.Http;

public interface IImageHttpClientService
{
    protected const string TAG = "ImageHttpClient";

    /// <summary>
    /// (带本地缓存)通过 Get 请求 Image Stream
    /// </summary>
    /// <param name="requestUri"></param>
    /// <param name="channelType">渠道类型，根据不同的类型建立不同的缓存文件夹</param>
    /// <param name="cancellationToken"></param>
    /// <returns></returns>
    Task<Stream?> GetImageStreamAsync(
        string requestUri,
        string channelType,
        CancellationToken cancellationToken = default);

    /// <summary>
    /// (带本地缓存)通过 Get 请求 Image FilePath
    /// </summary>
    /// <param name="requestUri"></param>
    /// <param name="channelType">渠道类型，根据不同的类型建立不同的缓存文件夹</param>
    /// <param name="cancellationToken"></param>
    /// <returns></returns>
    Task<string?> GetImageAsync(
        string requestUri,
        string channelType,
        CancellationToken cancellationToken = default);

    /// <summary>
    /// (无本地缓存)通过 Get 请求 Image 内容
    /// </summary>
    /// <param name="requestUri"></param>
    /// <param name="cancellationToken"></param>
    /// <returns></returns>
    Task<Stream?> GetImageStreamAsync(string requestUri, CancellationToken cancellationToken = default);

    static string GetImagesCacheDirectory(string? channelType)
    {
        const string dirName = "Images";
        var dirPath = !string.IsNullOrWhiteSpace(channelType) ?
            Path.Combine(IOPath.CacheDirectory, dirName, channelType) :
            Path.Combine(IOPath.CacheDirectory, dirName);
        IOPath.DirCreateByNotExists(dirPath);
        return dirPath;
    }
}

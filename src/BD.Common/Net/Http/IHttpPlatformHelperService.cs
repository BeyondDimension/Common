// ReSharper disable once CheckNamespace
namespace System.Net.Http;

/// <summary>
/// 由平台实现的 HTTP 工具助手服务
/// </summary>
public interface IHttpPlatformHelperService
{
    static IHttpPlatformHelperService Instance => Ioc.Get<IHttpPlatformHelperService>();

    /// <summary>
    /// 用户代理
    /// <para>https://developer.mozilla.org/zh-CN/docs/Web/HTTP/Headers/User-Agent</para>
    /// <para>通常可以从平台的 WebView 控件中调用 js 获取</para>
    /// </summary>
    string UserAgent { get; }

    string AcceptImages { get; }

    string AcceptLanguage { get; }

    /// <summary>
    /// 支持的图片格式
    /// </summary>
    ImageFormat[] SupportedImageFormats { get; }

    /// <summary>
    /// 尝试将需要上传的文件流[处理]后保存到临时路径中并返回信息
    /// <para>[处理]由客户端平台原生实现</para>
    /// <para>例如：文件压缩、格式转换(heic)</para>
    /// </summary>
    /// <param name="fileStream"></param>
    /// <returns></returns>
    (string filePath, string mime)? TryHandleUploadFile(Stream fileStream);

    /// <summary>
    /// 是否有网络链接
    /// </summary>
    /// <returns></returns>
    Task<bool> IsConnectedAsync();
}

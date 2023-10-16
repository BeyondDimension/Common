namespace System.Net.Http;

/// <summary>
/// 由平台实现的 HTTP 工具助手服务
/// </summary>
public interface IHttpPlatformHelperService
{
#if (!NETFRAMEWORK && (NETSTANDARD && NETSTANDARD2_1_OR_GREATER)) || NETCOREAPP
    /// <summary>
    /// 获取 <see cref="IHttpPlatformHelperService"/> 实例
    /// </summary>
    static IHttpPlatformHelperService Instance => Ioc.Get<IHttpPlatformHelperService>();
#endif

    /// <summary>
    /// 用户代理
    /// <para>https://developer.mozilla.org/zh-CN/docs/Web/HTTP/Headers/User-Agent</para>
    /// <para>通常可以从平台的 WebView 控件中调用 js 获取</para>
    /// </summary>
    string UserAgent { get; }

    /// <summary>
    /// Accept 请求头用来告知（服务器）客户端可以处理的内容类型，这种内容类型用 MIME 类型来表示。
    /// </summary>
    string AcceptImages { get; }

    /// <summary>
    /// Accept-Language 请求头允许客户端声明它可以理解的自然语言，以及优先选择的区域方言。借助内容协商机制，服务器可以从诸多备选项中选择一项进行应用，并使用 Content-Language 应答头通知客户端它的选择。
    /// </summary>
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
    ValueTask<bool> IsConnectedAsync();
}

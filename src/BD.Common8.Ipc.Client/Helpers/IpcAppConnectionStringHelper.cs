namespace BD.Common8.Ipc.Helpers;

/// <summary>
/// Ipc 应用程序连接字符串助手类
/// </summary>
public static partial class IpcAppConnectionStringHelper
{
    /// <summary>
    /// SignalR 的 Hub 名称
    /// </summary>
    public const string HubName = "IpcHub";

    /// <summary>
    /// 超时时间
    /// </summary>
    public const double TimeoutFromSeconds = 2.9;

    /// <summary>
    /// 根据 Ipc 应用程序连接字符串创建 <see cref="HttpMessageHandler"/>
    /// </summary>
    /// <param name="connectionString">连接字符串</param>
    /// <param name="ignoreRemoteCertificateValidation">是否忽略服务端证书验证</param>
    /// <returns></returns>
    public static IpcAppConnDelegatingHandler GetHttpMessageHandler(IpcAppConnectionString connectionString,
        bool ignoreRemoteCertificateValidation = true)
    {
        var connectionStringType = connectionString.Type;
        SocketsHttpHandler innerHandler = new()
        {
            UseProxy = false,
            UseCookies = false,
        };
        IpcAppConnDelegatingHandler delegatingHandler = new(connectionString, innerHandler);
        if (ignoreRemoteCertificateValidation)
            innerHandler.SslOptions.RemoteCertificateValidationCallback = (_, _, _, _) => true;
        return delegatingHandler;
    }

    /// <summary>
    /// 根据 Ipc 应用程序连接字符串创建 <see cref="HttpClient"/>
    /// </summary>
    /// <param name="connectionString">连接字符串</param>
    /// <param name="ignoreRemoteCertificateValidation">是否忽略服务端证书验证</param>
    /// <param name="timeoutFromSeconds">超时时间，单位秒</param>
    /// <returns></returns>
    public static (HttpClient httpClient, IpcAppConnDelegatingHandler handler) GetHttpClient(IpcAppConnectionString connectionString,
        bool ignoreRemoteCertificateValidation = true,
        double timeoutFromSeconds = TimeoutFromSeconds)
    {
        var handler = GetHttpMessageHandler(connectionString, ignoreRemoteCertificateValidation);
        HttpClient client = new(handler)
        {
            DefaultRequestVersion = HttpVersion.Version20,
            BaseAddress = new(handler.BaseAddress),
            Timeout = TimeSpan.FromSeconds(timeoutFromSeconds),
        };
        return (client, handler);
    }
}
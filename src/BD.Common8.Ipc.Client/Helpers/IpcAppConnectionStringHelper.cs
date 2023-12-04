namespace BD.Common8.Ipc.Client.Helpers;

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
    public static (string baseAddress, HttpMessageHandler handler) GetHttpMessageHandler(IpcAppConnectionString connectionString,
        bool ignoreRemoteCertificateValidation = true)
    {
        var connectionStringType = connectionString.Type;
        string baseAddress = "https://localhost";
        Func<SocketsHttpConnectionContext, CancellationToken, ValueTask<Stream>>? connectCallback = null;
        switch (connectionStringType)
        {
            case IpcAppConnectionStringType.Https:
                baseAddress = $"https://localhost:{connectionString.Int32Value}";
                break;

            case IpcAppConnectionStringType.UnixSocket:
                connectCallback = UnixDomainSocketsConnectionFactory.GetConnectCallback(connectionString.StringValue.ThrowIsNull());
                break;

            case IpcAppConnectionStringType.NamedPipe:
                connectCallback = NamedPipesConnectionFactory.GetConnectCallback(connectionString.StringValue.ThrowIsNull());
                break;

            default:
                throw ThrowHelper.GetArgumentOutOfRangeException(connectionStringType);
        }
        SocketsHttpHandler handler = new()
        {
            UseProxy = false,
            UseCookies = false,
            ConnectCallback = connectCallback,
        };
        if (ignoreRemoteCertificateValidation)
            handler.SslOptions.RemoteCertificateValidationCallback = (_, _, _, _) => true;
        return (baseAddress, handler);
    }

    /// <summary>
    /// 根据 Ipc 应用程序连接字符串创建 <see cref="HttpClient"/>
    /// </summary>
    /// <param name="connectionString">连接字符串</param>
    /// <param name="ignoreRemoteCertificateValidation">是否忽略服务端证书验证</param>
    /// <param name="timeoutFromSeconds">超时时间，单位秒</param>
    /// <returns></returns>
    public static HttpClient GetHttpClient(IpcAppConnectionString connectionString,
        bool ignoreRemoteCertificateValidation = true,
        double timeoutFromSeconds = TimeoutFromSeconds)
    {
        (string baseAddress, HttpMessageHandler handler) = GetHttpMessageHandler(connectionString, ignoreRemoteCertificateValidation);
        HttpClient client = new(handler)
        {
            DefaultRequestVersion = HttpVersion.Version20,
            BaseAddress = new(baseAddress),
            Timeout = TimeSpan.FromSeconds(timeoutFromSeconds),
        };
        return client;
    }
}
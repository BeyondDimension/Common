namespace BD.Common8.Ipc.Client.Helpers;

/// <summary>
/// Ipc 应用程序连接字符串助手类
/// </summary>
public static partial class IpcAppConnectionStringHelper
{
    /// <summary>
    /// 根据 Ipc 应用程序连接字符串创建 <see cref="HttpClient"/>
    /// </summary>
    /// <param name="connectionString">连接字符串</param>
    /// <param name="ignoreRemoteCertificateValidation">是否忽略服务端证书验证</param>
    /// <param name="timeoutFromSeconds">超时时间，单位秒</param>
    /// <returns></returns>
    public static HttpClient GetHttpClient(IpcAppConnectionString connectionString,
        bool ignoreRemoteCertificateValidation = true,
        double timeoutFromSeconds = 3d)
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
        HttpClient client = new(handler)
        {
            DefaultRequestVersion = HttpVersion.Version20,
            BaseAddress = new(baseAddress),
            Timeout = TimeSpan.FromSeconds(timeoutFromSeconds),
        };
        return client;
    }
}

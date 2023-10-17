namespace BD.Common8.Ipc.Client.Helpers;

/// <summary>
/// Ipc 应用程序连接字符串助手类
/// </summary>
public static partial class IpcAppConnectionStringHelper
{
    /// <summary>
    /// 根据 Ipc 应用程序连接字符串创建 <see cref="HttpClient"/>
    /// </summary>
    /// <param name="connectionString"></param>
    /// <returns></returns>
    public static HttpClient GetHttpClient(string connectionString)
    {
        var bytes = connectionString.Base64DecodeToByteArray();
        var connectionStringType = (IpcAppConnectionStringType)bytes[0];
        var str = Encoding.UTF8.GetString(bytes, 1, bytes.Length - 1);
        string baseAddress = "http://localhost";
        Func<SocketsHttpConnectionContext, CancellationToken, ValueTask<Stream>>? connectCallback = null;
        switch (connectionStringType)
        {
            case IpcAppConnectionStringType.Https:
                baseAddress = str;
                break;
            case IpcAppConnectionStringType.UnixSocket:
                connectCallback = UnixDomainSocketsConnectionFactory.GetConnectCallback(str);
                break;
            case IpcAppConnectionStringType.NamedPipe:
                connectCallback = NamedPipesConnectionFactory.GetConnectCallback(str);
                break;
            default:
                ThrowHelper.ThrowArgumentOutOfRangeException(connectionStringType);
                break;
        }
        SocketsHttpHandler handler = new()
        {
            UseProxy = false,
            UseCookies = false,
            ConnectCallback = connectCallback,
        };
        HttpClient client = new(handler)
        {
            DefaultRequestVersion = HttpVersion.Version20,
            BaseAddress = new(baseAddress),
            Timeout = TimeSpan.FromSeconds(9.9d),
        };
        return client;
    }
}

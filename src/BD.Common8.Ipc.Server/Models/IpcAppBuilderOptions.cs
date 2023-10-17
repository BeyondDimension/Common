namespace BD.Common8.Ipc.Server.Models;

/// <summary>
/// 构建 <see cref="WebApplication"/> 的选项配置
/// </summary>
public sealed partial class IpcAppBuilderOptions
{
    /// <summary>
    /// 设置 Ipc 应用程序连接字符串类型
    /// </summary>
    /// <param name="connectionStringType"></param>
    public void SetConnectionString(IpcAppConnectionStringType connectionStringType)
    {
        using var stream = new MemoryStream();
        stream.WriteByte((byte)connectionStringType);
        switch (connectionStringType)
        {
            case IpcAppConnectionStringType.Https:
                stream.Write("https://localhost:"u8);
                stream.Write(Encoding.UTF8.GetBytes(HttpsPort.ToString()));
                break;
            case IpcAppConnectionStringType.UnixSocket:
                stream.Write(Encoding.UTF8.GetBytes(UnixSocketPath.ThrowIsNull()));
                break;
            case IpcAppConnectionStringType.NamedPipe:
                stream.Write(Encoding.UTF8.GetBytes(PipeName.ThrowIsNull()));
                break;
            default:
                ThrowHelper.ThrowArgumentOutOfRangeException(connectionStringType);
                return;
        }
        ConnectionString = stream.ToArray().Base64UrlEncode();
    }

    /// <summary>
    /// 自动选择监听配置
    /// </summary>
    /// <param name="uniqueIdentifier">唯一识别符，一个随机的字符串，要求本机唯一性</param>
    /// <param name="httpsPort">端口号，大于 0 时启用 https 监听</param>
    public void ListenAuto(string uniqueIdentifier, int httpsPort = 0)
    {
        if (OperatingSystem.IsWindows())
        {
            ListenNamedPipe = true;
            PipeName = uniqueIdentifier;
            SetConnectionString(IpcAppConnectionStringType.NamedPipe);
        }
        else
        {
            ListenUnixSocket = true;
            string rootPath;
            try
            {
                rootPath = IOPath.CacheDirectory;
            }
            catch
            {
                rootPath = Path.GetTempPath();
            }
            var socketPath = Path.Combine(rootPath, uniqueIdentifier);
            UnixSocketPath = socketPath;
            SetConnectionString(IpcAppConnectionStringType.UnixSocket);
        }
        if (httpsPort > 0)
        {
            ListenLocalhost = true;
            HttpsPort = httpsPort;
        }
#if DEBUG
        if (!ListenLocalhost)
        {
            ListenLocalhost = true;
            HttpsPort = 26443;
        }
#endif
    }

    /// <summary>
    /// 构建 <see cref="WebApplication"/>
    /// </summary>
    /// <param name="o"></param>
    /// <returns></returns>
    public IpcApp Build()
    {
        // https://learn.microsoft.com/zh-cn/aspnet/core/fundamentals/native-aot?view=aspnetcore-8.0#the-web-api-native-aot-template
        var builder = WebApplication.CreateEmptyBuilder(Options ?? new WebApplicationOptions());
        builder.Services.AddRoutingCore();
        builder.WebHost.UseKestrelCore();
        builder.Services.ConfigureHttpJsonOptions(options =>
        {
            options.SerializerOptions.TypeInfoResolverChain.Insert(0, JsonTypeInfoResolver);
        });
        builder.WebHost.ConfigureKestrel(options =>
        {
            if (ListenLocalhost)
            {
                if (HttpsPort <= 0)
                    ThrowHelper.ThrowArgumentOutOfRangeException(HttpsPort);
                options.ListenLocalhost(HttpsPort, listenOptions =>
                {
                    listenOptions.Protocols = HttpProtocols.Http2; // 必须使用 Http2 协议
                    listenOptions.UseHttps(ServerCertificate.ThrowIsNull()); // Http2 协议必须有 SSL 证书
                });
            }
            if (ListenUnixSocket)
            {
                options.ListenUnixSocket(UnixSocketPath.ThrowIsNull(), listenOptions =>
                {
                    listenOptions.Protocols = HttpProtocols.Http2; // 必须使用 Http2 协议
                });
            }
            if (ListenNamedPipe)
            {
                options.ListenNamedPipe(PipeName.ThrowIsNull(), listenOptions =>
                {
                    listenOptions.Protocols = HttpProtocols.Http2; // 必须使用 Http2 协议
                });
            }
        });
        OnConfigureServices?.Invoke(builder);
        var app = builder.Build();
        OnMapGroupEvent?.Invoke(app);
        return new(app, this);
    }
}
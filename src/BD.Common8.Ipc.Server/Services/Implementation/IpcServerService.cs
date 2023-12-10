using Microsoft.AspNetCore.Diagnostics;
using Microsoft.AspNetCore.Http.Connections;
using AspNetCoreHttpJsonOptions = Microsoft.AspNetCore.Http.Json.JsonOptions;

namespace BD.Common8.Ipc.Services.Implementation;

#pragma warning disable SA1600 // Elements should be documented

public partial class IpcServerService(X509Certificate2 serverCertificate) : IIpcServerService, IDisposable, IAsyncDisposable
{
    WebApplication? app;

    /// <summary>
    /// 是否监听 Localhost
    /// </summary>
    protected virtual bool ListenLocalhost =>
#if DEBUG
        true;
#else
        false;
#endif

    /// <summary>
    /// 是否监听命名管道
    /// </summary>
    protected virtual bool ListenNamedPipe =>
#if DEBUG
        true;
#else
        OperatingSystem.IsWindows();
#endif

    AsyncExclusiveLock lock_RunAsync = new();

    /// <summary>
    /// 是否监听 Unix 套接字
    /// </summary>
    protected virtual bool ListenUnixSocket =>
#if DEBUG
        true;
#else
        !OperatingSystem.IsWindows();
#endif

    /// <inheritdoc/>
    public async ValueTask RunAsync()
    {
        if (app != null)
            return;

        using (await lock_RunAsync.AcquireLockAsync(CancellationToken.None))
        {
            Build();
        }

        Task2.InBackground(() =>
        {
            app.ThrowIsNull().Run();
        }, longRunning: true);
    }

    const HttpProtocols protocols = HttpProtocols.Http2; // 必须使用 Http2 协议

    void Build()
    {
        var builder = WebApplication.CreateEmptyBuilder(new WebApplicationOptions());
        builder.WebHost.UseKestrelCore();
        builder.WebHost.ConfigureKestrel(options =>
        {
            if (ListenLocalhost)
            {
                static int GetRandomUnusedPort(IPAddress address)
                {
                    using var listener = new TcpListener(address, 0);
                    listener.Start();
                    var port = ((IPEndPoint)listener.LocalEndpoint).Port;
                    return port;
                }
                static bool IsUsePort(IPAddress address, int port)
                {
                    try
                    {
                        using var listener = new TcpListener(address, port);
                        listener.Start();
                        return false;
                    }
                    catch
                    {
                        return true;
                    }
                }
                if (IsUsePort(IPAddress.Loopback, Http2Port))
                    Http2Port = GetRandomUnusedPort(IPAddress.Loopback);
                options.ListenLocalhost(Http2Port, listenOptions =>
                {
                    listenOptions.Protocols = protocols;
                    listenOptions.UseHttps(serverCertificate);
                });
            }
            if (ListenNamedPipe)
            {
                options.ListenNamedPipe(PipeName, listenOptions =>
                {
                    listenOptions.Protocols = protocols;
                    listenOptions.UseHttps(serverCertificate);
                });
            }
            if (ListenUnixSocket)
            {
                if (string.IsNullOrWhiteSpace(UnixSocketPath))
                {
                    UnixSocketPath = GetUnixSocketPath();
                }
                options.ListenUnixSocket(UnixSocketPath, listenOptions =>
                {
                    listenOptions.Protocols = protocols; // 必须使用 Http2 协议
                    listenOptions.UseHttps(serverCertificate);
                });
            }
        });

        builder.Services.AddRoutingCore();
        builder.Services.AddLogging(ConfigureLogging);
        builder.Services.ConfigureHttpJsonOptions(ConfigureHttpJsonOptions);
        builder.Services.AddSignalR(ConfigureSignalR).AddJsonProtocol();

        ConfigureServices(builder.Services);

        app = builder.Build();
        Configure(app);
    }

    /// <summary>
    /// 获取优先使用的连接字符串类型
    /// </summary>
    /// <returns></returns>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    static IpcAppConnectionStringType GetFirstIpcAppConnectionStringType()
    {
        if (OperatingSystem.IsWindows())
        {
            return IpcAppConnectionStringType.NamedPipe;
        }
        return IpcAppConnectionStringType.UnixSocket;
    }

    /// <inheritdoc/>
    public IpcAppConnectionString GetConnectionString(IpcAppConnectionStringType? type = null)
    {
        if (app == null)
            throw new InvalidOperationException("The service has not been started yet.");

        type ??= GetFirstIpcAppConnectionStringType();

        switch (type.Value)
        {
            case IpcAppConnectionStringType.Https:
                if (!ListenLocalhost)
                    throw new NotSupportedException(
                        "The current service does not support listening localhost.");
                return new()
                {
                    Type = IpcAppConnectionStringType.Https,
                    Int32Value = Http2Port,
                };
            case IpcAppConnectionStringType.UnixSocket:
                if (!ListenUnixSocket)
                    throw new NotSupportedException(
                        "The current service does not support listening unix socket.");
                return new()
                {
                    Type = IpcAppConnectionStringType.UnixSocket,
                    StringValue = UnixSocketPath,
                };
            case IpcAppConnectionStringType.NamedPipe:
                if (!ListenNamedPipe)
                    throw new NotSupportedException(
                        "The current service does not support listening named pipe.");
                return new()
                {
                    Type = IpcAppConnectionStringType.NamedPipe,
                    StringValue = PipeName,
                };
            default:
                throw ThrowHelper.GetArgumentOutOfRangeException(type);
        }
    }

    /// <summary>
    /// 走 Http2 传输协议默认端口号，如果端口占用将随机一个新的
    /// </summary>
    protected virtual int Http2Port { get; set; } = 15076;

    /// <summary>
    /// 走命名管道传输协议的管道名
    /// </summary>
    protected virtual string PipeName
    {
        get
        {
            var thisType = GetType();
            var value = thisType.FullName ?? thisType.Name;
#if !DEBUG
            value = Hashs.String.SHA256(value, isLower: false);
#endif
            return value;
        }
    }

    /// <summary>
    /// Unix 套接字文件路径
    /// </summary>
    protected virtual string? UnixSocketPath { get; private set; }

    /// <summary>
    /// 获取 Unix 套接字文件路径，默认值 %TEMP%\BD.Common8.Ipc\{Hashs.String.Crc32(PipeName)}.uds
    /// </summary>
    /// <returns></returns>
    protected virtual string GetUnixSocketPath()
    {
        var dirPath = Path.Combine(IOPath.GetTempPath(), "BD.Common8.Ipc");
        IOPath.DirCreateByNotExists(dirPath);
        var filePath = Path.Combine(dirPath, $"{Hashs.String.Crc32(PipeName)}.uds"); // Unix Domain Socket
        IOPath.FileTryDelete(filePath);
        return filePath;
    }

    /// <summary>
    /// Http2 数据协议的服务器证书
    /// </summary>
    X509Certificate2 serverCertificate = serverCertificate;

    /// <summary>
    /// 配置日志
    /// </summary>
    /// <param name="builder"></param>
    protected virtual void ConfigureLogging(ILoggingBuilder builder)
    {
        builder.AddConsole();
    }

    /// <summary>
    /// Json 源生成的解析器
    /// </summary>
    protected virtual IJsonTypeInfoResolver? JsonTypeInfoResolver => null;

    protected virtual void ConfigureHttpJsonOptions(AspNetCoreHttpJsonOptions options)
    {
        var resolver = JsonTypeInfoResolver;
        if (resolver != null)
        {
            options.SerializerOptions.TypeInfoResolverChain.Insert(0, resolver);
        }
    }

    protected virtual void ConfigureJsonHubProtocolOptions(JsonHubProtocolOptions options)
    {
        var resolver = JsonTypeInfoResolver;
        if (resolver != null)
        {
            // 添加源生成的 Json 解析器
            options.PayloadSerializerOptions.TypeInfoResolverChain.Insert(0, resolver);
        }
        // 添加默认的 Json 解析器，用作简单类型的解析
        options.PayloadSerializerOptions.TypeInfoResolverChain.Add(new DefaultJsonTypeInfoResolver());
    }

    protected virtual void ConfigureSignalR(HubOptions options)
    {
        // 如果在此间隔时间内未收到消息（包括保持连接状态），服务器将认为客户端已断开连接。
        // 由于实现方式的不同，将客户端标记为断开连接可能需要更长的超时间隔时间。
        // 建议的值为 KeepAliveInterval 值的两倍。
        // 默认值：30 秒
        //options.ClientTimeoutInterval = TimeSpan.MaxValue;

        // 如果客户端在此时间间隔内未发送初始握手消息，则连接将关闭。
        // 这是一个高级设置，只应在由于严重的网络延迟而发生握手超时错误时才考虑修改。
        // 有关握手过程的更多详细信息，请参阅 SignalR 中心协议规范。
        // 默认值：15 秒
        //options.HandshakeTimeout = TimeSpan.FromSeconds(5);

        // 如果服务器在此间隔内未发送消息，将自动发送 ping 消息以保持连接处于开启状态。
        // 更改 KeepAliveInterval 时，请更改客户端上的 ServerTimeout 或 serverTimeoutInMilliseconds 设置。 建议的 ServerTimeout 或 serverTimeoutInMilliseconds 值是 KeepAliveInterval 值的两倍。
        // 默认值：15 秒
        //options.KeepAliveInterval = TimeSpan.FromSeconds(5);
#if DEBUG
        //options.EnableDetailedErrors = true;
#endif
    }

    protected virtual void ConfigureServices(IServiceCollection services)
    {
    }

    /// <summary>
    /// <see cref="IEndpointRouteMapGroup.OnMapGroup(IEndpointRouteBuilder)"/> 的事件
    /// </summary>
    internal static event Action<IEndpointRouteBuilder>? OnMapGroupEvent;

    protected virtual void Configure(WebApplication app)
    {
        app.UseWelcomePage("/");
        app.UseExceptionHandler(builder => builder.Run(OnError));
        OnMapGroupEvent?.Invoke(app);
    }

    protected virtual void ConfigureHub(HttpConnectionDispatcherOptions options)
    {
        options.Transports = HttpTransportType.WebSockets;
    }

    protected HubEndpointConventionBuilder MapHub<[DynamicallyAccessedMembers(DynamicallyAccessedMemberTypes.PublicConstructors | DynamicallyAccessedMemberTypes.PublicMethods)] THub>([StringSyntax("Route")] string pattern) where THub : Hub
    {
        return app!.MapHub<THub>(pattern, ConfigureHub);
    }

    protected virtual async Task OnError(HttpContext ctx)
    {
        ctx.Response.StatusCode = (int)HttpStatusCode.InternalServerError;
        ctx.Response.ContentType = MediaTypeNames.JSON;

        var exceptionHandlerPathFeature = ctx.Features.Get<IExceptionHandlerFeature>();
        if (exceptionHandlerPathFeature != null)
        {
            OnError(exceptionHandlerPathFeature);
        }

        await Task.CompletedTask;
    }

    protected virtual void OnError(IExceptionHandlerFeature exceptionHandlerPathFeature)
    {
#if DEBUG
        Console.WriteLine("OnError: ");
        Console.WriteLine(exceptionHandlerPathFeature?.Error);
#endif
    }

    #region 同时实现释放模式和异步释放模式 https://learn.microsoft.com/zh-cn/dotnet/standard/garbage-collection/implementing-disposeasync#implement-both-dispose-and-async-dispose-patterns

    public async ValueTask DisposeAsync()
    {
        await DisposeAsyncCore().ConfigureAwait(false);
        Dispose(disposing: false);
        GC.SuppressFinalize(this);
    }

    protected virtual async ValueTask DisposeAsyncCore()
    {
        if (app is not null)
        {
            await app.DisposeAsync().ConfigureAwait(false);
        }
        if (lock_RunAsync is not null)
        {
            await lock_RunAsync.DisposeAsync().ConfigureAwait(false);
            lock_RunAsync = null!;
        }

        if (serverCertificate != null)
        {
            serverCertificate.Dispose();
            serverCertificate = null!;
        }

        app = null;

        if (UnixSocketPath != null)
        {
            IOPath.FileTryDelete(UnixSocketPath);
            UnixSocketPath = null;
        }
    }

    protected virtual void Dispose(bool disposing)
    {
        if (disposing)
        {
            // 释放托管状态(托管对象)
            if (app is IDisposable disposable_app)
            {
                disposable_app.Dispose();
                app = null;
            }

            if (lock_RunAsync is not null)
            {
                lock_RunAsync.Dispose();
                lock_RunAsync = null!;
            }

            if (serverCertificate != null)
            {
                serverCertificate.Dispose();
                serverCertificate = null!;
            }

            if (UnixSocketPath != null)
            {
                IOPath.FileTryDelete(UnixSocketPath);
                UnixSocketPath = null;
            }
        }
    }

    public void Dispose()
    {
        // 不要更改此代码。请将清理代码放入“Dispose(bool disposing)”方法中
        Dispose(disposing: true);
        GC.SuppressFinalize(this);
    }

    #endregion
}

public abstract class IpcServerHub : Hub
{
    public virtual CancellationToken RequestAborted
    {
        get
        {
            var httpContext = Context.GetHttpContext();
            if (httpContext == null) return default;
            return httpContext.RequestAborted;
        }
    }
}

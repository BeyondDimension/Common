using Microsoft.AspNetCore.Diagnostics;
using Microsoft.AspNetCore.Http.Connections;
using Microsoft.AspNetCore.Server.Kestrel.Transport.NamedPipes;
using Microsoft.Extensions.Primitives;
using System.Security.AccessControl;
using AspNetCoreHttpJsonOptions = Microsoft.AspNetCore.Http.Json.JsonOptions;

namespace BD.Common8.Ipc.Services.Implementation;

#pragma warning disable SA1600 // Elements should be documented

public abstract class IpcServerService(X509Certificate2 serverCertificate) : IIpcServerService, IDisposable, IAsyncDisposable
{
    /// <summary>
    /// <see cref="IEndpointRouteMapGroup.OnMapGroup(IEndpointRouteBuilder)"/> 的事件
    /// </summary>
    internal static event Action<IEndpointRouteBuilder>? OnMapGroupEvent;

    protected static void OnMapGroup(IEndpointRouteBuilder builder) => OnMapGroupEvent?.Invoke(builder);

    static readonly long tickCount64 = long.MaxValue / 2; // 不可修改！！！

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

        if (ListenNamedPipe &&
            OperatingSystem.IsWindows() &&
            Environment.IsPrivilegedProcess)
        {
            builder.Services.Configure<NamedPipeTransportOptions>(static options =>
            {
                // 在 Windows 上允许不同用户连接到命名管道
#pragma warning disable CA1416 // 验证平台兼容性
                SecurityIdentifier securityIdentifier = new(WellKnownSidType.AuthenticatedUserSid, null);
                PipeSecurity pipeSecurity = new();
                pipeSecurity.AddAccessRule(new PipeAccessRule(securityIdentifier,
                    PipeAccessRights.ReadWrite | PipeAccessRights.CreateNewInstance,
                    AccessControlType.Allow));
                options.CurrentUserOnly = false;
                options.PipeSecurity = pipeSecurity;
#pragma warning restore CA1416 // 验证平台兼容性
            });
        }

        builder.Services.Configure<KestrelServerOptions>(static options =>
        {
            options.AddServerHeader = false;
        });
        builder.Services.AddRoutingCore();
        builder.Services.AddLogging(ConfigureLogging);
        builder.Services.ConfigureHttpJsonOptions(ConfigureHttpJsonOptions);
        builder.Services.AddSignalR(ConfigureSignalR).AddJsonProtocol();
        builder.Services.AddHttpContextAccessor();
        ConfigureAuthentication(builder.Services.AddAuthentication(DefaultAuthenticationScheme));

        ConfigureServices(builder.Services);

        app = builder.Build();
        Configure(app);
    }

    ///// <summary>
    ///// 获取优先使用的连接字符串类型
    ///// </summary>
    ///// <returns></returns>
    //[MethodImpl(MethodImplOptions.AggressiveInlining)]
    //static IpcAppConnectionStringType GetFirstIpcAppConnectionStringType()
    //{
    //    if (OperatingSystem.IsWindows())
    //    {
    //        return IpcAppConnectionStringType.NamedPipe;
    //    }
    //    return IpcAppConnectionStringType.UnixSocket;
    //}

    /// <inheritdoc/>
    public IpcAppConnectionString GetConnectionString(IpcAppConnectionStringType/*?*/ type/* = null*/)
    {
        if (app == null)
            throw new InvalidOperationException("The service has not been started yet.");

        //type ??= GetFirstIpcAppConnectionStringType();

        switch (type/*.Value*/)
        {
            case IpcAppConnectionStringType.Https:
                if (!ListenLocalhost)
                    throw new NotSupportedException(
                        "The current service does not support listening localhost.");
                return new()
                {
                    Type = IpcAppConnectionStringType.Https,
                    Int32Value = Http2Port,
                    TickCount64 = tickCount64,
                    ProcessId = Environment.ProcessId,
                };
            case IpcAppConnectionStringType.UnixSocket:
                if (!ListenUnixSocket)
                    throw new NotSupportedException(
                        "The current service does not support listening unix socket.");
                return new()
                {
                    Type = IpcAppConnectionStringType.UnixSocket,
                    StringValue = UnixSocketPath,
                    TickCount64 = tickCount64,
                    ProcessId = Environment.ProcessId,
                };
            case IpcAppConnectionStringType.NamedPipe:
                if (!ListenNamedPipe)
                    throw new NotSupportedException(
                        "The current service does not support listening named pipe.");
                return new()
                {
                    Type = IpcAppConnectionStringType.NamedPipe,
                    StringValue = PipeName,
                    TickCount64 = tickCount64,
                    ProcessId = Environment.ProcessId,
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
    ///  获取 Unix 套接字文件夹路径，默认值 %TEMP%\BD.Common8.Ipc
    /// </summary>
    /// <returns></returns>
    protected virtual string GetUnixSocketDirPath()
    {
        const string dirName = "BD.Common8.Ipc";
        // The length must be between 1 and 108 characters, inclusive.
        string dirPath;
        try
        {
            dirPath = Path.Combine(IOPath.CacheDirectory, dirName);
            if (dirPath.Length <= 108)
                return dirPath;
        }
        catch { }
        dirPath = Path.Combine(IOPath.GetTempPath(), dirName);
        return dirPath;
    }

    /// <summary>
    /// 获取 Unix 套接字文件路径，默认值 %TEMP%\BD.Common8.Ipc\{Hashs.String.Crc32(PipeName)}.uds
    /// </summary>
    /// <returns></returns>
    protected virtual string GetUnixSocketPath()
    {
        var dirPath = GetUnixSocketDirPath();
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
    /// 用于序列化的类型信息，由 Json 源生成，值指向 SystemTextJsonSerializerContext.Default.Options，由实现类重写
    /// </summary>
    protected virtual SystemTextJsonSerializerOptions JsonSerializerOptions
        => SystemTextJsonSerializerOptions.Default;

    protected virtual void ConfigureHttpJsonOptions(AspNetCoreHttpJsonOptions options)
    {
        JsonSerializerOptions.CopyTypeInfoResolverChainTo(options.SerializerOptions);
    }

    protected virtual void ConfigureJsonHubProtocolOptions(JsonHubProtocolOptions options)
    {
        JsonSerializerOptions.CopyTypeInfoResolverChainTo(options.PayloadSerializerOptions);
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

    protected virtual string DefaultAuthenticationScheme => IpcAppConnectionString.AuthenticationScheme;

    protected virtual void ConfigureAuthentication(AuthenticationBuilder builder)
    {
        builder.AddScheme<IpcAuthenticationSchemeOptions, IpcAuthenticationHandler>(
            DefaultAuthenticationScheme, options =>
            {
                options.IpcServerService = this;
            });
    }

    /// <inheritdoc cref="IpcAppConnectionString.HubUrl"/>
    protected virtual string HubUrl => IpcAppConnectionString.HubUrl;

    protected virtual void Configure(WebApplication app)
    {
        app.UseWelcomePage("/");
        //app.UseCors();
        app.UseAuthentication();
        app.UseAuthorization();
        app.UseExceptionHandler(builder => builder.Run(OnError));
        OnMapGroup(app);
    }

    protected virtual void ConfigureHub(HttpConnectionDispatcherOptions options)
    {
        options.Transports = HttpTransportType.WebSockets;
    }

    public IServiceProvider Services => app.ThrowIsNull().Services;

    public abstract IHubContext HubContext { get; }

    readonly Dictionary<string, Type> hubTypes = [];

    public IHubContext? GetHubContextByHubUrl(string? hubUrl = null)
    {
        if (hubTypes.TryGetValue(hubUrl ?? HubUrl, out var hubType))
        {
            var hubContextType = typeof(IHubContext<>).MakeGenericType(hubType);
            return (IHubContext)Services.GetRequiredService(hubContextType);
        }
        return null;
    }

    public HubEndpointConventionBuilder MapHub<[DynamicallyAccessedMembers(DynamicallyAccessedMemberTypes.PublicConstructors | DynamicallyAccessedMemberTypes.PublicMethods)] THub>([StringSyntax("Route")] string hubUrl) where THub : Hub
    {
        hubTypes.Add(hubUrl, typeof(THub));
        return app!.MapHub<THub>(hubUrl, ConfigureHub);
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

    protected byte[]? _AccessToken;

    /// <summary>
    /// 获取持有者令牌身份验证
    /// <para>https://learn.microsoft.com/zh-cn/aspnet/core/signalr/authn-and-authz?view=aspnetcore-8.0#bearer-token-authentication</para>
    /// </summary>
    /// <returns></returns>
    protected virtual byte[] GetAccessToken()
    {
        if (_AccessToken == null)
        {
            using var stream = new MemoryStream();
            IpcAppConnectionString.WriteAccessToken(stream, tickCount64, Environment.ProcessId);
            _AccessToken = Hashs.ByteArray.SHA256(stream);
        }
        return _AccessToken;
    }

    /// <inheritdoc cref="GetAccessToken"/>
    internal byte[] AccessToken => GetAccessToken();

    #region 同时实现释放模式和异步释放模式 https://learn.microsoft.com/zh-cn/dotnet/standard/garbage-collection/implementing-disposeasync#implement-both-dispose-and-async-dispose-patterns

    public async ValueTask DisposeAsync()
    {
        await DisposeAsyncCore().ConfigureAwait(false);
        Dispose(disposing: false);
        GC.SuppressFinalize(this);
    }

    void DeleteUnixSocketFile()
    {
        if (ListenUnixSocket)
        {
            UnixSocketPath ??= GetUnixSocketPath();
            IOPath.FileTryDelete(UnixSocketPath);
            UnixSocketPath = null;
        }
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

        DeleteUnixSocketFile();
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

            DeleteUnixSocketFile();
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

file sealed class IpcAuthenticationSchemeOptions : AuthenticationSchemeOptions
{
    public IpcServerService IpcServerService { get; set; } = null!;
}

file sealed class IpcAuthenticationHandler(IOptionsMonitor<IpcAuthenticationSchemeOptions> options, ILoggerFactory logger, UrlEncoder encoder)
    : AuthenticationHandler<IpcAuthenticationSchemeOptions>(options, logger, encoder)
{
    AuthenticateResult HandleAuthenticate()
    {
        var accessToken = Request.Headers.Authorization;
        var path = Request.Path;

        if (StringValues.IsNullOrEmpty(accessToken) &&
            path.StartsWithSegments("/Hubs"))
        {
            // 在标准 Web API 中，持有者令牌在 HTTP 标头中发送。
            // 但是，当使用某些传输时，SignalR 无法在浏览器中设置这些标头。
            // 使用 WebSocket 和服务器发送的事件时，令牌作为查询字符串参数传输。
            // https://learn.microsoft.com/zh-cn/aspnet/core/signalr/authn-and-authz?view=aspnetcore-8.0#built-in-jwt-authentication
            accessToken = Request.Query["access_token"];
        }

        if (!StringValues.IsNullOrEmpty(accessToken))
        {
            string accessTokenString = accessToken!;
            const string authenticationSchemePrefix =
                $"{IpcAppConnectionString.AuthenticationScheme} ";
            const string authenticationSchemePrefix2 = // SignalR 中授权头会填充一个 Bearer
                $"Bearer {IpcAppConnectionString.AuthenticationScheme} ";

            ReadOnlySpan<char> hexString = default;
            if (accessTokenString.StartsWith(authenticationSchemePrefix, StringComparison.OrdinalIgnoreCase))
            {
                hexString = accessTokenString
                    .AsSpan()[authenticationSchemePrefix.Length..];
            }
            else if (accessTokenString.StartsWith(authenticationSchemePrefix2, StringComparison.OrdinalIgnoreCase))
            {
                hexString = accessTokenString
                    .AsSpan()[authenticationSchemePrefix2.Length..];
            }
            if (hexString.Length == Hashs.String.Lengths.SHA256)
            {
                var accessTokenBytes = Convert.FromHexString(hexString);
                if (accessTokenBytes.SequenceEqual(Options.IpcServerService.AccessToken))
                {
                    var identity = new ClaimsIdentity(IpcAppConnectionString.AuthenticationScheme);
                    var principal = new ClaimsPrincipal(identity);
                    AuthenticationTicket ticket = new(principal, null, IpcAppConnectionString.AuthenticationScheme);
                    return AuthenticateResult.Success(ticket);
                }
            }
        }
        return AuthenticateResult.Fail("The accessToken is incorrect.");
    }

    /// <inheritdoc/>
    protected override Task<AuthenticateResult> HandleAuthenticateAsync()
    {
        var result = HandleAuthenticate();
        return Task.FromResult(result);
    }
}
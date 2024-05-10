//using Grpc.Core;
//using Microsoft.AspNetCore.Diagnostics;
//using System.Security.AccessControl;
//using AspNetCoreHttpJsonOptions = Microsoft.AspNetCore.Http.Json.JsonOptions;
//using PB = Google.Protobuf;
//using PBC = Google.Protobuf.Collections;

//namespace BD.Common8.Ipc.Services.Implementation;

//public abstract class IpcServerService(X509Certificate2 serverCertificate) : IIpcServerService, IDisposable, IAsyncDisposable
//{
//    internal static List<nint> HandleStreamEvents = new();
//    internal static List<nint> HandleAsyncEnumerableEvents = new();

//    static readonly Lazy<ImmutableArray<nint>> _HandleStreamEvents = new(() =>
//    {
//        var result = HandleStreamEvents.ToImmutableArray();
//        HandleStreamEvents = null!;
//        return result;
//    }, LazyThreadSafetyMode.ExecutionAndPublication);

//    static readonly Lazy<ImmutableArray<nint>> _HandleAsyncEnumerableEvents = new(() =>
//    {
//        var result = HandleAsyncEnumerableEvents.ToImmutableArray();
//        HandleAsyncEnumerableEvents = null!;
//        return result;
//    }, LazyThreadSafetyMode.ExecutionAndPublication);

//    public static async Task OnHandleStream(PBC::RepeatedField<string> names, PB::ByteString data, Stream stream, CancellationToken cancellationToken)
//    {
//        foreach (var handleEvent in _HandleStreamEvents.Value)
//        {
//            if (handleEvent == default)
//                continue;
//            Task<bool> result;
//            unsafe
//            {
//                var @delegate = (delegate* managed<PBC::RepeatedField<string>, PB::ByteString, Stream, CancellationToken, Task<bool>>)handleEvent;
//                result = @delegate(names, data, stream, cancellationToken);
//            }
//            if (await result)
//            {
//                return;
//            }
//        }
//    }

//    public static IAsyncEnumerable<byte[]>? OnHandleAsyncEnumerable(PBC::RepeatedField<string> names, PB::ByteString data, CancellationToken cancellationToken)
//    {
//        foreach (var handleEvent in _HandleAsyncEnumerableEvents.Value)
//        {
//            if (handleEvent == default)
//                continue;
//            (bool, IAsyncEnumerable<byte[]>) result;
//            unsafe
//            {
//                var @delegate = (delegate* managed<PBC::RepeatedField<string>, PB::ByteString, CancellationToken, (bool, IAsyncEnumerable<byte[]>)>)handleEvent;
//                result = @delegate(names, data, cancellationToken);
//            }
//            if (result.Item1)
//            {
//                return result.Item2;
//            }
//        }
//        return null;
//    }

//    static readonly long tickCount64 = long.MaxValue / 2; // 不可修改！！！

//    WebApplication? app;

//    /// <summary>
//    /// 是否监听 Localhost
//    /// </summary>
//    protected virtual bool ListenLocalhost =>
//#if DEBUG
//        true;
//#else
//        false;
//#endif

//    /// <summary>
//    /// 是否监听命名管道
//    /// </summary>
//    protected virtual bool ListenNamedPipe =>
//#if DEBUG
//        true;
//#else
//        OperatingSystem.IsWindows();
//#endif

//    AsyncExclusiveLock lock_RunAsync = new();

//    /// <summary>
//    /// 是否监听 Unix 套接字
//    /// </summary>
//    protected virtual bool ListenUnixSocket =>
//#if DEBUG
//        true;
//#else
//        !OperatingSystem.IsWindows();
//#endif

//    /// <inheritdoc/>
//    public async ValueTask RunAsync()
//    {
//        if (app != null)
//            return;

//        using (await lock_RunAsync.AcquireLockAsync(CancellationToken.None))
//        {
//            Build();
//        }

//        Task2.InBackground(() =>
//        {
//            app.ThrowIsNull().Run();
//        }, longRunning: true);
//    }

//    const HttpProtocols protocols = HttpProtocols.Http2; // 必须使用 Http2 协议

//    void Build()
//    {
//        var builder = WebApplication.CreateEmptyBuilder(new WebApplicationOptions());
//        builder.WebHost.UseKestrelCore();
//        builder.WebHost.ConfigureKestrel(options =>
//        {
//            if (ListenLocalhost)
//            {
//                static int GetRandomUnusedPort(IPAddress address)
//                {
//                    using var listener = new TcpListener(address, 0);
//                    listener.Start();
//                    var port = ((IPEndPoint)listener.LocalEndpoint).Port;
//                    return port;
//                }
//                static bool IsUsePort(IPAddress address, int port)
//                {
//                    try
//                    {
//                        using var listener = new TcpListener(address, port);
//                        listener.Start();
//                        return false;
//                    }
//                    catch
//                    {
//                        return true;
//                    }
//                }
//                if (IsUsePort(IPAddress.Loopback, Http2Port))
//                    Http2Port = GetRandomUnusedPort(IPAddress.Loopback);
//                options.ListenLocalhost(Http2Port, listenOptions =>
//                {
//                    listenOptions.Protocols = protocols;
//                    listenOptions.UseHttps(serverCertificate);
//                });
//            }
//            if (ListenNamedPipe)
//            {
//                options.ListenNamedPipe(PipeName, listenOptions =>
//                {
//                    listenOptions.Protocols = protocols;
//                    listenOptions.UseHttps(serverCertificate);
//                });
//            }
//            if (ListenUnixSocket)
//            {
//                if (string.IsNullOrWhiteSpace(UnixSocketPath))
//                {
//                    UnixSocketPath = GetUnixSocketPath();
//                }
//                options.ListenUnixSocket(UnixSocketPath, listenOptions =>
//                {
//                    listenOptions.Protocols = protocols; // 必须使用 Http2 协议
//                    listenOptions.UseHttps(serverCertificate);
//                });
//            }
//        });

//        if (ListenNamedPipe &&
//            OperatingSystem.IsWindows() &&
//            Environment.IsPrivilegedProcess)
//        {
//            builder.WebHost.UseNamedPipes(static options =>
//            {
//                // 在 Windows 上允许不同用户连接到命名管道
//#pragma warning disable CA1416 // 验证平台兼容性
//                SecurityIdentifier securityIdentifier = new(WellKnownSidType.AuthenticatedUserSid, null);
//                PipeSecurity pipeSecurity = new();
//                pipeSecurity.AddAccessRule(new PipeAccessRule(securityIdentifier,
//                    PipeAccessRights.ReadWrite | PipeAccessRights.CreateNewInstance,
//                    AccessControlType.Allow));
//                options.CurrentUserOnly = false;
//                options.PipeSecurity = pipeSecurity;
//#pragma warning restore CA1416 // 验证平台兼容性
//            });
//        }

//        builder.Services.Configure<KestrelServerOptions>(static options =>
//        {
//            options.AddServerHeader = false;
//        });
//        builder.Services.Configure<HostOptions>(static options =>
//        {
//            // 忽略后台服务中的异常避免整个 Host 中止
//            options.BackgroundServiceExceptionBehavior =
//                BackgroundServiceExceptionBehavior.Ignore;
//        });
//        builder.Services.AddLogging(ConfigureLogging);
//        builder.Services.AddHttpContextAccessor();
//        builder.Services.AddGrpc();

//        ConfigureServices(builder.Services);

//        app = builder.Build();

//        app.UseWelcomePage("/");
//        app.MapGrpcService<GrpcIpcServiceImpl>();
//        app.UseExceptionHandler(builder => builder.Run(OnError));

//        Configure(app);
//    }

//    /// <inheritdoc/>
//    public IpcAppConnectionString GetConnectionString(IpcAppConnectionStringType type)
//    {
//        if (app == null)
//            throw new InvalidOperationException("The service has not been started yet.");

//        switch (type)
//        {
//            case IpcAppConnectionStringType.Https:
//                if (!ListenLocalhost)
//                    throw new NotSupportedException(
//                        "The current service does not support listening localhost.");
//                return new()
//                {
//                    Type = IpcAppConnectionStringType.Https,
//                    Int32Value = Http2Port,
//                    TickCount64 = tickCount64,
//                    ProcessId = Environment.ProcessId,
//                };
//            case IpcAppConnectionStringType.UnixSocket:
//                if (!ListenUnixSocket)
//                    throw new NotSupportedException(
//                        "The current service does not support listening unix socket.");
//                return new()
//                {
//                    Type = IpcAppConnectionStringType.UnixSocket,
//                    StringValue = UnixSocketPath,
//                    TickCount64 = tickCount64,
//                    ProcessId = Environment.ProcessId,
//                };
//            case IpcAppConnectionStringType.NamedPipe:
//                if (!ListenNamedPipe)
//                    throw new NotSupportedException(
//                        "The current service does not support listening named pipe.");
//                return new()
//                {
//                    Type = IpcAppConnectionStringType.NamedPipe,
//                    StringValue = PipeName,
//                    TickCount64 = tickCount64,
//                    ProcessId = Environment.ProcessId,
//                };
//            default:
//                throw ThrowHelper.GetArgumentOutOfRangeException(type);
//        }
//    }

//    /// <summary>
//    /// 走 Http2 传输协议默认端口号，如果端口占用将随机一个新的
//    /// </summary>
//    protected virtual int Http2Port { get; set; } = 15076;

//    /// <summary>
//    /// 走命名管道传输协议的管道名
//    /// </summary>
//    protected virtual string PipeName
//    {
//        get
//        {
//            var thisType = GetType();
//            var value = thisType.FullName ?? thisType.Name;
//#if !DEBUG
//            value = Hashs.String.SHA256(value, isLower: false);
//#endif
//            return value;
//        }
//    }

//    /// <summary>
//    /// Unix 套接字文件路径
//    /// </summary>
//    protected virtual string? UnixSocketPath { get; private set; }

//    /// <summary>
//    ///  获取 Unix 套接字文件夹路径，默认值 %TEMP%\BD.Common8.Ipc
//    /// </summary>
//    /// <returns></returns>
//    protected virtual string GetUnixSocketDirPath()
//    {
//        const string dirName = "BD.Common8.Ipc";
//        // The length must be between 1 and 108 characters, inclusive.
//        string dirPath;
//        try
//        {
//            dirPath = Path.Combine(IOPath.CacheDirectory, dirName);
//            if (dirPath.Length <= 108)
//                return dirPath;
//        }
//        catch { }
//        dirPath = Path.Combine(IOPath.GetTempPath(), dirName);
//        return dirPath;
//    }

//    /// <summary>
//    /// 获取 Unix 套接字文件路径，默认值 %TEMP%\BD.Common8.Ipc\{Hashs.String.Crc32(PipeName)}.uds
//    /// </summary>
//    /// <returns></returns>
//    protected virtual string GetUnixSocketPath()
//    {
//        var dirPath = GetUnixSocketDirPath();
//        IOPath.DirCreateByNotExists(dirPath);
//        var filePath = Path.Combine(dirPath, $"{Hashs.String.Crc32(PipeName)}.uds"); // Unix Domain Socket
//        IOPath.FileTryDelete(filePath);
//        return filePath;
//    }

//    /// <summary>
//    /// Http2 数据协议的服务器证书
//    /// </summary>
//    X509Certificate2 serverCertificate = serverCertificate;

//    /// <summary>
//    /// 配置日志
//    /// </summary>
//    /// <param name="builder"></param>
//    protected virtual void ConfigureLogging(ILoggingBuilder builder)
//    {
//        builder.AddConsole();
//    }

//    /// <summary>
//    /// 用于序列化的类型信息，由 Json 源生成，值指向 SystemTextJsonSerializerContext.Default.Options，由实现类重写
//    /// </summary>
//    protected virtual SystemTextJsonSerializerOptions JsonSerializerOptions
//        => SystemTextJsonSerializerOptions.Default;

//    protected virtual void ConfigureHttpJsonOptions(AspNetCoreHttpJsonOptions options)
//    {
//        JsonSerializerOptions.CopyTypeInfoResolverChainTo(options.SerializerOptions);
//        Serializable.CreateOptions(options.SerializerOptions, isReadOnly: true);
//    }

//    protected virtual bool UseMemoryPack => false;

//    protected virtual void ConfigureServices(IServiceCollection services)
//    {
//    }

//    /// <inheritdoc cref="IpcAppConnectionString.HubUrl"/>
//    protected virtual string HubUrl => IpcAppConnectionString.HubUrl;

//    protected virtual void Configure(WebApplication app)
//    {
//    }

//    public IServiceProvider Services => app.ThrowIsNull().Services;

//    protected virtual async Task OnError(HttpContext ctx)
//    {
//        ctx.Response.StatusCode = (int)HttpStatusCode.InternalServerError;
//        ctx.Response.ContentType = MediaTypeNames.JSON;

//        var exceptionHandlerPathFeature = ctx.Features.Get<IExceptionHandlerFeature>();
//        if (exceptionHandlerPathFeature != null)
//        {
//            OnError(exceptionHandlerPathFeature);
//        }

//        await Task.CompletedTask;
//    }

//    protected virtual void OnError(IExceptionHandlerFeature exceptionHandlerPathFeature)
//    {
//#if DEBUG
//        Console.WriteLine("OnError: ");
//        Console.WriteLine(exceptionHandlerPathFeature?.Error);
//#endif
//    }

//    /// <inheritdoc cref="GetAccessToken"/>
//    protected byte[]? _AccessToken;

//    /// <summary>
//    /// 获取持有者令牌身份验证
//    /// <para>https://learn.microsoft.com/zh-cn/aspnet/core/signalr/authn-and-authz?view=aspnetcore-8.0#bearer-token-authentication</para>
//    /// </summary>
//    /// <returns></returns>
//    protected virtual byte[] GetAccessToken()
//    {
//        if (_AccessToken == null)
//        {
//            using var stream = new MemoryStream();
//            IpcAppConnectionString.WriteAccessToken(stream, tickCount64, Environment.ProcessId);
//            _AccessToken = Hashs.ByteArray.SHA256(stream);
//            Console.WriteLine($"Server，GetAccessToken：{_AccessToken.ToHexString()}, tickCount64: {tickCount64}, pid: {Environment.ProcessId}");
//        }
//        return _AccessToken;
//    }

//    /// <inheritdoc cref="GetAccessToken"/>
//    internal byte[] AccessToken => GetAccessToken();

//    #region 同时实现释放模式和异步释放模式 https://learn.microsoft.com/zh-cn/dotnet/standard/garbage-collection/implementing-disposeasync#implement-both-dispose-and-async-dispose-patterns

//    /// <inheritdoc/>
//    public async ValueTask DisposeAsync()
//    {
//        await DisposeAsyncCore().ConfigureAwait(false);
//        Dispose(disposing: false);
//        GC.SuppressFinalize(this);
//    }

//    void DeleteUnixSocketFile()
//    {
//        if (ListenUnixSocket)
//        {
//            UnixSocketPath ??= GetUnixSocketPath();
//            IOPath.FileTryDelete(UnixSocketPath);
//            UnixSocketPath = null;
//        }
//    }

//    /// <inheritdoc cref="IAsyncDisposable.DisposeAsync"/>
//    protected virtual async ValueTask DisposeAsyncCore()
//    {
//        if (lock_RunAsync is not null)
//        {
//            await lock_RunAsync.DisposeAsync().ConfigureAwait(false);
//            lock_RunAsync = null!;
//        }

//        if (serverCertificate != null)
//        {
//            serverCertificate.Dispose();
//            serverCertificate = null!;
//        }

//        app = null;

//        DeleteUnixSocketFile();
//    }

//    /// <inheritdoc cref="IDisposable.Dispose"/>
//    protected virtual void Dispose(bool disposing)
//    {
//        if (disposing)
//        {
//            // 释放托管状态(托管对象)
//            if (lock_RunAsync is not null)
//            {
//                lock_RunAsync.Dispose();
//                lock_RunAsync = null!;
//            }

//            if (serverCertificate != null)
//            {
//                serverCertificate.Dispose();
//                serverCertificate = null!;
//            }

//            DeleteUnixSocketFile();
//        }
//    }

//    /// <inheritdoc/>
//    public void Dispose()
//    {
//        // 不要更改此代码。请将清理代码放入“Dispose(bool disposing)”方法中
//        Dispose(disposing: true);
//        GC.SuppressFinalize(this);
//    }

//    #endregion
//}

//file sealed class GrpcIpcServiceImpl(IpcServerService ipcServerService) : GrpcIpcService.GrpcIpcServiceBase
//{
//    readonly IpcServerService ipcServerService = ipcServerService;

//    [UnsafeAccessor(UnsafeAccessorKind.Constructor)]
//    static extern PB::ByteString ToByteString(ReadOnlyMemory<byte> bytes);

//    [MethodImpl(MethodImplOptions.AggressiveInlining)]
//    async Task WriteResponseAsync(IServerStreamWriter<GrpcIpcServiceResponseModel> responseStream, byte[] data)
//    {
//        GrpcIpcServiceResponseModel response = new()
//        {
//            Data = ToByteString(data),
//        };
//        await responseStream.WriteAsync(response);
//    }

//    [MethodImpl(MethodImplOptions.AggressiveInlining)]
//    async Task WriteResponseAsync(IServerStreamWriter<GrpcIpcServiceResponseModel> responseStream, int code)
//    {
//        GrpcIpcServiceResponseModel response = new()
//        {
//            Code = code,
//        };
//        await responseStream.WriteAsync(response);
//    }

//    [MethodImpl(MethodImplOptions.AggressiveInlining)]
//    bool CheckAuthorization(GrpcIpcServiceRequestModel request)
//    {
//        var accessToken = ipcServerService.AccessToken;
//        if (accessToken != null && accessToken.Length != 0)
//        {
//            if (request.Authorization == null ||
//                request.Authorization.Length == 0 ||
//                !accessToken.SequenceEqual(request.Authorization))
//            {
//                return false;
//            }
//        }
//        return true;
//    }

//    public sealed override async Task Stream(
//        IAsyncStreamReader<GrpcIpcServiceRequestModel> requestStream,
//        IServerStreamWriter<GrpcIpcServiceResponseModel> responseStream,
//        ServerCallContext context)
//    {
//        await foreach (var request in requestStream.ReadAllAsync())
//        {
//            if (!CheckAuthorization(request))
//            {
//                await WriteResponseAsync(responseStream, (int)HttpStatusCode.Unauthorized);
//                continue;
//            }

//            using var stream = new MemoryStream();
//            await IpcServerService.OnHandleStream(request.Names, request.Data, stream, context.CancellationToken);
//            await WriteResponseAsync(responseStream, stream.ToArray());
//        }
//    }

//    public sealed override async Task AsyncEnumerable(
//        GrpcIpcServiceRequestModel request,
//        IServerStreamWriter<GrpcIpcServiceResponseModel> responseStream,
//        ServerCallContext context)
//    {
//        if (!CheckAuthorization(request))
//        {
//            await WriteResponseAsync(responseStream, (int)HttpStatusCode.Unauthorized);
//            return;
//        }

//        var items = IpcServerService.OnHandleAsyncEnumerable(request.Names, request.Data, context.CancellationToken);
//        if (items == null)
//            return;
//        await foreach (var item in items)
//        {
//            await WriteResponseAsync(responseStream, item);
//        }
//    }

//    public sealed override Task BidirectionalStream(
//        IAsyncStreamReader<GrpcIpcServiceRequestModel> requestStream,
//        IServerStreamWriter<GrpcIpcServiceResponseModel> responseStream,
//        ServerCallContext context)
//    {
//        return base.BidirectionalStream(requestStream, responseStream, context);
//    }
//}
// https://learn.microsoft.com/zh-cn/aspnet/core/fundamentals/native-aot?view=aspnetcore-8.0#the-web-api-native-aot-template
// https://learn.microsoft.com/zh-cn/aspnet/core/fundamentals/minimal-apis/openapi?view=aspnetcore-8.0
// Ipc 服务端实验样品
// ssl 证书
// https 端口号使用 5076 被占用时使用随机端口

using BD.Common8.Ipc.Server.Services;
using Ipc.Sample;
using Ipc.Server.Sample.Experimental.Hubs;
using Microsoft.AspNetCore.SignalR;

namespace BD.Common8.Ipc.Server.Sample.Experimental;

#pragma warning disable SA1600 // Elements should be documented

public static partial class Program
{
    private const string X500DistinguishedCNName = "BeyondDimension Console Test Certificate";

    private const string X500DistinguishedName =
        $"C=CN, O=BeyondDimension, OU=Technical Department, CN={X500DistinguishedCNName}";

    public static void Main()
    {
        var builder = WebApplication.CreateEmptyBuilder(new WebApplicationOptions());
        builder.Services.AddRoutingCore();
        builder.WebHost.UseKestrelCore();

        builder.Services.AddLogging(l => l.AddConsole());

        builder.Services.ConfigureHttpJsonOptions(options =>
        {
            options.SerializerOptions.TypeInfoResolverChain.Insert(0, AppJsonSerializerContext.Default);
        });

        using var rootCertificate = X509Certificate2Generator.CreateRootCertificate(new()
        {
            DistinguishedNameString = X500DistinguishedName,
            SubjectAlternativeName = X500DistinguishedCNName,
        });
        using var serverCertificate = X509Certificate2Generator.CreateServerCertificate(new()
        {
            RootCertificate = rootCertificate,
            DistinguishedNameString = "CN=localhost",
            SubjectAlternativeName = "localhost",
            ExtraDnsNameOrIpAddresses = [
                X509Certificate2Generator.DnsNameOrIpAddress.GetMachineName(),
                IPAddress.Loopback,
                IPAddress.IPv6Loopback,
            ],
        });

        builder.WebHost.ConfigureKestrel(options =>
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
            int port = 5076;
            if (IsUsePort(IPAddress.Loopback, port))
                port = GetRandomUnusedPort(IPAddress.Loopback);
            Console.WriteLine($"https://localhost:{port}/todos");
            options.ListenLocalhost(port, listenOptions =>
            {
                listenOptions.Protocols = HttpProtocols.Http1AndHttp2;
                listenOptions.UseHttps(serverCertificate);
            });
            const string pipeName = "BD.Common8.Ipc.Server.Sample.Experimental";
            options.ListenNamedPipe(pipeName, listenOptions =>
            {
                listenOptions.Protocols = HttpProtocols.Http2;
                listenOptions.UseHttps(serverCertificate);
            });
        });

        //注册 SignalR 相关服务
        builder.Services.AddSignalR(opt =>
        {
            /*
             * // 客户端超时时间 默认30s(建议的值为 KeepAliveInterval 值的两倍)
             * opt.ClientTimeoutInterval = TimeSpan.FromSeconds(30);
             * // 如果客户端在此时间间隔内未发送初始握手消息，则连接将关闭。
             * // 这是一个高级设置，只应在由于严重的网络延迟而发生握手超时错误时才考虑修改。
             * // 有关握手过程的更多详细信息，请参阅
             * // https://github.com/aspnet/SignalR/blob/master/specs/HubProtocol.md
             * opt.HandshakeTimeout = TimeSpan.FromSeconds(15);
             * // 如果服务器在此间隔内未发送消息，将自动发送 ping 消息以保持连接处于开启状态
             * opt.KeepAliveInterval = TimeSpan.FromSeconds(15);
             *
             */
#if DEBUG
            opt.EnableDetailedErrors = true;
#endif
        });

        builder.Services.AddSingleton<ITodoService, TodoServiceImpl>();

        var app = builder.Build();

        // SignalR hub 注册
        app.MapHub<TestHub>("/test", opt =>
        {
            //opt.Transports = Microsoft.AspNetCore.Http.Connections.HttpTransportType.WebSockets;
        });

        OnMapGroup<TodoServiceImpl>(app);

        app.Run();
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    private static void OnMapGroup<T>(IEndpointRouteBuilder endpoints) where T : IEndpointRouteMapGroup
    {
        T.OnMapGroup(endpoints);
    }
}

internal sealed class TodoServiceImpl : ITodoService, IEndpointRouteMapGroup
{
    private readonly ITodoService.Todo[] todos = [
        new(1, "Walk the dog"),
        new(2, "Do the dishes", DateOnly.FromDateTime(DateTime.Now)),
        new(3, "Do the laundry", DateOnly.FromDateTime(DateTime.Now.AddDays(1))),
        new(4, "Clean the bathroom"),
        new(5, "Clean the car", DateOnly.FromDateTime(DateTime.Now.AddDays(2))),
    ];

    public async Task<ApiRspImpl<ITodoService.Todo[]?>> All()
    {
        await Task.Delay(1);
        return todos.Concat(new[] {
            new ITodoService.Todo(6, DateTimeOffset.Now.ToString()),
        }).ToArray();
    }

    public async Task<ApiRspImpl<ITodoService.Todo?>> GetById(int id)
    {
        await Task.Delay(1);
        return todos.FirstOrDefault(x => x.Id == id);
    }

    /// <inheritdoc cref="IEndpointRouteMapGroup.OnMapGroup(IEndpointRouteBuilder)"/>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    static void IEndpointRouteMapGroup.OnMapGroup(IEndpointRouteBuilder endpoints)
    {
        var builder = endpoints.MapGroup("/ITodoService");
        builder.MapPost("/All", ([FromServices] ITodoService s) => s.All());

        // 通过路由访问 连接到 TestHub的 Client 并发送消息
        builder.MapGet("/TestHub", async ([FromServices] IHubContext<TestHub> c) =>
        {
            await c.Clients.All.SendAsync("ServerReceivedMsg", "接收路由请求消息");
            return "hello test hub";
        });

        // 暂时注释掉这些路由 (格式不正确,有FromRoute特性但是 route 中并不包含这些参数会导致启动异常
        //builder.MapPost("/GetById/{id}", ([FromServices] ITodoService s, [FromRoute] int id) => s.GetById(id));
        //builder.MapPost("/SimpleTypes", ([FromServices] ITodoService s, [FromRoute] bool p0, [FromRoute] byte p1, [FromRoute] sbyte p2, [FromRoute] char p3, [FromRoute] DateOnly p4, [FromRoute] DateTime p5, [FromRoute] DateTimeOffset p6, [FromRoute] decimal p7, [FromRoute] double p8, [FromRoute] System.Reflection.ProcessorArchitecture p9, [FromRoute] Guid p10, [FromRoute] short p11, [FromRoute] int p12, [FromRoute] long p13, [FromRoute] float p14, [FromRoute] TimeOnly p15, [FromRoute] TimeSpan p16, [FromRoute] ushort p17, [FromRoute] uint p18, [FromRoute] ulong p19, [FromRoute] Uri p20, [FromRoute] Version p21) => s.SimpleTypes(p0, p1, p2, p3, p4, p5, p6, p7, p8, p9, p10, p11, p12, p13, p14, p15, p16, p17, p18, p19, p20, p21));
        //builder.MapPost("/BodyTest", ([FromServices] ITodoService s, [FromBody] ITodoService.Todo todo) => s.BodyTest(todo));
    }

    public Task<ApiRspImpl> SimpleTypes(bool p0, byte p1, sbyte p2, char p3, DateOnly p4, DateTime p5, DateTimeOffset p6, decimal p7, double p8, ProcessorArchitecture p9, Guid p10, short p11, int p12, long p13, float p14, TimeOnly p15, TimeSpan p16, ushort p17, uint p18, ulong p19, Uri p20, Version p21)
    {
        return Task.FromResult(ApiRspHelper.Ok());
    }
}

[JsonSerializable(typeof(ApiRspImpl<ITodoService.Todo[]>))]
[JsonSerializable(typeof(ApiRspImpl<ITodoService.Todo>))]
[JsonSerializable(typeof(ApiRspImpl))]
internal partial class AppJsonSerializerContext : JsonSerializerContext
{
}
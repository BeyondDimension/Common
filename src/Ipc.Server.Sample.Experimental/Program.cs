// https://learn.microsoft.com/zh-cn/aspnet/core/fundamentals/native-aot?view=aspnetcore-8.0#the-web-api-native-aot-template
// https://learn.microsoft.com/zh-cn/aspnet/core/fundamentals/minimal-apis/openapi?view=aspnetcore-8.0
// Ipc 服务端实验样品
// ssl 证书
// https 端口号使用 5076 被占用时使用随机端口

using BD.Common8.Ipc.Server.Services;
using Ipc.Sample;

namespace BD.Common8.Ipc.Server.Sample.Experimental;

#pragma warning disable SA1600 // Elements should be documented

public static partial class Program
{
    const string X500DistinguishedCNName = "BeyondDimension Console Test Certificate";
    const string X500DistinguishedName =
        $"C=CN, O=BeyondDimension, OU=Technical Department, CN={X500DistinguishedCNName}";

    public static void Main()
    {
        var builder = WebApplication.CreateEmptyBuilder(new WebApplicationOptions());
        builder.Services.AddRoutingCore();
        builder.WebHost.UseKestrelCore();

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
                listenOptions.Protocols = HttpProtocols.Http2;
                listenOptions.UseHttps(serverCertificate);
            });
            const string pipeName = "BD.Common8.Ipc.Server.Sample.Experimental";
            options.ListenNamedPipe(pipeName, listenOptions =>
            {
                listenOptions.Protocols = HttpProtocols.Http2;
                listenOptions.UseHttps(serverCertificate);
            });
        });

        builder.Services.AddSingleton<ITodoService, TodoServiceImpl>();

        var app = builder.Build();

        OnMapGroup<TodoServiceImpl>(app);

        app.Run();
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    static void OnMapGroup<T>(IEndpointRouteBuilder endpoints) where T : IEndpointRouteMapGroup
    {
        T.OnMapGroup(endpoints);
    }
}

sealed class TodoServiceImpl : ITodoService, IEndpointRouteMapGroup
{
    readonly ITodoService.Todo[] todos = [
        new(1, "Walk the dog"),
        new(2, "Do the dishes", DateOnly.FromDateTime(DateTime.Now)),
        new(3, "Do the laundry", DateOnly.FromDateTime(DateTime.Now.AddDays(1))),
        new(4, "Clean the bathroom"),
        new(5, "Clean the car", DateOnly.FromDateTime(DateTime.Now.AddDays(2))),
    ];

    public async Task<ApiRspImpl<ITodoService.Todo[]?>> All()
    {
        await Task.Delay(1);
        return todos;
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
        var todosApi = endpoints.MapGroup("/todos");
        todosApi.MapPost("/", ([FromServices] ITodoService todoService) => todoService.All());
        todosApi.MapPost("/{id}", ([FromServices] ITodoService todoService, [FromRoute] int id) =>
            todoService.GetById(id));
    }

    public Task<ApiRspImpl> SimpleTypes(bool p0, byte p1, sbyte p2, char p3, DateOnly p4, DateTime p5, DateTimeOffset p6, decimal p7, double p8, ProcessorArchitecture p9, Guid p10, short p11, int p12, long p13, float p14, TimeOnly p15, TimeSpan p16, ushort p17, uint p18, ulong p19, Uri p20, Version p21)
    {
        return Task.FromResult(ApiRspHelper.Ok());
    }
}

[JsonSerializable(typeof(ApiRspImpl<ITodoService.Todo[]>))]
[JsonSerializable(typeof(ApiRspImpl<ITodoService.Todo>))]
internal partial class AppJsonSerializerContext : JsonSerializerContext
{
}
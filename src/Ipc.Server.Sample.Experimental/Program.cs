// https://learn.microsoft.com/zh-cn/aspnet/core/fundamentals/native-aot?view=aspnetcore-8.0#the-web-api-native-aot-template
// https://learn.microsoft.com/zh-cn/aspnet/core/fundamentals/minimal-apis/openapi?view=aspnetcore-8.0
// Ipc 服务端实验样品
// ssl 证书
// https 端口号使用 5076 被占用时使用随机端口

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

        var app = builder.Build();

        var sampleTodos = new Todo[] {
            new(1, "Walk the dog"),
            new(2, "Do the dishes", DateOnly.FromDateTime(DateTime.Now)),
            new(3, "Do the laundry", DateOnly.FromDateTime(DateTime.Now.AddDays(1))),
            new(4, "Clean the bathroom"),
            new(5, "Clean the car", DateOnly.FromDateTime(DateTime.Now.AddDays(2))),
        };

        var todosApi = app.MapGroup("/todos");
        todosApi.MapGet("/", () => sampleTodos);
        todosApi.MapGet("/{id}", (int id) =>
            sampleTodos.FirstOrDefault(a => a.Id == id) is { } todo
                ? Results.Ok(todo)
                : Results.NotFound());

        app.Run();
    }
}

public record Todo(int Id, string? Title, DateOnly? DueBy = null, bool IsComplete = false);

[JsonSerializable(typeof(Todo[]))]
internal partial class AppJsonSerializerContext : JsonSerializerContext
{
}
using Microsoft.OpenApi.Models;

namespace Ipc.Server.Sample.Experimental;

#pragma warning disable SA1600 // Elements should be documented

/// <summary>
/// Ipc 服务端示例
/// </summary>
static partial class Program
{
    static readonly TaskCompletionSource tcs = new();

    static void TestSimpleTypes()
    {
        var paras = typeof(ITodoService).GetMethod(nameof(ITodoService.SimpleTypes))!.GetParameters();
        Dictionary<string, object?> dict = new();
        foreach (var item in paras)
        {
            if (item.ParameterType == typeof(CancellationToken))
                continue;
            dict.Add(item.ParameterType.Name, GeneratorRandomValueByType(item.ParameterType));
        }
        var result = Serializable.SJSON(dict, writeIndented: true);
        Console.WriteLine(result);
    }

    internal static IpcServerService2 IpcServerService = null!;

    static async Task<int> Main()
    {
        Console.WriteLine("Ipc 服务端示例【已启动】");
        TestSimpleTypes();
        try
        {
            Ioc.ConfigureServices(static s =>
            {
                s.AddSingletonWithIpcServer<ITodoService, TodoServiceImpl>();
            });

            IpcServerService = new IpcServerService2(SamplePathHelper.ServerCertificate);
            await IpcServerService.RunAsync();

            var connectionStrings = Enum.GetValues<IpcAppConnectionStringType>()
                .Select(x => IpcServerService.GetConnectionString(x)).ToArray();
            SamplePathHelper.SetConnectionStrings(connectionStrings);
            Console.WriteLine($"已写入连接字符串，路径：{SamplePathHelper.ConnectionStringsFilePath}");

            Console.WriteLine("【前】模拟 UI 线程阻塞");
            Console.CancelKeyPress += static (_, _) =>
            {
                tcs.SetResult();
            };
            await tcs.Task;
            Console.WriteLine("【后】模拟 UI 线程阻塞");
        }
        finally
        {
            Console.WriteLine("正在释放资源");
            SamplePathHelper.Dispose();
        }

        return (int)ExitCode.Ok;
    }

    /// <summary>
    /// 定义服务端进程退出码
    /// </summary>
    enum ExitCode
    {
        Ok = 0,
    }
}

sealed class IpcServerService2(X509Certificate2 serverCertificate) : IpcServerService(serverCertificate)
{
    protected override bool ListenLocalhost => true;

    protected override bool ListenNamedPipe => true;

    protected override bool ListenUnixSocket => true;

    static readonly Lazy<SystemTextJsonSerializerOptions> _JsonSerializerOptions = new(SampleJsonSerializerContext.Default.Options.AddDefaultJsonTypeInfoResolver);

    protected override SystemTextJsonSerializerOptions JsonSerializerOptions
        => _JsonSerializerOptions.Value;

    static bool UseSwagger => true;

    public override IHubContext HubContext => GetHubContextByHubUrl()!;

    protected override void ConfigureServices(IServiceCollection services)
    {
        base.ConfigureServices(services);

        if (UseSwagger)
        {
            services.AddEndpointsApiExplorer();
            services.AddSwaggerGen(options =>
            {
                var scheme = new OpenApiSecurityScheme
                {
                    Description = $"{IpcAppConnectionString.AuthenticationScheme} {GetAccessToken().ToHexString()}",
                    Name = "Authorization",
                    In = ParameterLocation.Header,
                    Type = SecuritySchemeType.ApiKey,
                    Scheme = IpcAppConnectionString.AuthenticationScheme,
                };
                options.AddSecurityDefinition(IpcAppConnectionString.AuthenticationScheme, scheme);
                options.AddSecurityRequirement(new OpenApiSecurityRequirement
                {
                    {
                        new OpenApiSecurityScheme
                        {
                            Reference = new OpenApiReference {
                                Type = ReferenceType.SecurityScheme,
                                Id = IpcAppConnectionString.AuthenticationScheme,
                            },
                        },
                        Array.Empty<string>()
                    },
                });
            });
        }
    }

    protected override void Configure(WebApplication app)
    {
        base.Configure(app);
        MapHub<IpcHub>(HubUrl);
        MapHub<IpcHub2>(IpcHub2.HubUrl);

        if (UseSwagger)
        {
            app.UseSwagger();
            app.UseSwaggerUI();
        }

        // 测试元组 Tuple，System.Text.Json 不支持 ValueTuple
        //var a = System.TupleExtensions.ToTuple((1, 2, 3));
        app.MapGroup("/Test")
            .MapPost("Tuple",
                (Delegate)(([FromBody] Tuple<int, string, string, string, string, string, string, Tuple<string, string, string, string, string, string, string, Tuple<string, string, string, string, string, string, string>>> body) => body));
    }
}

[ServiceContractImpl(typeof(ITodoService), IpcGeneratorType.Server)]
sealed partial class TodoServiceImpl : ITodoService
{
    static void OnMapGroup2(IEndpointRouteBuilder endpoints)
    {
        var builder = endpoints.MapGroup("/ITodoService").RequireAuthorization();
        builder.MapGet("/All", (Delegate)(static async (HttpContext ctx) =>
        {
            ApiRspImpl<Todo[]?> result;
            try
            {
                result = await Ioc.Get<ITodoService>().All(ctx.RequestAborted);
            }
            catch (Exception ex)
            {
                result = ex;
            }
            return result;
        }));
        builder.MapGet("/GetById/{id}", (Delegate)(static async (HttpContext ctx, [FromRoute] int id)
            => await Ioc.Get<ITodoService>().GetById(id, ctx.RequestAborted)));
        builder.MapGet("/SimpleTypes/{p0}/{p1}/{p2}/{p3}/{p4}/{p5}/{p6}/{p7}/{p8}/{p9}/{p10}/{p11}/{p12}/{p13}/{p14}/{p15}/{p16}/{p17}/{p18}/{p19}/{p20}/{p21}", (Delegate)(static async (HttpContext ctx, [FromRoute] bool p0, [FromRoute] byte p1, [FromRoute] sbyte p2, [FromRoute] char p3, [FromRoute] DateOnly p4, [FromRoute] DateTime p5, [FromRoute] DateTimeOffset p6, [FromRoute] decimal p7, [FromRoute] double p8, [FromRoute] ProcessorArchitecture p9, [FromRoute] Guid p10, [FromRoute] short p11, [FromRoute] int p12, [FromRoute] long p13, [FromRoute] float p14, [FromRoute] TimeOnly p15, [FromRoute] TimeSpan p16, [FromRoute] ushort p17, [FromRoute] uint p18, [FromRoute] ulong p19, [FromRoute] Uri p20, [FromRoute] Version p21)
            => await Ioc.Get<ITodoService>().SimpleTypes(p0, p1, p2, p3, p4, p5, p6, p7, p8, p9, p10, p11, p12, p13, p14, p15, p16, p17, p18, p19, p20, p21, ctx.RequestAborted)));
        builder.MapPost("/BodyTest", (Delegate)(static async (HttpContext ctx, [FromBody] Todo? todo)
            => await Ioc.Get<ITodoService>().BodyTest(todo, ctx.RequestAborted)));
        builder.MapGet("/AsyncEnumerable/{len}", (Delegate)(static (HttpContext ctx, [FromRoute] int len)
            => Ioc.Get<ITodoService>().AsyncEnumerable(len, ctx.RequestAborted)));
        builder.MapPost("/Tuple", (Delegate)(static async (HttpContext ctx, [FromBody] Tuple<bool, byte, sbyte, char, DateOnly, DateTime, DateTimeOffset, Tuple<decimal, double, ProcessorArchitecture[], Guid, short, int, long, Tuple<float, TimeOnly, TimeSpan, ushort, uint, ulong[], Uri>>> body)
            => await Ioc.Get<ITodoService>().Tuple(body.Item1, body.Item2, body.Item3, body.Item4, body.Item5, body.Item6, body.Item7, body.Rest.Item1, body.Rest.Item2, body.Rest.Item3, body.Rest.Item4, body.Rest.Item5, body.Rest.Item6, body.Rest.Item7, body.Rest.Rest.Item1, body.Rest.Rest.Item2, body.Rest.Rest.Item3, body.Rest.Rest.Item4, body.Rest.Rest.Item5, body.Rest.Rest.Item6, body.Rest.Rest.Item7, ctx.RequestAborted)));
        builder.MapGet("/Exception1", (Delegate)(static async (HttpContext ctx)
            => await Ioc.Get<ITodoService>().Exception1(ctx.RequestAborted)));
        builder.MapGet("/Exception2", (Delegate)(static (HttpContext ctx)
            => Ioc.Get<ITodoService>().Exception2(ctx.RequestAborted)));
        builder.MapGet("/Exception3", (Delegate)(static (HttpContext ctx)
            => Ioc.Get<ITodoService>().Exception3(ctx.RequestAborted)));
        //builder.MapGet("/Exception3", (Delegate)(static async (HttpContext ctx) =>
        //{
        //    var result = Ioc.Get<ITodoService>().Exception3(ctx.RequestAborted);
        //    await foreach (var item in result)
        //    {
        //        yield return item;
        //    }
        //    return result;
        //}));
    }

    readonly Todo[] todos = [
        new(1, "Walk the dog"),
        new(2, "Do the dishes", DateOnly.FromDateTime(DateTime.Now)),
        new(3, "Do the laundry", DateOnly.FromDateTime(DateTime.Now.AddDays(1))),
        new(4, "Clean the bathroom"),
        new(5, "Clean the car", DateOnly.FromDateTime(DateTime.Now.AddDays(2))),
    ];

    IHubContext HubContext
    {
        get
        {
            var hubContext = Program.IpcServerService.HubContext;
            return hubContext;
        }
    }

    IHubClients Clients
    {
        get
        {
            var hubContext = HubContext;
            return hubContext.Clients;
        }
    }

    ISingleClientProxy? Caller
    {
        get
        {
            var httpContextAccessor = Program.IpcServerService.Services.GetRequiredService<IHttpContextAccessor>();
            var connId = httpContextAccessor.HttpContext?.Connection.Id;
            if (connId != null)
            {
                return Clients.Client(connId);
            }
            return null;
        }
    }

    CancellationToken RequestAborted()
    {
        var httpContextAccessor = Program.IpcServerService.Services.GetRequiredService<IHttpContextAccessor>();
        var httpContext = httpContextAccessor.HttpContext;
        if (httpContext != null)
            return httpContext.RequestAborted;
        return default;
    }

    public async Task<ApiRspImpl<Todo[]?>> All(CancellationToken cancellationToken = default)
    {
        await Clients.All.SendAsync(nameof(ITodoService), nameof(All), RequestAborted());
        await Task.Delay(1, cancellationToken);
        var result = todos.Concat([
            new Todo(6, DateTimeOffset.Now.ToString()),
        ]).ToArray();
        return result;
    }

    public async Task<ApiRspImpl<Todo?>> GetById(int id, CancellationToken cancellationToken = default)
    {
        await Clients.All.SendAsync(nameof(ITodoService), nameof(GetById), RequestAborted());
        await Task.Delay(1, cancellationToken);
        return todos.FirstOrDefault(x => x.Id == id);
    }

    public async Task<ApiRspImpl> SimpleTypes(bool p0, byte p1, sbyte p2,
        char p3, DateOnly p4, DateTime p5,
        DateTimeOffset p6, decimal p7, double p8,
        ProcessorArchitecture p9, Guid p10, short p11,
        int p12, long p13, float p14,
        TimeOnly p15, TimeSpan p16, ushort p17,
        uint p18, ulong p19, Uri p20,
        Version p21, CancellationToken cancellationToken = default)
    {
        await Clients.All.SendAsync(nameof(ITodoService), nameof(SimpleTypes), RequestAborted());
        var result = ApiRspHelper.Ok();
        result.InternalMessage = $"{p0}/{p1}/{p2}/{p3}/{p4}/{p5}/{p6}/{p7}/{p8}/{p9}/{p10}/{p11}/{p12}/{p13}/{p14}/{p15}/{p16}/{p17}/{p18}/{p19}/{p20}/{p21}";
        return result;
    }

    public async Task<ApiRspImpl> BodyTest(Todo? todo, CancellationToken cancellationToken = default)
    {
        Console.WriteLine("BodyTest");
        await Clients.All.SendAsync(nameof(ITodoService), nameof(SimpleTypes), RequestAborted());
        var result = ApiRspHelper.Ok();
        result.InternalMessage = todo == null ? "todo is null!" : todo.Title;
        var a = Serializable.SMP2(result);
        Console.WriteLine("BodyTest result: " + a.ToHexString());
        return result;
    }

    public async IAsyncEnumerable<Todo> AsyncEnumerable(int len, [EnumeratorCancellation] CancellationToken cancellationToken = default)
    {
        await Clients.All.SendAsync(nameof(ITodoService), nameof(AsyncEnumerable), RequestAborted());
        for (int i = 0; i < len; i++)
        {
            var millisecondsDelay = Random.Shared.Next(199, 419);
            Console.WriteLine($"异步迭代器[{i}]，随机等待毫秒：{millisecondsDelay}");
            await Task.Delay(millisecondsDelay, cancellationToken); // 模拟循环中耗时操作
            if (i < todos.Length)
            {
                yield return todos[i];
            }
            yield return todos[^1]; // 超出长度返回最后一个
        }
    }

    public async Task<ApiRspImpl> Tuple(bool p0, byte p1, sbyte p2,
        char p3, DateOnly p4, DateTime p5,
        DateTimeOffset p6, decimal p7, double p8,
        ProcessorArchitecture[] p9, Guid p10, short p11,
        int p12, long p13, float p14,
        TimeOnly p15, TimeSpan p16, ushort p17,
        uint p18, ulong[] p19, Uri p20, CancellationToken cancellationToken = default)
    {
        await Clients.All.SendAsync(nameof(ITodoService), nameof(SimpleTypes), RequestAborted());
        var result = ApiRspHelper.Ok();
        result.InternalMessage = $"{p0}/{p1}/{p2}/{p3}/{p4}/{p5}/{p6}/{p7}/{p8}/{string.Join(", ", p9 ?? [])}/{p10}/{p11}/{p12}/{p13}/{p14}/{p15}/{p16}/{p17}/{p18}/{string.Join(", ", p19 ?? [])}/{p20}";
        return result;
    }

    public Task<ApiRspImpl> Exception1(CancellationToken cancellationToken = default)
    {
        throw new ApplicationException("test by Exception1.");
    }

    public IAsyncEnumerable<Todo> Exception2(CancellationToken cancellationToken = default)
    {
        throw new ApplicationException("test by Exception2.");
    }

    public async IAsyncEnumerable<ApiRspImpl<Todo>> Exception3(CancellationToken cancellationToken = default)
    {
        await Task.Delay(31, cancellationToken);
        ApiRspImpl<Todo> result;
        try
        {
            throw new ApplicationException("test by Exception3.");
        }
        catch (Exception ex)
        {
            result = ex;
        }
        yield return result;
    }
}

[Authorize]
sealed class IpcHub : BD.Common8.SourceGenerator.Ipc.Server.IpcHub
{
    public async Task<ApiRspImpl<Todo[]?>> ITodoService_All_1()
    {
        var result = await Ioc.Get<ITodoService>().All(this.RequestAborted());
        return result!;
    }

    //public async Task<ApiRspImpl<Todo[]?>> ITodoService_All_2()
    //{
    //    ApiRspImpl<Todo[]> result;
    //    try
    //    {
    //        result = await Ioc.Get<ITodoService>().All(this.RequestAborted());
    //    }
    //    catch (Exception ex)
    //    {
    //        result = ex;
    //    }
    //    return result;
    //}
}

[Authorize]
sealed class IpcHub2 : IpcHub2Base
{
    /// <summary>
    /// SignalR 的 HubUrl
    /// </summary>
    public const string HubUrl = "/Hubs/GameTools";
}

class IpcHub2Base : Hub
{
    public async IAsyncEnumerable<ApiRspImpl<NativeWindowModel?>> INativeWindowServices_GetMoveMouseDownWindow()
    {
        Console.WriteLine("IpcHub2Base");
        await Task.CompletedTask;
        ApiRspImpl<NativeWindowModel> result;
        try
        {
            result = new NativeWindowModel(2551, "title", "default", 1, default, default)
            {
            }!;
        }
        catch (Exception ex)
        {
            result = ex!;
        }
        yield return result!;
    }
}
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
            dict.Add(item.ParameterType.Name, SamplePathHelper.GetRandomValue(item.ParameterType));
        }
        var result = Serializable.SJSON(dict, writeIndented: true);
        Console.WriteLine(result);
    }

    static async Task<int> Main()
    {
        Console.WriteLine("Ipc 服务端示例【已启动】");
        TestSimpleTypes();
        try
        {
            Ioc.ConfigureServices(static s =>
            {
                s.AddSingletonWithIpc<ITodoService, TodoServiceImpl>();
            });

            var ipcServerService = new IpcServerService2(SamplePathHelper.ServerCertificate);
            await ipcServerService.RunAsync();

            var connectionStrings = Enum.GetValues<IpcAppConnectionStringType>()
                .Select(x => ipcServerService.GetConnectionString(x)).ToArray();
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

    protected override IJsonTypeInfoResolver? JsonTypeInfoResolver => SampleJsonSerializerContext.Default;

    protected override void Configure(WebApplication app)
    {
        base.Configure(app);
        MapHub<TodoServiceImpl_Hub>("/ITodoService_Hub");
    }
}

sealed partial class TodoServiceImpl : ITodoService
{
    readonly ITodoService.Todo[] todos = [
        new(1, "Walk the dog"),
        new(2, "Do the dishes", DateOnly.FromDateTime(DateTime.Now)),
        new(3, "Do the laundry", DateOnly.FromDateTime(DateTime.Now.AddDays(1))),
        new(4, "Clean the bathroom"),
        new(5, "Clean the car", DateOnly.FromDateTime(DateTime.Now.AddDays(2))),
    ];

    public async Task<ApiRspImpl<ITodoService.Todo[]?>> All(CancellationToken cancellationToken = default)
    {
        await Task.Delay(1, cancellationToken);
        var result = todos.Concat([
            new ITodoService.Todo(6, DateTimeOffset.Now.ToString()),
        ]).ToArray();
        return result;
    }

    public async Task<ApiRspImpl<ITodoService.Todo?>> GetById(int id, CancellationToken cancellationToken = default)
    {
        await Task.Delay(1, cancellationToken);
        return todos.FirstOrDefault(x => x.Id == id);
    }

    public Task<ApiRspImpl> SimpleTypes(bool p0, byte p1, sbyte p2,
        char p3, DateOnly p4, DateTime p5,
        DateTimeOffset p6, decimal p7, double p8,
        ProcessorArchitecture p9, Guid p10, short p11,
        int p12, long p13, float p14,
        TimeOnly p15, TimeSpan p16, ushort p17,
        uint p18, ulong p19, Uri p20,
        Version p21, CancellationToken cancellationToken = default)
    {
        var result = ApiRspHelper.Ok();
        result.InternalMessage = $"{p0}/{p1}/{p2}/{p3}/{p4}/{p5}/{p6}/{p7}/{p8}/{p9}/{p10}/{p11}/{p12}/{p13}/{p14}/{p15}/{p16}/{p17}/{p18}/{p19}/{p20}/{p21}";
        return Task.FromResult(result);
    }

    public Task<ApiRspImpl> BodyTest(ITodoService.Todo todo, CancellationToken cancellationToken = default)
    {
        var result = ApiRspHelper.Ok();
        result.InternalMessage = todo.Title;
        return Task.FromResult(result);
    }

    public async IAsyncEnumerable<ITodoService.Todo> AsyncEnumerable(int len, [EnumeratorCancellation] CancellationToken cancellationToken = default)
    {
        for (int i = 0; i < len; i++)
        {
            var millisecondsDelay = Random.Shared.Next(1, 199);
            Console.WriteLine($"异步迭代器[{i}]，随机等待毫秒：{millisecondsDelay}");
            await Task.Delay(millisecondsDelay, cancellationToken); // 模拟循环中耗时操作
            if (i < todos.Length)
            {
                yield return todos[i];
            }
            yield return todos[^1]; // 超出长度返回最后一个
        }
    }
}

#region 可使用源生成服务的调用实现

partial class TodoServiceImpl : IEndpointRouteMapGroup
{
    /// <inheritdoc cref="IEndpointRouteMapGroup.OnMapGroup(IEndpointRouteBuilder)"/>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    static void IEndpointRouteMapGroup.OnMapGroup(IEndpointRouteBuilder endpoints)
    {
#pragma warning disable IDE0004 // 删除不必要的强制转换
        var builder = endpoints.MapGroup("/ITodoService");
        // 测试 Get 方法
        builder.MapGet("/All", (Delegate)(static async (HttpContext ctx) => await Ioc.Get<ITodoService>().All(ctx.RequestAborted)));
        builder.MapPost("/All", (Delegate)(static async (HttpContext ctx) => await Ioc.Get<ITodoService>().All(ctx.RequestAborted)));
        // 测试 Get 方法，路由使用 string 类型
        builder.MapGet("/GetById/{id}", (Delegate)(static async (HttpContext ctx, [FromRoute] string id) => await Ioc.Get<ITodoService>().GetById(int.Parse(id), ctx.RequestAborted)));
        builder.MapPost("/GetById/{id}", (Delegate)(static async (HttpContext ctx, [FromRoute] int id) => await Ioc.Get<ITodoService>().GetById(id, ctx.RequestAborted)));
        builder.MapPost("/SimpleTypes/{p0}/{p1}/{p2}/{p3}/{p4}/{p5}/{p6}/{p7}/{p8}/{p9}/{p10}/{p11}/{p12}/{p13}/{p14}/{p15}/{p16}/{p17}/{p18}/{p19}/{p20}/{p21}",
            (Delegate)(static async (HttpContext ctx,
            [FromRoute] bool p0, [FromRoute] byte p1, [FromRoute] sbyte p2,
            [FromRoute] char p3, [FromRoute] DateOnly p4, [FromRoute] DateTime p5,
            [FromRoute] DateTimeOffset p6, [FromRoute] decimal p7, [FromRoute] double p8,
            [FromRoute] ProcessorArchitecture p9, [FromRoute] Guid p10, [FromRoute] short p11,
            [FromRoute] int p12, [FromRoute] long p13, [FromRoute] float p14,
            [FromRoute] TimeOnly p15, [FromRoute] TimeSpan p16, [FromRoute] ushort p17,
            [FromRoute] uint p18, [FromRoute] ulong p19, [FromRoute] Uri p20,
            [FromRoute] Version p21)
                => await Ioc.Get<ITodoService>().SimpleTypes(
                    p0, p1, p2,
                    p3, p4, p5,
                    p6, p7, p8,
                    p9, p10, p11,
                    p12, p13, p14,
                    p15, p16, p17,
                    p18, p19, p20,
                    p21,
                    ctx.RequestAborted)));
        builder.MapPost("/BodyTest", (Delegate)(static async (HttpContext ctx, [FromBody] ITodoService.Todo todo) => await Ioc.Get<ITodoService>().BodyTest(todo, ctx.RequestAborted)));
        builder.MapGet("/AsyncEnumerable/{len}", (Delegate)(static (HttpContext ctx, [FromRoute] string len) => Ioc.Get<ITodoService>().AsyncEnumerable(int.Parse(len), ctx.RequestAborted)));
        builder.MapPost("/AsyncEnumerable/{len}", (Delegate)(static (HttpContext ctx, [FromRoute] int len) => Ioc.Get<ITodoService>().AsyncEnumerable(len, ctx.RequestAborted)));
#pragma warning restore IDE0004 // 删除不必要的强制转换
    }
}

sealed class TodoServiceImpl_Hub : IpcServerHub
{
    public async Task<ApiRspImpl<ITodoService.Todo[]?>> All()
    {
        var result = await Ioc.Get<ITodoService>().All(RequestAborted);
        return result;
    }

    public async Task<ApiRspImpl<ITodoService.Todo?>> GetById(int id)
    {
        var result = await Ioc.Get<ITodoService>().GetById(id, RequestAborted);
        return result;
    }

    public async Task<ApiRspImpl> SimpleTypes(bool p0, byte p1, sbyte p2,
        char p3, DateOnly p4, DateTime p5,
        DateTimeOffset p6, decimal p7, double p8,
        ProcessorArchitecture p9, Guid p10, short p11,
        int p12, long p13, float p14,
        TimeOnly p15, TimeSpan p16, ushort p17,
        uint p18, ulong p19, Uri p20,
        Version p21)
    {
        var result = await Ioc.Get<ITodoService>().SimpleTypes(p0, p1, p2, p3, p4, p5, p6, p7, p8, p9, p10, p11, p12, p13, p14, p15, p16, p17, p18, p19, p20, p21, RequestAborted);
        return result;
    }

    public async Task<ApiRspImpl> BodyTest(ITodoService.Todo todo)
    {
        var result = await Ioc.Get<ITodoService>().BodyTest(todo, RequestAborted);
        return result;
    }

    public IAsyncEnumerable<ITodoService.Todo> AsyncEnumerable(int len)
    {
        var result = Ioc.Get<ITodoService>().AsyncEnumerable(len, RequestAborted);
        return result;
    }
}

#endregion
namespace Ipc.Server.Sample.Experimental;

#pragma warning disable SA1600 // Elements should be documented

/// <summary>
/// Ipc 服务端示例
/// </summary>
static partial class Program
{
    static readonly TaskCompletionSource tcs = new();

    static async Task Main()
    {
        Console.WriteLine("Ipc 服务端示例【已启动】");
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
    }
}

sealed class IpcServerService2(X509Certificate2 serverCertificate) : IpcServerService(serverCertificate)
{
    protected override bool ListenLocalhost => true;

    protected override bool ListenNamedPipe => true;

    protected override bool ListenUnixSocket => true;
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
        return todos.Concat([
            new ITodoService.Todo(6, DateTimeOffset.Now.ToString()),
        ]).ToArray();
    }

    public async Task<ApiRspImpl<ITodoService.Todo?>> GetById(int id, CancellationToken cancellationToken = default)
    {
        await Task.Delay(1, cancellationToken);
        return todos.FirstOrDefault(x => x.Id == id);
    }

    public Task<ApiRspImpl> SimpleTypes(bool p0, byte p1, sbyte p2, char p3, DateOnly p4, DateTime p5, DateTimeOffset p6, decimal p7, double p8, ProcessorArchitecture p9, Guid p10, short p11, int p12, long p13, float p14, TimeOnly p15, TimeSpan p16, ushort p17, uint p18, ulong p19, Uri p20, Version p21, CancellationToken cancellationToken = default)
    {
        var result = ApiRspHelper.Ok();
        result.InternalMessage = $"{p0}/{p1}/{p2}/{p3}/{p4},{p5},{p6}/{p7}/{p8}/{p9}/{p10}/{p11}/{p12}/{p13}/{p14}/{p15}/{p16}/{p17}/{p18}/{p19}/{p20}/{p21}";
        return Task.FromResult(result);
    }

    public Task<ApiRspImpl> SimpleTypes2(byte p1, sbyte p2, decimal p7, double p8, short p11, int p12, long p13, float p14, ushort p17, uint p18, ulong p19, CancellationToken cancellationToken = default)
    {
        var result = ApiRspHelper.Ok();
        result.InternalMessage = $"{p1}/{p2}/{p7}/{p8}/{p11}/{p12}/{p13}/{p14}/{p17}/{p18}/{p19}";
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
        foreach (var item in todos)
        {
            await Task.Delay(Random.Shared.Next(1, 1500), cancellationToken);
            yield return item;
        }
    }
}

partial class TodoServiceImpl : IEndpointRouteMapGroup
{
    /// <inheritdoc cref="IEndpointRouteMapGroup.OnMapGroup(IEndpointRouteBuilder)"/>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    static void IEndpointRouteMapGroup.OnMapGroup(IEndpointRouteBuilder endpoints)
    {
        var builder = endpoints.MapGroup("/ITodoService");
        builder.MapPost("/All", static async (HttpContext ctx) => await Ioc.Get<ITodoService>().All(ctx.RequestAborted));
        builder.MapPost("/GetById/{id}", static async (HttpContext ctx, [FromRoute] int id) => await Ioc.Get<ITodoService>().GetById(id, ctx.RequestAborted));
        builder.MapPost("/SimpleTypes/{p0}/{p1}/{p2}/{p3}/{p4}/{p5}/{p6}/{p7}/{p8}/{p9}/{p10}/{p11}/{p12}/{p13}/{p14}/{p15}/{p16}/{p17}/{p18}/{p19}/{p20}/{p21}", static async (HttpContext ctx, [FromRoute] bool p0, [FromRoute] byte p1, [FromRoute] sbyte p2, [FromRoute] char p3, [FromRoute] DateOnly p4, [FromRoute] DateTime p5, [FromRoute] DateTimeOffset p6, [FromRoute] decimal p7, [FromRoute] double p8, [FromRoute] ProcessorArchitecture p9, [FromRoute] Guid p10, [FromRoute] short p11, [FromRoute] int p12, [FromRoute] long p13, [FromRoute] float p14, [FromRoute] TimeOnly p15, [FromRoute] TimeSpan p16, [FromRoute] ushort p17, [FromRoute] uint p18, [FromRoute] ulong p19, [FromRoute] Uri p20, [FromRoute] Version p21) => await Ioc.Get<ITodoService>().SimpleTypes(p0, p1, p2, p3, p4, p5, p6, p7, p8, p9, p10, p11, p12, p13, p14, p15, p16, p17, p18, p19, p20, p21, ctx.RequestAborted));
        builder.MapPost("/SimpleTypes2/{p1}/{p2}/{p7}/{p8}/{p11}/{p12}/{p13}/{p14}/{p17}/{p18}/{p19}", static async (HttpContext ctx, [FromRoute] byte p1, [FromRoute] sbyte p2, [FromRoute] decimal p7, [FromRoute] double p8, [FromRoute] short p11, [FromRoute] int p12, [FromRoute] long p13, [FromRoute] float p14, [FromRoute] ushort p17, [FromRoute] uint p18, [FromRoute] ulong p19) => await Ioc.Get<ITodoService>().SimpleTypes2(p1, p2, p7, p8, p11, p12, p13, p14, p17, p18, p19, ctx.RequestAborted));
        builder.MapPost("/BodyTest", static async (HttpContext ctx, [FromBody] ITodoService.Todo todo) => await Ioc.Get<ITodoService>().BodyTest(todo, ctx.RequestAborted));
    }
}
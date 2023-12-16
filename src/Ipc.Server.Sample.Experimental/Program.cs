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

    internal static IpcServerService IpcServerService = null!;

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

    protected override void ConfigureServices(IServiceCollection services)
    {
        base.ConfigureServices(services);

        if (UseSwagger)
        {
            services.AddEndpointsApiExplorer();
            services.AddSwaggerGen();
        }
    }

    protected override void Configure(WebApplication app)
    {
        base.Configure(app);

        if (UseSwagger)
        {
            app.UseSwagger();
            app.UseSwaggerUI();
        }
    }
}

[ServiceContractImpl(typeof(ITodoService), IpcGeneratorType.Server)]
sealed partial class TodoServiceImpl : ITodoService
{
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
            var hubContext = Program.IpcServerService.GetHubContext<ITodoService>();
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

    public async Task<ApiRspImpl> BodyTest(Todo todo, CancellationToken cancellationToken = default)
    {
        await Clients.All.SendAsync(nameof(ITodoService), nameof(SimpleTypes), RequestAborted());
        var result = ApiRspHelper.Ok();
        result.InternalMessage = todo.Title;
        return result;
    }

    public async IAsyncEnumerable<Todo> AsyncEnumerable(int len, [EnumeratorCancellation] CancellationToken cancellationToken = default)
    {
        await Clients.All.SendAsync(nameof(ITodoService), nameof(AsyncEnumerable), RequestAborted());
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
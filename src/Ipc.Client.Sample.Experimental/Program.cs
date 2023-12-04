using Ipc.Sample;
using Microsoft.AspNetCore.SignalR.Client;

const string pipeName = "BD.Common8.Ipc.Server.Sample.Experimental";

var clientNamedPipe = IpcAppConnectionStringHelper.GetHttpClient(
    new IpcAppConnectionString
    {
        Type = IpcAppConnectionStringType.NamedPipe,
        StringValue = pipeName,
    });

var clientHttps = IpcAppConnectionStringHelper.GetHttpClient(
    new IpcAppConnectionString
    {
        Type = IpcAppConnectionStringType.Https,
        Int32Value = 5076,
    });

var hubConn = await GetSignalRHubConnection();
var hubStreamConn = await GetSignalRHubConnection("/streaming");

static async Task<string> GetStringAsync(HttpClient client)
{
    using var rsp = await client.PostAsync("/ITodoService/All", content: null);
    var str = await rsp.Content.ReadAsStringAsync();
    return str;
}

static async Task<HubConnection> GetSignalRHubConnection(string pattern = "/test")
{
    (string baseAddress, var httpMessageHandler) = CreateHandlerFunc();

    HubConnection conn = new HubConnectionBuilder()
       //.WithServerTimeout(TimeSpan.FromSeconds(1)) // 多久未接收到服务端消息断开连接(开启了自动重连会触发自动重连)
       // .WithUrl("https://localhost:5076/test")
       .WithUrl(baseAddress, opt =>
       {
           opt.Transports = Microsoft.AspNetCore.Http.Connections.HttpTransportType.WebSockets;
           //opt.SkipNegotiation = true;
           opt.HttpMessageHandlerFactory = (message) =>
           {
               message.Dispose();
               return CreateHandlerFunc().handler;
           };
           opt.WebSocketConfiguration = static o =>
           {
               o.HttpVersion = HttpVersion.Version20;
           };
           opt.Url = new Uri(baseAddress + pattern);
       })
       .WithAutomaticReconnect()
       .Build();

    conn.Reconnected += (s) =>
    {
        Console.WriteLine(@"已重连:" + DateTime.Now);
        return Task.CompletedTask;
    };

    conn.Reconnecting += (s) =>
    {
        Console.WriteLine(s);
        Console.WriteLine(@"正在重连:" + DateTime.Now);
        return Task.CompletedTask;
    };

    // 监听服务端消息
    conn.On<string>("ServerReceivedMsg", (serverMsg) =>
    {
        Console.WriteLine(@"ServerReceivedMsg => " + serverMsg);
    });

    await conn.StartAsync();

    return conn;
    // 创建自定义 handler
    static (string baseAddress, HttpMessageHandler handler) CreateHandlerFunc() =>
        IpcAppConnectionStringHelper.GetHttpMessageHandler(new IpcAppConnectionString
        {
            Type = IpcAppConnectionStringType.Https,
            Int32Value = 5076,
            //Type = IpcAppConnectionStringType.NamedPipe,
            //StringValue = pipeName,
        });
}

static async Task<string> GetSignalRStringAsync(HubConnection conn)
{
    string s = DateTime.Now.ToString(CultureInfo.InvariantCulture);

    var str = await conn.InvokeAsync<string>("ReturnClientResult", s);

    return str;
}

static async Task GetSignalRStringAsync2(HubConnection conn)
{
    await foreach (var pack in conn.StreamAsync<Pack>("AsyncEnumerable", 20))
    {
        await Console.Out.WriteLineAsync(pack.ToString());
    }
}

static async Task GetSignalRStringAsync3(HubConnection conn)
{
    var channel = await conn.StreamAsChannelAsync<Pack>("ChannelAsync", 99);

    await foreach (var pack in channel.ReadAllAsync())
    {
        await Console.Out.WriteLineAsync(pack.ToString());
    }
}

while (true)
{
    try
    {
        var resultNamedPipe = await GetStringAsync(clientNamedPipe);
        Console.WriteLine("resultNamedPipe: ");
        Console.WriteLine(resultNamedPipe);

        var resultHttps = await GetStringAsync(clientHttps);
        Console.WriteLine("resultHttps: ");
        Console.WriteLine(resultHttps);

        var resultSignalR = await GetSignalRStringAsync(hubConn);
        Console.WriteLine("resultSignalR: ");
        Console.WriteLine(resultSignalR);

        await GetSignalRStringAsync3(hubStreamConn);
    }
    catch (Exception ex)
    {
        Console.WriteLine(ex);
    }

    var line = Console.ReadLine();
    if ("exit".Equals(line, StringComparison.OrdinalIgnoreCase))
    {
        break;
    }
}

#pragma warning disable SA1600 // Elements should be documented

internal sealed class TodoServiceImpl(
    ILoggerFactory loggerFactory,
    IClientHttpClientFactory clientFactory) :
    WebApiClientBaseService(
        loggerFactory.CreateLogger(TAG),
        clientFactory,
        null),
    ITodoService
{
    private const string TAG = "Todo";

    protected override string ClientName => TAG;

    public async Task<ApiRspImpl<ITodoService.Todo[]?>> All()
    {
        var client = CreateClient();
        using var rsp = await client.PostAsync("/All", null);
        var r = await ReadFromAsync<ApiRspImpl<ITodoService.Todo[]?>>(rsp.Content);
        return r!;
    }

    public async Task<ApiRspImpl<ITodoService.Todo?>> GetById(int id)
    {
        var client = CreateClient();
        using var rsp = await client.PostAsync($"/GetById/{id}", null);
        var r = await ReadFromAsync<ApiRspImpl<ITodoService.Todo?>>(rsp.Content);
        return r!;
    }

    public Task<ApiRspImpl> SimpleTypes(bool p0, byte p1, sbyte p2, char p3, DateOnly p4, DateTime p5, DateTimeOffset p6, decimal p7, double p8, ProcessorArchitecture p9, Guid p10, short p11, int p12, long p13, float p14, TimeOnly p15, TimeSpan p16, ushort p17, uint p18, ulong p19, Uri p20, Version p21)
    {
        throw new NotImplementedException();
    }

    public async Task<ApiRspImpl> BodyTest(ITodoService.Todo todo)
    {
        var client = CreateClient();
        using var content = GetHttpContent(todo);
        using var rsp = await client.PostAsync("/BodyTest", content);
        var r = await ReadFromAsync<ApiRspImpl>(rsp.Content);
        return r!;
    }
}

/// <summary>
/// 测试包对象
/// </summary>
public class Pack
{
    /// <summary>
    /// 时间
    /// </summary>
    public DateTime DateTime { get; set; }

    /// <summary>
    ///  id
    /// </summary>
    public string PackId { get; set; } = Guid.NewGuid().ToString("N");

    /// <summary>
    /// 数据
    /// </summary>
    public object? Data { get; set; }

    public override string ToString()
    {
        return $"{DateTime.ToString("yyyy-MM-dd HH:mm:ss fff")}, packId:{PackId} ";
    }
}
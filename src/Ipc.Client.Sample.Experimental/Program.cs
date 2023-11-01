using Ipc.Sample;
using Microsoft.Extensions.Logging;
using System.Net.Http.Client;

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

static Task<string> GetStringAsync(HttpClient client) => client.GetStringAsync("/todos");

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

sealed class TodoServiceImpl(
    ILoggerFactory loggerFactory,
    IClientHttpClientFactory clientFactory) :
    WebApiClientBaseService(
        loggerFactory.CreateLogger(TAG),
        clientFactory,
        null),
    ITodoService
{
    const string TAG = "Todo";

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
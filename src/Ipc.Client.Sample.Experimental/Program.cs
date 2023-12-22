using DotNext.Reflection;

namespace Ipc.Client.Sample.Experimental;

#pragma warning disable SA1600 // Elements should be documented

/// <summary>
/// Ipc 客户端示例
/// </summary>
static partial class Program
{
    static async Task Main()
    {
        Ioc.ConfigureServices(static s =>
        {
            s.AddLogging(static l =>
            {
                l.AddConsole();
            });
        });

        IpcAppConnectionString[]? connectionStrings;
        while (true) // 循环直到服务端启动并写入连接字符串数据文件
        {
            connectionStrings = SamplePathHelper.GetConnectionStrings();
            if (connectionStrings != null && connectionStrings.Length != 0)
                break;

            Console.WriteLine("读取连接字符串失败，可能是服务端未启动，键入回车重试。");
            Console.ReadLine();
        }

        List<ITodoService> ipcClientServices = [];
        foreach (var connectionString in connectionStrings)
        {
            IpcClientService2 ipcClient = new(connectionString);

            // WebApi 实现的 Ipc 调用
            var ipcClientService = new TodoService_WebApi(ipcClient);
            ipcClientServices.Add(ipcClientService);

            // SignalR 实现的 Ipc 调用
            var ipcClientService2 = new TodoService_SignalR(ipcClient);
            ipcClientServices.Add(ipcClientService2);
        }

        var methods = (from m in typeof(ITodoService).GetMethods()
                       where m.ReturnType.IsGenericType && m.ReturnType.GetGenericTypeDefinition() == typeof(Task<>)
                       select m).ToArray();

        static object?[]? GetMethodParas(MethodInfo method)
        {
            var types = method.GetParameterTypes();
            if (types != null && types.Length != 0)
            {
                return GetMethodParas(types).ToArray();
                static IEnumerable<object?> GetMethodParas(IEnumerable<Type> types)
                {
                    foreach (var type in types)
                    {
                        yield return SamplePathHelper.GetRandomValue(type);
                    }
                }
            }
            return null;
        }

        static Task MethodInvoke(MethodInfo method, ITodoService todoService)
        {
            var paras = GetMethodParas(method);
            var result = method.Invoke(todoService, paras);
            return (Task)result!;
        }

        while (true) // 循环向服务端请求
        {
            foreach (var ipcClientService in ipcClientServices)
            {
                Console.WriteLine($"{((IIpcClientService2)ipcClientService).Title}: ");
                if (ipcClientService is ITodoService todoService)
                {
                    //var resultAll = await todoService.All();
                    //Console.WriteLine($"{nameof(ITodoService.All)}: ");
                    //Console.WriteLine(Serializable.SJSON_Original(resultAll, NewtonsoftJsonFormatting.Indented));

                    //var resultGetById = await todoService.GetById(Random.Shared.Next(int.MaxValue));
                    //Console.WriteLine($"{nameof(ITodoService.GetById)}: ");
                    //Console.WriteLine(Serializable.SJSON_Original(resultGetById, NewtonsoftJsonFormatting.Indented));

                    //var resultSimpleTypes = await todoService.SimpleTypes(true, 2, 3, '4', DateOnly.FromDateTime(DateTime.Today), DateTime.UtcNow, DateTimeOffset.Now, 7.1m, 8.2d, ProcessorArchitecture.MSIL, Guid.NewGuid(), 11, 12, long.MaxValue, 14.5f, TimeOnly.FromDateTime(DateTime.Now), TimeSpan.FromHours(3), 17, 18, 19, new Uri("http://github.com/BeyondDimension"), new Version(9, 12));
                    //Console.WriteLine($"{nameof(ITodoService.SimpleTypes)}: ");
                    //Console.WriteLine(Serializable.SJSON_Original(resultSimpleTypes, NewtonsoftJsonFormatting.Indented));

                    //var resultBodyTest = await todoService.BodyTest(new(9, Random2.GenerateRandomString(64)));
                    //Console.WriteLine($"{nameof(ITodoService.BodyTest)}: ");
                    //Console.WriteLine(Serializable.SJSON_Original(resultBodyTest, NewtonsoftJsonFormatting.Indented));

                    foreach (var method in methods)
                    {
                        Console.WriteLine($"ThreadId: {Environment.CurrentManagedThreadId}");

                        var resultMethod = MethodInvoke(method, todoService);
                        await resultMethod;
                        Console.WriteLine($"{method.Name}: ");
                        var resultValue = resultMethod.GetType().GetProperty(nameof(Task<object>.Result))?.GetValue(resultMethod);
                        Console.WriteLine(Serializable.SJSON_Original(resultValue, NewtonsoftJsonFormatting.Indented));
                    }

                    Console.WriteLine($"{nameof(ITodoService.AsyncEnumerable)}: ");
                    await foreach (var resultAsyncEnumerableItem in todoService.AsyncEnumerable(5))
                    {
                        Console.WriteLine(Serializable.SJSON_Original(resultAsyncEnumerableItem, NewtonsoftJsonFormatting.Indented));
                    }
                }
            }
            Console.WriteLine("键入回车再次发送请求。");
            Console.ReadLine();
        }
    }
}

interface IIpcClientService2
{
    string Title { get; }
}

sealed class IpcClientService2(IpcAppConnectionString connectionString) : IpcClientService(connectionString), IIpcClientService2
{
    public string Title => $"{connectionString.Type}_{GetType().Name}";

    public IpcAppConnectionStringType Type => connectionString.Type;

    /// <inheritdoc/>
    protected sealed override void ConfigureSocketsHttpHandler(SocketsHttpHandler handler)
    {
        handler.SslOptions.ClientCertificates ??= new();
        handler.SslOptions.ClientCertificates.Add(SamplePathHelper.ServerCertificate);
    }

    static readonly Lazy<SystemTextJsonSerializerOptions> _JsonSerializerOptions =
        new(SampleJsonSerializerContext.Default.Options.AddDefaultJsonTypeInfoResolver);

    /// <inheritdoc/>
    protected override SystemTextJsonSerializerOptions JsonSerializerOptions => _JsonSerializerOptions.Value;

    /// <inheritdoc/>
    protected sealed override bool EnableLogOnError => false;

    protected override void OnBuildHubConnection(HubConnection connection)
    {
        connection.On<string>(nameof(ITodoService), s =>
        {
            Console.WriteLine($"收到服务器消息：{s}, ThreadId: {Environment.CurrentManagedThreadId}");
        });
    }
}

[ServiceContractImpl(typeof(ITodoService), IpcGeneratorType.ClientWebApi)]
partial class TodoService_WebApi : IIpcClientService2
{
    public string Title => $"{((IpcClientService2)ipcClientService).Type}_{GetType().Name}";
}

[ServiceContractImpl(typeof(ITodoService), IpcGeneratorType.ClientSignalR)]
partial class TodoService_SignalR : IIpcClientService2
{
    public string Title => $"{((IpcClientService2)ipcClientService).Type}_{GetType().Name}";
}
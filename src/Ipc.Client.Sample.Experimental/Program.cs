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

        List<IIpcClientService2> ipcClientServices = [];
        foreach (var connectionString in connectionStrings)
        {
            IpcClientService2 ipcClientService = new TodoService_WebApi(connectionString);
            ipcClientServices.Add(ipcClientService);
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
                        if (type == typeof(int))
                        {
                            yield return Random.Shared.Next(int.MaxValue);
                        }
                        else if (type == typeof(ITodoService.Todo))
                        {
                            yield return new ITodoService.Todo(Random.Shared.Next(3, 9), Random2.GenerateRandomString(64));
                        }
                        else if (type == typeof(char))
                        {
                            yield return 'A';
                        }
                        else if (type == typeof(byte))
                        {
                            yield return (byte)25;
                        }
                        else if (type == typeof(sbyte))
                        {
                            yield return (sbyte)26;
                        }
                        else if (type == typeof(DateOnly))
                        {
                            yield return DateOnly.FromDateTime(DateTime.Today);
                        }
                        else if (type == typeof(DateTime))
                        {
                            yield return DateTime.UtcNow;
                        }
                        else if (type == typeof(DateTimeOffset))
                        {
                            yield return DateTimeOffset.Now;
                        }
                        else if (type == typeof(decimal))
                        {
                            yield return 27.1m;
                        }
                        else if (type == typeof(double))
                        {
                            yield return 27.1d;
                        }
                        else if (type == typeof(ProcessorArchitecture))
                        {
                            yield return ProcessorArchitecture.MSIL;
                        }
                        else if (type == typeof(Guid))
                        {
                            yield return Guid.NewGuid();
                        }
                        else if (type == typeof(short))
                        {
                            yield return (short)28;
                        }
                        else if (type == typeof(int))
                        {
                            yield return 29;
                        }
                        else if (type == typeof(long))
                        {
                            yield return 9223372036854775806L;
                        }
                        else if (type == typeof(float))
                        {
                            yield return 30.9f;
                        }
                        else if (type == typeof(TimeOnly))
                        {
                            yield return TimeOnly.FromDateTime(DateTime.Now);
                        }
                        else if (type == typeof(TimeSpan))
                        {
                            yield return TimeSpan.FromHours(3);
                        }
                        else if (type == typeof(ushort))
                        {
                            yield return (ushort)31;
                        }
                        else if (type == typeof(uint))
                        {
                            yield return 32u;
                        }
                        else if (type == typeof(ulong))
                        {
                            yield return 33ul;
                        }
                        else if (type == typeof(Uri))
                        {
                            yield return new Uri("http://github.com/BeyondDimension");
                        }
                        else if (type == typeof(Version))
                        {
                            yield return new Version("10.0.10240");
                        }
                        else if (type == typeof(CancellationToken))
                        {
                            yield return CancellationToken.None;
                        }
                        else
                        {
                            if (type.IsClass)
                            {
                                yield return null;
                            }
                            else
                            {
                                yield return Activator.CreateInstance(type);
                            }
                        }
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
                Console.WriteLine($"{ipcClientService.Title}: ");
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

                    //var resultSimple2Types = await todoService.SimpleTypes2(true, 2, 3, /*DateTime.UtcNow, DateTimeOffset.Now,*/ 7.1m, 8.2d, ProcessorArchitecture.MSIL, Guid.NewGuid(), 11, 12, long.MaxValue, 14.5f, TimeOnly.FromDateTime(DateTime.Now), TimeSpan.FromHours(3), 17, 18, 19, new Uri("http://github.com/BeyondDimension"), new Version(9, 12));
                    //Console.WriteLine($"{nameof(ITodoService.SimpleTypes2)}: ");
                    //Console.WriteLine(Serializable.SJSON_Original(resultSimple2Types, NewtonsoftJsonFormatting.Indented));

                    //var resultBodyTest = await todoService.BodyTest(new(9, Random2.GenerateRandomString(64)));
                    //Console.WriteLine($"{nameof(ITodoService.BodyTest)}: ");
                    //Console.WriteLine(Serializable.SJSON_Original(resultBodyTest, NewtonsoftJsonFormatting.Indented));

                    foreach (var method in methods)
                    {
                        var resultMethod = MethodInvoke(method, todoService);
                        await resultMethod;
                        Console.WriteLine($"{method.Name}: ");
                        var resultValue = resultMethod.GetType().GetProperty(nameof(Task<object>.Result))?.GetValue(resultMethod);
                        Console.WriteLine(Serializable.SJSON_Original(resultValue, NewtonsoftJsonFormatting.Indented));
                    }
                }
            }
            Console.WriteLine("键入回车再次发送请求。");
            Console.ReadLine();
        }
    }

    interface IIpcClientService2 : IIpcClientService
    {
        string Title { get; }
    }

    abstract class IpcClientService2(IpcAppConnectionString connectionString) : IpcClientService(connectionString), IIpcClientService2
    {
        public string Title => $"{connectionString.Type}_{GetType().Name}";

        /// <inheritdoc/>
        protected sealed override void ConfigureSocketsHttpHandler(SocketsHttpHandler handler)
        {
            handler.SslOptions.ClientCertificates ??= new();
            handler.SslOptions.ClientCertificates.Add(SamplePathHelper.ServerCertificate);
        }

        /// <inheritdoc/>
        protected sealed override SystemTextJsonSerializerContext? JsonSerializerContext
            => SampleJsonSerializerContext.Default;
    }

    #region 可使用源生成服务的调用实现

    sealed class TodoService_WebApi(IpcAppConnectionString connectionString) : IpcClientService2(connectionString), ITodoService
    {
        public async Task<ApiRspImpl<ITodoService.Todo[]?>> All(CancellationToken cancellationToken = default)
        {
            WebApiClientSendArgs args = new("/ITodoService/All")
            {
                Method = HttpMethod.Post,
            };
            var result = await SendAsync<ApiRspImpl<ITodoService.Todo[]?>>(args, cancellationToken);
            return result!;
        }

        public async Task<ApiRspImpl<ITodoService.Todo?>> GetById(int id, CancellationToken cancellationToken = default)
        {
            WebApiClientSendArgs args = new($"/ITodoService/GetById/{id}")
            {
                Method = HttpMethod.Post,
            };
            var result = await SendAsync<ApiRspImpl<ITodoService.Todo?>>(args, cancellationToken);
            return result!;
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
            WebApiClientSendArgs args = new($"/ITodoService/SimpleTypes/{WebUtility.UrlEncode(p0.ToString())}/{WebUtility.UrlEncode(p1.ToString())}/{WebUtility.UrlEncode(p2.ToString())}/{WebUtility.UrlEncode(p3.ToString())}/{WebUtility.UrlEncode(p4.ToString())}/{WebUtility.UrlEncode(p5.ToString())}/{WebUtility.UrlEncode(p6.ToString())}/{WebUtility.UrlEncode(p7.ToString())}/{WebUtility.UrlEncode(p8.ToString())}/{WebUtility.UrlEncode(((int)p9).ToString())}/{WebUtility.UrlEncode(p10.ToString())}/{WebUtility.UrlEncode(p11.ToString())}/{WebUtility.UrlEncode(p12.ToString())}/{WebUtility.UrlEncode(p13.ToString())}/{WebUtility.UrlEncode(p14.ToString())}/{WebUtility.UrlEncode(p15.ToString())}/{WebUtility.UrlEncode(p16.ToString())}/{WebUtility.UrlEncode(p17.ToString())}/{WebUtility.UrlEncode(p18.ToString())}/{WebUtility.UrlEncode(p19.ToString())}/{WebUtility.UrlEncode(p20.ToString())}/{WebUtility.UrlEncode(p21.ToString())}")
            {
                Method = HttpMethod.Post,
            };
            var result = await SendAsync<ApiRspImpl>(args, cancellationToken);
            return result!;
        }

        public async Task<ApiRspImpl> SimpleTypes2(bool p0, byte p1, sbyte p2,
            /*DateOnly p4,*/ /*DateTime p5,*/
            /*DateTimeOffset p6,*/ decimal p7, double p8,
            ProcessorArchitecture p9, Guid p10, short p11,
            int p12, long p13, float p14,
            TimeOnly p15, TimeSpan p16, ushort p17,
            uint p18, ulong p19, Uri p20,
            Version p21, CancellationToken cancellationToken = default)
        {
            WebApiClientSendArgs args = new($"/ITodoService/SimpleTypes2/{WebUtility.UrlEncode(p0.ToLowerString())}/{WebUtility.UrlEncode(p1.ToString())}/{WebUtility.UrlEncode(p2.ToString())}/{WebUtility.UrlEncode(p7.ToString())}/{WebUtility.UrlEncode(p8.ToString())}/{WebUtility.UrlEncode(p9.ToString()/*.ToLowerInvariant()*/)}/{WebUtility.UrlEncode(p10.ToString())}/{WebUtility.UrlEncode(p11.ToString())}/{WebUtility.UrlEncode(p12.ToString())}/{WebUtility.UrlEncode(p13.ToString())}/{WebUtility.UrlEncode(p14.ToString())}/{WebUtility.UrlEncode(p15.ToString())}/{WebUtility.UrlEncode(p16.ToString())}/{WebUtility.UrlEncode(p17.ToString())}/{WebUtility.UrlEncode(p18.ToString())}/{WebUtility.UrlEncode(p19.ToString())}/{WebUtility.UrlEncode(p20.ToString())}/{WebUtility.UrlEncode(p21.ToString())}")
            {
                Method = HttpMethod.Post,
            };
            var result = await SendAsync<ApiRspImpl>(args, cancellationToken);
            return result!;
        }

        public async Task<ApiRspImpl> BodyTest(ITodoService.Todo todo, CancellationToken cancellationToken = default)
        {
            WebApiClientSendArgs args = new("/ITodoService/BodyTest")
            {
                Method = HttpMethod.Post,
            };
            var result = await SendAsync<ApiRspImpl, ITodoService.Todo>(args, todo, cancellationToken);
            return result!;
        }

        public IAsyncEnumerable<ITodoService.Todo> AsyncEnumerable(int len, CancellationToken cancellationToken = default)
        {
            // https://learn.microsoft.com/zh-cn/dotnet/core/compatibility/serialization/6.0/iasyncenumerable-serialization
            // System.Text.Json 现支持 IAsyncEnumerable<T> 实例的序列化和反序列化。
            throw new NotImplementedException("TODO：改进 SerializableExtensions 支持 WebApi 的异步迭代器实现");
        }
    }

    //sealed class TodoService_SignalR(IIpcClientService ipcClientService)

    #endregion
}

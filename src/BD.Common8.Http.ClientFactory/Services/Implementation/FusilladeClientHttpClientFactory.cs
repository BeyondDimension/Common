using Splat;
using IClientHttpClientFactory = System.Net.Http.Client.IClientHttpClientFactory;

namespace BD.Common8.Http.ClientFactory.Services.Implementation;

/// <summary>
/// 使用 Fusillade 实现的 <see cref="IClientHttpClientFactory"/>
/// <para>https://github.com/dotnet/runtime/blob/v7.0.3/src/libraries/Microsoft.Extensions.Http/src/DefaultHttpClientFactory.cs</para>
/// </summary>
public class FusilladeClientHttpClientFactory : IClientHttpClientFactory, IDisposable
{
    bool disposedValue;

    readonly HttpMessageHandler handler;

    readonly ConcurrentDictionary<(string, HttpHandlerCategory), HttpClient> activeClients = new();

    /// <summary>
    /// 默认的 <see cref="HttpMessageHandler"/> 实例的字典
    /// /// </summary>
    internal static readonly Dictionary<string, DefaultHttpClientBuilder> Builders = [];

    /// <summary>
    /// 初始化 <see cref="FusilladeClientHttpClientFactory"/> 的实例
    /// </summary>
    /// <param name="registerConstant">是否将 <see cref="HttpMessageHandler"/> 注册到服务定位器</param>
    public FusilladeClientHttpClientFactory(bool registerConstant = true) : this(IClientHttpClientFactory.CreateHandler(), registerConstant)
    {
    }

    /// <summary>
    /// 初始化 <see cref="FusilladeClientHttpClientFactory"/> 的实例
    /// </summary>
    /// <param name="handler">自定义的 <see cref="HttpMessageHandler"/> 实例</param>
    /// <param name="registerConstant">是否将 <see cref="HttpMessageHandler"/> 注册到服务定位器</param>
    public FusilladeClientHttpClientFactory(HttpMessageHandler handler, bool registerConstant = true)
    {
        this.handler = handler;
        if (registerConstant)
            Locator.CurrentMutable.RegisterConstant(handler, typeof(HttpMessageHandler));
    }

    /// <summary>
    /// 创建一个 <see cref="HttpClient"/> 实例并设置默认超时时间
    /// </summary>
    protected virtual HttpClient CreateClient(HttpMessageHandler handler)
    {
        var client = IClientHttpClientFactory.CreateClient(handler);
        return client;
    }

    /// <summary>
    /// 创建或获取基于给定名称和分类的 HttpClient 实例
    /// </summary>
    HttpClient IClientHttpClientFactory.CreateClient(string name, HttpHandlerCategory category)
    {
        if (!category.IsDefined())
            category = HttpHandlerCategory.Default;

        ValueTuple<string, HttpHandlerCategory> key = new(name, category);

        if (activeClients.TryGetValue(key, out var result))
            return result;

        HttpMessageHandler? handler = default;

        if (Builders.TryGetValue(name, out var builder))
            if (builder.ConfigureHandler != null)
                handler = builder.ConfigureHandler(GetHandler);

        HttpMessageHandler GetHandler()
        {
            HttpMessageHandler handler = category switch
            {
                HttpHandlerCategory.Speculative => NetCache.Speculative,
                HttpHandlerCategory.UserInitiated => NetCache.UserInitiated,
                HttpHandlerCategory.Background => NetCache.Background,
                HttpHandlerCategory.Offline => NetCache.Offline,
                _ => this.handler,
            };
            return handler;
        }

        handler ??= GetHandler();
        var client = CreateClient(handler);

        if (builder != default)
            builder.ConfigureClient?.Invoke(client);

        return activeClients[key] = client;
    }

    void Dispose(bool disposing)
    {
        if (!disposedValue)
        {
            if (disposing)
            {
                // 释放托管状态(托管对象)
                foreach (var activeClient in activeClients.Values)
                    try
                    {
                        activeClient.Dispose();
                    }
                    catch
                    {
                    }

                try
                {
                    handler.Dispose();
                }
                catch
                {
                }

                try
                {
                    NetCache.Speculative?.Dispose();
                }
                catch
                {
                }

                try
                {
                    NetCache.UserInitiated?.Dispose();
                }
                catch
                {
                }

                try
                {
                    NetCache.Background?.Dispose();
                }
                catch
                {
                }

                try
                {
                    NetCache.Offline?.Dispose();
                }
                catch
                {
                }
            }

            // 释放未托管的资源(未托管的对象)并重写终结器
            // 将大型字段设置为 null
            disposedValue = true;
        }
    }

    /// <inheritdoc/>
    public void Dispose()
    {
        // 不要更改此代码。请将清理代码放入“Dispose(bool disposing)”方法中
        Dispose(disposing: true);
        GC.SuppressFinalize(this);
    }

    /// <summary>
    /// 将 <see cref="FusilladeClientHttpClientFactory"/> 添加到服务集合中
    /// </summary>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    internal static IServiceCollection AddFusilladeHttpClientFactory(
        IServiceCollection services,
        HttpMessageHandler? handler,
        bool registerConstant)
    {
        IClientHttpClientFactory.AddHttpClientDelegateValue = static (services, type) =>
        {
            services.AddFusilladeHttpClient(TypeNameHelper.GetTypeDisplayName(type));
        };
        FusilladeClientHttpClientFactory factory = handler == null ? new(registerConstant) :
            new(handler, registerConstant);
        services.AddSingleton(factory);
        services.AddSingleton<IClientHttpClientFactory>(factory);

        services.AddSingleton<CookieClientHttpClientFactory>();

        return services;
    }
}

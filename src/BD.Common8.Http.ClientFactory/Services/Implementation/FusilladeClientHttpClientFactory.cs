using Fusillade;
using Splat;
using IClientHttpClientFactory = System.Net.Http.Client.IClientHttpClientFactory;

namespace BD.Common8.Http.ClientFactory.Services.Implementation;

#pragma warning disable SA1600 // Elements should be documented

/// <summary>
/// 使用 Fusillade 实现的 <see cref="IClientHttpClientFactory"/>
/// <para>https://github.com/dotnet/runtime/blob/v7.0.3/src/libraries/Microsoft.Extensions.Http/src/DefaultHttpClientFactory.cs</para>
/// </summary>
public class FusilladeClientHttpClientFactory : IClientHttpClientFactory, IDisposable
{
    bool disposedValue;
    readonly HttpMessageHandler handler;
    readonly ConcurrentDictionary<(string, HttpHandlerCategory), HttpClient> activeClients = new();
    internal static readonly Dictionary<string, DefaultHttpClientBuilder> Builders = [];

    public FusilladeClientHttpClientFactory(bool registerConstant = true) : this(CreateHandler(), registerConstant)
    {
    }

    public FusilladeClientHttpClientFactory(HttpMessageHandler handler, bool registerConstant = true)
    {
        this.handler = handler;
        if (registerConstant)
            Locator.CurrentMutable.RegisterConstant(handler, typeof(HttpMessageHandler));
    }

    public static HttpMessageHandler CreateHandler()
    {
        HttpClientHandler handler = new()
        {
            UseCookies = false,
            AutomaticDecompression = DecompressionMethods.Brotli | DecompressionMethods.GZip | DecompressionMethods.Deflate,
        };
        return handler;
    }

    protected virtual HttpClient CreateClient(HttpMessageHandler handler)
    {
        var client = new HttpClient(handler);
        try
        {
            client.Timeout = IClientHttpClientFactory.DefaultTimeout;
        }
        catch
        {
        }
        return client;
    }

    HttpClient IClientHttpClientFactory.CreateClient(string name, HttpHandlerCategory category)
    {
        if (!category.IsDefined())
            category = HttpHandlerCategory.Default;

        ValueTuple<string, HttpHandlerCategory> key = new(name, category);

        if (activeClients.TryGetValue(key, out var result))
            return result;

        HttpMessageHandler? handler = default;
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

        if (Builders.TryGetValue(name, out var builder))
            if (builder.ConfigureHandler != null)
                handler = builder.ConfigureHandler(GetHandler);

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

    public void Dispose()
    {
        // 不要更改此代码。请将清理代码放入“Dispose(bool disposing)”方法中
        Dispose(disposing: true);
        GC.SuppressFinalize(this);
    }

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
        return services.AddSingleton<IClientHttpClientFactory>(factory);
    }
}

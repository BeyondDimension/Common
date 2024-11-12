#if !NETFRAMEWORK && !PROJ_SETUP
using static BD.Common8.Http.ClientFactory.Services.Implementation.FusilladeClientHttpClientFactory;

namespace BD.Common8.Http.ClientFactory.Services.Implementation;

/// <summary>
/// 使用 Cookie 的 HttpClientFactory
/// <para>对于需要使用 Cookie 的场景，定义一个唯一键 object id，由此工厂创建与复用 <see cref="HttpClient"/> 并且调用 <see cref="Dispose(string, object)"/> 进行释放</para>
/// </summary>
public partial class CookieClientHttpClientFactory : IDisposable
{
    bool disposedValue;

    readonly ConcurrentDictionary<(string, object), (HttpClient, HttpMessageHandler)> activeClients = new();

    /// <summary>
    /// 创建一个 <see cref="HttpClient"/> 实例并设置默认超时时间
    /// </summary>
    protected virtual HttpClient CreateClient(HttpMessageHandler handler)
    {
        var client = IClientHttpClientFactory.CreateClient(handler);
        return client;
    }

    /// <summary>
    /// 获取 Cookie 容器
    /// </summary>
    /// <param name="handler"></param>
    /// <returns></returns>
    protected virtual CookieContainer GetCookieContainer(HttpMessageHandler handler)
    {
        if (handler is HttpClientHandler httpClientHandler)
        {
            return httpClientHandler.CookieContainer;
        }
        else if (handler is SocketsHttpHandler socketsHttpHandler)
        {
            return socketsHttpHandler.CookieContainer;
        }
        else
        {
            throw ThrowHelper.GetArgumentOutOfRangeException(handler);
        }
    }

    /// <summary>
    /// 获取 Cookie 容器
    /// </summary>
    /// <param name="name"></param>
    /// <param name="id"></param>
    /// <returns></returns>
    public CookieContainer GetCookieContainer(string name, object id)
    {
        (_, var handler) = CreateClientCore(name, id);
        var container = GetCookieContainer(handler);
        return container;
    }

    /// <summary>
    /// 创建或获取基于给定名称和唯一键的 HttpClient 实例，返回 HttpClient 与 HttpMessageHandler
    /// </summary>
    /// <param name="name"></param>
    /// <param name="id"></param>
    /// <returns></returns>
    protected (HttpClient httpClient, HttpMessageHandler handler) CreateClientCore(string name, object id)
    {
        ValueTuple<string, object> key = new(name, id);

        if (activeClients.TryGetValue(key, out var result))
            return result;

        HttpMessageHandler? handler = default;

        if (Builders.TryGetValue(name, out var builder))
            if (builder.ConfigureHandler != null)
                handler = builder.ConfigureHandler(GetHandler);

        static HttpMessageHandler GetHandler()
        {
            HttpMessageHandler handler = IClientHttpClientFactory.CreateHandler(useCookies: true);
            return handler;
        }

        handler ??= GetHandler();
        var client = CreateClient(handler);

        if (builder != default)
            builder.ConfigureClient?.Invoke(client);

        return activeClients[key] = (client, handler);
    }

    /// <summary>
    /// 创建或获取基于给定名称和唯一键的 HttpClient 实例
    /// </summary>
    /// <param name="name"></param>
    /// <param name="id"></param>
    /// <returns></returns>
    public HttpClient CreateClient(string name, object id)
    {
        (var httpClient, _) = CreateClientCore(name, id);
        return httpClient;
    }

    /// <summary>
    /// 创建或获取基于给定名称和唯一键的 HttpClient 实例
    /// </summary>
    /// <param name="name"></param>
    /// <param name="id"></param>
    /// <param name="cookies"></param>
    /// <returns></returns>
    public HttpClient CreateClient(string name, object id, IEnumerable<Cookie> cookies)
    {
        (var httpClient, var handler) = CreateClientCore(name, id);
        var container = GetCookieContainer(handler);
        foreach (var cookie in cookies)
        {
            container.Add(cookie);
        }
        return httpClient;
    }

    /// <summary>
    /// 创建或获取基于给定名称和唯一键的 HttpClient 实例
    /// </summary>
    /// <param name="name"></param>
    /// <param name="id"></param>
    /// <param name="cookies"></param>
    /// <returns></returns>
    public HttpClient CreateClient(string name, object id, params Cookie[] cookies)
    {
        return CreateClient(name, id, cookies.AsEnumerable());
    }

    /// <summary>
    /// 创建或获取基于给定名称和唯一键的 HttpClient 实例
    /// </summary>
    /// <param name="name"></param>
    /// <param name="id"></param>
    /// <param name="cookieCollection"></param>
    /// <returns></returns>
    public HttpClient CreateClient(string name, object id, CookieCollection cookieCollection)
    {
        (var httpClient, var handler) = CreateClientCore(name, id);
        var container = GetCookieContainer(handler);
        container.Add(cookieCollection);
        return httpClient;
    }

    /// <summary>
    /// 创建或获取基于给定名称和唯一键的 HttpClient 实例
    /// </summary>
    /// <param name="name"></param>
    /// <param name="id"></param>
    /// <param name="cookieContainer"></param>
    /// <returns></returns>
    public HttpClient CreateClient(string name, object id, CookieContainer cookieContainer)
    {
        (var httpClient, var handler) = CreateClientCore(name, id);
        var container = GetCookieContainer(handler);
        container.Add(cookieContainer.GetAllCookies());
        return httpClient;
    }

    /// <summary>
    /// 释放基于给定名称和唯一键的 HttpClient 实例
    /// </summary>
    /// <param name="name"></param>
    /// <param name="id"></param>
    public void Dispose(string name, object id)
    {
        ValueTuple<string, object> key = new(name, id);
        if (activeClients.TryRemove(key, out var activeClient))
        {
            HttpClient httpClient = activeClient.Item1;
            httpClient.Dispose();
        }
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
                        HttpClient httpClient = activeClient.Item1;
                        httpClient.Dispose();
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
}
#endif
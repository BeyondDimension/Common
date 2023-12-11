namespace BD.Common8.Http.ClientFactory.Services;

/// <summary>
/// 使用 <see cref="IClientHttpClientFactory"/> 工厂构建 <see cref="HttpClient"/> 的 WebApiClient 基类服务，继承自 <see cref="WebApiClientService"/>
/// </summary>
/// <param name="logger"></param>
/// <param name="serviceProvider"></param>
public abstract partial class WebApiClientFactoryService(
    ILogger logger,
    IServiceProvider serviceProvider) : WebApiClientService(logger, serviceProvider.GetRequiredService<IHttpPlatformHelperService>(), newtonsoftJsonSerializer: null)
{
    /// <inheritdoc cref="IClientHttpClientFactory"/>
    protected readonly IClientHttpClientFactory? clientHttpClientFactory = serviceProvider.GetService<IClientHttpClientFactory>();

    /// <inheritdoc cref="CookieClientHttpClientFactory"/>
    protected readonly CookieClientHttpClientFactory? cookieClientHttpClientFactory = serviceProvider.GetService<CookieClientHttpClientFactory>();

    /// <summary>
    /// 服务名称，通常使用 TAG，格式规范应为 typeName.TrimStart(I).TrimEnd(Impl).TrimEnd(Service)
    /// </summary>
    protected abstract string ClientName { get; }

    /// <summary>
    /// HttpHandler 类别，默认为 <see cref="HttpHandlerCategory.Default"/>
    /// </summary>
    protected virtual HttpHandlerCategory Category => HttpHandlerCategory.Default;

    /// <inheritdoc cref="HttpClient"/>
    HttpClient? httpClient;

    /// <summary>
    /// 创建或获取【不需要】使用 Cookie 的 <see cref="HttpClient"/>
    /// </summary>
    /// <returns></returns>
    protected override HttpClient CreateClient()
    {
        return httpClient ??= clientHttpClientFactory.ThrowIsNull().CreateClient(ClientName, Category);
    }

    /// <summary>
    /// 创建或获取【需要】使用 Cookie 的 <see cref="HttpClient"/>，状态由 id 对应字典保存
    /// </summary>
    /// <param name="id"></param>
    /// <returns></returns>
    protected virtual HttpClient CreateClient(object id)
    {
        return cookieClientHttpClientFactory.ThrowIsNull().CreateClient(ClientName, id);
    }

    /// <summary>
    /// 根据状态 id 获取对应的 Cookie 容器
    /// </summary>
    /// <param name="id"></param>
    /// <returns></returns>
    protected CookieContainer GetCookieContainer(object id)
    {
        return cookieClientHttpClientFactory.ThrowIsNull().GetCookieContainer(ClientName, id);
    }
}

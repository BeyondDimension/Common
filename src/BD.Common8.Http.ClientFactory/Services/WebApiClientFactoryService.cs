namespace BD.Common8.Http.ClientFactory.Services;

/// <summary>
/// 使用 <see cref="IClientHttpClientFactory"/> 工厂构建 <see cref="HttpClient"/> 的 WebApiClient 基类服务，继承自 <see cref="WebApiClientService"/>
/// </summary>
/// <param name="logger"></param>
/// <param name="httpPlatformHelper"></param>
/// <param name="clientHttpClientFactory"></param>
/// <param name="newtonsoftJsonSerializer">如果需要使用 <see cref="Newtonsoft.Json"/> 则需要传递自定义实例或通过直接 new()，否则应保持为 <see langword="null"/></param>
public abstract partial class WebApiClientFactoryService(
    ILogger logger,
    IHttpPlatformHelperService httpPlatformHelper,
    IClientHttpClientFactory? clientHttpClientFactory) : WebApiClientService(logger, httpPlatformHelper, newtonsoftJsonSerializer: null)
{
    /// <inheritdoc cref="IClientHttpClientFactory"/>
    protected readonly IClientHttpClientFactory? clientHttpClientFactory = clientHttpClientFactory;

    /// <summary>
    /// 服务名称，通常使用 TAG，格式规范应为 typeName.TrimStart(I).TrimEnd(Impl).TrimEnd(Service)
    /// </summary>
    protected abstract string ClientName { get; }

    /// <summary>
    /// HttpHandler 类别，默认为 <see cref="HttpHandlerCategory.Default"/>
    /// </summary>
    protected virtual HttpHandlerCategory Category => HttpHandlerCategory.Default;

    /// <inheritdoc cref="HttpClient"/>
    protected HttpClient? httpClient;

    /// <inheritdoc/>
    protected override HttpClient CreateClient()
    {
        return httpClient ??= clientHttpClientFactory.ThrowIsNull().CreateClient(ClientName, Category);
    }
}

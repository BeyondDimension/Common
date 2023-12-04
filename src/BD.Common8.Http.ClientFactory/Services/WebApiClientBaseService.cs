namespace BD.Common8.Http.ClientFactory.Services;

/// <summary>
/// WebApiClient 基类服务，实现序列化相关，统一使用方式
/// </summary>
/// <param name="logger"></param>
/// <param name="httpPlatformHelper"></param>
/// <param name="clientFactory"></param>
/// <param name="newtonsoftJsonSerializer">如果需要使用 <see cref="Newtonsoft.Json"/> 则需要传递自定义实例或通过直接 new()，否则应保持为 <see langword="null"/></param>
public abstract partial class WebApiClientBaseService(
    ILogger logger,
    IHttpPlatformHelperService httpPlatformHelper,
    IClientHttpClientFactory clientFactory,
    NewtonsoftJsonSerializer? newtonsoftJsonSerializer = null) : Serializable.IService
{
    /// <inheritdoc cref="IHttpPlatformHelperService"/>
    protected readonly IHttpPlatformHelperService httpPlatformHelper = httpPlatformHelper;

    /// <inheritdoc cref="IHttpPlatformHelperService.UserAgent"/>
    protected virtual string? UserAgent => httpPlatformHelper.UserAgent;

    /// <inheritdoc cref="ILogger"/>
    protected readonly ILogger logger = logger;

    /// <inheritdoc/>
    ILogger Log.I.Logger => logger;

    /// <inheritdoc cref="IClientHttpClientFactory"/>
    protected readonly IClientHttpClientFactory clientFactory = clientFactory;

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

    /// <inheritdoc cref="IClientHttpClientFactory.CreateClient(string, HttpHandlerCategory)"/>
    protected virtual HttpClient CreateClient()
    {
        return httpClient ??= clientFactory.CreateClient(ClientName, Category);
    }

    /// <summary>
    /// 将请求模型类序列化为 <see cref="HttpContent"/>（catch 时将返回 <see langword="null"/> ），默认使用 System.Text.Json 实现
    /// </summary>
    /// <typeparam name="T"></typeparam>
    /// <param name="inputValue"></param>
    /// <param name="mediaType"></param>
    /// <returns></returns>
    protected virtual HttpContent? GetHttpContent<T>(T inputValue, MediaTypeHeaderValue? mediaType = null)
    {
        var content = GetSJsonContent(inputValue, null, mediaType);
        return content;
    }

    /// <summary>
    /// 将响应内容读取并反序列化成实例（catch 时将返回 <see langword="null"/> ），默认使用 System.Text.Json 实现
    /// </summary>
    /// <typeparam name="T"></typeparam>
    /// <param name="content"></param>
    /// <param name="cancellationToken"></param>
    /// <returns></returns>
    protected virtual async Task<T?> ReadFromAsync<T>(HttpContent content, CancellationToken cancellationToken = default)
    {
        T? result;
        result = await ReadFromSJsonAsync<T>(content, null, cancellationToken);
        return result;
    }

    /// <summary>
    /// 使用的默认文本编码，默认值为 <see cref="Encoding.UTF8"/>
    /// </summary>
    protected virtual Encoding DefaultEncoding => Encoding.UTF8;

    /// <inheritdoc/>
    Encoding Serializable.IService.DefaultEncoding => DefaultEncoding;

    /// <inheritdoc cref="Serializable.IService.OnSerializerError(Exception, bool, Type)"/>
    protected virtual void OnSerializerError(Exception ex,
        bool isSerializeOrDeserialize,
        Type modelType)
    {
        Serializable.IService.DefaultOnSerializerError(logger, ex, isSerializeOrDeserialize, modelType);
    }

    /// <inheritdoc/>
    void Serializable.IService.OnSerializerError(Exception ex,
        bool isSerializeOrDeserialize,
        Type modelType)
        => OnSerializerError(ex, isSerializeOrDeserialize, modelType);
}

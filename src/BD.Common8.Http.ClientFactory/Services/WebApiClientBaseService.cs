namespace BD.Common8.Http.ClientFactory.Services;

/// <summary>
/// WebApiClient 基类服务，实现序列化相关，统一使用方式
/// </summary>
/// <param name="logger"></param>
/// <param name="clientFactory"></param>
/// <param name="newtonsoftJsonSerializer">如果需要使用 <see cref="Newtonsoft.Json"/> 则需要传递自定义实例或通过直接 new()，否则应保持为 <see langword="null"/></param>
public abstract partial class WebApiClientBaseService(
    ILogger logger,
    IClientHttpClientFactory clientFactory,
    NewtonsoftJsonSerializer? newtonsoftJsonSerializer = null)
{
    /// <inheritdoc cref="ILogger"/>
    protected readonly ILogger logger = logger;

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

    /// <summary>
    /// 当序列化出现错误时
    /// </summary>
    /// <param name="ex"></param>
    /// <param name="isSerializeOrDeserialize">是序列化还是反序列化</param>
    /// <param name="modelType">模型类类型</param>
    protected virtual void OnSerializerError(Exception ex,
        bool isSerializeOrDeserialize,
        Type modelType)
    {
        // 记录错误时，不需要带上 requestUrl 等敏感信息
        if (isSerializeOrDeserialize)
        {
            logger.LogError(ex,
                "Error serializing request model class. (Parameter '{type}')",
                modelType);
        }
        else
        {
            logger.LogError(ex,
                "Error reading and deserializing the response content into an instance. (Parameter '{type}')",
                modelType);
        }
    }
}

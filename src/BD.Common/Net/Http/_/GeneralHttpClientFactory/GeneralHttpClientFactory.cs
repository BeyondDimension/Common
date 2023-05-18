// ReSharper disable once CheckNamespace
namespace System.Net.Http;

/// <summary>
/// 通用 <see cref="HttpClient"/> 工厂
/// </summary>
public abstract partial class GeneralHttpClientFactory : IHttpClientFactory
{
    protected readonly ILogger logger;
    protected readonly IHttpPlatformHelperService http_helper;
    protected readonly IHttpClientFactory _clientFactory;

    public GeneralHttpClientFactory(
        ILogger logger,
        IHttpPlatformHelperService http_helper,
        IHttpClientFactory clientFactory)
    {
        this.logger = logger;
        this.http_helper = http_helper;
        _clientFactory = clientFactory;
    }

    /// <summary>
    /// 用于 <see cref="IHttpClientFactory.CreateClient(string, HttpHandlerCategory)"/> 中传递的 name
    /// </summary>
    protected virtual string? DefaultClientName { get; }

    /// <inheritdoc cref="HttpClient.Timeout"/>
    protected virtual TimeSpan? Timeout => DefaultTimeout;

    protected virtual HttpClient CreateClient(string? clientName, HttpHandlerCategory category)
    {
        if (clientName == null)
        {
            clientName = DefaultClientName;
            // https://github.com/dotnet/runtime/blob/v7.0.5/src/libraries/Microsoft.Extensions.Http/src/HttpClientFactoryExtensions.cs#L22
            clientName ??= string.Empty;
        }

        var client = _clientFactory.CreateClient(clientName, category);
        var timeout = Timeout;
        if (timeout.HasValue)
        {
            try
            {
                client.Timeout = timeout.Value;
            }
            catch
            {

            }
        }
        return client;
    }

    [Obsolete("use CreateClient(string? clientName = null, HttpHandlerCategory category = default)")]
    protected virtual HttpClient CreateClient(string? clientName = null)
        => CreateClient(clientName, HttpHandlerCategory.Default);

    HttpClient IHttpClientFactory.CreateClient(string name, HttpHandlerCategory category) => CreateClient(name, category);
}
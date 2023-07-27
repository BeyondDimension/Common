using BD.Common.Services.Implementation;

// ReSharper disable once CheckNamespace
namespace Microsoft.Extensions.DependencyInjection;

public static class FusilladeHttpClientFactoryServiceCollectionExtensions
{
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static IServiceCollection AddFusilladeHttpClient(
        this IServiceCollection services,
        HttpMessageHandler? handler = null)
    {
        FusilladeHttpClientFactory factory = handler == null ? new() : new(handler);
        return services.AddSingleton<IHttpClientFactory>(factory);
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static IFusilladeHttpClientBuilder AddFusilladeHttpClient(
        this IServiceCollection services,
        string name,
        Action<HttpClient>? configureClient = null)
    {
        var b = new DefaultHttpClientBuilder(name, services, configureClient);
        FusilladeHttpClientFactory.Builders.Add(name, b);
        return b;
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static IFusilladeHttpClientBuilder ConfigurePrimaryHttpMessageHandler(
        this IFusilladeHttpClientBuilder builder,
        Func<Func<HttpMessageHandler>, HttpMessageHandler> configureHandler)
    {
        if (builder is DefaultHttpClientBuilder b)
        {
            b.ConfigureHandler = configureHandler;
        }
        return builder;
    }
}

/// <summary>
/// A builder for configuring named <see cref="HttpClient"/> instances returned by <see cref="IHttpClientFactory"/>.
/// </summary>
public interface IFusilladeHttpClientBuilder
{
    /// <summary>
    /// Gets the name of the client configured by this builder.
    /// </summary>
    string Name { get; }

    /// <summary>
    /// Gets the application service collection.
    /// </summary>
    IServiceCollection Services { get; }
}

record class DefaultHttpClientBuilder(
    string Name,
    IServiceCollection Services,
    Action<HttpClient>? ConfigureClient) : IFusilladeHttpClientBuilder
{
    public Func<Func<HttpMessageHandler>, HttpMessageHandler>? ConfigureHandler { get; set; }
}
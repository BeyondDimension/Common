#if !NETFRAMEWORK && !PROJ_SETUP
namespace BD.Common8.Http.ClientFactory.Extensions;

public static partial class ServiceCollectionExtensions
{
    /// <summary>
    /// 添加使用 Fusillade 实现的 <see cref="IClientHttpClientFactory"/>
    /// </summary>
    /// <param name="services"></param>
    /// <param name="handler"></param>
    /// <returns></returns>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static IServiceCollection AddFusilladeHttpClientFactory(
        this IServiceCollection services,
        HttpMessageHandler? handler = null)
        => FusilladeClientHttpClientFactory.AddFusilladeHttpClientFactory(services, handler);

    /// <summary>
    /// 根据名称配置使用 Fusillade 实现的 <see cref="HttpClient"/>
    /// </summary>
    /// <param name="services"></param>
    /// <param name="name"></param>
    /// <param name="configureClient"></param>
    /// <returns></returns>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static IFusilladeHttpClientBuilder AddFusilladeHttpClient(
        this IServiceCollection services,
        string name,
        Action<HttpClient>? configureClient = null)
    {
        var b = new DefaultHttpClientBuilder(name, services, configureClient);
        FusilladeClientHttpClientFactory.Builders.Add(name, b);
        return b;
    }

    /// <summary>
    /// 根据名称配置使用 Fusillade 实现的 <see cref="HttpMessageHandler"/>
    /// </summary>
    /// <param name="builder"></param>
    /// <param name="configureHandler"></param>
    /// <returns></returns>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static IFusilladeHttpClientBuilder ConfigurePrimaryHttpMessageHandler(
        this IFusilladeHttpClientBuilder builder,
        Func<Func<HttpMessageHandler>, HttpMessageHandler> configureHandler)
    {
        if (builder is DefaultHttpClientBuilder b)
            b.ConfigureHandler = configureHandler;
        return builder;
    }

    /// <summary>
    /// 将 <see cref="CookieClientHttpClientFactory"/> 添加到服务集合中
    /// </summary>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static IServiceCollection AddCookieClientHttpClientFactory(this IServiceCollection services, Func<HttpMessageHandler> configureHandler)
    {
        services.AddSingleton(new CookieClientHttpClientFactory(configureHandler));

        return services;
    }
}
#endif
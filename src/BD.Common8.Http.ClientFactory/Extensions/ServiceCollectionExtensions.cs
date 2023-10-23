#pragma warning disable IDE0130 // 命名空间与文件夹结构不匹配
namespace Microsoft.Extensions.DependencyInjection;

public static partial class ServiceCollectionExtensions
{
    /// <summary>
    /// 添加使用 Fusillade 实现的 <see cref="IClientHttpClientFactory"/>
    /// </summary>
    /// <param name="services"></param>
    /// <param name="handler"></param>
    /// <param name="registerConstant">是否调用 <see cref="Splat.DependencyResolverMixins.RegisterConstant(Splat.IMutableDependencyResolver, object?, Type?, string?)"/>，默认值为 <see langword="true"/></param>
    /// <returns></returns>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static IServiceCollection AddFusilladeHttpClientFactory(
        this IServiceCollection services,
        HttpMessageHandler? handler = null,
        bool registerConstant = true)
        => FusilladeClientHttpClientFactory.AddFusilladeHttpClientFactory(services, handler, registerConstant);

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
        {
            b.ConfigureHandler = configureHandler;
        }
        return builder;
    }
}
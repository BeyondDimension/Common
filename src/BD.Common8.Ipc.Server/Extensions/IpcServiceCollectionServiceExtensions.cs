namespace BD.Common8.Ipc;

public static partial class IpcServiceCollectionServiceExtensions
{
    /// <summary>
    /// 添加 Ipc 服务器配置接口与实现类
    /// </summary>
    /// <typeparam name="TService"></typeparam>
    /// <typeparam name="TImplementation"></typeparam>
    /// <param name="services"></param>
    /// <returns></returns>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static IServiceCollection AddIpcServer<TService, [DynamicallyAccessedMembers(DynamicallyAccessedMemberTypes.PublicConstructors)] TImplementation>(this IServiceCollection services)
        where TService : class
        where TImplementation : class, TService, IEndpointRouteMapGroup/*, IHubEndpointRouteMapHub*/
    {
        IpcServerService.OnMapGroupEvent += TImplementation.OnMapGroup;
        //IpcServerService.OnMapHubEvent += TImplementation.OnMapHub;
        return services;
    }

    /// <summary>
    /// Adds a singleton service of the type specified in <typeparamref name="TService"/> with an
    /// implementation type specified in <typeparamref name="TImplementation"/> to the
    /// specified <see cref="IServiceCollection"/>.
    /// </summary>
    /// <typeparam name="TService">The type of the service to add.</typeparam>
    /// <typeparam name="TImplementation">The type of the implementation to use.</typeparam>
    /// <param name="services">The <see cref="IServiceCollection"/> to add the service to.</param>
    /// <returns>A reference to this instance after the operation has completed.</returns>
    /// <seealso cref="ServiceLifetime.Singleton"/>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static IServiceCollection AddSingletonWithIpcServer<TService, [DynamicallyAccessedMembers(DynamicallyAccessedMemberTypes.PublicConstructors)] TImplementation>(this IServiceCollection services)
        where TService : class
        where TImplementation : class, TService, IEndpointRouteMapGroup/*, IHubEndpointRouteMapHub*/
    {
        services.AddIpcServer<TService, TImplementation>();
        return services.AddSingleton(typeof(TService), typeof(TImplementation));
    }
}

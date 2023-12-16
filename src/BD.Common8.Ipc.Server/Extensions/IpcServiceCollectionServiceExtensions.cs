#pragma warning disable IDE0130 // 命名空间与文件夹结构不匹配
namespace BD.Common8.Ipc;

public static partial class IpcServiceCollectionServiceExtensions
{
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
    public static IServiceCollection AddSingletonWithIpc<TService, [DynamicallyAccessedMembers(DynamicallyAccessedMemberTypes.PublicConstructors)] TImplementation>(this IServiceCollection services)
        where TService : class
        where TImplementation : class, TService, IEndpointRouteMapGroup, IHubEndpointRouteMapHub
    {
        IpcServerService.OnMapGroupEvent += TImplementation.OnMapGroup;
        IpcServerService.OnMapHubEvent += TImplementation.OnMapHub;
        return services.AddSingleton(typeof(TService), typeof(TImplementation));
    }
}

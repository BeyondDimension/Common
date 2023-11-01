namespace BD.Common8.Ipc.Server.Extensions;

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
        where TService : class, IEndpointRouteMapGroup
        where TImplementation : class, TService
    {
        IpcAppBuilderOptions.OnMapGroupEvent += TService.OnMapGroup;
        return services.AddSingleton(typeof(TService), typeof(TImplementation));
    }
}

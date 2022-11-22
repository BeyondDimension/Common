// ReSharper disable once CheckNamespace
namespace Microsoft.Extensions.DependencyInjection;

public static partial class ServiceCollectionExtensions
{
    /// <summary>
    /// 添加由 <see cref="PreferencesPlatformServiceImpl"/> 实现的 <see cref="IPreferencesPlatformService"/>
    /// </summary>
    /// <param name="services"></param>
    /// <returns></returns>
    public static IServiceCollection AddRepositoryPreferences(this IServiceCollection services)
    {
        services.AddSingleton<IPreferencesPlatformService, PreferencesPlatformServiceImpl>();
        return services;
    }
}
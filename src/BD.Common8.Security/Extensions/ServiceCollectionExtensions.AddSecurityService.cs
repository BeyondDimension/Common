using static BD.Common8.Security.Services.ILocalDataProtectionProvider;
using static BD.Common8.Security.Services.Implementation.LocalDataProtectionProviderBase;

namespace Microsoft.Extensions.DependencyInjection;

public static partial class ServiceCollectionExtensions
{
    /// <summary>
    /// 添加安全服务
    /// </summary>
    /// <typeparam name="TEmbeddedAes"></typeparam>
    /// <typeparam name="TLocal"></typeparam>
    /// <param name="services"></param>
    /// <returns></returns>
    public static IServiceCollection AddSecurityService<TEmbeddedAes, TLocal>(this IServiceCollection services)
        where TEmbeddedAes : EmbeddedAesDataProtectionProviderBase
        where TLocal : class, ILocalDataProtectionProvider
    {
        services.TryAddSingleton<IProtectedData, EmptyProtectedData>();
        services.TryAddSingleton<IDataProtectionProvider, EmptyDataProtectionProvider>();
        services.TryAddSingleton<ISecondaryPasswordDataProtectionProvider, SecondaryPasswordDataProtectionProvider>();
        services.AddSingleton<IEmbeddedAesDataProtectionProvider, TEmbeddedAes>();
        services.AddSingleton<ILocalDataProtectionProvider, TLocal>();
        services.AddSingleton<ISecurityService, SecurityService>();
        return services;
    }
}
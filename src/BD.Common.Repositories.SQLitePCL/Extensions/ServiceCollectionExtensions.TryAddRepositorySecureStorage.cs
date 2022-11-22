using BD.Common.Repositories;
using System.Security;

// ReSharper disable once CheckNamespace
namespace Microsoft.Extensions.DependencyInjection;

public static class ServiceCollectionExtensions
{
    /// <summary>
    /// 添加由 Repository 实现的 <see cref="ISecureStorage"/>
    /// </summary>
    /// <param name="services"></param>
    /// <returns></returns>
    public static IServiceCollection TryAddRepositorySecureStorage(this IServiceCollection services)
    {
        services.TryAddSingleton<ISecureStorage, RepositorySecureStorage>();
        return services;
    }
}
#pragma warning disable IDE0130 // 命名空间与文件夹结构不匹配
namespace Microsoft.Extensions.DependencyInjection;

public static partial class ServiceCollectionExtensions
{
    /// <summary>
    /// 添加由 Repository 实现的 <see cref="ISecureStorage"/>
    /// </summary>
    /// <typeparam name="TDbContext"></typeparam>
    /// <param name="services"></param>
    /// <returns></returns>
    public static IServiceCollection TryAddSecureStorage<TDbContext>(this IServiceCollection services) where TDbContext : DbContext
    {
        services.TryAddScoped<ISecureStorage, RepositorySecureStorage<TDbContext>>();
        return services;
    }
}
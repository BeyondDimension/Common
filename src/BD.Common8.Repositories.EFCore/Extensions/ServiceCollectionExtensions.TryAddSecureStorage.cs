namespace BD.Common8.Repositories.EFCore.Extensions;

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
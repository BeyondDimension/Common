namespace BD.Common8.Repositories.SQLitePCL.Extensions;

public static partial class ServiceCollectionExtensions
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
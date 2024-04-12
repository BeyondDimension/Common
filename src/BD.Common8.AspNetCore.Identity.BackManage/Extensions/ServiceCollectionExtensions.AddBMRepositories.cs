namespace Microsoft.Extensions.DependencyInjection;

public static partial class ServiceCollectionExtensions
{
    /// <summary>
    /// 添加 后端管理 仓储层
    /// </summary>
    /// <typeparam name="TDbContext"></typeparam>
    /// <param name="services"></param>
    /// <returns></returns>
    public static IServiceCollection AddBMRepositories<TDbContext>(this IServiceCollection services)
        where TDbContext : DbContext, IApplicationDbContext
    {
        services.AddScoped<ISysUserRepository, SysUserRepository<TDbContext>>();
        services.AddScoped<ISysRoleRepository, SysRoleRepository<TDbContext>>();
        services.AddScoped<ISysMenuRepository, SysMenuRepository<TDbContext>>();
        return services;
    }
}
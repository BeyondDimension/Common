using BD.Common8.AspNetCore.Data.Abstractions;
using BD.Common8.AspNetCore.Repositories;
using BD.Common8.AspNetCore.Repositories.Abstractions;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;

namespace BD.Common8.AspNetCore.Extensions;

public static partial class ServiceCollectionExtensions
{
    /// <summary>
    /// 添加 后端管理 仓储层
    /// </summary>
    /// <typeparam name="TDbContext"></typeparam>
    /// <param name="services"></param>
    /// <returns></returns>
    public static IServiceCollection AddBMRepositories<TDbContext>(this IServiceCollection services)
        where TDbContext : DbContext, IBMDbContext
    {
        services.AddScoped<IBMUserRepository, BMUserRepository<TDbContext>>();
        services.AddScoped<IBMRoleRepository, BMRoleRepository<TDbContext>>();
        services.AddScoped<IBMMenuRepository, BMMenuRepository<TDbContext>>();
        return services;
    }
}
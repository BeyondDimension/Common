using BD.Common.Services;
using Impl = BD.Common.Services.Implementation;

#pragma warning disable IDE0130 // 命名空间与文件夹结构不匹配
namespace Microsoft.Extensions.DependencyInjection;

/// <summary>
/// Contains extension methods to <see cref="IServiceCollection"/> for configuring identity services.
/// </summary>
public static class IdentityServiceCollectionExtensions
{
    /// <summary>
    /// 添加多租户的 Identity 相关服务
    /// </summary>
    /// <typeparam name="TDbContext"></typeparam>
    /// <param name="services"></param>
    /// <param name="setupAction"></param>
    /// <returns></returns>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static IServiceCollection AddTenantIdentity<TDbContext>(this IServiceCollection services, Action<IdentityOptions>? setupAction = null) where TDbContext : DbContext, IApplicationDbContext
    {
        // https://github.com/dotnet/aspnetcore/blob/v7.0.0-rc.2.22476.2/src/Identity/Extensions.Core/src/IdentityServiceCollectionExtensions.cs#L33
        // Services used by identity
        services.AddScoped<IPasswordValidator, Impl.PasswordValidator>();
        services.AddScoped<IPasswordHasher<SysUser>, PasswordHasher<SysUser>>();
        services.AddScoped<ILookupNormalizer, UpperInvariantLookupNormalizer>();
        // No interface for the error describer so we can add errors without rev'ing the interface
        services.AddScoped<IdentityErrorDescriber>();
        services.AddScoped<IUserManager, Impl.UserManager<TDbContext>>();
        // https://github.com/dotnet/aspnetcore/blob/v7.0.0-rc.2.22476.2/src/Identity/Extensions.Core/src/IdentityBuilder.cs#L165
        services.AddScoped<IRoleManager, Impl.RoleManager<TDbContext>>();

        if (setupAction != null)
        {
            services.Configure(setupAction);
        }

        return services;
    }
}
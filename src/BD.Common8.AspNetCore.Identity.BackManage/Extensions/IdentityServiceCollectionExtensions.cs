namespace BD.Common8.AspNetCore.Extensions;

/// <summary>
/// 用于配置 Identity 服务到 <see cref="IServiceCollection"/>
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
    public static IServiceCollection AddTenantIdentity<TDbContext>(this IServiceCollection services, Action<IdentityOptions>? setupAction = null) where TDbContext : DbContext, IBMDbContext
    {
        // https://github.com/dotnet/aspnetcore/blob/v7.0.0-rc.2.22476.2/src/Identity/Extensions.Core/src/IdentityServiceCollectionExtensions.cs#L33
        // Services used by identity
        services.AddScoped<IPasswordValidator, PasswordValidatorImpl>();
        services.AddScoped<IPasswordHasher<BMUser>, PasswordHasher<BMUser>>();
        services.AddScoped<ILookupNormalizer, UpperInvariantLookupNormalizer>();
        // No interface for the error describer so we can add errors without rev'ing the interface
        services.AddScoped<IdentityErrorDescriber>();
        services.AddScoped<IUserManager, UserManagerImpl<TDbContext>>();
        // https://github.com/dotnet/aspnetcore/blob/v7.0.0-rc.2.22476.2/src/Identity/Extensions.Core/src/IdentityBuilder.cs#L165
        services.AddScoped<IRoleManager, RoleManagerImpl<TDbContext>>();

        if (setupAction != null)
            services.Configure(setupAction);

        return services;
    }
}
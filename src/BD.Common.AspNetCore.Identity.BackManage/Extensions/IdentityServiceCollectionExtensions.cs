using BD.Common.Services;
using Impl = BD.Common.Services.Implementation;

// ReSharper disable once CheckNamespace
namespace Microsoft.Extensions.DependencyInjection;

/// <summary>
/// Contains extension methods to <see cref="IServiceCollection"/> for configuring identity services.
/// </summary>
public static class IdentityServiceCollectionExtensions
{
    public static IServiceCollection AddTenantIdentity<TDbContext>(this IServiceCollection services, Action<IdentityOptions>? setupAction = null) where TDbContext : ApplicationDbContextBase
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
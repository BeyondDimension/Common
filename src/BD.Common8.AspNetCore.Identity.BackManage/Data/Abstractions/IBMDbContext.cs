namespace BD.Common8.AspNetCore.Data.Abstractions;

/// <summary>
/// 应用程序数据上下文
/// </summary>
public interface IBMDbContext : IApplicationDbContext<BMUser>
{
    /// <inheritdoc cref="BMRole"/>
    DbSet<BMRole> SysRoles { get; }

    /// <inheritdoc cref="BMUserRole"/>
    DbSet<BMUserRole> SysUserRoles { get; }

    /// <inheritdoc cref="BMMenuButtonRole"/>
    DbSet<BMMenuButtonRole> MenuButtonRoles { get; }

    /// <inheritdoc cref="BMTenant"/>
    DbSet<BMTenant> Tenants { get; }

    /// <inheritdoc cref="BMButton"/>
    DbSet<BMButton> Buttons { get; }

    /// <inheritdoc cref="BMMenu"/>
    DbSet<BMMenu> Menus { get; }

    /// <inheritdoc cref="BMMenuButton"/>
    DbSet<BMMenuButton> MenuButtons { get; }

    /// <inheritdoc cref="BMInfo"/>
    DbSet<BMInfo> SysInfos { get; }

    /// <inheritdoc cref="BMOrganization"/>
    DbSet<BMOrganization> Organizations { get; }

    /// <inheritdoc cref="BMUserOrganization"/>
    DbSet<BMUserOrganization> UserOrganizations { get; }
}
namespace BD.Common8.AspNetCore.Data.Abstractions;

/// <summary>
/// 应用程序数据上下文
/// </summary>
public interface IApplicationDbContext : IApplicationDbContext<SysUser>
{
    /// <inheritdoc cref="SysRole"/>
    DbSet<SysRole> Roles { get; }

    /// <inheritdoc cref="SysUserRole"/>
    DbSet<SysUserRole> UserRoles { get; }

    /// <inheritdoc cref="SysMenuButtonRole"/>
    DbSet<SysMenuButtonRole> MenuButtonRoles { get; }

    /// <inheritdoc cref="SysTenant"/>
    DbSet<SysTenant> Tenants { get; }

    /// <inheritdoc cref="SysButton"/>
    DbSet<SysButton> Buttons { get; }

    /// <inheritdoc cref="SysMenu"/>
    DbSet<SysMenu> Menus { get; }

    /// <inheritdoc cref="SysMenuButton"/>
    DbSet<SysMenuButton> MenuButtons { get; }

    /// <inheritdoc cref="SysInfo"/>
    DbSet<SysInfo> SysInfos { get; }

    /// <inheritdoc cref="SysOrganization"/>
    DbSet<SysOrganization> Organizations { get; }

    /// <inheritdoc cref="SysUserOrganization"/>
    DbSet<SysUserOrganization> UserOrganizations { get; }
}
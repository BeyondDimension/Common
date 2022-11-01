namespace BD.Common.Data.Abstractions;

public interface IApplicationDbContext : IApplicationDbContext<SysUser>
{
    DbSet<SysRole> Roles { get; }

    DbSet<SysUserRole> UserRoles { get; }

    DbSet<SysMenuButtonRole> MenuButtonRoles { get; }

    DbSet<SysTenant> Tenants { get; }

    DbSet<SysButton> Buttons { get; }

    DbSet<SysMenu> Menus { get; }

    DbSet<SysMenuButton> MenuButtons { get; }
}
namespace BD.Common8.AspNetCore.Data;

#pragma warning disable SA1600 // Elements should be documented

[method: RequiresUnreferencedCode("EF Core isn't fully compatible with trimming, and running the application may generate unexpected runtime failures. Some specific coding pattern are usually required to make trimming work properly, see https://aka.ms/efcore-docs-trimming for more details.")]
[method: RequiresDynamicCode("EF Core isn't fully compatible with NativeAOT, and running the application may generate unexpected runtime failures.")]
public abstract class ApplicationDbContextBase(DbContextOptions options) : DbContext(options), IApplicationDbContext
{
    /// <inheritdoc cref="SysUser"/>
    public DbSet<SysUser> Users { get; set; } = null!;

    /// <inheritdoc cref="SysRole"/>
    public DbSet<SysRole> Roles { get; set; } = null!;

    /// <inheritdoc cref="SysUserRole"/>
    public DbSet<SysUserRole> UserRoles { get; set; } = null!;

    /// <inheritdoc cref="SysMenuButtonRole"/>
    public DbSet<SysMenuButtonRole> MenuButtonRoles { get; set; } = null!;

    /// <inheritdoc cref="SysTenant"/>
    public DbSet<SysTenant> Tenants { get; set; } = null!;

    /// <inheritdoc cref="SysButton"/>
    public DbSet<SysButton> Buttons { get; set; } = null!;

    /// <inheritdoc cref="SysMenu"/>
    public DbSet<SysMenu> Menus { get; set; } = null!;

    /// <inheritdoc cref="SysMenuButton"/>
    public DbSet<SysMenuButton> MenuButtons { get; set; } = null!;

    /// <inheritdoc cref="SysInfo"/>
    public DbSet<SysInfo> SysInfos { get; set; } = null!;

    /// <inheritdoc cref="SysOrganization"/>
    public DbSet<SysOrganization> Organizations { get; set; } = null!;

    /// <inheritdoc cref="SysUserOrganization"/>
    public DbSet<SysUserOrganization> UserOrganizations { get; set; } = null!;

    protected override void OnModelCreating(ModelBuilder b)
    {
        base.OnModelCreating(b);
        b.BuildEntities(AppendBuildEntities_);
    }

    Action<EntityTypeBuilder>? AppendBuildEntities_(ModelBuilder modelBuilder, IMutableEntityType entityType, Type type, Action<EntityTypeBuilder>? buildAction)
    {
        buildAction = AppendBuildEntities(modelBuilder, entityType, type, buildAction);
        return buildAction;
    }

    protected virtual Action<EntityTypeBuilder>? AppendBuildEntities(ModelBuilder modelBuilder, IMutableEntityType entityType, Type type, Action<EntityTypeBuilder>? buildAction)
    {
        return buildAction;
    }

    public static readonly Type PCreateUser = typeof(ICreateUser);
    public static readonly Type POperatorUser = typeof(IOperatorUser);
}

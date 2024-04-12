namespace BD.Common8.AspNetCore.Data;

/// <summary>
/// Initializes a new instance of the <see cref="ApplicationDbContextBase"/> class.
/// </summary>
/// <param name="httpContextAccessor"></param>
/// <param name="options"></param>
public abstract class ApplicationDbContextBase(
    IHttpContextAccessor httpContextAccessor,
    DbContextOptions options) : DbContext(options), IDbContext, IApplicationDbContext
{
    DbContext IDbContext.Thiz => this;

    readonly IHttpContextAccessor httpContextAccessor = httpContextAccessor;

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

    /// <summary>
    /// 配置实体的模型
    /// </summary>
    /// <param name="b"><see cref="ModelBuilder"/> 对象，用于构建模型</param>
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

    /// <summary>
    /// 用于追加构建实体的操作方法
    /// </summary>
    protected virtual Action<EntityTypeBuilder>? AppendBuildEntities(ModelBuilder modelBuilder, IMutableEntityType entityType, Type type, Action<EntityTypeBuilder>? buildAction)
    {
        return buildAction;
    }

    /// <summary>
    /// 创建实体的用户属性类型 <see cref="ICreateUser"/>
    /// </summary>
    public static readonly Type PCreateUser = typeof(ICreateUser);

    /// <summary>
    /// 操作实体的用户属性类型 <see cref="IOperatorUser"/>
    /// </summary>
    public static readonly Type POperatorUser = typeof(IOperatorUser);

    protected virtual Guid GetCurrentlyLoggedInUserId()
    {
        var ctx = httpContextAccessor.HttpContext;
        if (ctx != null)
        {
            var uid = UserIsLockedOutFilterAttribute.GetUserId(ctx);
            return uid;
        }
        return default;
    }

    public static bool IgnoreOnSaveChanges { get; protected set; }

    void OnSaveChanges()
    {
        if (IgnoreOnSaveChanges)
            return;

        foreach (var entity in ChangeTracker.Entries())
        {
            switch (entity.State)
            {
                case EntityState.Modified:
                    if (entity.Entity is IUpdateTime u) // 设置更新时间
                    {
                        u.UpdateTime = DateTimeOffset.Now;
                    }
                    if (entity.Entity is IOperatorUserId operatorUserId) // 设置操作人
                    {
                        var uid = GetCurrentlyLoggedInUserId();
                        if (uid != default)
                            operatorUserId.OperatorUserId = uid;
                    }
                    break;
                case EntityState.Added:
                    if (entity.Entity is ICreateUserId createUserId) // 设置创建人
                    {
                        var uid = GetCurrentlyLoggedInUserId();
                        if (uid != default)
                            createUserId.CreateUserId = uid;
                    }
                    else if (entity.Entity is ICreateUserIdNullable createUserIdNullable)
                    {
                        var uid = GetCurrentlyLoggedInUserId();
                        if (uid != default)
                            createUserIdNullable.CreateUserId = uid;
                    }
                    break;
            }
        }
    }

    public sealed override int SaveChanges(bool acceptAllChangesOnSuccess)
    {
        OnSaveChanges();
        return base.SaveChanges(acceptAllChangesOnSuccess);
    }

    public sealed override Task<int> SaveChangesAsync(bool acceptAllChangesOnSuccess, CancellationToken cancellationToken = default)
    {
        OnSaveChanges();
        return base.SaveChangesAsync(acceptAllChangesOnSuccess, cancellationToken);
    }
}

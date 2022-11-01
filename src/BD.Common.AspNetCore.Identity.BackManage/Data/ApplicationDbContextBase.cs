using Microsoft.EntityFrameworkCore.Metadata;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace BD.Common.Data;

public abstract class ApplicationDbContextBase : DbContext, IApplicationDbContext
{
#pragma warning disable IDE0079 // 请删除不必要的忽略
#pragma warning disable CS8618 // 在退出构造函数时，不可为 null 的字段必须包含非 null 值。请考虑声明为可以为 null。
    public ApplicationDbContextBase(DbContextOptions options) : base(options)
#pragma warning restore CS8618 // 在退出构造函数时，不可为 null 的字段必须包含非 null 值。请考虑声明为可以为 null。
#pragma warning restore IDE0079 // 请删除不必要的忽略
    {

    }

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

    protected override void OnModelCreating(ModelBuilder b)
    {
        base.OnModelCreating(b);
        b.BuildEntities(AppendBuildEntities_);

        OnModelCreatingCore(b);
    }

    public static void OnModelCreatingCore(ModelBuilder b)
    {
        b.Entity<SysMenu>(p =>
        {
            p.HasMany(x => x.Children)
                .WithOne()
                .HasForeignKey(x => x.ParentId)
                .IsRequired(false)
                .OnDelete(DeleteBehavior.NoAction);

            p.HasMany(x => x.Buttons)
                .WithMany(x => x.Menus)
                .UsingEntity<SysMenuButton>(
                    x => x.HasOne(y => y.Button)
                            .WithMany(y => y.MenuButtons)
                            .HasForeignKey(y => y.ButtonId)
                            .OnDelete(DeleteBehavior.Cascade),
                    x => x.HasOne(y => y.Menu)
                            .WithMany(y => y.MenuButtons)
                            .HasForeignKey(y => y.MenuId)
                            .OnDelete(DeleteBehavior.NoAction),
                    x => x.HasKey(y => new { y.MenuId, y.ButtonId, y.TenantId })
                );
        });
        b.Entity<SysUserRole>(p =>
        {
            p.HasKey(x => new { x.UserId, x.RoleId, x.TenantId });
        });
        b.Entity<SysMenuButtonRole>(p =>
        {
            p.HasKey(x => new { x.ButtonId, x.RoleId, x.MenuId, x.TenantId, x.ControllerName });
        });
    }

    public static Action<EntityTypeBuilder>? AppendBuildEntitiesCore(ModelBuilder modelBuilder, IMutableEntityType entityType, Type type, Action<EntityTypeBuilder>? buildAction)
    {
        if (PCreateUser.IsAssignableFrom(type))
        {
            buildAction += p =>
            {
                p.HasOne(nameof(ICreateUser.CreateUser)).WithMany().HasForeignKey(nameof(ICreateUserId.CreateUserId));
            };
        }

        if (POperatorUser.IsAssignableFrom(type))
        {
            buildAction += p =>
            {
                p.HasOne(nameof(IOperatorUser.OperatorUser)).WithMany().HasForeignKey(nameof(IOperatorUserId.OperatorUserId));
            };
        }

        return buildAction;
    }

    Action<EntityTypeBuilder>? AppendBuildEntities_(ModelBuilder modelBuilder, IMutableEntityType entityType, Type type, Action<EntityTypeBuilder>? buildAction)
    {
        buildAction = AppendBuildEntitiesCore(modelBuilder, entityType, type, buildAction);
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

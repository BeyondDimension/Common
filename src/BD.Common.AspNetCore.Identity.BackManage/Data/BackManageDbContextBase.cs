using Microsoft.EntityFrameworkCore.Metadata;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace BD.Common.Data;

public partial class BackManageDbContextBase : IdentityDbContext<BMUser, IdentityRole<Guid>, Guid>, IApplicationDbContext<BMUser>
{
#pragma warning disable CS8618 // 在退出构造函数时，不可为 null 的字段必须包含非 null 值。请考虑声明为可以为 null。
    public BackManageDbContextBase(DbContextOptions<BackManageDbContextBase> options) : base(options)
#pragma warning restore CS8618 // 在退出构造函数时，不可为 null 的字段必须包含非 null 值。请考虑声明为可以为 null。
    {

    }

    protected override void OnModelCreating(ModelBuilder b)
    {
        base.OnModelCreating(b);
        this.ReNameAspNetIdentityByBackManageDbContext(b);
        b.BuildEntities(AppendBuildEntities);
    }

    protected virtual void AppendBuildEntities(ModelBuilder modelBuilder, IMutableEntityType entityType, Type type, Action<EntityTypeBuilder>? buildAction)
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
    }

    public static readonly Type PCreateUser = typeof(ICreateUser);
    public static readonly Type POperatorUser = typeof(IOperatorUser);
}

namespace BD.Common.Data;

public partial class BackManageDbContext : IdentityDbContext<BMUser, IdentityRole<Guid>, Guid>, IApplicationDbContext<BMUser>
{
#pragma warning disable CS8618 // 在退出构造函数时，不可为 null 的字段必须包含非 null 值。请考虑声明为可以为 null。
    public BackManageDbContext(DbContextOptions<BackManageDbContext> options) : base(options)
#pragma warning restore CS8618 // 在退出构造函数时，不可为 null 的字段必须包含非 null 值。请考虑声明为可以为 null。
    {

    }

    protected override void OnModelCreating(ModelBuilder b)
    {
        base.OnModelCreating(b);
        this.ReNameAspNetIdentityByBackManageDbContext(b);
        b.BuildEntities();
    }
}

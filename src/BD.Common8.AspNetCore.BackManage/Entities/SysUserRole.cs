namespace BD.Common8.AspNetCore.Entities;

/// <summary>
/// 权限控制相关实体类 - 角色用户关系
/// </summary>
[Table("BM_UserRoles")]
[EntityTypeConfiguration(typeof(EntityTypeConfiguration))]
public class SysUserRole : ITenant
{
    [Comment("租户 Id")]
    public Guid TenantId { get; set; }

    /// <summary>
    /// 用户 Id
    /// </summary>
    [Comment("用户 Id")]
    public Guid UserId { get; set; }

    /// <summary>
    /// 角色 Id
    /// </summary>
    [Comment("角色 Id")]
    public Guid RoleId { get; set; }

    public class EntityTypeConfiguration : IEntityTypeConfiguration<SysUserRole>
    {
        public void Configure(EntityTypeBuilder<SysUserRole> builder)
        {
            builder.HasKey(x => new { x.UserId, x.RoleId, x.TenantId });
        }
    }
}
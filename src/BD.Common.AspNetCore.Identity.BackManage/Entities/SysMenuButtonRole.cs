namespace BD.Common.Entities;

/// <summary>
/// 权限控制相关实体类 - 角色菜单按钮关系
/// </summary>
[Table("BM_MenuButtonRoles")]
public class SysMenuButtonRole : ITenant
{
    [Comment("系统租户 Id")]
    public Guid TenantId { get; set; }

    /// <summary>
    /// 系统角色 Id
    /// </summary>
    [Comment("系统角色 Id")]
    public Guid RoleId { get; set; }

    /// <summary>
    /// 系统菜单 Id
    /// </summary>
    [Comment("系统菜单 Id")]
    public Guid MenuId { get; set; }

    /// <summary>
    /// 系统按钮 Id
    /// </summary>
    [Comment("系统按钮 Id")]
    public Guid ButtonId { get; set; }

    /// <summary>
    /// 控制器名称
    /// </summary>
    [Required]
    [MaxLength(MaxLengths.LongName)]
    [Comment("控制器名称")]
    public string ControllerName { get; set; } = "";

    public class EntityTypeConfiguration : IEntityTypeConfiguration<SysMenuButtonRole>
    {
        public void Configure(EntityTypeBuilder<SysMenuButtonRole> builder)
        {
            builder.HasKey(x => new { x.ButtonId, x.RoleId, x.MenuId, x.TenantId, x.ControllerName });
        }
    }
}
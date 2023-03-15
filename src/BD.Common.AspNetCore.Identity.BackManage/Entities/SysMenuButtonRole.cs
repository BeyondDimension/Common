namespace BD.Common.Entities;

/// <summary>
/// 权限控制相关实体类 - 角色菜单按钮关系
/// </summary>
[Table("BM_MenuButtonRoles")]
public class SysMenuButtonRole : ITenant
{
    /// <summary>
    /// 系统租户ID
    /// </summary>
    [Comment("系统租户ID")]
    public Guid TenantId { get; set; }

    /// <summary>
    /// 系统角色ID
    /// </summary>
    [Comment("系统角色ID")]
    public Guid RoleId { get; set; }

    /// <summary>
    /// 系统菜单ID
    /// </summary>
    [Comment("系统菜单ID")]
    public Guid MenuId { get; set; }

    /// <summary>
    /// 系统按钮
    /// </summary>
    [Comment("系统按钮")]
    public Guid ButtonId { get; set; }

    /// <summary>
    /// 控制器名称
    /// </summary>
    [Required]
    [Comment("控制器名称")]
    public string ControllerName { get; set; } = "";
}
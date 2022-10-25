namespace BD.Common.Entities;

/// <summary>
/// 权限控制相关实体类 - 角色菜单按钮关系
/// </summary>
[Table(nameof(SysMenuButtonRole) + "s")]
public class SysMenuButtonRole
{
    public Guid TenantId { get; set; }

    public Guid RoleId { get; set; }

    public Guid MenuId { get; set; }

    public Guid ButtonId { get; set; }

    [Required]
    public string ControllerName { get; set; } = "";
}

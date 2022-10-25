namespace BD.Common.Entities;

/// <summary>
/// 权限控制相关实体类 - 菜单按钮关系
/// </summary>
[Table(nameof(SysMenuButton) + "s")]
public sealed class SysMenuButton : ITenant
{
    public Guid TenantId { get; set; }

    public Guid MenuId { get; set; }

    public Guid ButtonId { get; set; }
}

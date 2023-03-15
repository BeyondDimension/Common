namespace BD.Common.Entities;

/// <summary>
/// 权限控制相关实体类 - 菜单按钮关系
/// </summary>
[Table("BM_MenuButtons")]
public class SysMenuButton : ITenant
{
    /// <summary>
    /// 系统租户ID
    /// </summary>
    [Comment("系统租户ID")]
    public Guid TenantId { get; set; }

    /// <summary>
    /// 系统菜单ID
    /// </summary>
    [Comment("系统菜单ID")]
    public Guid MenuId { get; set; }

    /// <summary>
    /// 系统按钮ID
    /// </summary>
    [Comment("系统按钮ID")]
    public Guid ButtonId { get; set; }

    /// <inheritdoc cref="SysButton"/>
    public virtual SysButton? Button { get; set; }

    /// <inheritdoc cref="SysMenu"/>
    public virtual SysMenu? Menu { get; set; }
}
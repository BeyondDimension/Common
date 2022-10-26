namespace BD.Common.Entities;

/// <summary>
/// 权限控制相关实体类 - 系统按钮
/// </summary>
[Table("BM_Buttons")]
public class SysButton : TenantBaseEntity
{
    public const int MaxLength_Name = 20;

    /// <summary>
    /// 按钮名称
    /// </summary>
    [Required]
    [MaxLength(MaxLength_Name)]
    public string Name { get; set; } = string.Empty;

    /// <summary>
    /// 按钮类型
    /// </summary>
    [Required]
    public SysButtonType Type { get; set; }

    public virtual ICollection<SysMenu>? Menus { get; set; }

    public virtual ICollection<SysMenuButton>? MenuButtons { get; set; }
}

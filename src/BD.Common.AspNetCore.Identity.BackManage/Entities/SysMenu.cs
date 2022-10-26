namespace BD.Common.Entities;

/// <summary>
/// 权限控制相关实体类 - 系统菜单
/// </summary>
[Table("BM_Menus")]
public class SysMenu : TenantBaseEntity, IOrder
{
    /// <summary>
    /// 按钮多语言名称
    /// </summary>
    [Required]
    [MaxLength(128)]
    public string Key { get; set; } = null!;

    /// <summary>
    /// 按钮名称
    /// </summary>
    [Required]
    [MaxLength(200)]
    public string Name { get; set; } = null!;

    /// <summary>
    /// 父菜单 Id
    /// </summary>
    public Guid? ParentId { get; set; }

    /// <summary>
    /// 菜单相对 Url 路径
    /// </summary>
    [Required]
    [MaxLength(MaxLengths.Url)]
    public string Path { get; set; } = null!;

    /// <summary>
    /// 图标
    /// </summary>
    [MaxLength(128)]
    public string? Icon { get; set; }

    /// <summary>
    /// 排序
    /// </summary>
    public long Order { get; set; }

    /// <summary>
    /// 备注
    /// </summary>
    [MaxLength(MaxLengths.Remarks)]
    public string? Remark { get; set; }

    public virtual ICollection<SysMenu>? Children { get; set; }

    public virtual ICollection<SysButton>? Buttons { get; set; }

    public virtual ICollection<SysMenuButton>? MenuButtons { get; set; }
}

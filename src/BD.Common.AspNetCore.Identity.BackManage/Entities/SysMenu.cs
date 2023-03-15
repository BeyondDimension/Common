using BD.SteamPointShop.Entities.Abstractions;

namespace BD.Common.Entities;

/// <summary>
/// 权限控制相关实体类 - 系统菜单
/// </summary>
[Table("BM_Menus")]
public class SysMenu : TenantBaseEntityV2, INEWSEQUENTIALID, IOrder, IRemarks
{
    /// <summary>
    /// 按钮多语言名称
    /// </summary>
    [Required]
    [MaxLength(128)]
    [Comment("按钮多语言名称")]
    public string Key { get; set; } = null!;

    /// <summary>
    /// 菜单名称
    /// </summary>
    [Required]
    [MaxLength(200)]
    [Comment("菜单名称")]
    public string Name { get; set; } = null!;

    /// <summary>
    /// 父菜单Id
    /// </summary>
    [Comment("父菜单Id")]
    public Guid? ParentId { get; set; }

    /// <summary>
    /// 菜单Url地址
    /// </summary>
    [Required]
    [MaxLength(MaxLengths.Url)]
    [Comment("菜单Url地址")]
    public string Path { get; set; } = null!;

    /// <summary>
    /// 图标
    /// </summary>
    [MaxLength(128)]
    [Comment("图标")]
    public string? Icon { get; set; }

    /// <summary>
    /// 图标Url地址
    /// </summary>
    [Comment("图标Url地址")]
    [MaxLength(MaxLengths.Url)]
    public string? IconUrlAddress { get; set; }

    /// <summary>
    /// 排序
    /// </summary>
    [Comment("排序")]
    public long Order { get; set; }

    /// <summary>
    /// 状态
    /// </summary>
    [Comment("状态")]
    public SysMenuStatus Status { get; set; }

    /// <summary>
    /// 是否绝对路径
    /// </summary>
    [Comment("是否绝对路径")]
    public bool IsAbsolutePath { get; set; }

    /// <summary>
    /// 打开方式
    /// </summary>
    [Comment("打开方式")]
    public SysMenuOpenMethod OpenMethod { get; set; }

    /// <summary>
    /// 备注
    /// </summary>
    [MaxLength(MaxLengths.Remarks)]
    [Column("Remark")]
    [Comment("备注")]
    public string? Remarks { get; set; }

    /// <summary>
    /// 子级系统菜单
    /// </summary>
    public virtual ICollection<SysMenu>? Children { get; set; }

    /// <inheritdoc cref="SysButton"/>
    public virtual ICollection<SysButton>? Buttons { get; set; }

    /// <inheritdoc cref="SysMenuButton"/>
    public virtual ICollection<SysMenuButton>? MenuButtons { get; set; }
}
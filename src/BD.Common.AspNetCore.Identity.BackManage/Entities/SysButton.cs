namespace BD.Common.Entities;

/// <summary>
/// 权限控制相关实体类 - 系统按钮
/// </summary>
[Table("BM_Buttons")]
public class SysButton : TenantBaseEntityV2, INEWSEQUENTIALID, IDisable
{
    public const int MaxLength_Name = 20;

    /// <summary>
    /// 按钮名称
    /// </summary>
    [Required]
    [MaxLength(MaxLength_Name)]
    [Comment("按钮名称")]
    public string Name { get; set; } = string.Empty;

    /// <summary>
    /// 按钮样式
    /// </summary>
    [Comment("按钮样式")]
    public SysButtonStyle Style { get; set; }

    /// <summary>
    /// 按钮类型
    /// </summary>
    [Required]
    [Comment("按钮类型")]
    public SysButtonType Type { get; set; }

    [Comment("是否禁用")]
    public bool Disable { get; set; }

    /// <inheritdoc cref="SysMenu"/>
    public virtual ICollection<SysMenu>? Menus { get; set; }

    /// <inheritdoc cref="SysMenuButton"/>
    public virtual ICollection<SysMenuButton>? MenuButtons { get; set; }
}
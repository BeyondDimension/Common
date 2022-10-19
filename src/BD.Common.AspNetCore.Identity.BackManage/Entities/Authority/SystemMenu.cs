namespace BD.Common.Entities;

/// <summary>
/// 权限控制 - 系统菜单
/// </summary>
public class SystemMenu : TenantEntity<Guid>, IOrder
{
    public const int MaxLength_Title = 50;

    /// <summary>
    /// 按钮多语言名称
    /// </summary>
    [MaxLength(SharedMaxLengths.NickName)]
    [Required]
    public string Name { get; set; } = string.Empty;

    /// <summary>
    /// 按钮名称
    /// </summary>
    [Required]
    [MaxLength(MaxLength_Title)]
    public string Title { get; set; } = string.Empty;

    /// <summary>
    /// 父菜单ID
    /// </summary>
    public Guid? ParentId { get; set; }

    /// <summary>
    /// 菜单Url路径
    /// </summary>
    [Required]
    [MaxLength(SharedMaxLengths.Url)]
    public string Url { get; set; } = string.Empty;

    /// <summary>
    /// 图标
    /// </summary>
    [MaxLength(SharedMaxLengths.Url)]
    public string? Icon { get; set; }

    /// <summary>
    /// 排序
    /// </summary>
    public long Order { get; set; }

    /// <summary>
    /// 备注
    /// </summary>
    [MaxLength(SharedMaxLengths.Remarks)]
    public string? Remark { get; set; }
}

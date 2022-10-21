namespace BD.Common.Entities;

/// <summary>
/// 权限控制 - 系统菜单
/// </summary>
/// <typeparam name="TBMUser"></typeparam>
public class SystemMenu<TBMUser> :
    TenantEntity<Guid, TBMUser>,
    IOrder
    where TBMUser : BMUser
{
    public const int MaxLength_Title = 50;

    /// <summary>
    /// 按钮多语言名称
    /// </summary>
    [MaxLength(MaxLengths.NickName)]
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
    [MaxLength(MaxLengths.Url)]
    public string Url { get; set; } = string.Empty;

    /// <summary>
    /// 图标
    /// </summary>
    [MaxLength(MaxLengths.Url)]
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
}

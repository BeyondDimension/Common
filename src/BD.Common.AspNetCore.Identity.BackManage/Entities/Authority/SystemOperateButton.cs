namespace BD.Common.Entities;

/// <summary>
///  权限控制 - 系统按钮
/// </summary>
/// <typeparam name="TBMUser"></typeparam>
public class SystemOperateButton<TBMUser> :
    TenantEntity<Guid, TBMUser>
    where TBMUser : BMUser
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
    public SystemOperateButtonType Type { get; set; }

    /// <summary>
    /// 按钮名称_前端UI用来判断显示
    /// </summary>
    [Required]
    [MaxLength(MaxLength_Name)]
    public string ButtonId { get; set; }
}
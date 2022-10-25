namespace BD.Common.Entities;

/// <summary>
/// 权限控制相关实体类 - 系统按钮
/// </summary>
[Table(nameof(SysButton) + "s")]
public sealed class SysButton : TenantBaseEntity
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
}

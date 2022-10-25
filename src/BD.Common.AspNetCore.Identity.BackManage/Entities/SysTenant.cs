namespace BD.Common.Entities;

/// <summary>
/// 租户信息实体类
/// </summary>
[Table(nameof(SysTenant) + "s")]
public sealed class SysTenant : OperatorBaseEntity, ISoftDeleted
{
    public bool SoftDeleted { get; set; }

    /// <summary>
    /// 租户名称
    /// </summary>
    [Required]
    [MaxLength(MaxLengths.NickName)]
    public string Name { get; set; } = "";

    /// <summary>
    /// 联系人名称
    /// </summary>
    [MaxLength(MaxLengths.NickName)]
    public string? ContactName { get; set; }

    /// <summary>
    /// 联系人电话
    /// </summary>
    [MaxLength(PhoneNumberHelper.ChineseMainlandPhoneNumberLength)]
    public string? ContactPhone { get; set; }

    /// <summary>
    /// 地址
    /// </summary>
    [MaxLength(MaxLengths.RealityAddress)]
    public string? Address { get; set; }

    /// <summary>
    /// 邮箱
    /// </summary>
    [MaxLength(MaxLengths.Email)]
    public string? Email { get; set; }

    public static Guid AdministratorTenantId => Guid.ParseExact("00000000000000000000000000000001", "N");
}

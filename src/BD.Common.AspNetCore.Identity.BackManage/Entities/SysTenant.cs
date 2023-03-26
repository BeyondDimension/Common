namespace BD.Common.Entities;

/// <summary>
/// 租户信息实体类
/// </summary>
[Table("BM_Tenants")]
public class SysTenant : TenantBaseEntityV2, INEWSEQUENTIALID, IDisable, IRemarks
{
    /// <summary>
    /// 租户名称
    /// </summary>
    [Required]
    [MaxLength(MaxLengths.NickName)]
    [Comment("租户名称")]
    public string Name { get; set; } = "";

    /// <summary>
    /// 租户唯一编码
    /// </summary>
    [MaxLength(256)]
    [Comment("租户唯一编码")]
    public string? UniqueCode { get; set; }

    /// <summary>
    /// 联系人名称
    /// </summary>
    [MaxLength(MaxLengths.NickName)]
    [Comment("联系人名称")]
    public string? ContactName { get; set; }

    /// <summary>
    /// 联系人电话
    /// </summary>
    [MaxLength(PhoneNumberHelper.DatabaseMaxLength)]
    [Comment("联系人电话")]
    public string? ContactPhone { get; set; }

    /// <summary>
    /// 地址
    /// </summary>
    [MaxLength(MaxLengths.RealityAddress)]
    [Comment("地址")]
    public string? Address { get; set; }

    /// <summary>
    /// 邮箱
    /// </summary>
    [MaxLength(MaxLengths.Email)]
    [Comment("邮箱")]
    public string? Email { get; set; }

    /// <summary>
    /// 注册手机号
    /// </summary>
    [MaxLength(PhoneNumberHelper.DatabaseMaxLength)]
    [Comment("注册手机号")]
    public string? RegisterPhoneNumber { get; set; }

    /// <summary>
    /// 注册邮箱
    /// </summary>
    [MaxLength(MaxLengths.Email)]
    [Comment("注册邮箱")]
    public string? RegisterEmail { get; set; }

    /// <summary>
    /// 审核人 Id
    /// </summary>
    [Comment("审核人 Id")]
    public Guid? AuditorId { get; set; }

    /// <summary>
    /// 审核人
    /// </summary>
    [Comment("审核人")]
    [MaxLength(MaxLengths.NickName)]
    public string? Auditor { get; set; }

    /// <summary>
    /// 审核时间
    /// </summary>
    [Comment("审核时间")]
    public DateTimeOffset? AuditTime { get; set; }

    /// <summary>
    /// 审核状态
    /// </summary>
    [Comment("审核状态")]
    public SysTenantAuditStatus AuditStatus { get; set; }

    /// <summary>
    /// 审核备注
    /// </summary>
    [Comment("审核备注")]
    public string? AuditRemarks { get; set; }

    /// <summary>
    /// 授权开始时间
    /// </summary>
    [Comment("授权开始时间")]
    public DateTimeOffset AuthorizationStartTime { get; set; }

    /// <summary>
    /// 授权结束时间
    /// </summary>
    [Comment("授权结束时间")]
    public DateTimeOffset AuthorizationEndTime { get; set; }

    [Comment("是否禁用")]
    public bool Disable { get; set; }

    [Comment("备注")]
    public string? Remarks { get; set; }

    /// <summary>
    /// 是否为平台管理员
    /// </summary>
    [Comment("是否为平台管理员")]
    public bool IsPlatformAdministrator { get; set; }

    /// <summary>
    /// 管理员 TenantId
    /// </summary>
    public static Guid AdministratorTenantId => Guid.ParseExact("00000000000000000000000000000001", "N");
}
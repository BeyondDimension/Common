using BD.SteamPointShop.Entities.Abstractions;

namespace BD.Common.Entities;

/// <summary>
/// 租户信息实体类
/// </summary>
[Table("BM_Tenants")]
public class SysTenant : TenantBaseEntityV2, INEWSEQUENTIALID
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
    /// </sumary>
    [MaxLength(100)]
    [Comment("租户唯一编码")]
    public string UniqueCode { get; set; } = null!;

    /// <summary>
    /// 联系人名称
    /// </summary>
    [MaxLength(MaxLengths.NickName)]
    [Comment("联系人名称")]
    public string? ContactName { get; set; }

    /// <summary>
    /// 联系人电话
    /// </summary>
    [MaxLength(PhoneNumberHelper.ChineseMainlandPhoneNumberLength)]
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
    /// </sumary>
    [MaxLength(50)]
    [Comment("注册手机号")]
    public string? RegisterPhoneNumber { get; set; }

    /// <summary>
    /// 注册邮箱
    /// </sumary>
    [MaxLength(100)]
    [Comment("注册邮箱")]
    public string RegisterEmail { get; set; } = null!;

    /// <summary>
    /// 审核人Id
    /// </sumary>
    [Comment("审核人Id")]
    public Guid? AuditorId { get; set; }

    /// <summary>
    /// 审核人
    /// </sumary>
    [MaxLength(50)]
    [Comment("审核人")]
    public string? Auditor { get; set; }

    /// <summary>
    /// 审核时间
    /// </sumary>
    [Comment("审核时间")]
    public DateTimeOffset? ReviewTime { get; set; }

    /// <summary>
    /// 审核状态
    /// </sumary>
    [Comment("审核状态")]
    public SysTenantApprovalStatus? ApprovalStatus { get; set; }

    /// <summary>
    /// 审核备注
    /// </sumary>
    [MaxLength(MaxLengths.Remarks)]
    [Comment("审核备注")]
    public string? ReviewRemarks { get; set; }

    /// <summary>
    /// 授权开始时间
    /// </sumary>
    [Comment("授权开始时间")]
    public DateTimeOffset AuthorizationStartTime { get; set; }

    /// <summary>
    /// 授权结束时间
    /// </sumary>
    [Comment("授权结束时间")]
    public DateTimeOffset AuthorizationEndTime { get; set; }

    /// <summary>
    /// 状态
    /// </sumary>
    [Comment("状态")]
    public SysTenantStatus Status { get; set; }

    /// <summary>
    /// 备注
    /// </sumary>
    [MaxLength(MaxLengths.Remarks)]
    [Comment("备注")]
    public string? Remarks { get; set; }

    /// <summary>
    /// 是否为平台管理员
    /// </sumary>
    [Comment("是否为平台管理员")]
    public bool IsPlatformAdministrator { get; set; }

    /// <summary>
    /// 管理员TenantId
    /// </summary>
    public static Guid AdministratorTenantId => Guid.ParseExact("00000000000000000000000000000001", "N");
}
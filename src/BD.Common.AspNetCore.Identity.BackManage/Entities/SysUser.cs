namespace BD.Common.Entities;

/// <summary>
/// 系统用户(多租户)实体类
/// </summary>
[Table("BM_Users")]
public class SysUser : TenantBaseEntityV2, INEWSEQUENTIALID, IJWTUser, IRemarks, INickName, IPhoneNumber, IDisable
{
    /// <summary>
    /// 组织架构 Id
    /// </summary>
    [Comment("组织架构 Id")]
    public Guid? OrganizationalId { get; set; }

    /// <summary>
    /// 用户名
    /// </summary>
    [Required]
    [Comment("用户名")]
    [MaxLength(256)]
    public string UserName { get; set; } = string.Empty;

    /// <summary>
    /// 规范化用户名
    /// </summary>
    [Required]
    [Comment("规范化用户名")]
    [MaxLength(256)]
    public string NormalizedUserName { get; set; } = string.Empty;

    /// <summary>
    /// 昵称
    /// </summary>
    [Comment("昵称")]
    public string? NickName { get; set; }

    /// <summary>
    /// 密码哈希
    /// </summary>
    [Comment("密码哈希")]
    public string? PasswordHash { get; set; }

    /// <summary>
    /// 用户锁定结束时的时间
    /// </summary>
    [Comment("用户锁定结束时的时间")]
    public DateTimeOffset? LockoutEnd { get; set; }

    /// <summary>
    /// 用户是否被锁定
    /// </summary>
    [Comment("用户是否被锁定")]
    public bool LockoutEnabled { get; set; }

    /// <summary>
    /// 用户的登录尝试失败次数
    /// </summary>
    [Comment("用户的登录尝试失败次数")]
    public int AccessFailedCount { get; set; }

    /// <summary>
    /// 刷新 Token 值
    /// </summary>
    [StringLength(MaxLengths.Url)]
    [Comment("刷新 Token 值")]
    public string? RefreshToken { get; set; }

    /// <summary>
    /// 刷新 Token 值有效期
    /// </summary>
    [Comment("刷新 Token 值有效期")]
    public DateTimeOffset RefreshExpiration { get; set; }

    /// <summary>
    /// 禁止在此时间之前刷新
    /// </summary>
    [Comment("禁止在此时间之前刷新")]
    public DateTimeOffset NotBefore { get; set; }

    [MaxLength(PhoneNumberHelper.DatabaseMaxLength)]
    [Comment("手机号码")]
    public string? PhoneNumber { get; set; }

    /// <summary>
    /// 邮箱
    /// </summary>
    [Comment("邮箱")]
    [MaxLength(MaxLengths.Email)]
    public string? Email { get; set; }

    /// <summary>
    /// 性别
    /// </summary>
    [Comment("性别")]
    public Gender Gender { get; set; }

    /// <summary>
    /// 备注
    /// </summary>
    [Comment("备注")]
    public string? Remarks { get; set; }

    [Comment("是否禁用")]
    public bool Disable { get; set; }

    /// <summary>
    /// 并发令牌 https://learn.microsoft.com/zh-cn/ef/core/modeling/concurrency?tabs=data-annotations#timestamprowversion
    /// </summary>
    [Timestamp]
    [Comment("并发令牌")]
    public byte[]? Timestamp { get; set; }
}
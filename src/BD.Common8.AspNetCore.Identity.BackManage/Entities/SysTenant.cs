namespace BD.Common8.AspNetCore.Entities;

#pragma warning disable SA1600 // Elements should be documented

/// <summary>
/// 租户信息实体类
/// </summary>
[Table("BM_Tenants")]
[EntityTypeConfiguration(typeof(EntityTypeConfiguration))]
public sealed class SysTenant : OperatorBaseEntity, INEWSEQUENTIALID, IDisable, IRemarks, ISoftDeleted
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
    [Comment("租户唯一编码")]
    public string? UniqueCode { get; set; }

    /// <summary>
    /// 联系人
    /// </summary>
    [MaxLength(MaxLengths.NickName)]
    [Comment("联系人")]
    public string? Contact { get; set; }

    /// <summary>
    /// 联系人电话
    /// </summary>
    [MaxLength(PhoneNumberHelper.DatabaseMaxLength)]
    [Comment("联系人电话")]
    public string? ContactPhoneNumber { get; set; }

    /// <summary>
    /// 注册手机号
    /// </summary>
    [MaxLength(PhoneNumberHelper.DatabaseMaxLength)]
    [Comment("注册手机号")]
    public string? RegisterPhoneNumber { get; set; }

    /// <summary>
    /// 地址
    /// </summary>
    [MaxLength(MaxLengths.RealityAddress)]
    [Comment("地址")]
    public string? Address { get; set; }

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
    public SysUser? Auditor { get; set; }

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

    [Comment("是否软删除")]
    public bool SoftDeleted { get; set; }

    /// <summary>
    /// 并发令牌 https://learn.microsoft.com/zh-cn/ef/core/modeling/concurrency?tabs=data-annotations#timestamprowversion
    /// </summary>
    [Timestamp]
    [Comment("并发令牌")]
    public byte[]? Timestamp { get; set; }

    public sealed class EntityTypeConfiguration : EntityTypeConfiguration<SysTenant>
    {
        public sealed override void Configure(EntityTypeBuilder<SysTenant> builder)
        {
            base.Configure(builder);

            builder.HasOne(x => x.Auditor)
                .WithMany(x => x.AuditorTenants)
                .HasForeignKey(x => x.AuditorId)
                .OnDelete(DeleteBehavior.SetNull);
        }
    }

    /// <summary>
    /// 管理员 TenantId
    /// </summary>
    public static Guid AdministratorTenantId => Guid.ParseExact("00000000000000000000000000000001", "N");
}
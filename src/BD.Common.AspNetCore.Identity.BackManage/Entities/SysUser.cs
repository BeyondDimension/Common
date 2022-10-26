namespace BD.Common.Entities;

/// <summary>
/// 系统用户(多租户)实体类
/// </summary>
[Table(nameof(SysUser) + "s")]
public class SysUser : Entity<Guid>, ITenant, ICreationTime, ICreateUserIdNullable, IUpdateTime, IOperatorUserId, IJWTUser, ISoftDeleted
{
    [Required]
    [StringLength(256)]
    public string UserName { get; set; } = "";

    [Required]
    [StringLength(256)]
    public string NormalizedUserName { get; set; } = "";

    public string? PasswordHash { get; set; }

    public DateTimeOffset? LockoutEnd { get; set; }

    public bool LockoutEnabled { get; set; }

    public int AccessFailedCount { get; set; }

    public Guid TenantId { get; set; }

    public DateTimeOffset CreationTime { get; set; }

    public Guid? CreateUserId { get; set; }

    public DateTimeOffset UpdateTime { get; set; }

    public Guid? OperatorUserId { get; set; }

    [StringLength(MaxLengths.Url)]
    public string? RefreshToken { get; set; }

    public DateTimeOffset RefreshExpiration { get; set; }

    public DateTimeOffset NotBefore { get; set; }

    public bool SoftDeleted { get; set; }

    /// <summary>
    /// https://learn.microsoft.com/zh-cn/ef/core/modeling/concurrency?tabs=data-annotations#timestamprowversion
    /// </summary>
    [Timestamp]
    public byte[]? Timestamp { get; set; }
}

namespace BD.Common.Entities;

/// <summary>
/// 系统角色(权限)实体类
/// </summary>
[Table(nameof(SysRole) + "s")]
public class SysRole : Entity<Guid>, ITenant, ICreationTime, ICreateUserIdNullable, IOrder, IUpdateTime, IOperatorUserId
{
    [Required]
    [StringLength(256)]
    public string Name { get; set; } = "";

    [Required]
    [StringLength(256)]
    public string NormalizedName { get; set; } = "";

    [MaxLength(MaxLengths.Describe)]
    public string? Describe { get; set; }

    public Guid UserId { get; set; }

    public Guid TenantId { get; set; }

    public DateTimeOffset CreationTime { get; set; }

    public Guid? CreateUserId { get; set; }

    public long Order { get; set; }

    public DateTimeOffset UpdateTime { get; set; }

    public Guid? OperatorUserId { get; set; }

    /// <summary>
    /// https://learn.microsoft.com/zh-cn/ef/core/modeling/concurrency?tabs=data-annotations#timestamprowversion
    /// </summary>
    [Timestamp]
    public byte[]? Timestamp { get; set; }
}

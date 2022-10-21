namespace BD.Common.Entities;

public class BMRoleEntity : IdentityRole<Guid>,
    ICreationTime,
    ICreateUserId,
    IOrder,
    ITenant,
    IUpdateTime,
    IOperatorUserId
{

    [MaxLength(MaxLengths.Describe)]
    public string Describe { get; set; } = string.Empty;

    public long Order { get; set; }

    /// <summary>
    /// 更新时间
    /// </summary>
    public DateTimeOffset UpdateTime { get; set; }

    /// <summary>
    /// 操作人
    /// </summary>
    public Guid? OperatorUserId { get; set; }

    /// <summary>
    /// 创建时间
    /// </summary>
    public DateTimeOffset CreationTime { get; set; }

    /// <summary>
    /// 创建人
    /// </summary>
    public Guid CreateUserId { get; set; }

    /// <summary>
    /// 租户
    /// </summary>
    public Guid TenantId { get; set; }
}

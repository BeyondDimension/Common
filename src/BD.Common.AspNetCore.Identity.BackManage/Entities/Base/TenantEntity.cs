namespace BD.Common.Entities;

/// <summary>
/// 租户相关类
/// </summary>
/// <typeparam name="TPrimaryKey"></typeparam>
public class TenantEntity<TPrimaryKey> : OperatorEntity<TPrimaryKey>, ISoftDeleted, ITenantId
    where TPrimaryKey : notnull, IEquatable<TPrimaryKey>
{
    /// <summary>
    /// 是否删除
    /// </summary>
    public bool SoftDeleted { get; set; }

    /// <summary>
    /// 租户
    /// </summary>
    public Guid TenantId { get; set; }
}
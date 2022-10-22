namespace BD.Common.Entities;

/// <summary>
/// 租户相关类
/// </summary>
/// <typeparam name="TPrimaryKey"></typeparam>
/// <typeparam name="TBMUser"></typeparam>
public abstract class TenantEntity<TPrimaryKey, TBMUser> :
    OperatorEntity<TPrimaryKey, TBMUser>,
    ISoftDeleted, ITenant
    where TPrimaryKey : notnull, IEquatable<TPrimaryKey>
    where TBMUser : BMUser
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
namespace BD.Common.Entities.Abstractions;

public abstract class OperatorEntity<TPrimaryKey, TBMUser> :
    CreationEntity<TPrimaryKey, TBMUser>,
    IUpdateTime,
    IOperatorUser<TBMUser>,
    IOperatorUserId
    where TPrimaryKey : notnull, IEquatable<TPrimaryKey>
    where TBMUser : BMUser
{
    /// <summary>
    /// 更新时间
    /// </summary>
    public DateTimeOffset UpdateTime { get; set; }

    /// <summary>
    /// 操作人
    /// </summary>
    public Guid? OperatorUserId { get; set; }

    /// <summary>
    /// 操作人详情
    /// </summary>
    public virtual TBMUser? OperatorUser { get; set; }
}


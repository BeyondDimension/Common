// ReSharper disable once CheckNamespace
namespace BD.Common.Entities.Abstractions;

/// <summary>
/// 基类实体 - 包含修改时间与操作人与创建时间与创建人
/// </summary>
/// <typeparam name="TPrimaryKey"></typeparam>
public abstract class OperatorBaseEntity<TPrimaryKey> :
    CreationBaseEntity<TPrimaryKey>,
    IUpdateTime,
    IOperatorUser,
    IOperatorUserId
    where TPrimaryKey : notnull, IEquatable<TPrimaryKey>
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
    public virtual SysUser? OperatorUser { get; set; }
}

/// <inheritdoc cref="OperatorBaseEntity{TPrimaryKey}"/>
public abstract class OperatorBaseEntity : OperatorBaseEntity<Guid>
{

}
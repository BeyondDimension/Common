// ReSharper disable once CheckNamespace
namespace BD.Common.Entities.Abstractions;

/// <summary>
/// 基类实体 - 包含创建时间与创建人
/// </summary>
/// <typeparam name="TPrimaryKey"></typeparam>
[Obsolete("use V2")]
public abstract class CreationBaseEntity<TPrimaryKey> :
    Entity<TPrimaryKey>,
    ICreationTime,
    ICreateUser,
    ICreateUserId
    where TPrimaryKey : notnull, IEquatable<TPrimaryKey>
{
    /// <summary>
    /// 创建时间
    /// </summary>
    public DateTimeOffset CreationTime { get; set; }

    /// <summary>
    /// 创建人
    /// </summary>
    public Guid CreateUserId { get; set; }

    /// <summary>
    /// 创建人关联
    /// </summary>
    public virtual SysUser? CreateUser { get; set; }

}

/// <inheritdoc cref="CreationBaseEntity{TPrimaryKey}"/>
[Obsolete("use V2")]
public abstract class CreationBaseEntity : CreationBaseEntity<Guid>
{

}
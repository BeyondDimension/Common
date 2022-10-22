namespace BD.Common.Entities.Abstractions;

public abstract class CreationEntity<TPrimaryKey, TBMUser> :
    Entity<TPrimaryKey>,
    ICreationTime,
    ICreateUser<TBMUser>,
    ICreateUserId
    where TPrimaryKey : notnull, IEquatable<TPrimaryKey>
    where TBMUser : BMUser
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
    public virtual TBMUser? CreateUser { get; set; }

}

namespace BD.Common8.Entities.Abstractions;

/// <summary>
/// (数据库表)实体模型抽象基类
/// </summary>
/// <typeparam name="TPrimaryKey"></typeparam>
public abstract class Entity<TPrimaryKey> : IEntity<TPrimaryKey> where TPrimaryKey : notnull, IEquatable<TPrimaryKey>
{
    /// <summary>
    /// Id，即主键
    /// </summary>
    [Key]
    public virtual TPrimaryKey Id { get; set; } = default!;
}

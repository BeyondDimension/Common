// ReSharper disable once CheckNamespace

namespace BD.Common.Entities.Abstractions;

public abstract class Entity<TPrimaryKey> : IEntity<TPrimaryKey> where TPrimaryKey : notnull, IEquatable<TPrimaryKey>
{
    [Key]
#pragma warning disable CS8618 // 在退出构造函数时，不可为 null 的字段必须包含非 null 值。请考虑声明为可以为 null。
    public virtual TPrimaryKey Id { get; set; }
#pragma warning restore CS8618 // 在退出构造函数时，不可为 null 的字段必须包含非 null 值。请考虑声明为可以为 null。
}

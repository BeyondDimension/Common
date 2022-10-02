// ReSharper disable once CheckNamespace
namespace BD.Common.Models.Abstractions;

public abstract class KeyModel<TPrimaryKey> : IKeyModel<TPrimaryKey> where TPrimaryKey : notnull, IEquatable<TPrimaryKey>
{
#pragma warning disable CS8618 // 在退出构造函数时，不可为 null 的字段必须包含非 null 值。请考虑声明为可以为 null。
    public virtual TPrimaryKey Id { get; set; }
#pragma warning restore CS8618 // 在退出构造函数时，不可为 null 的字段必须包含非 null 值。请考虑声明为可以为 null。
}

public interface IKeyModel<TPrimaryKey> where TPrimaryKey : notnull, IEquatable<TPrimaryKey>
{
    TPrimaryKey Id { get; set; }
}
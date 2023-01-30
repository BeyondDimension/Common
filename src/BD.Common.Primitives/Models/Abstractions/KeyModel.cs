// ReSharper disable once CheckNamespace
namespace BD.Common.Models.Abstractions;

#if !BLAZOR
[MPObj]
#endif
public abstract partial class KeyModel<TPrimaryKey> : IKeyModel<TPrimaryKey> where TPrimaryKey : notnull, IEquatable<TPrimaryKey>
{
#if !BLAZOR
    /// <summary>
    /// 最后一个 MessagePack 序列化 下标，继承自此类，新增需要序列化的字段/属性，标记此值+1，+2
    /// </summary>
    protected const int LastMKeyIndex = 0;
#endif

#if !BLAZOR
    [MPKey(LastMKeyIndex)]
    [MP2Key(LastMKeyIndex)]
#endif
#pragma warning disable CS8618 // 在退出构造函数时，不可为 null 的字段必须包含非 null 值。请考虑声明为可以为 null。
    public virtual TPrimaryKey Id { get; set; }
#pragma warning restore CS8618 // 在退出构造函数时，不可为 null 的字段必须包含非 null 值。请考虑声明为可以为 null。
}

public interface IKeyModel<TPrimaryKey> where TPrimaryKey : notnull, IEquatable<TPrimaryKey>
{
    TPrimaryKey Id { get; set; }
}
namespace BD.Common8.Primitives.Models.Abstractions;

#pragma warning disable SA1600 // Elements should be documented

[MPObj]
public abstract partial class KeyModel<TPrimaryKey> : IKeyModel<TPrimaryKey> where TPrimaryKey : notnull, IEquatable<TPrimaryKey>
{
    /// <summary>
    /// 最后一个 MessagePack 序列化 下标，继承自此类，新增需要序列化的字段/属性，标记此值+1，+2
    /// </summary>
    protected const int LastMKeyIndex = 0;

    [MPKey(LastMKeyIndex)]
    [MP2Key(LastMKeyIndex)]
    public virtual TPrimaryKey Id { get; set; } = default!;
}

public interface IKeyModel<TPrimaryKey> where TPrimaryKey : notnull, IEquatable<TPrimaryKey>
{
    TPrimaryKey Id { get; set; }
}
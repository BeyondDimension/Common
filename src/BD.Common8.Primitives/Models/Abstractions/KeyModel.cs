namespace BD.Common8.Primitives.Models.Abstractions;

/// <summary>
/// 带有主键的键模型抽象类，实现了<see cref="IKeyModel{TPrimaryKey}"/> 接口
/// </summary>
/// <typeparam name="TPrimaryKey">主键的类型</typeparam>
[MPObj]
public abstract partial class KeyModel<TPrimaryKey> : IKeyModel<TPrimaryKey> where TPrimaryKey : notnull, IEquatable<TPrimaryKey>
{
    /// <summary>
    /// 最后一个 MessagePack 序列化 下标，继承自此类，新增需要序列化的字段/属性，标记此值+1，+2
    /// </summary>
    protected const int LastMKeyIndex = 0;

    /// <inheritdoc/>
    [MPKey(LastMKeyIndex)]
    [MP2Key(LastMKeyIndex)]
    public virtual TPrimaryKey Id { get; set; } = default!;
}

/// <summary>
/// 主键模型接口，定义了具备主键的属性
/// </summary>
/// <typeparam name="TPrimaryKey">主键的类型</typeparam>
public interface IKeyModel<TPrimaryKey> where TPrimaryKey : notnull, IEquatable<TPrimaryKey>
{
    /// <summary>
    /// 获取或设置模型的主键
    /// </summary>
    TPrimaryKey Id { get; set; }
}
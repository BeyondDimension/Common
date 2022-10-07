// ReSharper disable once CheckNamespace
namespace BD.Common.Columns;

/// <summary>
/// 排序
/// </summary>
public interface IOrder
{
    /// <inheritdoc cref="IOrder"/>
    long Order { get; set; }

    const string SequenceStartsAt = "SequenceStartsAt";
}

/// <summary>
/// 是否置顶
/// </summary>
public interface IIsTop
{
    /// <inheritdoc cref="IIsTop"/>
    bool IsTop { get; set; }
}
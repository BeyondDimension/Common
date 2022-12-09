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

/// <summary>
/// 排序（兼容之前的数据，排序使用 Int32，且不生成序列）
/// </summary>
public interface IOrderInt32
{
    /// <inheritdoc cref="IOrder"/>
    int Order { get; set; }
}
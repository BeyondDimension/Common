namespace BD.Common8.Columns;

/// <summary>
/// 排序
/// </summary>
public interface IOrder
{
    /// <inheritdoc cref="IOrder"/>
    long Order { get; set; }

    /// <summary>
    /// 序列起始位置
    /// </summary>
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
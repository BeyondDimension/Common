using BD.Common8.Models.Abstractions;
using System.Diagnostics;

namespace BD.Common8.Models;

/// <inheritdoc cref="IPagedModel"/>
[global::MessagePack.MessagePackObject]
[global::MemoryPack.MemoryPackable(global::MemoryPack.SerializeLayout.Explicit)]
[Serializable]
[DebuggerDisplay("{DebuggerDisplay(),nq}")]
public partial class PagedModel<T> : IPagedModel<T>, IReadOnlyPagedModel<T>
{
    string DebuggerDisplay() => $"Current: {Current}, Total: {Total}, Count: {mDataSource?.Length ?? 0}, PageSize: {PageSize}";

    /// <summary>
    /// 数据源数组
    /// </summary>
    T[]? mDataSource;

    /// <summary>
    /// 获取或设置数据源数组
    /// </summary>
    [global::MessagePack.Key(0)]
    [global::MemoryPack.MemoryPackOrder(0)]
    public T[] DataSource
    {
        get
        {
            mDataSource ??= [];
            return mDataSource;
        }

        set => mDataSource = value;
    }

    /// <summary>
    /// 获取或设置当前页数
    /// </summary>
    [global::MessagePack.Key(1)]
    [global::MemoryPack.MemoryPackOrder(1)]
    public int Current { get; set; } = IPagedModel.DefaultCurrent;

    /// <summary>
    /// 获取或设置分页大小
    /// </summary>
    [global::MessagePack.Key(2)]
    [global::MemoryPack.MemoryPackOrder(2)]
    public int PageSize { get; set; } = IPagedModel.DefaultPageSize;

    /// <summary>
    /// 获取或设置总记录数
    /// </summary>
    [global::MessagePack.Key(3)]
    [global::MemoryPack.MemoryPackOrder(3)]
    public int Total { get; set; }

    /// <summary>
    /// 判断对象是否有值
    /// </summary>
    bool IExplicitHasValue.ExplicitHasValue()
    {
        return Total >= 0 && PageSize > 0 && Current > 0;
    }

    /// <summary>
    /// 获取数据源的只读列表
    /// </summary>
    IReadOnlyList<T> IReadOnlyPagedModel<T>.DataSource => DataSource;

    /// <inheritdoc cref="DataSource"/>
    IList<T> IPagedModel<T>.DataSource
    {
        get => DataSource;
        set => DataSource = value is T[] array ? array : value.ToArray();
    }
}
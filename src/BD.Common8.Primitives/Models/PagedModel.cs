namespace BD.Common8.Primitives.Models;

#pragma warning disable SA1600 // Elements should be documented

/// <inheritdoc cref="IPagedModel"/>
[MPObj]
[MP2Obj(MP2SerializeLayout.Explicit)]
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
    [MPKey(0)]
    [MP2Key(0)]
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
    [MPKey(1)]
    [MP2Key(1)]
    public int Current { get; set; } = IPagedModel.DefaultCurrent;

    /// <summary>
    /// 获取或设置分页大小
    /// </summary>
    [MPKey(2)]
    [MP2Key(2)]
    public int PageSize { get; set; } = IPagedModel.DefaultPageSize;

    /// <summary>
    /// 获取或设置总记录数
    /// </summary>
    [MPKey(3)]
    [MP2Key(3)]
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

    IList<T> IPagedModel<T>.DataSource
    {
        get => DataSource;
        set => DataSource = value is T[] array ? array : value.ToArray();
    }
}
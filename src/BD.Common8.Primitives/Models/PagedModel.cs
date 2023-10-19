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

    T[]? mDataSource;

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

    [MPKey(1)]
    [MP2Key(1)]
    public int Current { get; set; } = IPagedModel.DefaultCurrent;

    [MPKey(2)]
    [MP2Key(2)]
    public int PageSize { get; set; } = IPagedModel.DefaultPageSize;

    [MPKey(3)]
    [MP2Key(3)]
    public int Total { get; set; }

    bool IExplicitHasValue.ExplicitHasValue()
    {
        return Total >= 0 && PageSize > 0 && Current > 0;
    }

    IReadOnlyList<T> IReadOnlyPagedModel<T>.DataSource => DataSource;

    IList<T> IPagedModel<T>.DataSource
    {
        get => DataSource;
        set => DataSource = value is T[] array ? array : value.ToArray();
    }
}
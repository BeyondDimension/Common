// ReSharper disable once CheckNamespace
namespace BD.Common.Models;

/// <inheritdoc cref="IPagedModel"/>
[MPObj]
[Serializable]
[DebuggerDisplay("{DebuggerDisplay(),nq}")]
public class PagedModel<T> : IPagedModel<T>, IReadOnlyPagedModel<T>
{
    string DebuggerDisplay() => $"Current: {Current}, Total: {Total}, Count: {mDataSource?.Length ?? 0}, PageSize: {PageSize}";

    T[]? mDataSource;

    [MPKey(0)]
    public T[] DataSource
    {
        get
        {
            mDataSource ??= Array.Empty<T>();
            return mDataSource;
        }

        set => mDataSource = value;
    }

    [MPKey(1)]
    public int Current { get; set; } = IPagedModel.DefaultCurrent;

    [MPKey(2)]
    public int PageSize { get; set; } = IPagedModel.DefaultPageSize;

    [MPKey(3)]
    public int Total { get; set; }

    bool IExplicitHasValue.ExplicitHasValue()
    {
        return Total >= 0 && PageSize > 0 && Current > 0;
        //&&
        //Current <= ((IPagedModel)this).PageCount &&
        //mDataSource.Any_Nullable();
    }

    IReadOnlyList<T> IReadOnlyPagedModel<T>.DataSource => DataSource;

    IList<T> IPagedModel<T>.DataSource
    {
        get => DataSource;
        set => DataSource = value is T[] array ? array : value.ToArray();
    }
}
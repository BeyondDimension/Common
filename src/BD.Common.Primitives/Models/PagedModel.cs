// ReSharper disable once CheckNamespace
namespace BD.Common.Models;

/// <inheritdoc cref="IPagedModel"/>
[Serializable]
[DebuggerDisplay("{DebuggerDisplay(),nq}")]
public class PagedModel<T> : IPagedModel<T>, IReadOnlyPagedModel<T>
{
    string DebuggerDisplay() => $"Current: {Current}, Total: {Total}, Count: {mDataSource?.Length ?? 0}, PageSize: {PageSize}";

    T[]? mDataSource;

    public T[] DataSource
    {
        get
        {
            mDataSource ??= Array.Empty<T>();
            return mDataSource;
        }

        set => mDataSource = value;
    }

    public int Current { get; set; } = IPagedModel.DefaultCurrent;

    public int PageSize { get; set; } = IPagedModel.DefaultPageSize;

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
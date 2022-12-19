// ReSharper disable once CheckNamespace
namespace BD.Common.Models;

/// <inheritdoc cref="IPagedModel"/>
#if !BLAZOR
[MPObj]
#endif
[Serializable]
[DebuggerDisplay("{DebuggerDisplay(),nq}")]
public class PagedModel<T> : IPagedModel<T>, IReadOnlyPagedModel<T>
{
    string DebuggerDisplay() => $"Current: {Current}, Total: {Total}, Count: {mDataSource?.Length ?? 0}, PageSize: {PageSize}";

    T[]? mDataSource;

#if !BLAZOR
    [MPKey(0)]
#endif
    public T[] DataSource
    {
        get
        {
            mDataSource ??= Array.Empty<T>();
            return mDataSource;
        }

        set => mDataSource = value;
    }

#if !BLAZOR
    [MPKey(1)]
#endif
    public int Current { get; set; } = IPagedModel.DefaultCurrent;

#if !BLAZOR
    [MPKey(2)]
#endif
    public int PageSize { get; set; } = IPagedModel.DefaultPageSize;

#if !BLAZOR
    [MPKey(3)]
#endif
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
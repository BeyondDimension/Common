namespace System.Collections.Generic;

/// <summary>
/// 空的异步迭代器
/// </summary>
/// <typeparam name="T"></typeparam>
public readonly struct EmptyAsyncEnumerable<T> : IAsyncEnumerable<T>, IAsyncEnumerator<T>
{
    /// <inheritdoc/>
    public readonly T Current => default!;

    /// <inheritdoc/>
    public readonly ValueTask DisposeAsync()
    {
#if NET5_0_OR_GREATER
        return ValueTask.CompletedTask;
#else
        return default(ValueTask);
#endif
    }

    /// <inheritdoc/>
    public readonly IAsyncEnumerator<T> GetAsyncEnumerator(CancellationToken cancellationToken = default)
    {
        return this;
    }

    /// <inheritdoc/>
    public readonly ValueTask<bool> MoveNextAsync()
    {
        return new(false);
    }

    /// <summary>
    /// 将迭代器转换为异步迭代器
    /// </summary>
    /// <param name="values"></param>
    /// <returns></returns>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static async IAsyncEnumerable<T> ToAsyncEnumerable(IEnumerable<T> values)
    {
#if NET5_0_OR_GREATER
        await ValueTask.CompletedTask;
#else
        await default(ValueTask);
#endif
        foreach (var item in values)
        {
            yield return item;
        }
    }

    /// <summary>
    /// 将数组转换为异步迭代器
    /// </summary>
    /// <param name="values"></param>
    /// <returns></returns>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static IAsyncEnumerable<T> ToAsyncEnumerable(params T[] values)
        => ToAsyncEnumerable(values.AsEnumerable());
}
// ReSharper disable once CheckNamespace
namespace System;

public static class EnumerableExtensions
{
    /// <summary>
    /// 通过使用相应类型的默认相等比较器对序列的元素进行比较，以确定两个序列是否相等
    /// </summary>
    /// <typeparam name="TSource">输入序列中的元素的类型</typeparam>
    /// <param name="first">一个用于比较 second 的 <see cref="IEnumerable{T}"/></param>
    /// <param name="second">要与第一个序列进行比较的 <see cref="IEnumerable{T}"/></param>
    /// <returns></returns>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static bool SequenceEqual_Nullable<TSource>([NotNullWhen(true)] this IEnumerable<TSource>? first, IEnumerable<TSource>? second)
        => (first == null) ? (second == null) : ((second == null) ? (first == null) : first.SequenceEqual(second));

    /// <summary>
    /// 确定序列是否包含任何元素
    /// </summary>
    /// <typeparam name="TSource">source 的元素类型</typeparam>
    /// <param name="source">要检查是否为空的 <see cref="IEnumerable{T}"/></param>
    /// <returns>如果源序列包含任何元素，则为 <see langword="true"/>；否则为 <see langword="false"/></returns>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static bool Any_Nullable<TSource>([NotNullWhen(true)] this IEnumerable<TSource>? source) => source is not null && source.Any();

    /// <summary>
    /// 确定序列是否包含任何元素(带条件)
    /// </summary>
    /// <typeparam name="TSource"></typeparam>
    /// <param name="source"></param>
    /// <param name="predicate"></param>
    /// <returns></returns>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static bool Any_Nullable<TSource>([NotNullWhen(true)] this IEnumerable<TSource>? source, Func<TSource, bool> predicate) => source is not null && source.Any(predicate);

    /// <summary>
    /// 反转键值对
    /// </summary>
    /// <typeparam name="TValue"></typeparam>
    /// <typeparam name="TKey"></typeparam>
    /// <param name="source"></param>
    /// <returns></returns>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static Dictionary<TKey, TValue> ReverseKeyValue<TValue, TKey>(this IEnumerable<KeyValuePair<TValue, TKey>> source) where TKey : notnull => source.ToDictionary(k => k.Value, v => v.Key);

    /// <inheritdoc cref="List{T}.AddRange(IEnumerable{T})"/>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static void AddRange<T>(this IList<T> ts, IEnumerable<T> collection)
    {
        if (ts is List<T> list)
        {
            list.AddRange(collection);
        }
        else
        {
            if (ts.IsReadOnly) throw new NotSupportedException(SR.NotSupported_FixedSizeCollection);
            var c = collection == ts ? collection.ToArray() : collection;
            foreach (var item in c)
            {
                ts.Add(item);
            }
        }
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static IEnumerable<TSource> Distinct<TSource, TKey>(this IEnumerable<TSource> source, Func<TSource, TKey> keySelector)
    {
        var set = new HashSet<TKey>(EqualityComparer<TKey>.Default);
        foreach (var item in source)
        {
            var key = keySelector(item);
            if (set.Add(key)) yield return item;
        }
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static IEnumerable<TSource> DistinctBy<TSource, TKey>(this IEnumerable<TSource> source, Func<TSource, TKey> keySelector)
    {
        HashSet<TKey> seenKeys = new();
        foreach (TSource element in source)
        {
            if (seenKeys.Add(keySelector(element)))
            {
                yield return element;
            }
        }
    }

    /// <inheritdoc cref="string.Join{T}(string, IEnumerable{T})"/>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static string ToString<T>(this IEnumerable<T> source, string separator) => string.Join(separator, source);

    /// <inheritdoc cref="string.Join{T}(char, IEnumerable{T})"/>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static string ToString<T>(this IEnumerable<T> source, char separator) => string.Join(separator, source);

    /// <summary>
    /// 从 System.Collections.Generic.IEnumerable 创建 System.Collections.Generic.Dictionary (重复键使用第一个)
    /// </summary>
    /// <typeparam name="TElement"></typeparam>
    /// <typeparam name="TKey"></typeparam>
    /// <typeparam name="TValue"></typeparam>
    /// <param name="source"></param>
    /// <param name="keyGetter"></param>
    /// <param name="valueGetter"></param>
    /// <returns></returns>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static Dictionary<TKey, TValue> ToDictionaryIgnoreRepeat<TElement, TKey, TValue>(this IEnumerable<TElement> source, Func<TElement, TKey> keyGetter, Func<TElement, TValue> valueGetter) where TKey : notnull
    {
        var dict = new Dictionary<TKey, TValue>();
        foreach (var e in source)
        {
            var key = keyGetter(e);
            if (dict.ContainsKey(key))
            {
                continue;
            }
            dict.Add(key, valueGetter(e));
        }
        return dict;
    }

    /// <inheritdoc cref="List{T}.ForEach(Action{T})"/>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static void ForEach<T>(this IEnumerable<T> ts, Action<T> action)
    {
        if (ts is List<T> list)
        {
            list.ForEach(action);
        }
        else if (ts is T[] array)
        {
            Array.ForEach(array, action);
        }
        else
        {
            foreach (var item in ts)
            {
                action(item);
            }
        }
    }

    /// <inheritdoc cref="List{T}.ForEach(Action{T})"/>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static void FindAllForEach<T>(this IEnumerable<T> ts, Predicate<T> match, Action<T> action)
    {
        foreach (var item in ts)
        {
            if (match(item))
                action(item);
        }
    }
}
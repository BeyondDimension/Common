namespace System.Extensions;

/// <summary>
/// 表达式拼接扩展
/// </summary>
public static partial class QueryableExtensions
{
    /// <summary>
    /// 将多个表达式通过 OR 拼接返回查询的 <see cref="IQueryable"/>
    /// </summary>
    /// <typeparam name="T"></typeparam>
    /// <param name="source"></param>
    /// <param name="predicates"></param>
    /// <returns></returns>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    [RequiresDynamicCode("Enumerating collections as IQueryable can require creating new generic types or methods, which requires creating code at runtime. This may not work when AOT compiling.")]
    public static IQueryable<T> WhereOr<T>(this IQueryable<T> source, IReadOnlyList<Expression<Func<T, bool>>> predicates)
    {
        var predicate = ExpressionHelper.WhereOr(predicates);
        return source.Where(predicate);
    }

    /// <summary>
    /// 将多个表达式通过 OR 拼接返回查询的 <see cref="IQueryable"/>
    /// </summary>
    /// <typeparam name="T"></typeparam>
    /// <param name="source"></param>
    /// <param name="predicates"></param>
    /// <returns></returns>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    [RequiresDynamicCode("Enumerating collections as IQueryable can require creating new generic types or methods, which requires creating code at runtime. This may not work when AOT compiling.")]
    public static IQueryable<T> WhereOr<T>(this IQueryable<T> source, IEnumerable<Expression<Func<T, bool>>> predicates)
        => source.WhereOr(predicates.ToArray());

    /// <summary>
    /// 将多个表达式通过 OR 拼接返回查询的 <see cref="IQueryable"/>
    /// </summary>
    /// <typeparam name="T"></typeparam>
    /// <param name="source"></param>
    /// <param name="predicates"></param>
    /// <returns></returns>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    [RequiresDynamicCode("Enumerating collections as IQueryable can require creating new generic types or methods, which requires creating code at runtime. This may not work when AOT compiling.")]
    public static IQueryable<T> WhereOr<T>(this IQueryable<T> source, params Expression<Func<T, bool>>[] predicates)
    {
        IReadOnlyList<Expression<Func<T, bool>>> predicates_ = predicates;
        return source.WhereOr(predicates_);
    }
}
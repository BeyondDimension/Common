// ReSharper disable once CheckNamespace
namespace System;

public static partial class QueryableExtensions
{
    #region WhereOr

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static IQueryable<T> WhereOr<T>(this IQueryable<T> source, IReadOnlyList<Expression<Func<T, bool>>> predicates)
    {
        var predicate = ExpressionHelper.WhereOr(predicates);
        return source.Where(predicate);
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static IQueryable<T> WhereOr<T>(this IQueryable<T> source, IEnumerable<Expression<Func<T, bool>>> predicates)
        => source.WhereOr(predicates.ToArray());

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static IQueryable<T> WhereOr<T>(this IQueryable<T> source, params Expression<Func<T, bool>>[] predicates)
    {
        IReadOnlyList<Expression<Func<T, bool>>> predicates_ = predicates;
        return source.WhereOr(predicates_);
    }

    #endregion
}
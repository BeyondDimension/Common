namespace BD.Common8.Repositories.SQLitePCL.Extensions;

/// <summary>
/// 提供查询的扩展方法
/// </summary>
public static partial class AsyncTableQueryExtensions
{
    #region WhereOr

    /// <summary>
    /// 将多个表达式的逻辑条件通过逻辑或的方式进行合并，并执行查询筛选操作
    /// </summary>
    /// <typeparam name="T"></typeparam>
    /// <param name="source"></param>
    /// <param name="predicates">包含一个或多个指定条件的表达式的不可变列表</param>
    /// <returns></returns>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static AsyncTableQuery<T> WhereOr<T>(this AsyncTableQuery<T> source, IReadOnlyList<Expression<Func<T, bool>>> predicates) where T : new()
    {
        var predicate = ExpressionHelper.WhereOr(predicates);
        return source.Where(predicate);
    }

    /// <summary>
    /// 将多个表达式的逻辑条件通过逻辑或的方式进行合并，并执行查询筛选操作
    /// </summary>
    /// <typeparam name="T"></typeparam>
    /// <param name="source"></param>
    /// <param name="predicates">包含一个或多个指定条件的表达式的集合</param>
    /// <returns></returns>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static AsyncTableQuery<T> WhereOr<T>(this AsyncTableQuery<T> source, IEnumerable<Expression<Func<T, bool>>> predicates) where T : new()
        => source.WhereOr(predicates.ToArray());

    /// <summary>
    /// 将多个表达式的逻辑条件通过逻辑或的方式进行合并，并执行查询筛选操作
    /// </summary>
    /// <typeparam name="T"></typeparam>
    /// <param name="source"></param>
    /// <param name="predicates">包含一个或多个指定条件的表达式的参数数组</param>
    /// <returns></returns>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static AsyncTableQuery<T> WhereOr<T>(this AsyncTableQuery<T> source, params Expression<Func<T, bool>>[] predicates) where T : new()
    {
        IReadOnlyList<Expression<Func<T, bool>>> predicates_ = predicates;
        return source.WhereOr(predicates_);
    }

    #endregion

    #region Paging

    /// <summary>
    /// 根据页码进行分页查询，调用此方法前 必须 进行排序
    /// </summary>
    /// <typeparam name="TEntity"></typeparam>
    /// <param name="source"></param>
    /// <param name="current"></param>
    /// <param name="pageSize"></param>
    /// <returns></returns>
    public static async Task<PagedModel<TEntity>> PagingAsync<TEntity>(
        this AsyncTableQuery<TEntity> source,
        int current = IPagedModel.DefaultCurrent,
        int pageSize = IPagedModel.DefaultPageSize) where TEntity : new()
    {
        var skipCount = (current - 1) * pageSize;
        var total = await source.CountAsync();
        var dataSource = await source.Skip(skipCount).Take(pageSize).ToArrayAsync();
        var pagedModel = new PagedModel<TEntity>
        {
            Current = current,
            PageSize = pageSize,
            Total = total,
            DataSource = dataSource,
        };
        return pagedModel;
    }

    #endregion
}
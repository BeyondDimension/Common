namespace Microsoft.EntityFrameworkCore;

public static partial class DbContextExtensions
{
    /// <summary>
    /// 默认排序
    /// </summary>
    /// <typeparam name="TEntity"></typeparam>
    /// <typeparam name="TPrimaryKey"></typeparam>
    /// <param name="query"></param>
    /// <returns></returns>
    public static IQueryable<TEntity> Sort<TEntity, TPrimaryKey>(this IQueryable<TEntity> query)
        where TEntity : IOrder, IEntity<TPrimaryKey>
        where TPrimaryKey : notnull, IEquatable<TPrimaryKey>
        => query.OrderBy(x => x.Order).ThenBy(x => x.Id);

    /// <summary>
    /// 默认排序(主键为 <see cref="ulong"/>)
    /// </summary>
    /// <typeparam name="TEntity"></typeparam>
    /// <param name="query"></param>
    /// <returns></returns>
    public static IQueryable<TEntity> Sort<TEntity>(this IQueryable<TEntity> query)
        where TEntity : IOrder, IEntity<ulong>
        => query.Sort<TEntity, ulong>();

    /// <summary>
    /// 默认反转排序
    /// </summary>
    /// <typeparam name="TEntity"></typeparam>
    /// <typeparam name="TPrimaryKey"></typeparam>
    /// <param name="query"></param>
    /// <returns></returns>
    public static IQueryable<TEntity> SortDesc<TEntity, TPrimaryKey>(this IQueryable<TEntity> query)
        where TEntity : IOrder, IEntity<TPrimaryKey>
        where TPrimaryKey : notnull, IEquatable<TPrimaryKey>
        => query.OrderByDescending(x => x.Order).ThenByDescending(x => x.Id);

    /// <summary>
    /// 上移或下移，返回成功或找不到相邻的数据
    /// </summary>
    /// <typeparam name="TEntity"></typeparam>
    /// <typeparam name="TPrimaryKey"></typeparam>
    /// <param name="query">带排序的查询</param>
    /// <param name="db"></param>
    /// <param name="entity">当前要更改的实体</param>
    /// <param name="upOrDown">上移或下移</param>
    /// <param name="cancellationToken"></param>
    /// <returns></returns>
    public static async Task<bool> MoveUpOrDownAsync<TEntity, TPrimaryKey>(
        this IQueryable<TEntity> query,
        DbContext db,
        TEntity entity,
        bool upOrDown,
        CancellationToken cancellationToken = default)
        where TEntity : IOrder, IEntity<TPrimaryKey>
        where TPrimaryKey : notnull, IEquatable<TPrimaryKey>
    {
        if (upOrDown)
        {
            query = query.Where(x => x.Order < entity.Order).SortDesc<TEntity, TPrimaryKey>();
        }
        else
        {
            query = query.Where(x => x.Order > entity.Order).Sort<TEntity, TPrimaryKey>();
        }

        var entityVicinage = await query.FirstOrDefaultAsync(cancellationToken);
        if (entityVicinage == null) return false;
        (entityVicinage.Order, entity.Order) = (entity.Order, entityVicinage.Order);

        await db.SaveChangesAsync(cancellationToken);
        return true;
    }

    /// <summary>
    /// 上移或下移，返回成功或找不到相邻的数据(主键为 <see cref="ulong"/> )
    /// </summary>
    /// <typeparam name="TEntity"></typeparam>
    /// <param name="query"></param>
    /// <param name="db"></param>
    /// <param name="entity"></param>
    /// <param name="upOrDown"></param>
    /// <param name="cancellationToken"></param>
    /// <returns></returns>
    public static Task<bool> MoveUpOrDownAsync<TEntity>(
        this IQueryable<TEntity> query,
        DbContext db,
        TEntity entity,
        bool upOrDown,
        CancellationToken cancellationToken = default)
        where TEntity : IOrder, IEntity<ulong>
        => query.MoveUpOrDownAsync<TEntity, ulong>(db, entity, upOrDown, cancellationToken);
}

namespace BD.Common8.Orm.EFCore.Extensions;

public static partial class QueryableExtensions
{
    #region 根据条件执行删除

    /// <summary>
    /// 根据条件常规删除（批量删除或软删除），根据实体是否继承了 <see cref="ISoftDeleted"/> 自动检测
    /// </summary>
    /// <typeparam name="TEntity"></typeparam>
    /// <param name="query"></param>
    /// <param name="cancellationToken"></param>
    /// <returns></returns>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static async Task<int> GeneralDeleteAsync<[DynamicallyAccessedMembers(IEntity.DynamicallyAccessedMemberTypes)] TEntity>(
        this IQueryable<TEntity> query,
        CancellationToken cancellationToken = default)
        where TEntity : class, IEntity
    {
        if (query is not IQueryable<ISoftDeleted> query2)
        {
            var r = await query.ExecuteDeleteAsync(cancellationToken);
            return r;
        }
        else
        {
            var r = await query2.SoftDeleteAsync(cancellationToken);
            return r;
        }
    }

    /// <summary>
    /// 根据条件批量软删除
    /// </summary>
    /// <param name="query"></param>
    /// <param name="cancellationToken"></param>
    /// <returns></returns>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static async Task<int> SoftDeleteAsync(
        this IQueryable<ISoftDeleted> query,
        CancellationToken cancellationToken = default)
    {
        var r = await query.ExecuteUpdateAsync(s => s.SetProperty(b => b.SoftDeleted, true), cancellationToken);
        return r;
    }

    #endregion

    #region 根据主键进行删除

    /// <summary>
    /// 根据主键常规删除（批量删除或软删除），根据实体是否继承了 <see cref="ISoftDeleted"/> 自动检测
    /// </summary>
    /// <typeparam name="TEntity"></typeparam>
    /// <typeparam name="TPrimaryKey"></typeparam>
    /// <param name="query"></param>
    /// <param name="primaryKey"></param>
    /// <param name="cancellationToken"></param>
    /// <returns></returns>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static async Task<int> GeneralDeleteByIdAsync<[DynamicallyAccessedMembers(IEntity.DynamicallyAccessedMemberTypes)] TEntity, TPrimaryKey>(
        this IQueryable<TEntity> query,
        TPrimaryKey primaryKey,
        CancellationToken cancellationToken = default)
        where TPrimaryKey : IEquatable<TPrimaryKey>
        where TEntity : class, IEntity
    {
        query = query.Where(IEntity<TPrimaryKey>.LambdaEqualId<TEntity>(primaryKey));
        var r = await query.GeneralDeleteAsync(cancellationToken);
        return r;
    }

    /// <summary>
    /// 根据主键软删除
    /// </summary>
    /// <typeparam name="TEntity"></typeparam>
    /// <typeparam name="TPrimaryKey"></typeparam>
    /// <param name="query"></param>
    /// <param name="primaryKey"></param>
    /// <param name="cancellationToken"></param>
    /// <returns></returns>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static async Task<int> SoftDeleteByIdAsync<[DynamicallyAccessedMembers(IEntity.DynamicallyAccessedMemberTypes)] TEntity, TPrimaryKey>(
        this IQueryable<TEntity> query,
        TPrimaryKey primaryKey,
        CancellationToken cancellationToken = default)
        where TPrimaryKey : IEquatable<TPrimaryKey>
        where TEntity : class, IEntity, ISoftDeleted
    {
        query = query.Where(IEntity<TPrimaryKey>.LambdaEqualId<TEntity>(primaryKey));
        var r = await query.SoftDeleteAsync(cancellationToken);
        return r;
    }

    /// <summary>
    /// 根据主键硬删除（从数据库中删除数据）
    /// </summary>
    /// <typeparam name="TEntity"></typeparam>
    /// <typeparam name="TPrimaryKey"></typeparam>
    /// <param name="query"></param>
    /// <param name="primaryKey"></param>
    /// <param name="cancellationToken"></param>
    /// <returns></returns>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static async Task<int> HardDeleteByIdAsync<[DynamicallyAccessedMembers(IEntity.DynamicallyAccessedMemberTypes)] TEntity, TPrimaryKey>(
        this IQueryable<TEntity> query,
        TPrimaryKey primaryKey,
        CancellationToken cancellationToken = default)
        where TPrimaryKey : IEquatable<TPrimaryKey>
        where TEntity : class, IEntity, ISoftDeleted
    {
        query = query.Where(IEntity<TPrimaryKey>.LambdaEqualId<TEntity>(primaryKey));
        var r = await query.ExecuteDeleteAsync(cancellationToken);
        return r;
    }

    #endregion

    #region 根据多个主键进行删除

    /// <summary>
    /// 根据多个主键常规删除（批量删除或软删除），根据实体是否继承了 <see cref="ISoftDeleted"/> 自动检测
    /// </summary>
    /// <typeparam name="TEntity"></typeparam>
    /// <typeparam name="TPrimaryKey"></typeparam>
    /// <param name="query"></param>
    /// <param name="primaryKeys"></param>
    /// <param name="cancellationToken"></param>
    /// <returns></returns>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static async Task<int> GeneralDeleteByIdAsync<[DynamicallyAccessedMembers(IEntity.DynamicallyAccessedMemberTypes)] TEntity, TPrimaryKey>(
        this IQueryable<TEntity> query,
        IEnumerable<TPrimaryKey> primaryKeys,
        CancellationToken cancellationToken = default)
        where TPrimaryKey : IEquatable<TPrimaryKey>
        where TEntity : class, IEntity<TPrimaryKey>
    {
        query = query.Where(x => Enumerable.Contains(primaryKeys, x.Id));
        var r = await query.GeneralDeleteAsync(cancellationToken);
        return r;
    }

    /// <summary>
    /// 根据多个主键软删除
    /// </summary>
    /// <typeparam name="TEntity"></typeparam>
    /// <typeparam name="TPrimaryKey"></typeparam>
    /// <param name="query"></param>
    /// <param name="primaryKeys"></param>
    /// <param name="cancellationToken"></param>
    /// <returns></returns>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static async Task<int> SoftDeleteByIdAsync<[DynamicallyAccessedMembers(IEntity.DynamicallyAccessedMemberTypes)] TEntity, TPrimaryKey>(
        this IQueryable<TEntity> query,
        IEnumerable<TPrimaryKey> primaryKeys,
        CancellationToken cancellationToken = default)
        where TPrimaryKey : IEquatable<TPrimaryKey>
        where TEntity : class, IEntity<TPrimaryKey>, ISoftDeleted
    {
        query = query.Where(x => Enumerable.Contains(primaryKeys, x.Id));
        var r = await query.SoftDeleteAsync(cancellationToken);
        return r;
    }

    /// <summary>
    /// 根据多个主键硬删除（从数据库中删除数据）
    /// </summary>
    /// <typeparam name="TEntity"></typeparam>
    /// <typeparam name="TPrimaryKey"></typeparam>
    /// <param name="query"></param>
    /// <param name="primaryKeys"></param>
    /// <param name="cancellationToken"></param>
    /// <returns></returns>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static async Task<int> HardDeleteByIdAsync<[DynamicallyAccessedMembers(IEntity.DynamicallyAccessedMemberTypes)] TEntity, TPrimaryKey>(
        this IQueryable<TEntity> query,
        IEnumerable<TPrimaryKey> primaryKeys,
        CancellationToken cancellationToken = default)
        where TPrimaryKey : IEquatable<TPrimaryKey>
        where TEntity : class, IEntity<TPrimaryKey>, ISoftDeleted
    {
        query = query.Where(x => Enumerable.Contains(primaryKeys, x.Id));
        var r = await query.ExecuteDeleteAsync(cancellationToken);
        return r;
    }

    #endregion

    #region 根据实体进行删除

    /// <summary>
    /// 根据实体常规删除（批量删除或软删除），根据实体是否继承了 <see cref="ISoftDeleted"/> 自动检测
    /// </summary>
    /// <typeparam name="TEntity"></typeparam>
    /// <typeparam name="TPrimaryKey"></typeparam>
    /// <param name="query"></param>
    /// <param name="entity"></param>
    /// <param name="cancellationToken"></param>
    /// <returns></returns>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static async Task<int> GeneralDeleteAsync<[DynamicallyAccessedMembers(IEntity.DynamicallyAccessedMemberTypes)] TEntity, TPrimaryKey>(
        this IQueryable<TEntity> query,
        TEntity entity,
        CancellationToken cancellationToken = default)
        where TPrimaryKey : IEquatable<TPrimaryKey>
        where TEntity : class, IEntity<TPrimaryKey>
    {
        var r = await query.GeneralDeleteByIdAsync(entity.Id, cancellationToken);
        return r;
    }

    /// <summary>
    /// 根据实体软删除
    /// </summary>
    /// <typeparam name="TEntity"></typeparam>
    /// <typeparam name="TPrimaryKey"></typeparam>
    /// <param name="query"></param>
    /// <param name="entity"></param>
    /// <param name="cancellationToken"></param>
    /// <returns></returns>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static async Task<int> SoftDeleteAsync<[DynamicallyAccessedMembers(IEntity.DynamicallyAccessedMemberTypes)] TEntity, TPrimaryKey>(
        this IQueryable<TEntity> query,
        TEntity entity,
        CancellationToken cancellationToken = default)
        where TPrimaryKey : IEquatable<TPrimaryKey>
        where TEntity : class, IEntity<TPrimaryKey>, ISoftDeleted
    {
        var r = await query.SoftDeleteByIdAsync(entity.Id, cancellationToken);
        return r;
    }

    /// <summary>
    /// 根据实体硬删除（从数据库中删除数据）
    /// </summary>
    /// <typeparam name="TEntity"></typeparam>
    /// <typeparam name="TPrimaryKey"></typeparam>
    /// <param name="query"></param>
    /// <param name="entity"></param>
    /// <param name="cancellationToken"></param>
    /// <returns></returns>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static async Task<int> HardDeleteAsync<[DynamicallyAccessedMembers(IEntity.DynamicallyAccessedMemberTypes)] TEntity, TPrimaryKey>(
        this IQueryable<TEntity> query,
        TEntity entity,
        CancellationToken cancellationToken = default)
        where TPrimaryKey : IEquatable<TPrimaryKey>
        where TEntity : class, IEntity<TPrimaryKey>, ISoftDeleted
    {
        var r = await query.HardDeleteByIdAsync(entity.Id, cancellationToken);
        return r;
    }

    #endregion

    #region 根据多个实体进行删除

    /// <summary>
    /// 根据多个实体常规删除（批量删除或软删除），根据实体是否继承了 <see cref="ISoftDeleted"/> 自动检测
    /// </summary>
    /// <typeparam name="TEntity"></typeparam>
    /// <typeparam name="TPrimaryKey"></typeparam>
    /// <param name="query"></param>
    /// <param name="entities"></param>
    /// <param name="cancellationToken"></param>
    /// <returns></returns>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static async Task<int> GeneralDeleteAsync<[DynamicallyAccessedMembers(IEntity.DynamicallyAccessedMemberTypes)] TEntity, TPrimaryKey>(
        this IQueryable<TEntity> query,
        IEnumerable<TEntity> entities,
        CancellationToken cancellationToken = default)
        where TEntity : class, IEntity<TPrimaryKey>
        where TPrimaryKey : IEquatable<TPrimaryKey>
    {
        var r = await query.GeneralDeleteByIdAsync(entities.Select(x => x.Id), cancellationToken);
        return r;
    }

    /// <summary>
    /// 根据多个实体软删除
    /// </summary>
    /// <typeparam name="TEntity"></typeparam>
    /// <typeparam name="TPrimaryKey"></typeparam>
    /// <param name="query"></param>
    /// <param name="entities"></param>
    /// <param name="cancellationToken"></param>
    /// <returns></returns>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static async Task<int> SoftDeleteAsync<[DynamicallyAccessedMembers(IEntity.DynamicallyAccessedMemberTypes)] TEntity, TPrimaryKey>(
        this IQueryable<TEntity> query,
        IEnumerable<TEntity> entities,
        CancellationToken cancellationToken = default)
        where TEntity : class, IEntity<TPrimaryKey>, ISoftDeleted
        where TPrimaryKey : IEquatable<TPrimaryKey>
    {
        var r = await query.SoftDeleteByIdAsync(entities.Select(x => x.Id), cancellationToken);
        return r;
    }

    /// <summary>
    /// 根据多个实体硬删除（从数据库中删除数据）
    /// </summary>
    /// <typeparam name="TEntity"></typeparam>
    /// <typeparam name="TPrimaryKey"></typeparam>
    /// <param name="query"></param>
    /// <param name="entities"></param>
    /// <param name="cancellationToken"></param>
    /// <returns></returns>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static async Task<int> HardDeleteAsync<[DynamicallyAccessedMembers(IEntity.DynamicallyAccessedMemberTypes)] TEntity, TPrimaryKey>(
        this IQueryable<TEntity> query,
        IEnumerable<TEntity> entities,
        CancellationToken cancellationToken = default)
        where TPrimaryKey : IEquatable<TPrimaryKey>
        where TEntity : class, IEntity<TPrimaryKey>, ISoftDeleted
    {
        var r = await query.HardDeleteByIdAsync(entities.Select(x => x.Id), cancellationToken);
        return r;
    }

    #endregion
}
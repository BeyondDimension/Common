namespace BD.Common8.Repositories.SQLitePCL.Repositories.Abstractions;

/// <inheritdoc cref="Repository"/>
public abstract class Repository<[DynamicallyAccessedMembers(IEntity.DynamicallyAccessedMemberTypes)] TEntity> : Repository, IRepository<TEntity> where TEntity : class, new()
{
    /// <summary>
    /// 获取数据库连接
    /// </summary>
    /// <returns></returns>
    protected virtual ValueTask<SQLiteAsyncConnection> GetDbConnection() => GetDbConnection<TEntity>();

    #region 增(Insert Funs) 立即执行并返回受影响的行数

    /// <inheritdoc/>
    public virtual async Task<int> InsertAsync(TEntity entity, CancellationToken cancellationToken = default)
    {
        var dbConnection = await GetDbConnection().ConfigureAwait(false);
        return await AttemptAndRetry(t =>
        {
            t.ThrowIfCancellationRequested();
            return dbConnection.InsertAsync(entity);
        }, cancellationToken: cancellationToken).ConfigureAwait(false);
    }

    /// <inheritdoc/>
    public virtual async Task<int> InsertRangeAsync(IEnumerable<TEntity> entities, CancellationToken cancellationToken = default)
    {
        var dbConnection = await GetDbConnection().ConfigureAwait(false);
        return await AttemptAndRetry(t =>
        {
            t.ThrowIfCancellationRequested();
            return dbConnection.InsertAllAsync(entities);
        }, cancellationToken: cancellationToken).ConfigureAwait(false);
    }

    #endregion

    #region 删(Delete Funs) 立即执行并返回受影响的行数

    /// <inheritdoc/>
    public virtual async Task<int> DeleteAsync(TEntity entity, CancellationToken cancellationToken = default)
    {
        var dbConnection = await GetDbConnection().ConfigureAwait(false);
        return await AttemptAndRetry(t =>
        {
            t.ThrowIfCancellationRequested();
            return dbConnection.DeleteAsync(entity);
        }, cancellationToken: cancellationToken).ConfigureAwait(false);
    }

    #endregion

    #region 改(Update Funs) 立即执行并返回受影响的行数

    /// <inheritdoc/>
    public virtual async Task<int> UpdateAsync(TEntity entity, CancellationToken cancellationToken = default)
    {
        var dbConnection = await GetDbConnection().ConfigureAwait(false);
        return await AttemptAndRetry(t =>
        {
            t.ThrowIfCancellationRequested();
            return dbConnection.UpdateAsync(entity);
        }, cancellationToken: cancellationToken).ConfigureAwait(false);
    }

    /// <inheritdoc/>
    public virtual async Task<int> UpdateRangeAsync(IEnumerable<TEntity> entities, CancellationToken cancellationToken = default)
    {
        var dbConnection = await GetDbConnection().ConfigureAwait(false);
        return await AttemptAndRetry(t =>
        {
            t.ThrowIfCancellationRequested();
            return dbConnection.UpdateAllAsync(entities);
        }, cancellationToken: cancellationToken).ConfigureAwait(false);
    }

    #endregion

    #region 查(通用查询)

    /// <inheritdoc/>
    public virtual async Task<TEntity?> FirstOrDefaultAsync(Expression<Func<TEntity, bool>> predicate, CancellationToken cancellationToken = default)
    {
        var dbConnection = await GetDbConnection().ConfigureAwait(false);
        return await AttemptAndRetry(t =>
        {
            t.ThrowIfCancellationRequested();
            return dbConnection.FindAsync(predicate);
        }, cancellationToken: cancellationToken).ConfigureAwait(false);
    }

    #endregion
}
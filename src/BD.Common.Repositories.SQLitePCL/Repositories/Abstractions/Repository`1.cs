namespace BD.Common.Repositories.Abstractions;

/// <inheritdoc cref="Repository"/>
public abstract class Repository<TEntity> : Repository, IRepository<TEntity> where TEntity : class, new()
{
    protected static ValueTask<SQLiteAsyncConnection> GetDbConnection() => GetDbConnection<TEntity>();

    #region 增(Insert Funs) 立即执行并返回受影响的行数

    public virtual async Task<int> InsertAsync(TEntity entity)
    {
        var dbConnection = await GetDbConnection().ConfigureAwait(false);
        return await AttemptAndRetry(() => dbConnection.InsertAsync(entity)).ConfigureAwait(false);
    }

    public virtual async Task<int> InsertRangeAsync(IEnumerable<TEntity> entities)
    {
        var dbConnection = await GetDbConnection().ConfigureAwait(false);
        return await AttemptAndRetry(() => dbConnection.InsertAllAsync(entities)).ConfigureAwait(false);
    }

    #endregion

    #region 删(Delete Funs) 立即执行并返回受影响的行数

    public virtual async Task<int> DeleteAsync(TEntity entity)
    {
        var dbConnection = await GetDbConnection().ConfigureAwait(false);
        return await AttemptAndRetry(() => dbConnection.DeleteAsync(entity)).ConfigureAwait(false);
    }

    #endregion

    #region 改(Update Funs) 立即执行并返回受影响的行数

    public virtual async Task<int> UpdateAsync(TEntity entity)
    {
        var dbConnection = await GetDbConnection().ConfigureAwait(false);
        return await AttemptAndRetry(() => dbConnection.UpdateAsync(entity)).ConfigureAwait(false);
    }

    public virtual async Task<int> UpdateRangeAsync(IEnumerable<TEntity> entities)
    {
        var dbConnection = await GetDbConnection().ConfigureAwait(false);
        return await AttemptAndRetry(() => dbConnection.UpdateAllAsync(entities)).ConfigureAwait(false);
    }

    #endregion

    #region 查(通用查询)

    public virtual async Task<TEntity?> FirstOrDefaultAsync(Expression<Func<TEntity, bool>> predicate, CancellationToken cancellationToken = default)
    {
        var dbConnection = await GetDbConnection().ConfigureAwait(false);
        return await AttemptAndRetry(() => dbConnection.FindAsync(predicate)).ConfigureAwait(false);
    }

    #endregion
}
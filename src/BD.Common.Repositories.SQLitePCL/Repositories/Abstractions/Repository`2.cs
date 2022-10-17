namespace BD.Common.Repositories.Abstractions;

/// <inheritdoc cref="IRepository"/>
public abstract class Repository<TEntity, TPrimaryKey> : Repository<TEntity>, IRepository<TEntity, TPrimaryKey>
    where TEntity : class, IEntity<TPrimaryKey>, new()
    where TPrimaryKey : IEquatable<TPrimaryKey>
{
    object IGetPrimaryKey<TEntity>.GetPrimaryKey(TEntity entity) => GetPrimaryKey(entity);

    protected TPrimaryKey GetPrimaryKey(TEntity entity)
    {
        IGetPrimaryKey<TEntity, TPrimaryKey> getPrimaryKey = this;
        return getPrimaryKey.GetPrimaryKey(entity);
    }

    #region 删(Delete Funs) 立即执行并返回受影响的行数

    public override Task<int> DeleteAsync(TEntity entity)
    {
        var primaryKey = GetPrimaryKey(entity);
        return DeleteAsync(primaryKey);
    }

    public virtual async Task<int> DeleteAsync(TPrimaryKey primaryKey)
    {
        var dbConnection = await GetDbConnection().ConfigureAwait(false);
        return await AttemptAndRetry(() => dbConnection.DeleteAsync<TEntity>(primaryKey)).ConfigureAwait(false);
    }

    #endregion

    #region 查(通用查询)

    public virtual async ValueTask<TEntity?> FindAsync(TPrimaryKey primaryKey, CancellationToken cancellationToken = default)
    {
        if (IRepository<TEntity, TPrimaryKey>.IsDefault(primaryKey)) return default;
        var dbConnection = await GetDbConnection().ConfigureAwait(false);
        return await AttemptAndRetry(() => dbConnection.FindAsync<TEntity>(primaryKey)).ConfigureAwait(false);
    }

    public virtual async ValueTask<bool> ExistAsync(TPrimaryKey primaryKey, CancellationToken cancellationToken = default)
    {
        if (IRepository<TEntity, TPrimaryKey>.IsDefault(primaryKey)) return false;
        var dbConnection = await GetDbConnection().ConfigureAwait(false);
        var count = await AttemptAndRetry(CountAsync).ConfigureAwait(false);
        return count > 0;
        Task<int> CountAsync() => dbConnection.Table<TEntity>().CountAsync(IRepository<TEntity, TPrimaryKey>.LambdaEqualId(primaryKey));
    }

    #endregion

    #region 增或改(InsertOrUpdate Funs) 立即执行并返回受影响的行数

    public virtual async Task<(int rowCount, DbRowExecResult result)> InsertOrUpdateAsync(TEntity entity)
    {
        var dbConnection = await GetDbConnection().ConfigureAwait(false);
        var rowCount = await AttemptAndRetry(() =>
        {
            var primaryKey = GetPrimaryKey(entity);
            if (IRepository<TEntity, TPrimaryKey>.IsDefault(primaryKey))
            {
                return dbConnection.InsertAsync(entity);
            }
            else
            {
                return dbConnection.InsertOrReplaceAsync(entity);
            }
        }).ConfigureAwait(false);
        return (rowCount, DbRowExecResult.InsertOrUpdate);
    }

    #endregion
}
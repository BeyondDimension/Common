namespace BD.Common.Repositories.Abstractions;

/// <inheritdoc cref="IRepository"/>
public abstract class Repository<[DynamicallyAccessedMembers(IEntity.DynamicallyAccessedMemberTypes)] TEntity, TPrimaryKey> : Repository<TEntity>, IRepository<TEntity, TPrimaryKey>
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

    public override Task<int> DeleteAsync(TEntity entity, CancellationToken cancellationToken = default)
    {
        var primaryKey = GetPrimaryKey(entity);
        return DeleteAsync(primaryKey, cancellationToken);
    }

    public virtual async Task<int> DeleteAsync(TPrimaryKey primaryKey, CancellationToken cancellationToken = default)
    {
        var dbConnection = await GetDbConnection().ConfigureAwait(false);
        return await AttemptAndRetry(t =>
        {
            t.ThrowIfCancellationRequested();
            return dbConnection.DeleteAsync<TEntity>(primaryKey);
        }, cancellationToken: cancellationToken).ConfigureAwait(false);
    }

    #endregion

    #region 查(通用查询)

    public virtual async ValueTask<TEntity?> FindAsync(TPrimaryKey primaryKey, CancellationToken cancellationToken = default)
    {
        if (IRepository<TEntity, TPrimaryKey>.IsDefault(primaryKey))
            return default;
        var dbConnection = await GetDbConnection().ConfigureAwait(false);
        return await AttemptAndRetry(t =>
        {
            t.ThrowIfCancellationRequested();
            return dbConnection.FindAsync<TEntity>(primaryKey);
        }, cancellationToken: cancellationToken).ConfigureAwait(false);
    }

    public virtual async ValueTask<bool> ExistAsync(TPrimaryKey primaryKey, CancellationToken cancellationToken = default)
    {
        if (IRepository<TEntity, TPrimaryKey>.IsDefault(primaryKey))
            return false;
        var dbConnection = await GetDbConnection().ConfigureAwait(false);
        var count = await AttemptAndRetry(CountAsync, cancellationToken: cancellationToken).ConfigureAwait(false);
        return count > 0;
        Task<int> CountAsync(CancellationToken t)
        {
            t.ThrowIfCancellationRequested();
            return dbConnection.Table<TEntity>().CountAsync(IRepository<TEntity, TPrimaryKey>.LambdaEqualId(primaryKey));
        }
    }

    #endregion

    #region 增或改(InsertOrUpdate Funs) 立即执行并返回受影响的行数

    public virtual async Task<(int rowCount, DbRowExecResult result)> InsertOrUpdateAsync(TEntity entity, CancellationToken cancellationToken = default)
    {
        var dbConnection = await GetDbConnection().ConfigureAwait(false);
        var r = await AttemptAndRetry(async t =>
        {
            t.ThrowIfCancellationRequested();
            var primaryKey = GetPrimaryKey(entity);
            if (IRepository<TEntity, TPrimaryKey>.IsDefault(primaryKey))
            {
                var rowCount = await dbConnection.InsertAsync(entity);
                return (rowCount, DbRowExecResult.Insert);
            }
            else
            {
                var rowCount = await dbConnection.InsertOrReplaceAsync(entity);
                return (rowCount, DbRowExecResult.Update);
            }
        }, cancellationToken: cancellationToken).ConfigureAwait(false);
        return r;
    }

    #endregion
}
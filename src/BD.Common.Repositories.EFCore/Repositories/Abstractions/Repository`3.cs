using Microsoft.EntityFrameworkCore.Metadata;

namespace BD.Common.Repositories.Abstractions;

/// <inheritdoc cref="Repository{TDbContext}"/>
public abstract class Repository<TDbContext, TEntity, TPrimaryKey> : Repository<TDbContext, TEntity>, IRepository<TEntity, TPrimaryKey>
    where TDbContext : DbContext
    where TEntity : class, IEntity<TPrimaryKey>
    where TPrimaryKey : IEquatable<TPrimaryKey>
{
    object IGetPrimaryKey<TEntity>.GetPrimaryKey(TEntity entity) => GetPrimaryKey(entity);

    protected TPrimaryKey GetPrimaryKey(TEntity entity)
    {
        IGetPrimaryKey<TEntity, TPrimaryKey> getPrimaryKey = this;
        return getPrimaryKey.GetPrimaryKey(entity);
    }

    readonly Lazy<bool> mIsAutoGeneratedKey;

    public Repository(TDbContext dbContext, IRequestAbortedProvider requestAbortedProvider) : base(dbContext, requestAbortedProvider)
    {
        mIsAutoGeneratedKey = new Lazy<bool>(() =>
        {
            var key = EntityType.FindPrimaryKey();
            if (key?.Properties != null)
            {
                return key.Properties.All(p => p.ValueGenerated == ValueGenerated.OnAdd);
            }
            return false;
        });
    }

    #region 删(Delete Funs) 立即执行并返回受影响的行数

    public virtual Task<int> DeleteAsync(TPrimaryKey primaryKey) => DeleteAsync(primaryKey, default);

    protected virtual FormattableString GetSql_DeleteByKey(TPrimaryKey primaryKey)
    {
        FormattableString sql;
        if (IsSoftDeleted)
        {
            sql = SQLStrings.SoftDeleteFromTableNameWhereIdEqual(TableName, primaryKey);
        }
        else
        {
            sql = SQLStrings.DeleteFromTableNameWhereIdEqual(TableName, primaryKey);
        }
        return sql;
    }

    /// <inheritdoc cref="DeleteAsync(TPrimaryKey)"/>
    public virtual async Task<int> DeleteAsync(TPrimaryKey primaryKey, CancellationToken cancellationToken)
    {
        var sql = GetSql_DeleteByKey(primaryKey);
        return await db.Database.ExecuteSqlInterpolatedAsync(sql, cancellationToken);
    }

    #endregion

    #region 查(通用查询)

    public virtual async ValueTask<TEntity?> FindAsync(TPrimaryKey primaryKey, CancellationToken cancellationToken)
    {
        if (IRepository<TEntity, TPrimaryKey>.IsDefault(primaryKey)) return default;
        return await Entity.FindAsync(keyValues: new object[] { primaryKey }, cancellationToken: cancellationToken);
    }

    public virtual async ValueTask<bool> ExistAsync(TPrimaryKey primaryKey, CancellationToken cancellationToken)
    {
        if (IRepository<TEntity, TPrimaryKey>.IsDefault(primaryKey)) return false;
        return await Entity.AnyAsync(IRepository<TEntity, TPrimaryKey>.LambdaEqualId(primaryKey), cancellationToken);
    }

    #endregion

    #region 增或改(InsertOrUpdate Funs) 立即执行并返回受影响的行数

    /// <summary>
    /// 实体是否使用自动生成的键值
    /// </summary>
    public virtual bool IsAutoGeneratedKey => mIsAutoGeneratedKey.Value;

    /// <inheritdoc cref="InsertOrUpdateAsync(TEntity)"/>
    public virtual async Task<(int rowCount, DbRowExecResult result)> InsertOrUpdateAsync(TEntity entity, CancellationToken cancellationToken)
    {
        // https://docs.microsoft.com/zh-cn/ef/core/saving/disconnected-entities#saving-single-entities
        // 如果知道是需要插入还是需要更新，则可以相应地使用 Add 或 Update：
        // 但是，如果实体使用自动生成的键值，则 Update 方法可以用于以下两种情况：
        // Update 方法通常将实体标记为更新，而不是插入。 但是，如果实体具有自动生成的键且未设置任何键值，则实体会自动标记为插入。
        // EF Core 2.0 中已引入此行为。 对于早期版本，始终需要显式选择 Add 或 Update。
        if (IsAutoGeneratedKey)
        {
            var rowCount = await UpdateAsync(entity, cancellationToken);
            return (rowCount, DbRowExecResult.InsertOrUpdate);
        }
        else
        {
            // 如果实体不使用自动生成的键，则应用程序必须确定是应插入实体还是应更新实体：例如：
            var primaryKey = GetPrimaryKey(entity);
            var existingEntity = await FindAsync(primaryKey, cancellationToken);
            DbRowExecResult result;
            if (existingEntity == null)
            {
                Entity.Add(entity);
                result = DbRowExecResult.Insert;
            }
            else
            {
                db.Entry(existingEntity).CurrentValues.SetValues(entity);
                result = DbRowExecResult.Update;
            }
            var rowCount = await db.SaveChangesAsync(cancellationToken);
            return (rowCount, result);
        }
    }

    public virtual Task<(int rowCount, DbRowExecResult result)> InsertOrUpdateAsync(TEntity entity) => InsertOrUpdateAsync(entity, default);

    #endregion
}
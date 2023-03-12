namespace BD.Common.Repositories.Abstractions;

/// <inheritdoc cref="IRepository"/>
public interface IRepository<TEntity, TPrimaryKey> : IRepository<TEntity>, IGetPrimaryKey<TEntity, TPrimaryKey>
    where TEntity : class, IEntity<TPrimaryKey>
    where TPrimaryKey : IEquatable<TPrimaryKey>
{
    /// <summary>
    /// 判断主键是否为默认值
    /// </summary>
    /// <param name="primaryKey"></param>
    /// <returns></returns>
    public static bool IsDefault(TPrimaryKey primaryKey)
    {
        if (primaryKey == null) return true; // null is default
        var defPrimaryKey = default(TPrimaryKey);
        if (defPrimaryKey == null) return false; // primaryKey not null
        return EqualityComparer<TPrimaryKey>.Default.Equals(primaryKey, defPrimaryKey);
    }

    /// <summary>
    /// 返回主键匹配表达式
    /// </summary>
    /// <param name="primaryKey"></param>
    /// <returns></returns>
    public static Expression<Func<TEntity, bool>> LambdaEqualId(TPrimaryKey primaryKey)
    {
        var parameter = Expression.Parameter(typeof(TEntity));
        var left = Expression.PropertyOrField(parameter, nameof(IEntity<TPrimaryKey>.Id));
        var right = Expression.Constant(primaryKey, typeof(TPrimaryKey));
        var body = Expression.Equal(left, right);
        return Expression.Lambda<Func<TEntity, bool>>(body, parameter);
    }

    #region 删(Delete Funs) 立即执行并返回受影响的行数

    /// <summary>
    /// 根据主键将实体从数据库中删除
    /// </summary>
    /// <param name="primaryKey">要删除的实体主键</param>
    /// <returns>受影响的行数</returns>
    Task<int> DeleteAsync(TPrimaryKey primaryKey);

    /// <summary>
    /// 根据主键将多个实体从数据库中删除
    /// </summary>
    /// <param name="primaryKeys">要删除的多个实体主键</param>
    /// <returns>受影响的行数</returns>
    Task<int> DeleteRangeAsync(IEnumerable<TPrimaryKey> primaryKeys) => OperateRangeAsync(primaryKeys, DeleteAsync);

    /// <summary>
    /// 根据主键将多个实体从数据库中删除
    /// </summary>
    /// <param name="primaryKeys">要删除的多个实体主键</param>
    /// <returns>受影响的行数</returns>
    Task<int> DeleteRangeAsync(params TPrimaryKey[] primaryKeys) => DeleteRangeAsync(primaryKeys.AsEnumerable());

    ///// <summary>
    ///// 根据主键将多个实体从数据库中删除
    ///// </summary>
    ///// <param name="primaryKeys">要删除的多个实体主键</param>
    ///// <returns>受影响的行数</returns>
    //IAsyncEnumerable<(int rowCount, TPrimaryKey entity)> DeleteRangeAsyncEnumerable(IEnumerable<TPrimaryKey> primaryKeys)
    //    => OperateRangeAsyncEnumerable(primaryKeys, DeleteAsync);

    ///// <summary>
    ///// 根据主键将多个实体从数据库中删除
    ///// </summary>
    ///// <param name="primaryKeys">要删除的多个实体主键</param>
    ///// <returns>受影响的行数</returns>
    //IAsyncEnumerable<(int rowCount, TPrimaryKey entity)> DeleteRangeAsyncEnumerable(params TPrimaryKey[] primaryKeys)
    //    => DeleteRangeAsyncEnumerable(primaryKeys.AsEnumerable());

    #endregion

    #region 查(通用查询)

    /// <summary>
    /// 根据主键查询实体
    /// </summary>
    /// <param name="primaryKey">要查询的实体主键</param>
    /// <param name="cancellationToken"></param>
    /// <returns>查询到的实体</returns>
    ValueTask<TEntity?> FindAsync(TPrimaryKey primaryKey, CancellationToken cancellationToken = default);

    /// <summary>
    /// 判断实体是否已经存在
    /// </summary>
    /// <param name="primaryKey">要查询的实体主键</param>
    /// <param name="cancellationToken"></param>
    /// <returns>实体是否存在数据库中</returns>
    async ValueTask<bool> ExistAsync(TPrimaryKey primaryKey, CancellationToken cancellationToken = default)
    {
        var entity = await FindAsync(primaryKey, cancellationToken);
        return entity != null;
    }

    /// <summary>
    /// 判断实体是否已经存在
    /// </summary>
    /// <param name="entity">要查询的实体</param>
    /// <param name="cancellationToken"></param>
    /// <returns>实体是否存在数据库中</returns>
    ValueTask<bool> ExistAsync(TEntity entity, CancellationToken cancellationToken = default)
    {
        var primaryKey = GetPrimaryKey(entity);
        return ExistAsync(primaryKey, cancellationToken);
    }

    #endregion

    #region 增或改(InsertOrUpdate Funs) 立即执行并返回受影响的行数

    /// <summary>
    /// 新增或更新实体
    /// </summary>
    /// <param name="entity">要新增或更新的实体</param>
    /// <returns>受影响的行数与数据库逻辑</returns>
    async Task<(int rowCount, DbRowExecResult result)> InsertOrUpdateAsync(TEntity entity)
    {
        var exist = await ExistAsync(entity);
        int rowCount;
        DbRowExecResult result;
        if (exist)
        {
            rowCount = await UpdateAsync(entity);
            result = DbRowExecResult.Update;
        }
        else
        {
            rowCount = await InsertAsync(entity);
            result = DbRowExecResult.Insert;
        }
        return (rowCount, result);
    }

    /// <summary>
    /// 批量新增或更新实体
    /// </summary>
    /// <param name="entities">要新增或更新的多个实体</param>
    /// <returns>受影响的行数与数据库逻辑</returns>
    IAsyncEnumerable<(int rowCount, DbRowExecResult result, TEntity entity)> InsertOrUpdateAsync(IEnumerable<TEntity> entities)
        => OperateRangeAsyncEnumerable(entities, InsertOrUpdateAsync);

    /// <summary>
    /// 批量新增或更新实体
    /// </summary>
    /// <param name="entities">要新增或更新的多个实体</param>
    /// <returns>受影响的行数与数据库逻辑</returns>
    IAsyncEnumerable<(int rowCount, DbRowExecResult result, TEntity entity)> InsertOrUpdateAsync(params TEntity[] entities)
        => InsertOrUpdateAsync(entities.AsEnumerable());

    #endregion
}
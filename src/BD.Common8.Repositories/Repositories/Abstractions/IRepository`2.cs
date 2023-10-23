namespace BD.Common8.Repositories.Repositories.Abstractions;

/// <inheritdoc cref="IRepository"/>
public interface IRepository<[DynamicallyAccessedMembers(IEntity.DynamicallyAccessedMemberTypes)] TEntity, TPrimaryKey> : IRepository<TEntity>, IGetPrimaryKey<TEntity, TPrimaryKey>
    where TEntity : class, IEntity<TPrimaryKey>
    where TPrimaryKey : IEquatable<TPrimaryKey>
{
    /// <inheritdoc cref="IEntity{TPrimaryKey}.IsDefault(TPrimaryKey)"/>
    public static bool IsDefault(TPrimaryKey primaryKey)
        => IEntity<TPrimaryKey>.IsDefault(primaryKey);

    /// <inheritdoc cref="IEntity{TPrimaryKey}.LambdaEqualId{TEntity}(TPrimaryKey)"/>
    public static Expression<Func<TEntity, bool>> LambdaEqualId(TPrimaryKey primaryKey)
        => IEntity<TPrimaryKey>.LambdaEqualId<TEntity>(primaryKey);

    /// <summary>
    /// 根据主键设置禁用状态
    /// </summary>
    /// <param name="primaryKey"></param>
    /// <param name="disable"></param>
    /// <returns></returns>
    Task<int> SetDisableByIdAsync(TPrimaryKey primaryKey, bool disable)
    {
        throw new NotSupportedException();
    }

    #region 删(Delete Funs) 立即执行并返回受影响的行数

    /// <summary>
    /// 根据主键将实体从数据库中删除
    /// </summary>
    /// <param name="primaryKey">要删除的实体主键</param>
    /// <param name="cancellationToken"></param>
    /// <returns>受影响的行数</returns>
    Task<int> DeleteAsync(TPrimaryKey primaryKey, CancellationToken cancellationToken = default);

    /// <summary>
    /// 根据主键将多个实体从数据库中删除
    /// </summary>
    /// <param name="primaryKeys">要删除的多个实体主键</param>
    /// <param name="cancellationToken"></param>
    /// <returns>受影响的行数</returns>
    Task<int> DeleteRangeAsync(IEnumerable<TPrimaryKey> primaryKeys, CancellationToken cancellationToken = default) => OperateRangeAsync(primaryKeys, DeleteAsync, cancellationToken);

    /// <summary>
    /// 根据主键将多个实体从数据库中删除
    /// </summary>
    /// <param name="primaryKeys">要删除的多个实体主键</param>
    /// <returns>受影响的行数</returns>
    Task<int> DeleteRangeAsync(params TPrimaryKey[] primaryKeys) => DeleteRangeAsync(primaryKeys.AsEnumerable());

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

    /// <summary>
    /// 根据主键获取编辑视图模型
    /// </summary>
    /// <typeparam name="TEditViewModel"></typeparam>
    /// <param name="primaryKey"></param>
    /// <param name="cancellationToken"></param>
    /// <returns></returns>
    /// <exception cref="NotImplementedException"></exception>
    Task<TEditViewModel?> GetEditByIdAsync<TEditViewModel>(TPrimaryKey primaryKey, CancellationToken cancellationToken = default) where TEditViewModel : notnull, IKeyModel<TPrimaryKey>
    {
        throw new NotImplementedException();
    }

    #endregion

    #region 增或改(InsertOrUpdate Funs) 立即执行并返回受影响的行数

    /// <summary>
    /// 新增或更新实体
    /// </summary>
    /// <param name="entity">要新增或更新的实体</param>
    /// <param name="cancellationToken"></param>
    /// <returns>受影响的行数与数据库逻辑</returns>
    async Task<(int rowCount, DbRowExecResult result)> InsertOrUpdateAsync(TEntity entity, CancellationToken cancellationToken = default)
    {
        var exist = await ExistAsync(entity, cancellationToken);
        int rowCount;
        DbRowExecResult result;
        if (exist)
        {
            rowCount = await UpdateAsync(entity, cancellationToken);
            result = DbRowExecResult.Update;
        }
        else
        {
            rowCount = await InsertAsync(entity, cancellationToken);
            result = DbRowExecResult.Insert;
        }
        return (rowCount, result);
    }

    /// <summary>
    /// 根据编辑模型添加或更新一行数据
    /// </summary>
    /// <typeparam name="TViewModel"></typeparam>
    /// <param name="viewModel"></param>
    /// <param name="create"></param>
    /// <param name="onAdd"></param>
    /// <param name="onUpdate"></param>
    /// <param name="cancellationToken"></param>
    /// <returns></returns>
    /// <exception cref="NotSupportedException"></exception>
    Task<(int rowCount, DbRowExecResult result)> InsertOrUpdateAsync<TViewModel>(TViewModel viewModel,
        Func<TEntity>? create = null,
        Action<TEntity>? onAdd = null,
        Action<TEntity>? onUpdate = null,
        CancellationToken cancellationToken = default) where TViewModel : notnull
    {
        throw new NotSupportedException();
    }

    /// <summary>
    /// 批量新增或更新实体
    /// </summary>
    /// <param name="entities">要新增或更新的多个实体</param>
    /// <param name="cancellationToken"></param>
    /// <returns>受影响的行数与数据库逻辑</returns>
    IAsyncEnumerable<(int rowCount, DbRowExecResult result, TEntity entity)> InsertOrUpdateAsync(IEnumerable<TEntity> entities, CancellationToken cancellationToken = default)
        => OperateRangeAsyncEnumerable(entities, InsertOrUpdateAsync, cancellationToken);

    /// <summary>
    /// 批量新增或更新实体
    /// </summary>
    /// <param name="entities">要新增或更新的多个实体</param>
    /// <returns>受影响的行数与数据库逻辑</returns>
    IAsyncEnumerable<(int rowCount, DbRowExecResult result, TEntity entity)> InsertOrUpdateAsync(params TEntity[] entities)
        => InsertOrUpdateAsync(entities: entities.AsEnumerable());

    #endregion
}
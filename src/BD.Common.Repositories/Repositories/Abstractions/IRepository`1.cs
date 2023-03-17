namespace BD.Common.Repositories.Abstractions;

/// <inheritdoc cref="IRepository"/>
public interface IRepository<[DynamicallyAccessedMembers(IEntity.DynamicallyAccessedMemberTypes)] TEntity> : IRepository where TEntity : class
{
    #region 增(Insert Funs) 立即执行并返回受影响的行数

    /// <summary>
    /// 将实体插入到数据库中
    /// </summary>
    /// <param name="entity">要添加的实体</param>
    /// <param name="cancellationToken"></param>
    /// <returns>受影响的行数</returns>
    Task<int> InsertAsync(TEntity entity, CancellationToken cancellationToken = default);

    /// <summary>
    /// 将多个实体插入到数据库中
    /// </summary>
    /// <param name="entities">要添加的多个实体</param>
    /// <param name="cancellationToken"></param>
    /// <returns>受影响的行数</returns>
    Task<int> InsertRangeAsync(IEnumerable<TEntity> entities, CancellationToken cancellationToken = default) => OperateRangeAsync(entities, InsertAsync, cancellationToken);

    /// <summary>
    /// 将多个实体插入到数据库中
    /// </summary>
    /// <param name="entities">要添加的多个实体</param>
    /// <returns>受影响的行数</returns>
    Task<int> InsertRangeAsync(params TEntity[] entities) => InsertRangeAsync(entities.AsEnumerable());

    #endregion

    #region 删(Delete Funs) 立即执行并返回受影响的行数

    /// <summary>
    /// 将实体从数据库中删除
    /// </summary>
    /// <param name="entity">要删除的实体</param>
    /// <param name="cancellationToken"></param>
    /// <returns>受影响的行数</returns>
    Task<int> DeleteAsync(TEntity entity, CancellationToken cancellationToken = default);

    /// <summary>
    /// 将多个实体从数据库中删除
    /// </summary>
    /// <param name="entities">要删除的多个实体</param>
    /// <param name="cancellationToken"></param>
    /// <returns>受影响的行数</returns>
    Task<int> DeleteRangeAsync(IEnumerable<TEntity> entities, CancellationToken cancellationToken = default) => OperateRangeAsync(entities, DeleteAsync, cancellationToken);

    /// <summary>
    /// 将多个实体从数据库中删除
    /// </summary>
    /// <param name="entities">要删除的多个实体</param>
    /// <returns>受影响的行数</returns>
    Task<int> DeleteRangeAsync(params TEntity[] entities) => DeleteRangeAsync(entities.AsEnumerable());

    #endregion

    #region 改(Update Funs) 立即执行并返回受影响的行数

    /// <summary>
    /// 将实体更新到数据库中
    /// </summary>
    /// <param name="entity">要更新的实体</param>
    /// <param name="cancellationToken"></param>
    /// <returns>受影响的行数</returns>
    Task<int> UpdateAsync(TEntity entity, CancellationToken cancellationToken = default);

    /// <summary>
    /// 将多个实体更新到数据库中
    /// </summary>
    /// <param name="entities">要更新的多个实体</param>
    /// <param name="cancellationToken"></param>
    /// <returns>受影响的行数</returns>
    Task<int> UpdateRangeAsync(IEnumerable<TEntity> entities, CancellationToken cancellationToken = default) => OperateRangeAsync(entities, UpdateAsync, cancellationToken);

    /// <summary>
    /// 将多个实体更新到数据库中
    /// </summary>
    /// <param name="entities">要更新的多个实体</param>
    /// <returns>受影响的行数</returns>
    Task<int> UpdateRangeAsync(params TEntity[] entities) => UpdateRangeAsync(entities.AsEnumerable());

    #endregion

    #region 查(通用查询)

    /// <summary>
    /// 返回序列中满足指定的条件或默认值，如果找到这样的元素的第一个元素
    /// </summary>
    /// <param name="predicate">用于测试每个元素是否满足条件的函数</param>
    /// <param name="cancellation"></param>
    /// <returns>
    /// default(TEntity) 如果 集合 为空，或者如果没有元素通过由指定的测试 <paramref name="predicate" />;
    /// 否则为中的第一个元素 集合 通过由指定的测试 <paramref name="predicate" />
    /// </returns>
    /// <exception cref="ArgumentNullException">
    /// 集合 或 <paramref name="predicate" /> 为 <see langword="null" />
    /// </exception>
    Task<TEntity?> FirstOrDefaultAsync(Expression<Func<TEntity, bool>> predicate, CancellationToken cancellation = default);

    #endregion
}
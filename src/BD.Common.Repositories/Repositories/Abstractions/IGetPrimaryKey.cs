namespace BD.Common.Repositories.Abstractions;

public interface IGetPrimaryKey<TEntity> where TEntity : class
{
    /// <summary>
    /// 根据实体获取主键
    /// </summary>
    /// <param name="entity"></param>
    /// <returns></returns>
    object GetPrimaryKey(TEntity entity);
}

public interface IGetPrimaryKey<TEntity, TPrimaryKey> : IGetPrimaryKey<TEntity>
  where TEntity : class, IEntity<TPrimaryKey>
  where TPrimaryKey : IEquatable<TPrimaryKey>
{
    /// <summary>
    /// 根据实体获取主键
    /// </summary>
    /// <param name="entity"></param>
    /// <returns></returns>
    public new TPrimaryKey GetPrimaryKey(TEntity entity) => DefaultGetPrimaryKey(entity);

    /// <summary>
    /// 根据实体获取主键的默认实现
    /// </summary>
    /// <param name="entity"></param>
    /// <returns></returns>
    protected static TPrimaryKey DefaultGetPrimaryKey(TEntity entity) => entity.Id;
}
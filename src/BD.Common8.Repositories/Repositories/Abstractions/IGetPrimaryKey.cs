namespace BD.Common8.Repositories.Repositories.Abstractions;

/// <summary>
/// 根据实体获取主键
/// </summary>
/// <typeparam name="TEntity">实体类型</typeparam>
public interface IGetPrimaryKey<[DynamicallyAccessedMembers(IEntity.DynamicallyAccessedMemberTypes)] TEntity> where TEntity : class
{
    /// <summary>
    /// 根据实体获取主键
    /// </summary>
    /// <param name="entity"></param>
    /// <returns></returns>
    object GetPrimaryKey(TEntity entity);
}

/// <summary>
/// 根据实体获取主键
/// </summary>
/// <typeparam name="TEntity">实体类型</typeparam>
/// <typeparam name="TPrimaryKey">主键类型</typeparam>
public interface IGetPrimaryKey<[DynamicallyAccessedMembers(IEntity.DynamicallyAccessedMemberTypes)] TEntity, TPrimaryKey> : IGetPrimaryKey<TEntity>
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
    public static TPrimaryKey DefaultGetPrimaryKey(TEntity entity) => entity.Id;
}
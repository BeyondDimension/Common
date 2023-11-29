using DAMT = System.Diagnostics.CodeAnalysis.DynamicallyAccessedMemberTypes;

namespace BD.Common8.Entities.Abstractions;

/// <summary>
/// (数据库表)实体模型接口
/// </summary>
public interface IEntity
{
    /// <summary>
    /// Id，即主键
    /// </summary>
    object Id { get; }

    /// <inheritdoc cref="DynamicallyAccessedMemberTypes"/>
    const DAMT DynamicallyAccessedMemberTypes =
        DAMT.PublicConstructors
        | DAMT.NonPublicConstructors
        | DAMT.PublicProperties
        | DAMT.PublicFields
        | DAMT.NonPublicProperties
        | DAMT.NonPublicFields
        | DAMT.Interfaces;
}

/// <summary>
/// (数据库表)实体模型接口
/// </summary>
/// <typeparam name="TPrimaryKey"></typeparam>
public interface IEntity<TPrimaryKey> : IEntity where TPrimaryKey : notnull, IEquatable<TPrimaryKey>
{
    /// <inheritdoc cref="IEntity.Id"/>
    new TPrimaryKey Id { get; set; }

    /// <inheritdoc/>
    object IEntity.Id => Id;

    /// <summary>
    /// 判断主键是否为默认值
    /// </summary>
    /// <param name="primaryKey">主键</param>
    /// <returns></returns>
    static bool IsDefault(TPrimaryKey primaryKey)
    {
        if (primaryKey == null) return true; // null is default
        var defPrimaryKey = default(TPrimaryKey);
        if (defPrimaryKey == null) return false; // primaryKey not null
        return EqualityComparer<TPrimaryKey>.Default.Equals(primaryKey, defPrimaryKey);
    }

    /// <summary>
    /// 返回主键匹配表达式
    /// </summary>
    /// <typeparam name="TEntity">实体类型</typeparam>
    /// <param name="primaryKey">主键</param>
    /// <returns></returns>
    static Expression<Func<TEntity, bool>> LambdaEqualId<[DynamicallyAccessedMembers(DynamicallyAccessedMemberTypes)] TEntity>(TPrimaryKey primaryKey)
    {
        var parameter = Expression.Parameter(typeof(TEntity));
        var left = Expression.PropertyOrField(parameter, nameof(IEntity<TPrimaryKey>.Id));
        var right = Expression.Constant(primaryKey, typeof(TPrimaryKey));
        var body = Expression.Equal(left, right);
        return Expression.Lambda<Func<TEntity, bool>>(body, parameter);
    }
}

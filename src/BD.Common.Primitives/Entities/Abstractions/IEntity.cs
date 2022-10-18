// ReSharper disable once CheckNamespace

namespace BD.Common.Entities.Abstractions;

public interface IEntity
{
    object Id { get; }
}

/// <summary>
/// (数据库表)实体模型接口
/// </summary>
/// <typeparam name="TPrimaryKey"></typeparam>
public interface IEntity<TPrimaryKey> : IEntity where TPrimaryKey : notnull, IEquatable<TPrimaryKey>
{
    /// <summary>
    /// 主键
    /// </summary>
    new TPrimaryKey Id { get; set; }

    object IEntity.Id => Id;
}

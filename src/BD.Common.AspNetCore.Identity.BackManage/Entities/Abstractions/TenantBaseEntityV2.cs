// ReSharper disable once CheckNamespace
namespace BD.Common.Entities.Abstractions;

/// <summary>
/// 基类实体 - 包含租户与软删除与修改时间与操作人与创建时间与创建人
/// </summary>
/// <typeparam name="TPrimaryKey"></typeparam>
public abstract class TenantBaseEntityV2<TPrimaryKey> :
    OperatorBaseEntityV2<TPrimaryKey>,
    ISoftDeleted, ITenant
    where TPrimaryKey : notnull, IEquatable<TPrimaryKey>
{
    /// <summary>
    /// 是否删除
    /// </summary>
    public bool SoftDeleted { get; set; }

    /// <summary>
    /// 租户
    /// </summary>
    public Guid TenantId { get; set; }
}

/// <inheritdoc cref="TenantBaseEntityV2{TPrimaryKey}"/>
public abstract class TenantBaseEntityV2 : TenantBaseEntityV2<Guid>
{

}
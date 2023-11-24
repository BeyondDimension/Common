namespace BD.Common8.AspNetCore.Entities.Abstractions;

/// <summary>
/// 基类实体 - 包含租户与软删除与修改时间与操作人与创建时间与创建人
/// </summary>
/// <typeparam name="TPrimaryKey"></typeparam>
public abstract class TenantBaseEntity<TPrimaryKey> :
    OperatorBaseEntity<TPrimaryKey>,
    ISoftDeleted, ITenant
    where TPrimaryKey : notnull, IEquatable<TPrimaryKey>
{
    /// <inheritdoc/>
    [Comment("是否软删除")]
    public bool SoftDeleted { get; set; }

    /// <inheritdoc/>
    [Comment("租户 Id")]
    public Guid TenantId { get; set; }
}

/// <inheritdoc cref="TenantBaseEntity{TPrimaryKey}"/>
public abstract class TenantBaseEntity : TenantBaseEntity<Guid>
{
}
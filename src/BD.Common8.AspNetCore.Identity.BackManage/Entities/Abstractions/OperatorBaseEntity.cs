namespace BD.Common8.AspNetCore.Entities.Abstractions;

#pragma warning disable SA1600 // Elements should be documented

/// <summary>
/// 基类实体 - 包含修改时间与操作人与创建时间与创建人
/// </summary>
/// <typeparam name="TPrimaryKey"></typeparam>
public abstract class OperatorBaseEntity<TPrimaryKey> :
    CreationBaseEntity<TPrimaryKey>,
    IUpdateTime,
    IOperatorUser,
    IOperatorUserId
    where TPrimaryKey : notnull, IEquatable<TPrimaryKey>
{
    /// <inheritdoc/>
    [Comment("更新时间")]
    public DateTimeOffset UpdateTime { get; set; }

    /// <inheritdoc/>
    [Comment("操作人")]
    public Guid? OperatorUserId { get; set; }

    /// <inheritdoc/>
    public virtual SysUser? OperatorUser { get; set; }

    public new abstract class EntityTypeConfiguration<[DynamicallyAccessedMembers(IEntity.DynamicallyAccessedMemberTypes)] TEntity> : CreationBaseEntity<TPrimaryKey>.EntityTypeConfiguration<TEntity>
          where TEntity : OperatorBaseEntity<TPrimaryKey>
    {
        public override void Configure(EntityTypeBuilder<TEntity> builder)
        {
            base.Configure(builder);

            builder.HasOne(x => x.OperatorUser)
                .WithMany()
                .HasForeignKey(p => p.OperatorUserId)
                .OnDelete(DeleteBehavior.SetNull);
        }
    }
}

/// <inheritdoc cref="OperatorBaseEntity{TPrimaryKey}"/>
public abstract class OperatorBaseEntity : OperatorBaseEntity<Guid>
{
    public new abstract class EntityTypeConfiguration<[DynamicallyAccessedMembers(IEntity.DynamicallyAccessedMemberTypes)] TEntity> : OperatorBaseEntity<Guid>.EntityTypeConfiguration<TEntity>
        where TEntity : OperatorBaseEntity<Guid>
    {
    }
}
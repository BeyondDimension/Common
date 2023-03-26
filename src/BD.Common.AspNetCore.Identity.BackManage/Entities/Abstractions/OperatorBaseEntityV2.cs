// ReSharper disable once CheckNamespace
namespace BD.Common.Entities.Abstractions;

/// <summary>
/// 基类实体 - 包含修改时间与操作人与创建时间与创建人
/// </summary>
/// <typeparam name="TPrimaryKey"></typeparam>
public abstract class OperatorBaseEntityV2<TPrimaryKey> :
    CreationBaseEntityV2<TPrimaryKey>,
    IUpdateTime,
    IOperatorUser,
    IOperatorUserId
    where TPrimaryKey : notnull, IEquatable<TPrimaryKey>
{
    /// <summary>
    /// 更新时间
    /// </summary>
    [Comment("更新时间")]
    public DateTimeOffset UpdateTime { get; set; }

    /// <summary>
    /// 操作人
    /// </summary>
    [Comment("操作人")]
    public Guid? OperatorUserId { get; set; }

    /// <summary>
    /// 操作人详情
    /// </summary>
    public virtual SysUser? OperatorUser { get; set; }

    public new abstract class EntityTypeConfiguration<[DynamicallyAccessedMembers(IEntity.DynamicallyAccessedMemberTypes)] TEntity> : CreationBaseEntityV2<TPrimaryKey>.EntityTypeConfiguration<TEntity>
          where TEntity : OperatorBaseEntityV2<TPrimaryKey>
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

/// <inheritdoc cref="OperatorBaseEntityV2{TPrimaryKey}"/>
public abstract class OperatorBaseEntityV2 : OperatorBaseEntityV2<Guid>
{
    public new abstract class EntityTypeConfiguration<[DynamicallyAccessedMembers(IEntity.DynamicallyAccessedMemberTypes)] TEntity> : OperatorBaseEntityV2<Guid>.EntityTypeConfiguration<TEntity>
        where TEntity : OperatorBaseEntityV2<Guid>
    {

    }
}
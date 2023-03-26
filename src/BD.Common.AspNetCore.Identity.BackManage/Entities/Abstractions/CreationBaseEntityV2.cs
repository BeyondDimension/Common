// ReSharper disable once CheckNamespace
namespace BD.Common.Entities.Abstractions;

/// <summary>
/// 基类实体 - 包含创建时间与创建人
/// </summary>
/// <typeparam name="TPrimaryKey"></typeparam>
public abstract class CreationBaseEntityV2<TPrimaryKey> :
    Entity<TPrimaryKey>,
    ICreationTime,
    ICreateUser,
    ICreateUserIdNullable
    where TPrimaryKey : notnull, IEquatable<TPrimaryKey>
{
    /// <summary>
    /// 创建时间
    /// </summary>
    [Comment("创建时间")]
    public DateTimeOffset CreationTime { get; set; }

    /// <summary>
    /// 创建人
    /// </summary>
    [Comment("创建人")]
    public Guid? CreateUserId { get; set; }

    /// <summary>
    /// 创建人关联
    /// </summary>
    public virtual SysUser? CreateUser { get; set; }

    public abstract class EntityTypeConfiguration<[DynamicallyAccessedMembers(IEntity.DynamicallyAccessedMemberTypes)] TEntity> : IEntityTypeConfiguration<TEntity>
        where TEntity : CreationBaseEntityV2<TPrimaryKey>
    {
        public virtual void Configure(EntityTypeBuilder<TEntity> builder)
        {
            builder.HasOne(x => x.CreateUser)
                .WithMany()
                .HasForeignKey(p => p.CreateUserId)
                .OnDelete(DeleteBehavior.SetNull);
        }
    }
}

/// <inheritdoc cref="CreationBaseEntityV2{TPrimaryKey}"/>
public abstract class CreationBaseEntityV2 : CreationBaseEntityV2<Guid>
{
    public new abstract class EntityTypeConfiguration<[DynamicallyAccessedMembers(IEntity.DynamicallyAccessedMemberTypes)] TEntity> : CreationBaseEntityV2<Guid>.EntityTypeConfiguration<TEntity>
        where TEntity : OperatorBaseEntityV2<Guid>
    {

    }
}
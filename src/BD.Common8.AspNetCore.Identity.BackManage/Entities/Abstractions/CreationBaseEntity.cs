namespace BD.Common8.AspNetCore.Entities.Abstractions;

#pragma warning disable SA1600 // Elements should be documented

/// <summary>
/// 基类实体 - 包含创建时间与创建人
/// </summary>
/// <typeparam name="TPrimaryKey"></typeparam>
public abstract class CreationBaseEntity<TPrimaryKey> :
    Entity<TPrimaryKey>,
    ICreationTime,
    ICreateUser,
    ICreateUserIdNullable
    where TPrimaryKey : notnull, IEquatable<TPrimaryKey>
{
    /// <inheritdoc/>
    [Comment("创建时间")]
    public DateTimeOffset CreationTime { get; set; }

    /// <inheritdoc/>
    [Comment("创建人")]
    public Guid? CreateUserId { get; set; }

    /// <inheritdoc/>
    public virtual SysUser? CreateUser { get; set; }

    public abstract class EntityTypeConfiguration<[DynamicallyAccessedMembers(IEntity.DynamicallyAccessedMemberTypes)] TEntity> : IEntityTypeConfiguration<TEntity>
        where TEntity : CreationBaseEntity<TPrimaryKey>
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

/// <inheritdoc cref="CreationBaseEntity{TPrimaryKey}"/>
public abstract class CreationBaseEntity : CreationBaseEntity<Guid>
{
    public new abstract class EntityTypeConfiguration<[DynamicallyAccessedMembers(IEntity.DynamicallyAccessedMemberTypes)] TEntity> : CreationBaseEntity<Guid>.EntityTypeConfiguration<TEntity>
        where TEntity : CreationBaseEntity<Guid>
    {
    }
}
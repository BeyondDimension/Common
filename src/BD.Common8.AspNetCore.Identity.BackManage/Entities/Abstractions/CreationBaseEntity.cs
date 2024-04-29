namespace BD.Common8.AspNetCore.Entities.Abstractions;

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
    public virtual BMUser? CreateUser { get; set; }

    /// <summary>
    /// 配置实体类型
    /// </summary>
    /// <typeparam name="TEntity"></typeparam>
    public abstract class EntityTypeConfiguration<[DynamicallyAccessedMembers(IEntity.DynamicallyAccessedMemberTypes)] TEntity> : IEntityTypeConfiguration<TEntity>
        where TEntity : CreationBaseEntity<TPrimaryKey>
    {
        /// <summary>
        /// 配置关系映射
        /// </summary>
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
    /// <summary>
    /// 配置实体类型
    /// </summary>
    /// <typeparam name="TEntity"></typeparam>
    public new abstract class EntityTypeConfiguration<[DynamicallyAccessedMembers(IEntity.DynamicallyAccessedMemberTypes)] TEntity> : CreationBaseEntity<Guid>.EntityTypeConfiguration<TEntity>
        where TEntity : CreationBaseEntity<Guid>
    {
    }
}
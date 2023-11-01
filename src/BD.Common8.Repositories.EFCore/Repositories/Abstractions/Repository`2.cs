namespace BD.Common8.Repositories.EFCore.Repositories.Abstractions;

/// <inheritdoc cref="Repository{TDbContext}"/>
public abstract class Repository<TDbContext, [DynamicallyAccessedMembers(IEntity.DynamicallyAccessedMemberTypes)] TEntity> : Repository<TDbContext>, IRepository<TEntity>, IEFRepository<TEntity>
    where TDbContext : DbContext
    where TEntity : class
{
    /// <inheritdoc/>
    public DbSet<TEntity> Entity { get; }

    /// <inheritdoc/>
    public IQueryable<TEntity> EntityNoTracking => Entity.AsNoTrackingWithIdentityResolution();

    /// <inheritdoc/>
    DbContext IEFRepository.DbContext => db;

    readonly Lazy<string> mTableName;
    readonly Lazy<IEntityType> mEntityType;
    readonly IRequestAbortedProvider requestAbortedProvider;

    /// <inheritdoc/>
    public string TableName => mTableName.Value;

    /// <summary>
    /// 获取实体类型
    /// </summary>
    public IEntityType EntityType => mEntityType.Value;

    [DynamicallyAccessedMembers(IEntity.DynamicallyAccessedMemberTypes)]
    static readonly Type entityType = typeof(TEntity);

    static readonly Lazy<bool> mIsSoftDeleted = new(() => SqlConstants.IsSoftDeleted(entityType));

    /// <summary>
    /// 是否软删除
    /// </summary>
    public static bool IsSoftDeleted => mIsSoftDeleted.Value;

    /// <summary>
    /// 获取请求中断的令牌
    /// </summary>
    protected CancellationToken RequestAborted => requestAbortedProvider.RequestAborted;

    /// <inheritdoc cref="RequestAborted"/>
    CancellationToken IEFRepository<TEntity>.RequestAborted => RequestAborted;

    /// <summary>
    /// 初始化成员
    /// </summary>
    public Repository(TDbContext dbContext, IRequestAbortedProvider requestAbortedProvider) : base(dbContext)
    {
        Entity = dbContext.Set<TEntity>();
        mEntityType = new Lazy<IEntityType>(() => dbContext.Model.FindEntityType(entityType).ThrowIsNull(nameof(entityType)));
        mTableName = new Lazy<string>(() => dbContext.Database.GetTableNameByClrType(EntityType));
        this.requestAbortedProvider = requestAbortedProvider;
    }

    #region 增(Insert Funs) 立即执行并返回受影响的行数

    /// <inheritdoc/>
    public virtual async Task<int> InsertAsync(TEntity entity, CancellationToken cancellationToken = default)
    {
        await Entity.AddAsync(entity, cancellationToken);
        return await db.SaveChangesAsync(cancellationToken);
    }

    /// <inheritdoc/>
    public virtual async Task<int> InsertRangeAsync(IEnumerable<TEntity> entities, CancellationToken cancellationToken = default)
    {
        await Entity.AddRangeAsync(entities, cancellationToken);
        return await db.SaveChangesAsync(cancellationToken);
    }

    #endregion

    #region 删(Delete Funs) 立即执行并返回受影响的行数

    /// <inheritdoc/>
    public virtual async Task<int> DeleteAsync(TEntity entity, CancellationToken cancellationToken = default)
    {
        if (IsSoftDeleted && entity is ISoftDeleted softDeleted)
        {
            softDeleted.SoftDeleted = true;
            Entity.Update(entity);
        }
        else
        {
            Entity.Remove(entity);
        }
        return await db.SaveChangesAsync(cancellationToken);
    }

    /// <inheritdoc/>
    public virtual async Task<int> DeleteRangeAsync(IEnumerable<TEntity> entities, CancellationToken cancellationToken = default)
    {
        if (IsSoftDeleted && entities is IEnumerable<ISoftDeleted> softDeleted)
        {
            foreach (var item in softDeleted)
            {
                item.SoftDeleted = true;
            }
            Entity.UpdateRange(entities);
        }
        else
        {
            Entity.RemoveRange(entities);
        }
        return await db.SaveChangesAsync(cancellationToken);
    }

    #endregion

    #region 改(Update Funs) 立即执行并返回受影响的行数

    /// <inheritdoc/>
    public virtual async Task<int> UpdateAsync(TEntity entity, CancellationToken cancellationToken = default)
    {
        Entity.Update(entity);
        return await db.SaveChangesAsync(cancellationToken);
    }

    /// <inheritdoc/>
    public virtual async Task<int> UpdateRangeAsync(IEnumerable<TEntity> entities, CancellationToken cancellationToken = default)
    {
        Entity.UpdateRange(entities);
        return await db.SaveChangesAsync(cancellationToken);
    }

    #endregion

    #region 查(通用查询)

    /// <inheritdoc/>
    public virtual async Task<TEntity?> FirstOrDefaultAsync(Expression<Func<TEntity, bool>> predicate, CancellationToken cancellationToken = default)
        => await Entity.FirstOrDefaultAsync(predicate, cancellationToken);

    #endregion
}
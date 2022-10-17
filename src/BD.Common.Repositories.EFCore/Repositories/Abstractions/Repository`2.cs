using Microsoft.EntityFrameworkCore.Metadata;
using static BD.Common.EFCoreHelper;
using static BD.Common.ModelBuilderExtensions;

namespace BD.Common.Repositories.Abstractions;

/// <inheritdoc cref="Repository{TDbContext}"/>
public abstract class Repository<TDbContext, TEntity> : Repository<TDbContext>, IRepository<TEntity>, IEFRepository<TEntity>
    where TDbContext : DbContext
    where TEntity : class
{
    /// <summary>
    /// <see cref="DbSet{TEntity}"/>
    /// </summary>
    public DbSet<TEntity> Entity { get; }

    /// <summary>
    /// <see cref="EntityFrameworkQueryableExtensions.AsNoTracking{TEntity}(IQueryable{TEntity})"/>
    /// </summary>
    public IQueryable<TEntity> EntityNoTracking => Entity.AsNoTracking();

    DbContext IEFRepository.DbContext => db;

    readonly Lazy<string> mTableName;
    readonly Lazy<IEntityType> mEntityType;
    readonly IRequestAbortedProvider requestAbortedProvider;

    public string TableName => mTableName.Value;

    public IEntityType EntityType => mEntityType.Value;

    static readonly Type entityType = typeof(TEntity);
    static readonly Lazy<bool> mIsSoftDeleted = new(() => IsSoftDeleted(entityType));

    public static bool IsSoftDeleted => mIsSoftDeleted.Value;

    protected CancellationToken RequestAborted => requestAbortedProvider.RequestAborted;

    CancellationToken IEFRepository<TEntity>.RequestAborted => RequestAborted;

    public Repository(TDbContext dbContext, IRequestAbortedProvider requestAbortedProvider) : base(dbContext)
    {
        Entity = dbContext.Set<TEntity>();
        mEntityType = new Lazy<IEntityType>(() => dbContext.Model.FindEntityType(entityType).ThrowIsNull(nameof(entityType)));
        mTableName = new Lazy<string>(() => GetTableNameByClrType(dbContext.Database, EntityType));
        this.requestAbortedProvider = requestAbortedProvider;
    }

    #region 增(Insert Funs) 立即执行并返回受影响的行数

    public virtual Task<int> InsertAsync(TEntity entity) => InsertAsync(entity, default);

    /// <inheritdoc cref="InsertAsync(TEntity)"/>
    public virtual async Task<int> InsertAsync(TEntity entity, CancellationToken cancellationToken)
    {
        await Entity.AddAsync(entity, cancellationToken);
        return await db.SaveChangesAsync(cancellationToken);
    }

    public virtual Task<int> InsertRangeAsync(IEnumerable<TEntity> entities) => InsertRangeAsync(entities, default);

    /// <inheritdoc cref="InsertRangeAsync(IEnumerable{TEntity})"/>
    public virtual async Task<int> InsertRangeAsync(IEnumerable<TEntity> entities, CancellationToken cancellationToken)
    {
        await Entity.AddRangeAsync(entities, cancellationToken);
        return await db.SaveChangesAsync(cancellationToken);
    }

    #endregion

    #region 删(Delete Funs) 立即执行并返回受影响的行数

    public virtual Task<int> DeleteAsync(TEntity entity) => DeleteAsync(entity, default);

    /// <inheritdoc cref="DeleteAsync(TEntity)"/>
    public virtual async Task<int> DeleteAsync(TEntity entity, CancellationToken cancellationToken)
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

    public virtual Task<int> DeleteRangeAsync(IEnumerable<TEntity> entities) => DeleteRangeAsync(entities, default);

    /// <inheritdoc cref="DeleteRangeAsync(IEnumerable{TEntity})"/>
    public virtual async Task<int> DeleteRangeAsync(IEnumerable<TEntity> entities, CancellationToken cancellationToken)
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

    public virtual Task<int> UpdateAsync(TEntity entity) => UpdateAsync(entity, default);

    /// <inheritdoc cref="UpdateAsync(TEntity)"/>
    public virtual async Task<int> UpdateAsync(TEntity entity, CancellationToken cancellationToken)
    {
        Entity.Update(entity);
        return await db.SaveChangesAsync(cancellationToken);
    }

    public virtual Task<int> UpdateRangeAsync(IEnumerable<TEntity> entities) => UpdateRangeAsync(entities, default);

    /// <inheritdoc cref="UpdateRangeAsync(IEnumerable{TEntity})"/>
    public virtual async Task<int> UpdateRangeAsync(IEnumerable<TEntity> entities, CancellationToken cancellationToken)
    {
        Entity.UpdateRange(entities);
        return await db.SaveChangesAsync(cancellationToken);
    }

    #endregion

    #region 查(通用查询)

    public virtual async Task<TEntity?> FirstOrDefaultAsync(Expression<Func<TEntity, bool>> predicate, CancellationToken cancellationToken)
        => await Entity.FirstOrDefaultAsync(predicate, cancellationToken);

    #endregion
}
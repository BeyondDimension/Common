using Microsoft.EntityFrameworkCore.ChangeTracking;

namespace BD.Common.Repositories.Abstractions;

/// <inheritdoc cref="Repository{TDbContext}"/>
public abstract class Repository<TDbContext, [DynamicallyAccessedMembers(IEntity.DynamicallyAccessedMemberTypes)] TEntity, TPrimaryKey> : Repository<TDbContext, TEntity>, IRepository<TEntity, TPrimaryKey>
    where TDbContext : DbContext
    where TEntity : class, IEntity<TPrimaryKey>
    where TPrimaryKey : IEquatable<TPrimaryKey>
{
    object IGetPrimaryKey<TEntity>.GetPrimaryKey(TEntity entity) => GetPrimaryKey(entity);

    protected TPrimaryKey GetPrimaryKey(TEntity entity)
    {
        IGetPrimaryKey<TEntity, TPrimaryKey> getPrimaryKey = this;
        return getPrimaryKey.GetPrimaryKey(entity);
    }

    readonly Lazy<bool> mIsAutoGeneratedKey;

    public Repository(TDbContext dbContext, IRequestAbortedProvider requestAbortedProvider) : base(dbContext, requestAbortedProvider)
    {
        mIsAutoGeneratedKey = new Lazy<bool>(() =>
        {
            var key = EntityType.FindPrimaryKey();
            if (key?.Properties != null)
            {
                return key.Properties.All(p => p.ValueGenerated == ValueGenerated.OnAdd);
            }
            return false;
        });
    }

    [Obsolete("use SetDisableByIdAsync in BD.Common.Repositories.SourceGenerator")]
    protected virtual Task<int> SetDisableAsync(IQueryable<TEntity> query, bool disable)
    {
        if (query is IQueryable<IDisable> queryDisable)
        {
            return EFRepositoryExtensions.SetDisableAsync(queryDisable, disable);
        }
        else
        {
            throw new NotSupportedException();
        }
    }

    [Obsolete("use SetDisableByIdAsync in BD.Common.Repositories.SourceGenerator")]
    public virtual async Task<int> SetDisableByIdAsync(TPrimaryKey primaryKey, bool disable)
    {
        var query = EntityNoTracking.Where(IRepository<TEntity, TPrimaryKey>.LambdaEqualId(primaryKey));
        var r = await SetDisableAsync(query, disable);
        return r;
    }

    #region 删(Delete Funs) 立即执行并返回受影响的行数

    protected virtual async Task<int> ExecuteDeleteAsync(IQueryable<TEntity> query, CancellationToken cancellationToken = default)
    {
        var r = await query.GeneralDeleteAsync(cancellationToken);
        return r;
    }

    public virtual async Task<int> DeleteAsync(TPrimaryKey primaryKey, CancellationToken cancellationToken = default)
    {
        var r = await Entity.GeneralDeleteByIdAsync(primaryKey, cancellationToken);
        return r;
    }

    public virtual async Task<int> DeleteAsync(IEnumerable<TPrimaryKey> primaryKeys, CancellationToken cancellationToken = default)
    {
        var r = await Entity.GeneralDeleteByIdAsync(primaryKeys, cancellationToken);
        return r;
    }

    public override async Task<int> DeleteAsync(TEntity entity, CancellationToken cancellationToken = default)
    {
        var r = await Entity.GeneralDeleteAsync<TEntity, TPrimaryKey>(entity, cancellationToken);
        return r;
    }

    public override async Task<int> DeleteRangeAsync(IEnumerable<TEntity> entities, CancellationToken cancellationToken)
    {
        var r = await Entity.GeneralDeleteAsync<TEntity, TPrimaryKey>(entities, cancellationToken);
        return r;
    }

    #endregion

    protected virtual CancellationToken CreateLinkedTokenSource(CancellationToken cancellationToken = default)
    {
        if (cancellationToken == default || cancellationToken == RequestAborted) return RequestAborted;
        return CancellationTokenSource.CreateLinkedTokenSource(cancellationToken, RequestAborted).Token;
    }

    #region 查(通用查询)

    public virtual async ValueTask<TEntity?> FindAsync(TPrimaryKey primaryKey, CancellationToken cancellationToken)
    {
        if (IRepository<TEntity, TPrimaryKey>.IsDefault(primaryKey)) return default;
        return await Entity.FindAsync(keyValues: new object[] { primaryKey }, cancellationToken: CreateLinkedTokenSource(cancellationToken));
    }

    public virtual async ValueTask<bool> ExistAsync(TPrimaryKey primaryKey, CancellationToken cancellationToken)
    {
        if (IRepository<TEntity, TPrimaryKey>.IsDefault(primaryKey)) return false;
        return await Entity.AnyAsync(IRepository<TEntity, TPrimaryKey>.LambdaEqualId(primaryKey), CreateLinkedTokenSource(cancellationToken));
    }

    /// <summary>
    /// 将实体类型转换为视图模型类，可使用 AutoMapper 实现重写
    /// </summary>
    /// <typeparam name="TViewModel"></typeparam>
    /// <param name="query"></param>
    /// <returns></returns>
    /// <exception cref="NotSupportedException"></exception>
    protected virtual IQueryable<TViewModel> Cast<TViewModel>(IQueryable<TEntity> query)
    {
        throw new NotSupportedException();
    }

    public virtual Task<TEditViewModel?> GetEditByIdAsync<TEditViewModel>(TPrimaryKey primaryKey, CancellationToken cancellationToken = default) where TEditViewModel : notnull, IKeyModel<TPrimaryKey>
    {
        var query = EntityNoTracking.Where(IRepository<TEntity, TPrimaryKey>.LambdaEqualId(primaryKey));
        var query_edit = Cast<TEditViewModel>(query);
        return query_edit.FirstOrDefaultAsync(CreateLinkedTokenSource(cancellationToken));
    }

    #endregion

    #region 增或改(InsertOrUpdate Funs) 立即执行并返回受影响的行数

    /// <summary>
    /// 实体是否使用自动生成的键值
    /// </summary>
    public virtual bool IsAutoGeneratedKey => mIsAutoGeneratedKey.Value;

    public virtual async Task<(int rowCount, DbRowExecResult result)> InsertOrUpdateAsync(TEntity entity, CancellationToken cancellationToken)
    {
        // https://docs.microsoft.com/zh-cn/ef/core/saving/disconnected-entities#saving-single-entities
        // 如果知道是需要插入还是需要更新，则可以相应地使用 Add 或 Update：
        // 但是，如果实体使用自动生成的键值，则 Update 方法可以用于以下两种情况：
        // Update 方法通常将实体标记为更新，而不是插入。 但是，如果实体具有自动生成的键且未设置任何键值，则实体会自动标记为插入。
        // EF Core 2.0 中已引入此行为。 对于早期版本，始终需要显式选择 Add 或 Update。
        if (IsAutoGeneratedKey)
        {
            var rowCount = await UpdateAsync(entity, cancellationToken);
            return (rowCount, DbRowExecResult.InsertOrUpdate);
        }
        else
        {
            // 如果实体不使用自动生成的键，则应用程序必须确定是应插入实体还是应更新实体：例如：
            var primaryKey = GetPrimaryKey(entity);
            var existingEntity = await FindAsync(primaryKey, cancellationToken);

            DbRowExecResult result;
            if (existingEntity == null)
            {
                Entity.Add(entity);
                result = DbRowExecResult.Insert;
            }
            else
            {
                var entityEntry = db.Entry(existingEntity);

                entityEntry.CurrentValues.SetValues(entity);
                IgnoreCreationProperties(entityEntry);

                result = DbRowExecResult.Update;
            }

            var rowCount = await db.SaveChangesAsync(cancellationToken);
            return (rowCount, result);
        }
    }

    public virtual async Task<(int rowCount, DbRowExecResult result)> InsertOrUpdateAsync<TViewModel>(
        TViewModel viewModel,
        Func<TEntity>? create = null,
        Action<TEntity>? onAdd = null,
        Action<TEntity>? onUpdate = null,
        CancellationToken cancellationToken = default) where TViewModel : notnull
    {
        TPrimaryKey primaryKey;
        if (viewModel is IEntity<TPrimaryKey> entity_id)
        {
            primaryKey = entity_id.Id;
        }
        else if (viewModel is IKeyModel<TPrimaryKey> km_id)
        {
            primaryKey = km_id.Id;
        }
        else if (viewModel is TEntity entity)
        {
            primaryKey = GetPrimaryKey(entity);
        }
        else
        {
            throw new InvalidOperationException(
                $"Type {viewModel.GetType()} must inherit from {typeof(IKeyModel<TPrimaryKey>)}.");
        }
        var existingEntity = await FindAsync(primaryKey, cancellationToken);

        DbRowExecResult result;
        if (existingEntity == null)
        {
            var entity = create == null ? Activator.CreateInstance<TEntity>() : create();
            await Entity.AddAsync(entity, cancellationToken);
            db.Entry(entity).CurrentValues.SetValues(viewModel);
            onAdd?.Invoke(entity);
            result = DbRowExecResult.Insert;
        }
        else
        {
            var entityEntry = db.Entry(existingEntity);

            entityEntry.CurrentValues.SetValues(viewModel);
            IgnoreCreationProperties(entityEntry);

            onUpdate?.Invoke(existingEntity);
            result = DbRowExecResult.Update;
        }
        var rowCount = await db.SaveChangesAsync(cancellationToken);
        return (rowCount, result);
    }

    /// <summary>
    /// 忽略实体上与创建相关的属性的修改
    /// </summary>
    /// <param name="entityEntry"></param>
    private static void IgnoreCreationProperties(EntityEntry<TEntity> entityEntry)
    {
        const string strCreationTime = nameof(ICreationTime.CreationTime);
        const string strCreateUserId = nameof(ICreateUserId.CreateUserId);

        var properties = entityEntry.Properties
            .Where(p => p.IsModified)
            .Where(p => p.Metadata.Name == strCreationTime ||
                        p.Metadata.Name == strCreateUserId);

        foreach (var prop in properties)
        {
            prop.IsModified = false;
        }
    }

    #endregion
}
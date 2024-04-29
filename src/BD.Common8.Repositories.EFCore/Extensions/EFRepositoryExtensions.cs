namespace BD.Common8.Repositories.EFCore.Extensions;

public static partial class EFRepositoryExtensions
{
    /// <summary>
    /// 获取SQL字符串 SET IDENTITY_INSERT 允许将显式值插入到表的标识列中，仅支持 SqlServer
    /// </summary>
    /// <param name="repository"></param>
    /// <param name="enable"></param>
    public static string GetSqlSetIdentityInsert(this IEFRepository repository, bool enable)
    {
        var tableName = repository.TableName;
        return SqlConstants.IDENTITY_INSERT(tableName, enable);
    }

    /// <summary>
    /// 执行SQL字符串 SET IDENTITY_INSERT 允许将显式值插入到表的标识列中，仅支持 SqlServer
    /// </summary>
    /// <param name="repository"></param>
    /// <param name="enable"></param>
    public static int SetIdentityInsert(this IEFRepository repository, bool enable)
    {
        var sql = repository.GetSqlSetIdentityInsert(enable);
        return repository.DbContext.Database.ExecuteSqlRaw(sql);
    }

    /// <summary>
    /// 执行SQL字符串 SET IDENTITY_INSERT 允许将显式值插入到表的标识列中，仅支持 SqlServer
    /// </summary>
    /// <param name="repository"></param>
    /// <param name="enable"></param>
    /// <param name="cancellationToken"></param>
    /// <returns></returns>
    public static Task<int> SetIdentityInsertAsync(this IEFRepository repository, bool enable, CancellationToken cancellationToken = default)
    {
        var sql = repository.GetSqlSetIdentityInsert(enable);
        return repository.DbContext.Database.ExecuteSqlRawAsync(sql, cancellationToken);
    }

    /// <summary>
    /// 允许将显式值插入到表的标识列中的 <see cref="DbContext.SaveChanges(bool)"/>，仅支持 SqlServer
    /// </summary>
    /// <param name="repository"></param>
    /// <returns></returns>
    public static int IdentityInsertSaveChanges(this IEFRepository repository)
    {
        int result;
        repository.DbContext.Database.OpenConnection();
        try
        {
            repository.SetIdentityInsert(true);
            result = repository.DbContext.SaveChanges();
            repository.SetIdentityInsert(false);
        }
        finally
        {
            repository.DbContext.Database.CloseConnection();
        }
        return result;
    }

    /// <summary>
    /// 允许将显式值插入到表的标识列中的 <see cref="DbContext.SaveChangesAsync(CancellationToken)"/>，仅支持 SqlServer
    /// </summary>
    /// <param name="repository"></param>
    /// <param name="cancellationToken"></param>
    /// <returns></returns>
    public static async Task<int> IdentityInsertSaveChangesAsync(this IEFRepository repository, CancellationToken cancellationToken = default)
    {
        int result;
        await repository.DbContext.Database.OpenConnectionAsync(cancellationToken);
        try
        {
            await repository.SetIdentityInsertAsync(true, cancellationToken);
            result = await repository.DbContext.SaveChangesAsync(cancellationToken);
            await repository.SetIdentityInsertAsync(false, cancellationToken);
        }
        finally
        {
            await repository.DbContext.Database.CloseConnectionAsync();
        }
        return result;
    }

    /// <summary>
    /// 允许将显式值插入到表的标识列中的 多个实体插入到数据库中，仅支持 SqlServer
    /// </summary>
    /// <typeparam name="TEntity"></typeparam>
    /// <param name="repository"></param>
    /// <param name="entities"></param>
    /// <returns></returns>
    public static int IdentityInsertRange<[DynamicallyAccessedMembers(IEntity.DynamicallyAccessedMemberTypes)] TEntity>(this IEFRepository<TEntity> repository,
        IEnumerable<TEntity> entities) where TEntity : class
    {
        repository.Entity.AddRange(entities);
        var result = repository.IdentityInsertSaveChanges();
        return result;
    }

    /// <summary>
    /// 允许将显式值插入到表的标识列中的 多个实体插入到数据库中，仅支持 SqlServer
    /// </summary>
    /// <typeparam name="TEntity"></typeparam>
    /// <param name="repository"></param>
    /// <param name="entities"></param>
    /// <param name="cancellationToken"></param>
    /// <returns></returns>
    public static async Task<int> IdentityInsertRangeAsync<[DynamicallyAccessedMembers(IEntity.DynamicallyAccessedMemberTypes)] TEntity>(this IEFRepository<TEntity> repository,
        IEnumerable<TEntity> entities,
        CancellationToken cancellationToken = default) where TEntity : class
    {
        await repository.Entity.AddRangeAsync(entities, cancellationToken);
        var result = await repository.IdentityInsertSaveChangesAsync(cancellationToken);
        return result;
    }

    /// <summary>
    /// 设置禁用状态
    /// </summary>
    /// <param name="query"></param>
    /// <param name="disable"></param>
    /// <returns></returns>
    public static async Task<int> SetDisableAsync(this IQueryable<IDisable> query, bool disable)
    {
        var r = await query.ExecuteUpdateAsync(x => x.SetProperty(y => y.Disable, y => disable));
        return r;
    }

    /// <summary>
    /// 根据主键设置禁用状态
    /// </summary>
    /// <typeparam name="TEntity"></typeparam>
    /// <typeparam name="TPrimaryKey"></typeparam>
    /// <param name="query"></param>
    /// <param name="id"></param>
    /// <param name="disable"></param>
    /// <returns></returns>
    public static async Task<int> SetDisableByIdAsync<[DynamicallyAccessedMembers(IEntity.DynamicallyAccessedMemberTypes)] TEntity, TPrimaryKey>(this IQueryable<TEntity> query, TPrimaryKey id, bool disable)
        where TEntity : class, IDisable, IEntity<TPrimaryKey>
        where TPrimaryKey : IEquatable<TPrimaryKey>
    {
        var r = await query
            .Where(IRepository<TEntity, TPrimaryKey>.LambdaEqualId(id))
            .SetDisableAsync(disable);
        return r;
    }
}
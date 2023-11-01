using KeyValuePair = BD.Common8.Repositories.EFCore.Entities.KeyValuePair;

namespace BD.Common8.Repositories.EFCore.Repositories;

/// <summary>
/// 由 <see cref="Repository{TDbContext, TEntity, TPrimaryKey}"/> 实现的 <see cref="ISecureStorage"/>
/// <para>无加密(明文)</para>
/// </summary>
/// <typeparam name="TDbContext"></typeparam>
internal sealed class RepositorySecureStorage<TDbContext>(TDbContext dbContext, IRequestAbortedProvider requestAbortedProvider) : Repository<TDbContext, KeyValuePair, string>(dbContext, requestAbortedProvider), ISecureStorage
    where TDbContext : DbContext
{
    /// <inheritdoc/>
    bool ISecureStorage.IsNativeSupportedBytes => false;

    /// <summary>
    /// 根据键获取值的查询
    /// </summary>
    IQueryable<string> Get(string key)
        => Entity.Where(x => !x.SoftDeleted && x.Id == key).Select(x => x.Value);

    /// <inheritdoc/>
    async Task<string?> ISecureStorage.GetAsync(string key)
    {
        var value = await Get(key).FirstOrDefaultAsync(RequestAborted);
        return value;
    }

    /// <summary>
    /// 根据键获取键值对项的查询
    /// </summary>
    IQueryable<KeyValuePair> GetSetQuery(string key)
        => Entity.IgnoreQueryFilters().Where(x => x.Id == key);

    /// <summary>
    /// 获取更新值的表达式
    /// </summary>
    /// <param name="value"></param>
    /// <returns></returns>
    static Expression<Func<KeyValuePair, KeyValuePair>> GetSetValueUpdateExpression(string value)
           => x => new KeyValuePair { SoftDeleted = false, Value = value };

    /// <inheritdoc/>
    Task ISecureStorage.SetAsync(string key, string? value)
    {
        if (string.IsNullOrEmpty(value))
            return DeleteAsync(key);
        else
            return GetSetQuery(key).UpdateAsync(GetSetValueUpdateExpression(value));
    }

    /// <inheritdoc/>
    async Task<bool> ISecureStorage.RemoveAsync(string key)
    {
        var result = await DeleteAsync(key);
        return result > 0;
    }

    /// <inheritdoc/>
    async Task<bool> ISecureStorage.ContainsKeyAsync(string key)
    {
        var result = await Get(key).AnyAsync(RequestAborted);
        return result;
    }
}
using KeyValuePair = BD.Common8.Repositories.Entities.KeyValuePair;

#pragma warning disable SA1600 // Elements should be documented

namespace BD.Common8.Repositories.Repositories;

/// <summary>
/// 由 <see cref="Repository{TDbContext, TEntity, TPrimaryKey}"/> 实现的 <see cref="ISecureStorage"/>
/// <para>无加密(明文)</para>
/// </summary>
/// <typeparam name="TDbContext"></typeparam>
internal sealed class RepositorySecureStorage<TDbContext>(TDbContext dbContext, IRequestAbortedProvider requestAbortedProvider) : Repository<TDbContext, KeyValuePair, string>(dbContext, requestAbortedProvider), ISecureStorage
    where TDbContext : DbContext
{
    bool ISecureStorage.IsNativeSupportedBytes => false;

    IQueryable<string> Get(string key)
        => Entity.Where(x => !x.SoftDeleted && x.Id == key).Select(x => x.Value);

    async Task<string?> ISecureStorage.GetAsync(string key)
    {
        var value = await Get(key).FirstOrDefaultAsync(RequestAborted);
        return value;
    }

    IQueryable<KeyValuePair> GetSetQuery(string key)
        => Entity.IgnoreQueryFilters().Where(x => x.Id == key);

    static Expression<Func<KeyValuePair, KeyValuePair>> GetSetValueUpdateExpression(string value)
           => x => new KeyValuePair { SoftDeleted = false, Value = value };

    Task ISecureStorage.SetAsync(string key, string? value)
    {
        if (string.IsNullOrEmpty(value))
            return DeleteAsync(key);
        else
            return GetSetQuery(key).UpdateAsync(GetSetValueUpdateExpression(value));
    }

    async Task<bool> ISecureStorage.RemoveAsync(string key)
    {
        var result = await DeleteAsync(key);
        return result > 0;
    }

    async Task<bool> ISecureStorage.ContainsKeyAsync(string key)
    {
        var result = await Get(key).AnyAsync(RequestAborted);
        return result;
    }
}
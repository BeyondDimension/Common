using KeyValuePair = BD.Common8.Repositories.SQLitePCL.Entities.KeyValuePair;

namespace BD.Common8.Repositories.SQLitePCL.Repositories;

/// <summary>
/// 由 <see cref="Repository{TEntity, TPrimaryKey}"/> 实现的 <see cref="ISecureStorage"/>
/// <para>加密由 <see cref="ISecurityService"/> 实现</para>
/// </summary>
sealed class RepositorySecureStorage(ISecurityService ss) : Repository<KeyValuePair, string>, ISecureStorage
{
    /// <inheritdoc cref="ISecurityService"/>
    readonly ISecurityService ss = ss;

    /// <inheritdoc/>
    bool ISecureStorage.IsNativeSupportedBytes => true;

    /// <summary>
    /// 获取 SHA256 哈希算法处理后的密钥
    /// </summary>
    static string GetKey(string key) => Hashs.String.SHA256(key);

    /// <inheritdoc/>
    async Task<byte[]?> ISecureStorage.GetBytesAsync(string key)
    {
        key = GetKey(key);
        var item = await FirstOrDefaultAsync(x => x.Id == key);
        var value = item?.Value;
        var value2 = await ss.DecryptBytesToBytesAsync(value);
        return value2;
    }

    async Task InsertOrUpdateAsync(string key, byte[] value)
    {
        var value2 = await ss.EncryptBytesToBytesAsync(value);
        await InsertOrUpdateAsync(new KeyValuePair
        {
            Id = key,
            Value = value2.ThrowIsNull(nameof(value2)),
        });
    }

    /// <inheritdoc/>
    Task ISecureStorage.SetAsync(string key, byte[]? value)
    {
        key = GetKey(key);
        if (value == null || value.Length <= 0)
            return DeleteAsync(key);
        else
            return InsertOrUpdateAsync(key, value);
    }

    /// <inheritdoc/>
    async Task<bool> ISecureStorage.RemoveAsync(string key)
    {
        key = GetKey(key);
        var result = await DeleteAsync(key);
        return result > 0;
    }

    /// <inheritdoc/>
    async Task<bool> ISecureStorage.ContainsKeyAsync(string key)
    {
        key = GetKey(key);
        var item = await FirstOrDefaultAsync(x => x.Id == key);
        return item != null;
    }
}
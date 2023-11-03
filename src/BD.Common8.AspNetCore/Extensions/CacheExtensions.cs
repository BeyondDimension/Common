#pragma warning disable IDE0130 // 命名空间与文件夹结构不匹配
namespace Microsoft.Extensions.Caching;

#pragma warning disable SA1600 // Elements should be documented

/// <summary>
/// 缓存扩展类
/// </summary>
public static partial class CacheExtensions
{
    #region IMemoryCache

    /// <summary>
    /// 从缓存中获取指定键的值
    /// </summary>
    /// <typeparam name="T"></typeparam>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static bool Get<T>(this IMemoryCache cache, object key, out T? value) where T : notnull => cache.TryGetValue(key, out value);

    /// <summary>
    /// 将指定键和值添加到缓存中，并设置过期时间
    /// </summary>
    /// <typeparam name="T"></typeparam>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static void Set<T>(this IMemoryCache cache, object key, T value, int minutes) where T : notnull => cache.Set(key, value, TimeSpan.FromMinutes(minutes));

    #endregion

    #region IDistributedCache & MessagePack

    [Obsolete("仅用于旧业务中需要兼容的部分，对于新的功能需要使用 MemoryPack 实现的 V2 版本")]
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static async Task<T?> GetAsync<T>(this IDistributedCache cache, string key, CancellationToken cancellationToken = default) where T : notnull
    {
        var buffer = await cache.GetAsync(key, cancellationToken);
        if (buffer == null) return default;
        var r = Serializable.DMP<T>(buffer, cancellationToken);
        return r;
    }

    [Obsolete("仅用于旧业务中需要兼容的部分，对于新的功能需要使用 MemoryPack 实现的 V2 版本")]
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static async Task SetAsync<T>(this IDistributedCache cache, string key, T value, DistributedCacheEntryOptions options, CancellationToken cancellationToken = default) where T : notnull
    {
        var buffer = Serializable.SMP(value, cancellationToken);
        await cache.SetAsync(key, buffer, options, cancellationToken);
    }

    [Obsolete("仅用于旧业务中需要兼容的部分，对于新的功能需要使用 MemoryPack 实现的 V2 版本")]
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static Task SetAsync<T>(this IDistributedCache cache, string key, T value, TimeSpan absoluteExpirationRelativeToNow, CancellationToken cancellationToken = default) where T : notnull
        => cache.SetAsync(key, value, new DistributedCacheEntryOptions { AbsoluteExpirationRelativeToNow = absoluteExpirationRelativeToNow }, cancellationToken);

    [Obsolete("仅用于旧业务中需要兼容的部分，对于新的功能需要使用 MemoryPack 实现的 V2 版本")]
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static Task SetAsync<T>(this IDistributedCache cache, string key, T value, int minutes, CancellationToken cancellationToken = default) where T : notnull
        => cache.SetAsync(key, value, TimeSpan.FromMinutes(minutes), cancellationToken);

    #endregion

    #region IDistributedCache & MemoryPack

    /// <summary>
    /// 异步从缓存中获取指定键的值
    /// </summary>
    /// <typeparam name="T"></typeparam>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static async Task<T?> GetV2Async<T>(this IDistributedCache cache, string key, CancellationToken cancellationToken = default) where T : notnull
    {
        var buffer = await cache.GetAsync(key, cancellationToken);
        if (buffer == null) return default;
        var r = Serializable.DMP2<T>(buffer);
        return r;
    }

    /// <summary>
    /// 异步将指定键和值添加到缓存中，并指定缓存选项
    /// </summary>
    /// <typeparam name="T"></typeparam>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static async Task SetV2Async<T>(this IDistributedCache cache, string key, T value, DistributedCacheEntryOptions options, CancellationToken cancellationToken = default) where T : notnull
    {
        var buffer = Serializable.SMP2(value);
        await cache.SetAsync(key, buffer, options, cancellationToken);
    }

    /// <summary>
    /// 异步指定键和值添加到缓存中，并设置相对于当前时间的绝对过期时间
    /// </summary>
    /// <typeparam name="T"></typeparam>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static Task SetV2Async<T>(this IDistributedCache cache, string key, T value, TimeSpan absoluteExpirationRelativeToNow, CancellationToken cancellationToken = default) where T : notnull
        => cache.SetV2Async(key, value, new DistributedCacheEntryOptions { AbsoluteExpirationRelativeToNow = absoluteExpirationRelativeToNow }, cancellationToken);

    /// <summary>
    /// 异步将指定键和值添加到分布式缓存中，并设置过期时间
    /// </summary>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static Task SetV2Async<T>(this IDistributedCache cache, string key, T value, int minutes, CancellationToken cancellationToken = default) where T : notnull
        => cache.SetV2Async(key, value, TimeSpan.FromMinutes(minutes), cancellationToken);

    #endregion
}

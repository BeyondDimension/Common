// ReSharper disable once CheckNamespace
namespace System;

public static partial class CacheExtensions
{
    #region IMemoryCache

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static bool Get<T>(this IMemoryCache cache, object key, out T? value) where T : notnull => cache.TryGetValue(key, out value);

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static void Set<T>(this IMemoryCache cache, object key, T value, int minutes) where T : notnull => cache.Set(key, value, TimeSpan.FromMinutes(minutes));

    #endregion

    #region IDistributedCache

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static async Task<T?> GetAsync<T>(this IDistributedCache cache, string key, CancellationToken cancellationToken = default) where T : notnull
    {
        var buffer = await cache.GetAsync(key, cancellationToken);
        if (buffer == null) return default;
        var r = Serializable.DMP<T>(buffer, cancellationToken);
        return r;
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static async Task SetAsync<T>(this IDistributedCache cache, string key, T value, DistributedCacheEntryOptions options, CancellationToken cancellationToken = default) where T : notnull
    {
        var buffer = Serializable.SMP(value, cancellationToken);
        await cache.SetAsync(key, buffer, options, cancellationToken);
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static Task SetAsync<T>(this IDistributedCache cache, string key, T value, TimeSpan absoluteExpirationRelativeToNow, CancellationToken cancellationToken = default) where T : notnull
        => cache.SetAsync(key, value, new DistributedCacheEntryOptions { AbsoluteExpirationRelativeToNow = absoluteExpirationRelativeToNow }, cancellationToken);

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static Task SetAsync<T>(this IDistributedCache cache, string key, T value, int minutes, CancellationToken cancellationToken = default) where T : notnull
        => cache.SetAsync(key, value, TimeSpan.FromMinutes(minutes), cancellationToken);

    #endregion
}

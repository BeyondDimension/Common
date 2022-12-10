using Microsoft.Extensions.Caching.Distributed;
using Microsoft.Extensions.Caching.Memory;

// ReSharper disable once CheckNamespace
namespace System;

public static partial class CacheExtensions
{
    #region IMemoryCache

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static bool Get<T>(this IMemoryCache cache, string key, out T? value) where T : notnull => cache.TryGetValue(key, out value);

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static bool Get<T>(this IMemoryCache cache, Guid key, out T? value) where T : notnull => cache.TryGetValue(key, out value);

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static void Set<T>(this IMemoryCache cache, string key, T value, int minutes) where T : notnull => cache.Set(key, value, TimeSpan.FromMinutes(minutes));

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static void Set<T>(this IMemoryCache cache, Guid key, T value, int minutes) where T : notnull => cache.Set(key, value, TimeSpan.FromMinutes(minutes));

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
    public static Task<T?> GetAsync<T>(this IDistributedCache cache, Guid key, CancellationToken cancellationToken = default) where T : notnull => cache.GetAsync<T>(key.ToString(), cancellationToken);

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static async Task SetAsync<T>(this IDistributedCache cache, string key, T value, int minutes, CancellationToken cancellationToken = default) where T : notnull
    {
        var buffer = Serializable.SMP(value, cancellationToken);
        await cache.SetAsync(key, buffer, new DistributedCacheEntryOptions { AbsoluteExpirationRelativeToNow = TimeSpan.FromMinutes(minutes) }, cancellationToken);
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static Task SetAsync<T>(this IDistributedCache cache, Guid key, T value, int minutes, CancellationToken cancellationToken = default) where T : notnull => cache.SetAsync<T>(key.ToString(), value, minutes, cancellationToken);

    #endregion
}

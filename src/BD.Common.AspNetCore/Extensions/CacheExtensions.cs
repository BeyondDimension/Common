using Microsoft.Extensions.Caching.Memory;

// ReSharper disable once CheckNamespace
namespace System;

public static partial class CacheExtensions
{
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static bool Get<T>(this IMemoryCache cache, string key, out T? value) where T : class
    {
        return cache.TryGetValue(key, out value);
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static void Set<T>(this IMemoryCache cache, string key, T value, int minutes) where T : class
    {
        cache.Set(key, value, TimeSpan.FromMinutes(minutes));
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static bool Get<T>(this IMemoryCache cache, Guid key, out T? value) where T : class
    {
        return cache.TryGetValue(key, out value);
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static void Set<T>(this IMemoryCache cache, Guid key, T value, int minutes) where T : class
    {
        cache.Set(key, value, TimeSpan.FromMinutes(minutes));
    }
}

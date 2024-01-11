namespace BD.Common8.Essentials.Helpers;

/// <inheritdoc cref="IPreferencesPlatformService"/>
public static class Preferences2
{
    /// <inheritdoc cref="IPreferencesPlatformService.PlatformContainsKey(string, string?)"/>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static bool ContainsKey(string key) =>
        ContainsKey(key, null);

    /// <inheritdoc cref="IPreferencesPlatformService.PlatformRemove(string, string?)"/>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static void Remove(string key) =>
        Remove(key, null);

    /// <inheritdoc cref="IPreferencesPlatformService.PlatformClear(string?)"/>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static void Clear() =>
        Clear(null);

    /// <inheritdoc cref="IPreferencesPlatformService.PlatformGet(string, string?, string?)"/>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static string? Get(string key, string? defaultValue) =>
        Get(key, defaultValue, null);

    /// <inheritdoc cref="IPreferencesPlatformService.PlatformGet(string, bool, string?)"/>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static bool Get(string key, bool defaultValue) =>
        Get(key, defaultValue, null);

    /// <inheritdoc cref="IPreferencesPlatformService.PlatformGet(string, int, string?)"/>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static int Get(string key, int defaultValue) =>
        Get(key, defaultValue, null);

    /// <inheritdoc cref="IPreferencesPlatformService.PlatformGet(string, double, string?)"/>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static double Get(string key, double defaultValue) =>
        Get(key, defaultValue, null);

    /// <inheritdoc cref="IPreferencesPlatformService.PlatformGet(string, float, string?)"/>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static float Get(string key, float defaultValue) =>
        Get(key, defaultValue, null);

    /// <inheritdoc cref="IPreferencesPlatformService.PlatformGet(string, long, string?)"/>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static long Get(string key, long defaultValue) =>
        Get(key, defaultValue, null);

    /// <inheritdoc cref="IPreferencesPlatformService.PlatformSet(string, string?, string?)"/>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static void Set(string key, string value) =>
        Set(key, value, null);

    /// <inheritdoc cref="IPreferencesPlatformService.PlatformSet(string, bool, string?)"/>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static void Set(string key, bool value) =>
        Set(key, value, null);

    /// <inheritdoc cref="IPreferencesPlatformService.PlatformSet(string, int, string?)"/>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static void Set(string key, int value) =>
        Set(key, value, null);

    /// <inheritdoc cref="IPreferencesPlatformService.PlatformSet(string, double, string?)"/>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static void Set(string key, double value) =>
        Set(key, value, null);

    /// <inheritdoc cref="IPreferencesPlatformService.PlatformSet(string, float, string?)"/>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static void Set(string key, float value) =>
        Set(key, value, null);

    /// <inheritdoc cref="IPreferencesPlatformService.PlatformSet(string, long, string?)"/>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static void Set(string key, long value) =>
        Set(key, value, null);

    /// <inheritdoc cref="IPreferencesPlatformService.PlatformContainsKey(string, string?)"/>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static bool ContainsKey(string key, string? sharedName)
        => IPreferencesPlatformService.Instance.PlatformContainsKey(key, sharedName);

    /// <inheritdoc cref="IPreferencesPlatformService.PlatformRemove(string, string?)"/>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static void Remove(string key, string? sharedName)
        => IPreferencesPlatformService.Instance.PlatformRemove(key, sharedName);

    /// <inheritdoc cref="IPreferencesPlatformService.PlatformClear(string?)"/>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static void Clear(string? sharedName)
        => IPreferencesPlatformService.Instance.PlatformClear(sharedName);

    /// <inheritdoc cref="IPreferencesPlatformService.PlatformGet(string, string?, string?)"/>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static string? Get(string key, string? defaultValue, string? sharedName)
        => IPreferencesPlatformService.Instance.PlatformGet(key, defaultValue, sharedName);

    /// <inheritdoc cref="IPreferencesPlatformService.PlatformGet(string, bool, string?)"/>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static bool Get(string key, bool defaultValue, string? sharedName)
        => IPreferencesPlatformService.Instance.PlatformGet(key, defaultValue, sharedName);

    /// <inheritdoc cref="IPreferencesPlatformService.PlatformGet(string, int, string?)"/>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static int Get(string key, int defaultValue, string? sharedName)
        => IPreferencesPlatformService.Instance.PlatformGet(key, defaultValue, sharedName);

    /// <inheritdoc cref="IPreferencesPlatformService.PlatformGet(string, bool, string?)"/>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static double Get(string key, double defaultValue, string? sharedName) => IPreferencesPlatformService.Instance.PlatformGet(key, defaultValue, sharedName);

    /// <inheritdoc cref="IPreferencesPlatformService.PlatformGet(string, float, string?)"/>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static float Get(string key, float defaultValue, string? sharedName)
        => IPreferencesPlatformService.Instance.PlatformGet(key, defaultValue, sharedName);

    /// <inheritdoc cref="IPreferencesPlatformService.PlatformGet(string, long, string?)"/>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static long Get(string key, long defaultValue, string? sharedName)
        => IPreferencesPlatformService.Instance.PlatformGet(key, defaultValue, sharedName);

    /// <inheritdoc cref="IPreferencesPlatformService.PlatformSet(string, string?, string?)"/>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static void Set(string key, string value, string? sharedName)
        => IPreferencesPlatformService.Instance.PlatformSet(key, value, sharedName);

    /// <inheritdoc cref="IPreferencesPlatformService.PlatformSet(string, bool, string?)"/>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static void Set(string key, bool value, string? sharedName)
        => IPreferencesPlatformService.Instance.PlatformSet(key, value, sharedName);

    /// <inheritdoc cref="IPreferencesPlatformService.PlatformSet(string, int, string?)"/>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static void Set(string key, int value, string? sharedName)
        => IPreferencesPlatformService.Instance.PlatformSet(key, value, sharedName);

    /// <inheritdoc cref="IPreferencesPlatformService.PlatformSet(string, double, string?)"/>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static void Set(string key, double value, string? sharedName)
        => IPreferencesPlatformService.Instance.PlatformSet(key, value, sharedName);

    /// <inheritdoc cref="IPreferencesPlatformService.PlatformSet(string, float, string?)"/>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static void Set(string key, float value, string? sharedName)
        => IPreferencesPlatformService.Instance.PlatformSet(key, value, sharedName);

    /// <inheritdoc cref="IPreferencesPlatformService.PlatformSet(string, long, string?)"/>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static void Set(string key, long value, string? sharedName)
        => IPreferencesPlatformService.Instance.PlatformSet(key, value, sharedName);

    /// <inheritdoc cref="IPreferencesPlatformService.PlatformGet(string, DateTime, string?)"/>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static DateTime Get(string key, DateTime defaultValue) =>
        Get(key, defaultValue, null);

    /// <inheritdoc cref="IPreferencesPlatformService.PlatformSet(string, DateTime, string?)"/>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static void Set(string key, DateTime value) =>
        Set(key, value, null);

    /// <inheritdoc cref="IPreferencesPlatformService.PlatformGet(string, DateTime, string?)"/>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static DateTime Get(string key, DateTime defaultValue, string? sharedName)
        => DateTime.FromBinary(IPreferencesPlatformService.Instance.PlatformGet(key, defaultValue.ToBinary(), sharedName));

    /// <inheritdoc cref="IPreferencesPlatformService.PlatformSet(string, DateTime, string?)"/>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static void Set(string key, DateTime value, string? sharedName)
        => IPreferencesPlatformService.Instance.PlatformSet(key, value.ToBinary(), sharedName);
}
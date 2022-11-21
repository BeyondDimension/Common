namespace BD.Common;

partial class Browser2
{
    /// <inheritdoc cref="String2.IsHttpUrl(string?, bool)"/>
    [Obsolete("use String2.IsHttpUrl(string?, bool)")]
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static bool IsHttpUrl([NotNullWhen(true)] string? url, bool httpsOnly = false)
        => String2.IsHttpUrl(url, httpsOnly);

    /// <inheritdoc cref="String2.IsStoreUrl(string?)"/>
    [Obsolete("use String2.IsStoreUrl(string?)")]
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static bool IsStoreUrl([NotNullWhen(true)] string? url)
        => String2.IsStoreUrl(url);

    /// <inheritdoc cref="String2.IsEmailUrl(string?)"/>
    [Obsolete("use String2.IsEmailUrl(string?)")]
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static bool IsEmailUrl([NotNullWhen(true)] string? url)
        => String2.IsEmailUrl(url);

    /// <inheritdoc cref="String2.IsFileUrl(string?)"/>
    [Obsolete("use String2.IsFileUrl(string?)")]
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static bool IsFileUrl([NotNullWhen(true)] string? url)
        => String2.IsFileUrl(url);
}

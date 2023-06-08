// ReSharper disable once CheckNamespace
namespace System;

public static partial class StringExtensions
{
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static bool OICEquals(this string? l, string? r)
        => string.Equals(l, r, StringComparison.OrdinalIgnoreCase);
}
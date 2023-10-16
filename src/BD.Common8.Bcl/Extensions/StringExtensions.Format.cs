namespace System;

public static partial class StringExtensions // Format
{
    /// <inheritdoc cref="string.Format(string, object?[])"/>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static string Format(this string format, params object?[] args)
    {
        try
        {
            return string.Format(format, args);
        }
        catch
        {
#if NETCOREAPP2_0_OR_GREATER || NETSTANDARD2_1_OR_GREATER
            return string.Join(' ', new[] { format }.Concat(args));
#else
            return string.Join(" ", new[] { format }.Concat(args));
#endif
        }
    }
}

// ReSharper disable once CheckNamespace
namespace System;

public static partial class StringExtensions
{
    public static string Format(this string format, params object?[] args)
    {
        try
        {
            return string.Format(format, args);
        }
        catch
        {
            return string.Join(' ', new[] { format }.Concat(args));
        }
    }
}

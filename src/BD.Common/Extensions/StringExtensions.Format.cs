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
            var args_ = args.ToList();
            args_.Insert(0, format);
            return string.Join(' ', args_);
        }
    }
}

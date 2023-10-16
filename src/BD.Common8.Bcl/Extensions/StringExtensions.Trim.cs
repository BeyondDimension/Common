namespace System;

public static partial class StringExtensions // Trim
{
    /// <summary>
    /// 从当前字符串删除所有前导空白字符串。
    /// </summary>
    /// <param name="s"></param>
    /// <param name="value"></param>
    /// <returns></returns>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static string TrimStart(this string s, string value)
    {
        if (s.StartsWith(value))
        {
            return s[value.Length..];
        }
        else
        {
            return s;
        }
    }

    /// <summary>
    /// 从当前字符串删除所有尾随空白字符串。
    /// </summary>
    /// <param name="s"></param>
    /// <param name="value"></param>
    /// <returns></returns>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static string TrimEnd(this string s, string value)
    {
        if (s.EndsWith(value))
        {
            return s[..^value.Length];
        }
        else
        {
            return s;
        }
    }

    /// <summary>
    /// 从当前字符串删除所有前导空白字符串，使用 <see cref="StringComparison"/> 可选如何比较。
    /// </summary>
    /// <param name="s"></param>
    /// <param name="value"></param>
    /// <param name="comparisonType"></param>
    /// <returns></returns>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static string TrimStart(this string s, string value, StringComparison comparisonType)
    {
        if (s.StartsWith(value, comparisonType))
        {
            return s[value.Length..];
        }
        else
        {
            return s;
        }
    }

    /// <summary>
    /// 从当前字符串删除所有尾随空白字符串，使用 <see cref="StringComparison"/> 可选如何比较。
    /// </summary>
    /// <param name="s"></param>
    /// <param name="value"></param>
    /// <param name="comparisonType"></param>
    /// <returns></returns>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static string TrimEnd(this string s, string value, StringComparison comparisonType)
    {
        if (s.EndsWith(value, comparisonType))
        {
            return s[..^value.Length];
        }
        else
        {
            return s;
        }
    }
}
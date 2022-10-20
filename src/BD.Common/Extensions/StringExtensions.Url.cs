using System.Text.RegularExpressions;

// ReSharper disable once CheckNamespace
namespace System;

public static partial class StringExtensions
{
    /// <summary>
    /// 返回当前最后相对 URL
    /// </summary>
    /// <param name="url"></param>
    /// <returns></returns>
    public static string GetLastRelativeUrl(this string url)
    {
        int index = url.LastIndexOf("/");

        if (index != -1)
        {
            return url[..index] + "/";
        }
        else
        {
            return "";
        }
    }

    /// <summary>
    /// Converts a wildcard to a regex.
    /// </summary>
    /// <param name="str"></param>
    /// <param name="pattern">The wildcard pattern to convert.</param>
    /// <param name="options"></param>
    /// <returns>A regex equivalent of the given wildcard.</returns>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static bool IsWildcard(this string str, string pattern, RegexOptions options = RegexOptions.Compiled)
        => Regex.IsMatch(str, "^" + Regex.Escape(pattern)
            .Replace("\\*", ".*")
            .Replace("\\?", ".") + "$", options);

    /// <summary>
    /// 域名表达式
    /// *表示任意0到多个字符
    /// </summary>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static bool IsDomainPattern(this string str, string pattern, RegexOptions options = RegexOptions.IgnoreCase)
        => Regex.IsMatch(str, Regex.Escape(pattern)
            .Replace(@"\*", @"[^\.]*"), options);
}
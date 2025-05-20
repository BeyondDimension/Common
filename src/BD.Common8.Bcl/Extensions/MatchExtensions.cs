using System.Diagnostics.CodeAnalysis;
using System.Runtime.CompilerServices;
using System.Text.RegularExpressions;

namespace System.Extensions;

/// <summary>
/// 提供对 <see cref="Match"/> 类型的扩展函数
/// </summary>
public static partial class MatchExtensions
{
    /// <summary>
    /// 获取正则表达式匹配的单个字符串值
    /// </summary>
    /// <param name="match"></param>
    /// <param name="action"></param>
    /// <returns></returns>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static string GetValue(this Match match, Func<Match, bool> action)
        => action.Invoke(match) ? match.Value.Trim() : "";

    /// <summary>
    /// 获取正则表达式匹配的多个字符串值
    /// </summary>
    /// <param name="match"></param>
    /// <param name="action"></param>
    /// <returns></returns>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static IEnumerable<string> GetValues(this MatchCollection match, Func<Match, bool> action)
    {
        foreach (Match item in match.Cast<Match>())
        {
            if (action.Invoke(item))
            {
                yield return item.Value.Trim();
            }
        }
    }

    /// <summary>
    /// 获取正则表达式匹配的单个字符串值
    /// </summary>
    /// <param name="regex"></param>
    /// <param name="input"></param>
    /// <param name="defaultValue"></param>
    /// <returns></returns>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    [return: NotNullIfNotNull(nameof(defaultValue))]
    public static string? GetValue(
        this Regex regex,
        ReadOnlySpan<char> input,
        string? defaultValue = "")
    {
#if DEBUG
        var a = regex.Match(new string(input));
        var b = a.GetValue(it => it.Success);
#endif

        foreach (var it in regex.EnumerateMatches(input))
        {
            if (input.IsEmpty)
            {
                break;
            }
            var val = input.Slice(it.Index, it.Length).Trim();
            if (val.EndsWith(Environment.NewLine))
            {
                val = val[..^Environment.NewLine.Length];
            }
            val = val.Trim();
            return new(val);
        }
        return defaultValue;
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    [return: NotNullIfNotNull(nameof(defaultValue))]
    public static string? GetJoinValue(
        this Regex regex,
        ReadOnlySpan<char> input,
        char separator,
        string? defaultValue = "")
    {
        List<string> list = new();
        foreach (var it in regex.EnumerateMatches(input))
        {
            if (input.IsEmpty)
            {
                break;
            }
            list.Add(new(input.Slice(it.Index, it.Length).Trim()));
        }
        if (list.Count != 0)
        {
            return string.Join(separator, list);
        }
        return defaultValue;
    }
}
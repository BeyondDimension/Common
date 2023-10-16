namespace System.Extensions;

/// <summary>
/// 提供对 <see cref="System.Text.RegularExpressions.Match"/> 类型的扩展函数
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
            if (action.Invoke(item))
                yield return item.Value.Trim();
    }
}
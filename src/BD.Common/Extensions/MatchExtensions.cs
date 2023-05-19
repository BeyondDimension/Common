// ReSharper disable once CheckNamespace
namespace System;

public static partial class MatchExtensions
{
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static string GetValue(this Match match, Func<Match, bool> action)
    {
        return action.Invoke(match) ? match.Value.Trim() : "";
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static IEnumerable<string> GetValues(this MatchCollection match, Func<Match, bool> action)
    {
        foreach (Match item in match.Cast<Match>())
        {
            if (action.Invoke(item))
                yield return item.Value.Trim();
        }
    }
}
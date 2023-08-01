// ReSharper disable once CheckNamespace
namespace System;

public static partial class TimeSpanExtensions
{
    /// <summary>
    /// 将 <see cref="TimeSpan"/> 转换为显示字符串
    /// </summary>
    /// <param name="timeSpan"></param>
    /// <returns></returns>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static string ToDisplayString(this TimeSpan timeSpan)
    {
        var result = $"{Math.Floor(timeSpan.TotalHours):00}:{timeSpan.Minutes:00}:{timeSpan.Seconds:00}";
        return result;
    }
}

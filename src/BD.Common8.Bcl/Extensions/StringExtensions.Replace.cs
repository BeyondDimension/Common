namespace System;

public static partial class StringExtensions // Replace
{
    /// <summary>
    /// 移除字符串内所有\r \n \t
    /// </summary>
    /// <param name="s"></param>
    /// <returns></returns>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static string RemovePattern(this string s)
        => s.Replace("\t", "").Replace("\r", "").Replace("\n", "");
}
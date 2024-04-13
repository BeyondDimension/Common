namespace System.Extensions;

public static partial class StringExtensions
{
    /// <summary>
    /// 比较两个字符串是否相等，忽略大小写
    /// <para><see cref="string.Equals(string, string, StringComparison)"/> + <see cref="StringComparison.OrdinalIgnoreCase"/></para>
    /// </summary>
    /// <param name="l"></param>
    /// <param name="r"></param>
    /// <returns></returns>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static bool OICEquals(this string? l, string? r)
        => string.Equals(l, r, StringComparison.OrdinalIgnoreCase);
}

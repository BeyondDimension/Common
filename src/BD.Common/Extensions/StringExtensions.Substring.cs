// ReSharper disable once CheckNamespace
namespace System;

public static partial class StringExtensions
{
    /// <summary>
    /// 获取两个字符串中间的字符串
    /// </summary>
    /// <param name="s">要处理的字符串，例 ABCD</param>
    /// <param name="left">第1个字符串，例 AB</param>
    /// <param name="right">第2个字符串，例 D</param>
    /// <param name="isContains">是否包含标志字符串</param>
    /// <returns>例 返回C</returns>
    public static string Substring(this string s, string left, string right, bool isContains = false)
    {
        int i1 = s.IndexOf(left);
        if (i1 < 0) // 找不到返回空
        {
            return "";
        }

        int i2 = s.IndexOf(right, i1 + left.Length); // 从找到的第1个字符串后再去找
        if (i2 < 0) // 找不到返回空
        {
            return "";
        }

        if (isContains) return s.Substring(i1, i2 - i1 + left.Length);
        else return s.Substring(i1 + left.Length, i2 - i1 - left.Length);
    }
}
namespace System;

public static partial class StringExtensions // NumberLetter
{
    /// <summary>
    /// 字符串是否是一个数字（可以零开头 无符号不能+-.）
    /// </summary>
    /// <param name="input"></param>
    /// <returns></returns>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static bool IsDigital(this string? input) => input?.All(IsDigital) ?? false;

    /// <summary>
    /// 字符是否是一个数字 0~9
    /// </summary>
    /// <param name="input"></param>
    /// <returns></returns>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static bool IsDigital(this char input) => input >= '0' && input <= '9';

    /// <summary>
    /// 字符串内含有数字
    /// </summary>
    /// <param name="input"></param>
    /// <returns></returns>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static bool HasDigital(this string input) => input?.Any(x => x >= '0' && x <= '9') ?? false;

    /// <summary>
    /// 字符串内含有小写字母
    /// </summary>
    /// <param name="input"></param>
    /// <returns></returns>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static bool HasLowerLetter(this string input) => input?.Any(x => x >= 'a' && x <= 'z') ?? false;

    /// <summary>
    /// 字符串内含有大写字母
    /// </summary>
    /// <param name="input"></param>
    /// <returns></returns>
    public static bool HasUpperLetter(this string input) => input?.Any(x => x >= 'A' && x <= 'Z') ?? false;

    /// <summary>
    /// 字符串内含有其他字符
    /// </summary>
    /// <param name="input"></param>
    /// <returns></returns>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static bool HasOther(this string input) => input?.Any(x => !((x >= 'a' && x <= 'z') || (x >= 'A' && x <= 'Z') || (x >= '0' && x <= '9'))) ?? false;
}
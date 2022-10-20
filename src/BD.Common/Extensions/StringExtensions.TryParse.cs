// ReSharper disable once CheckNamespace
namespace System;

public static partial class StringExtensions
{
    /// <summary>
    /// 尝试将数字的字符串表示形式转换为 双精度浮点型
    /// </summary>
    /// <param name="value"></param>
    /// <returns></returns>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static decimal? TryParseDecimal(this string value) => decimal.TryParse(value, out var temp) ? temp : null;

    /// <summary>
    /// 尝试将数字的字符串表示形式转换为 双精度浮点型
    /// </summary>
    /// <param name="value"></param>
    /// <returns></returns>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static double? TryParseDouble(this string value) => double.TryParse(value, out var temp) ? temp : null;

    /// <summary>
    /// 尝试将数字的字符串表示形式转换为它的等效 16 位有符号整数
    /// </summary>
    /// <param name="value"></param>
    /// <returns></returns>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static short? TryParseInt16(this string value) => short.TryParse(value, out var temp) ? temp : null;

    /// <summary>
    /// 尝试将数字的字符串表示形式转换为它的等效 16 位无符号整数
    /// </summary>
    /// <param name="value"></param>
    /// <returns></returns>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static ushort? TryParseUInt16(this string value) => ushort.TryParse(value, out var temp) ? temp : null;

    /// <summary>
    /// 尝试将数字的字符串表示形式转换为它的等效 32 位有符号整数
    /// </summary>
    /// <param name="value"></param>
    /// <returns></returns>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static int? TryParseInt32(this string value) => int.TryParse(value, out var temp) ? temp : null;

    /// <summary>
    /// 尝试将数字的字符串表示形式转换为它的等效 32 位无符号整数
    /// </summary>
    /// <param name="value"></param>
    /// <returns></returns>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static uint? TryParseUInt32(this string value) => uint.TryParse(value, out var temp) ? temp : null;

    /// <summary>
    /// 尝试将数字的字符串表示形式转换为它的等效 64 位有符号整数
    /// </summary>
    /// <param name="value"></param>
    /// <returns></returns>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static long? TryParseInt64(this string value) => long.TryParse(value, out var temp) ? temp : null;

    /// <summary>
    /// 尝试将数字的字符串表示形式转换为它的等效 64 位无符号整数
    /// </summary>
    /// <param name="value"></param>
    /// <returns></returns>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static ulong? TryParseUInt64(this string value) => ulong.TryParse(value, out var temp) ? temp : null;

    /// <summary>
    /// 尝试将数字的字符串表示形式转换为它的等效 byte 值
    /// </summary>
    /// <param name="value"></param>
    /// <returns></returns>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static byte? TryParseByte(this string value) => byte.TryParse(value, out var temp) ? temp : null;

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static bool? TryParseBoolean(this string value) => bool.TryParse(value, out var temp) ? temp : null;

    /// <summary>
    /// 尝试将数字的字符串表示形式转换为它的等效 sbyte 值
    /// </summary>
    /// <param name="value"></param>
    /// <returns></returns>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static sbyte? TryParseSByte(this string value) => sbyte.TryParse(value, out sbyte temp) ? temp : null;

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static Version? TryParseVersion(this string value) => Version.TryParse(value, out var temp) ? temp : null;

    /// <summary>
    /// 将版本号字符串转换为 <see cref="Version"/> 对象，传入的字符串不能为 <see langword="null"/>
    /// </summary>
    /// <param name="value"></param>
    /// <returns></returns>
    public static Version? TryParseVersionTolerant(this string value)
    {
        var versionChars = GetVersion(value).ToArray();
        if (versionChars.Any() && Version.TryParse(new string(versionChars), out var versionObj))
        {
            return versionObj;
        }
        static IEnumerable<char> GetVersion(string s)
        {
            var dianCount = 0;
            var findChar = false;
            foreach (var item in s)
            {
                var isDian = item == '.';
                if (isDian)
                {
                    if (dianCount++ >= 3)
                    {
                        yield break;
                    }
                }
                if (isDian || (item >= '0' && item <= '9'))
                {
                    findChar = true;
                    yield return item;
                }
                else if (findChar)
                {
                    yield break;
                }
            }
        }
        return null;
    }

    [Obsolete("use TryParseVersionTolerant", true)]
    public static Version? VersionTryParse(string version)
    {
        return TryParseVersionTolerant(version);
    }
}
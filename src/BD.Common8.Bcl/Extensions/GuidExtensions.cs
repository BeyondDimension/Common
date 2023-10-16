namespace System.Extensions;

/// <summary>
/// 提供对 <see cref="Guid"/> 类型的扩展函数
/// </summary>
public static partial class GuidExtensions
{
    // N 32 digits:
    // 00000000000000000000000000000000
    // D 32 digits separated by hyphens:
    // 00000000-0000-0000-0000-000000000000
    // B 32 digits separated by hyphens, enclosed in braces:
    // {00000000-0000-0000-0000-000000000000}
    // P 32 digits separated by hyphens, enclosed in parentheses:
    // (00000000-0000-0000-0000-000000000000)
    // X Four hexadecimal values enclosed in braces, where the fourth value is a subset of eight hexadecimal values that is also enclosed in braces:
    // {0x00000000,0x0000,0x0000,{0x00,0x00,0x00,0x00,0x00,0x00,0x00,0x00}}

    // S Base64Url ShortGuid

    /// <summary>
    /// 返回 <see cref="Guid"/> 结构的此实例值的字符串表示形式，格式：N
    /// </summary>
    /// <param name="guid"></param>
    /// <returns></returns>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static string ToStringN(this Guid guid) => guid.ToString("N");

    /// <summary>
    /// 返回 <see cref="Guid"/> 结构的此实例值的字符串表示形式，格式：N
    /// </summary>
    /// <param name="guid"></param>
    /// <returns></returns>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static string? ToStringN(this Guid? guid) => guid.HasValue ? guid.Value.ToStringN() : null;

    /// <summary>
    /// 返回 <see cref="Guid"/> 结构的此实例值的字符串表示形式，格式：<see cref="ShortGuid"/>
    /// </summary>
    /// <param name="guid"></param>
    /// <returns></returns>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static string ToStringS(this Guid guid) => ShortGuid.Encode(guid);

    /// <summary>
    /// 返回 <see cref="Guid"/> 结构的此实例值的字符串表示形式，格式：<see cref="ShortGuid"/>
    /// </summary>
    /// <param name="guid"></param>
    /// <returns></returns>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static string? ToStringS(this Guid? guid) => guid.HasValue ? guid.Value.ToStringS() : null;

    /// <summary>
    /// 将包含 <see cref="Guid"/> 表示形式的指定只读字符范围转换为等效的 Guid 结构
    /// </summary>
    /// <param name="input"></param>
    /// <param name="result"></param>
    /// <returns></returns>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static bool TryParseGuid(this string? input, out Guid result)
        => ShortGuid.TryParse(input, out result);

    /// <summary>
    /// 将包含 <see cref="Guid"/> 表示形式的指定只读字符范围转换为等效的 Guid 结构
    /// <para>优先尝试使用格式D</para>
    /// </summary>
    /// <param name="input"></param>
    /// <returns></returns>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static Guid? TryParseGuid(this string? input)
        => input.TryParseGuid(out var result) ? result : default;
}
namespace System.Extensions;

/// <summary>
/// 提供对 <see cref="bool"/> 类型的扩展函数
/// </summary>
public static partial class BooleanExtensions
{
    /// <summary>
    /// 将 <see cref="bool"/> 转换为小写字母字符串
    /// </summary>
    /// <param name="value"></param>
    /// <returns></returns>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static string ToLowerString(this bool value) => value ? String2.TrueLowerString : String2.FalseLowerString;

    /// <summary>
    /// 将 <see cref="bool"/>? 转换为小写字母字符串
    /// </summary>
    /// <param name="value"></param>
    /// <param name="nullString"></param>
    /// <returns></returns>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    [return: NotNullIfNotNull(nameof(nullString))]
    public static string? ToLowerString(this bool? value, string? nullString = "") => value.HasValue ? value.Value.ToLowerString() : nullString;
}

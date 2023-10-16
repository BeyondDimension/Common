namespace System.Extensions;

/// <summary>
/// 提供对 <see cref="bool"/> 类型的扩展函数
/// </summary>
public static partial class BooleanExtensions
{
    /// <summary>
    /// <see langword="true"/> 的小写字母常量字符串
    /// </summary>
    public const string TrueLowerString = "true";

    /// <summary>
    /// <see langword="false"/> 的小写字母常量字符串
    /// </summary>
    public const string FalseLowerString = "false";

    /// <summary>
    /// 将 <see cref="bool"/> 转换为小写字母字符串
    /// </summary>
    /// <param name="value"></param>
    /// <returns></returns>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static string ToLowerString(this bool value) => value ? TrueLowerString : FalseLowerString;

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

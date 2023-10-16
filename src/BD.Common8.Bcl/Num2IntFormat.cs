namespace System;

/// <summary>
/// 小数数字转换为整数数字格式
/// </summary>
public enum Num2IntFormat : byte
{
    /// <summary>
    /// 四舍五入
    /// </summary>
    RoundAwayFromZero,

    /// <summary>
    /// 五舍六入
    /// </summary>
    Round,

    /// <summary>
    /// 仅取整数
    /// </summary>
    Floor,

    /// <summary>
    /// 进一法
    /// </summary>
    Ceiling,
}

/// <summary>
/// Enum 扩展 <see cref="Num2IntFormat"/>
/// </summary>
public static partial class Num2IntFormatEnumExtensions
{
#if NETCOREAPP2_0_OR_GREATER || NETSTANDARD2_1_OR_GREATER
    /// <summary>
    /// 使用 仅取整数/四舍五入/五舍六入/进一法 将 <see cref="float"/> 转换为 <see cref="int"/>，默认为 仅取整数
    /// </summary>
    /// <param name="value"></param>
    /// <param name="format"></param>
    /// <returns></returns>
    /// <exception cref="ArgumentOutOfRangeException"></exception>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static int ToInt32(this float value, Num2IntFormat format = Num2IntFormat.Floor) => format switch
    {
        Num2IntFormat.RoundAwayFromZero => (int)MathF.Round(value, MidpointRounding.AwayFromZero),
        Num2IntFormat.Round => (int)MathF.Round(value),
        Num2IntFormat.Floor => (int)MathF.Floor(value),
        Num2IntFormat.Ceiling => (int)MathF.Ceiling(value),
        _ => throw new ArgumentOutOfRangeException(nameof(format), format, null),
    };
#endif

    /// <summary>
    /// 使用 仅取整数/四舍五入/五舍六入/进一法 将 <see cref="double"/> 转换为 <see cref="int"/>，默认为 仅取整数
    /// </summary>
    /// <param name="value"></param>
    /// <param name="format"></param>
    /// <returns></returns>
    /// <exception cref="ArgumentOutOfRangeException"></exception>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static int ToInt32(this double value, Num2IntFormat format = Num2IntFormat.Floor) => format switch
    {
        Num2IntFormat.RoundAwayFromZero => (int)Math.Round(value, MidpointRounding.AwayFromZero),
        Num2IntFormat.Round => (int)Math.Round(value),
        Num2IntFormat.Floor => (int)Math.Floor(value),
        Num2IntFormat.Ceiling => (int)Math.Ceiling(value),
        _ => throw new ArgumentOutOfRangeException(nameof(format), format, null),
    };

    /// <summary>
    /// 使用 仅取整数/四舍五入/五舍六入/进一法 将 <see cref="double"/> 转换为 <see cref="int"/>，默认为 仅取整数
    /// </summary>
    /// <param name="value"></param>
    /// <param name="format"></param>
    /// <returns></returns>
    /// <exception cref="ArgumentOutOfRangeException"></exception>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static int ToInt32(this decimal value, Num2IntFormat format = Num2IntFormat.Floor) => format switch
    {
        Num2IntFormat.RoundAwayFromZero => (int)Math.Round(value, MidpointRounding.AwayFromZero),
        Num2IntFormat.Round => (int)Math.Round(value),
        Num2IntFormat.Floor => (int)Math.Floor(value),
        Num2IntFormat.Ceiling => (int)Math.Ceiling(value),
        _ => throw new ArgumentOutOfRangeException(nameof(format), format, null),
    };
}
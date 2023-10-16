#if !(NETCOREAPP2_0_OR_GREATER || NETSTANDARD2_1_OR_GREATER)
using MathF = System.Math;
#endif

namespace System.Extensions;

/// <summary>
/// 提供对 <see cref="float"/> 类型的扩展函数
/// </summary>
public static partial class SingleExtensions
{
    /// <summary>
    /// (向下取整)返回小于或等于指定数字的最大整数值
    /// </summary>
    /// <param name="f"></param>
    /// <returns></returns>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static int FloorToInt(this float f) => (int)MathF.Floor(f);

    /// <summary>
    /// (进一法)返回大于或等于指定数字的最小整数值
    /// </summary>
    /// <param name="f"></param>
    /// <returns></returns>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static int CeilingToInt(this float f) => (int)MathF.Ceiling(f);

    /// <summary>
    /// 将值舍入到最接近的整数或指定的小数位数
    /// </summary>
    /// <param name="f"></param>
    /// <param name="mode">在两个数字之间时如何舍入的规范</param>
    /// <returns></returns>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static int RoundToInt(this float f, MidpointRounding mode) => (int)MathF.Round(f, mode);

    /// <summary>
    /// (五舍六入)将值舍入到最接近的整数或指定的小数位数
    /// </summary>
    /// <param name="f"></param>
    /// <returns></returns>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static int RoundToInt(this float f) => (int)MathF.Round(f);

    /// <summary>
    /// (四舍五入)将值舍入到最接近的整数或指定的小数位数
    /// </summary>
    /// <param name="f"></param>
    /// <returns></returns>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static int Round2ToInt(this float f) => (int)MathF.Round(f, MidpointRounding.AwayFromZero);
}
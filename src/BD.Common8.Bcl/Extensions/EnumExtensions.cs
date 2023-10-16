namespace System.Extensions;

/// <summary>
/// 提供对 <see cref="Enum"/> 类型的扩展函数
/// </summary>
public static partial class EnumExtensions
{
    /// <summary>
    /// 返回一个布尔值，该值指示给定的整数值或其名称字符串是否存在于指定的枚举中
    /// </summary>
    /// <typeparam name="TEnum"></typeparam>
    /// <param name="value"></param>
    /// <returns>如果 enumType 中的某个常量的值等于 value，则为 <see langword="true"/>；否则为 <see langword="false"/></returns>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static bool IsDefined<TEnum>(this TEnum value) where TEnum : struct, Enum
#if NET5_0_OR_GREATER
        => Enum.IsDefined(value);
#else
        => Enum.IsDefined(typeof(TEnum), value);
#endif

    /// <summary>
    /// 将 <see cref="sbyte"/> 转换为泛型枚举
    /// </summary>
    /// <typeparam name="TEnum"></typeparam>
    /// <param name="value"></param>
    /// <returns></returns>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static TEnum ConvertToEnum<TEnum>(this sbyte value)
        where TEnum : Enum
        => Convert2.Convert<TEnum, sbyte>(value);

    /// <summary>
    /// 将 <see cref="byte"/> 转换为泛型枚举
    /// </summary>
    /// <typeparam name="TEnum"></typeparam>
    /// <param name="value"></param>
    /// <returns></returns>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static TEnum ConvertToEnum<TEnum>(this byte value)
        where TEnum : Enum
        => Convert2.Convert<TEnum, byte>(value);

    /// <summary>
    /// 将 <see cref="ushort"/> 转换为泛型枚举
    /// </summary>
    /// <typeparam name="TEnum"></typeparam>
    /// <param name="value"></param>
    /// <returns></returns>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static TEnum ConvertToEnum<TEnum>(this ushort value)
        where TEnum : Enum
        => Convert2.Convert<TEnum, ushort>(value);

    /// <summary>
    /// 将 <see cref="short"/> 转换为泛型枚举
    /// </summary>
    /// <typeparam name="TEnum"></typeparam>
    /// <param name="value"></param>
    /// <returns></returns>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static TEnum ConvertToEnum<TEnum>(this short value)
        where TEnum : Enum
        => Convert2.Convert<TEnum, short>(value);

    /// <summary>
    /// 将 <see cref="int"/> 转换为泛型枚举
    /// </summary>
    /// <typeparam name="TEnum"></typeparam>
    /// <param name="value"></param>
    /// <returns></returns>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static TEnum ConvertToEnum<TEnum>(this int value)
        where TEnum : Enum
        => Convert2.Convert<TEnum, int>(value);

    /// <summary>
    /// 将 <see cref="uint"/> 转换为泛型枚举
    /// </summary>
    /// <typeparam name="TEnum"></typeparam>
    /// <param name="value"></param>
    /// <returns></returns>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static TEnum ConvertToEnum<TEnum>(this uint value)
        where TEnum : Enum
        => Convert2.Convert<TEnum, uint>(value);

    /// <summary>
    /// 将 <see cref="long"/> 转换为泛型枚举
    /// </summary>
    /// <typeparam name="TEnum"></typeparam>
    /// <param name="value"></param>
    /// <returns></returns>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static TEnum ConvertToEnum<TEnum>(this long value)
        where TEnum : Enum
        => Convert2.Convert<TEnum, long>(value);

    /// <summary>
    /// 将 <see cref="ulong"/> 转换为泛型枚举
    /// </summary>
    /// <typeparam name="TEnum"></typeparam>
    /// <param name="value"></param>
    /// <returns></returns>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static TEnum ConvertToEnum<TEnum>(this ulong value)
        where TEnum : Enum
        => Convert2.Convert<TEnum, ulong>(value);

    /// <summary>
    /// 将泛型枚举转换为 <see cref="sbyte"/>
    /// </summary>
    /// <typeparam name="TEnum"></typeparam>
    /// <param name="value"></param>
    /// <returns></returns>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static sbyte ConvertToSByte<TEnum>(this TEnum value)
        where TEnum : Enum
        => Convert2.Convert<sbyte, TEnum>(value);

    /// <summary>
    /// 将泛型枚举转换为 <see cref="byte"/>
    /// </summary>
    /// <typeparam name="TEnum"></typeparam>
    /// <param name="value"></param>
    /// <returns></returns>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static byte ConvertToByte<TEnum>(this TEnum value)
        where TEnum : Enum
        => Convert2.Convert<byte, TEnum>(value);

    /// <summary>
    /// 将泛型枚举转换为 <see cref="ushort"/>
    /// </summary>
    /// <typeparam name="TEnum"></typeparam>
    /// <param name="value"></param>
    /// <returns></returns>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static ushort ConvertToUInt16<TEnum>(this TEnum value)
        where TEnum : Enum
        => Convert2.Convert<ushort, TEnum>(value);

    /// <summary>
    /// 将泛型枚举转换为 <see cref="short"/>
    /// </summary>
    /// <typeparam name="TEnum"></typeparam>
    /// <param name="value"></param>
    /// <returns></returns>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static short ConvertToInt16<TEnum>(this TEnum value)
        where TEnum : Enum
        => Convert2.Convert<short, TEnum>(value);

    /// <summary>
    /// 将泛型枚举转换为 <see cref="uint"/>
    /// </summary>
    /// <typeparam name="TEnum"></typeparam>
    /// <param name="value"></param>
    /// <returns></returns>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static uint ConvertToUInt32<TEnum>(this TEnum value)
        where TEnum : Enum
        => Convert2.Convert<uint, TEnum>(value);

    /// <summary>
    /// 将泛型枚举转换为 <see cref="int"/>
    /// </summary>
    /// <typeparam name="TEnum"></typeparam>
    /// <param name="value"></param>
    /// <returns></returns>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static int ConvertToInt32<TEnum>(this TEnum value)
        where TEnum : Enum
        => Convert2.Convert<int, TEnum>(value);

    /// <summary>
    /// 将泛型枚举转换为 <see cref="ulong"/>
    /// </summary>
    /// <typeparam name="TEnum"></typeparam>
    /// <param name="value"></param>
    /// <returns></returns>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static ulong ConvertToUInt64<TEnum>(this TEnum value)
        where TEnum : Enum
        => Convert2.Convert<ulong, TEnum>(value);

    /// <summary>
    /// 将泛型枚举转换为 <see cref="long"/>
    /// </summary>
    /// <typeparam name="TEnum"></typeparam>
    /// <param name="value"></param>
    /// <returns></returns>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static long ConvertToInt64<TEnum>(this TEnum value)
        where TEnum : Enum
        => Convert2.Convert<long, TEnum>(value);
}
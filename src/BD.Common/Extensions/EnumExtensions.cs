// ReSharper disable once CheckNamespace
namespace System;

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

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static TEnum ConvertToEnum<TEnum>(this sbyte value)
        where TEnum : Enum
        => ConvertibleHelper.Convert<TEnum, sbyte>(value);

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static TEnum ConvertToEnum<TEnum>(this byte value)
        where TEnum : Enum
        => ConvertibleHelper.Convert<TEnum, byte>(value);

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static TEnum ConvertToEnum<TEnum>(this ushort value)
        where TEnum : Enum
        => ConvertibleHelper.Convert<TEnum, ushort>(value);

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static TEnum ConvertToEnum<TEnum>(this short value)
        where TEnum : Enum
        => ConvertibleHelper.Convert<TEnum, short>(value);

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static TEnum ConvertToEnum<TEnum>(this int value)
        where TEnum : Enum
        => ConvertibleHelper.Convert<TEnum, int>(value);

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static TEnum ConvertToEnum<TEnum>(this uint value)
        where TEnum : Enum
        => ConvertibleHelper.Convert<TEnum, uint>(value);

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static TEnum ConvertToEnum<TEnum>(this long value)
        where TEnum : Enum
        => ConvertibleHelper.Convert<TEnum, long>(value);

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static TEnum ConvertToEnum<TEnum>(this ulong value)
        where TEnum : Enum
        => ConvertibleHelper.Convert<TEnum, ulong>(value);

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static sbyte ConvertToSByte<TEnum>(this TEnum value)
        where TEnum : Enum
        => ConvertibleHelper.Convert<sbyte, TEnum>(value);

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static byte ConvertToByte<TEnum>(this TEnum value)
        where TEnum : Enum
        => ConvertibleHelper.Convert<byte, TEnum>(value);

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static ushort ConvertToUInt16<TEnum>(this TEnum value)
        where TEnum : Enum
        => ConvertibleHelper.Convert<ushort, TEnum>(value);

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static short ConvertToInt16<TEnum>(this TEnum value)
        where TEnum : Enum
        => ConvertibleHelper.Convert<short, TEnum>(value);

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static uint ConvertToUInt32<TEnum>(this TEnum value)
        where TEnum : Enum
        => ConvertibleHelper.Convert<uint, TEnum>(value);

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static int ConvertToInt32<TEnum>(this TEnum value)
        where TEnum : Enum
        => ConvertibleHelper.Convert<int, TEnum>(value);

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static ulong ConvertToUInt64<TEnum>(this TEnum value)
        where TEnum : Enum
        => ConvertibleHelper.Convert<ulong, TEnum>(value);

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static long ConvertToInt64<TEnum>(this TEnum value)
        where TEnum : Enum
        => ConvertibleHelper.Convert<long, TEnum>(value);
}
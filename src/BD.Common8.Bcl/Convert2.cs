namespace System;

/// <inheritdoc cref="System.Convert"/>
public static partial class Convert2
{
    /// <summary>
    /// Converts the specified string, which encodes binary data as hex characters, to an equivalent 8-bit unsigned integer array.
    /// </summary>
    /// <param name="s">The string to convert.</param>
    /// <returns>An array of 8-bit unsigned integers that is equivalent to s.</returns>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static byte[] FromHexString(string s)
#if HEXMATE
        => HexMate.Convert.FromHexString(s);
#elif NET5_0_OR_GREATER
        => System.Convert.FromHexString(s);
#else
    {
        var bytes = new byte[s.Length / 2];
        for (var i = 0; i < bytes.Length; i++)
        {
            bytes[i] = System.Convert.ToByte(s.Substring(i * 2, 2), 16);
        }
        return bytes;
    }
#endif

    /// <inheritdoc cref="System.Convert"/>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static TOut Convert<TOut, TIn>(TIn value)
    {
        var parameter = Expression.Parameter(typeof(TIn), null);
        var dynamicMethod = Expression.Lambda<Func<TIn, TOut>>(
            Expression.Convert(parameter, typeof(TOut)),
            parameter);
        return dynamicMethod.Compile()(value);
    }

    /// <summary>
    /// 自定义 Object 转换
    /// </summary>
    public static Func<IConvertible?, Type, object?>? CustomConvertObject { private get; set; }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    [RequiresUnreferencedCode(Serializable.SerializationUnreferencedCodeMessage)]
    [RequiresDynamicCode(Serializable.SerializationRequiresDynamicCodeMessage)]
    static T? ConvertObject<T>(IConvertible value)
    {
        var custom = CustomConvertObject;
        if (custom != null)
            return custom(value, typeof(T)) is T t ? t : default;
        var json = value.ToString(CultureInfo.InvariantCulture);
        return Serializable.DJSON<T>(json);
    }

    /// <inheritdoc cref="System.Convert"/>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    [RequiresUnreferencedCode(Serializable.SerializationUnreferencedCodeMessage)]
    [RequiresDynamicCode(Serializable.SerializationRequiresDynamicCodeMessage)]
    public static T? Convert<T>(IConvertible? value) where T : notnull
    {
        if (value == default)
            return default;
        var typeCode = Type.GetTypeCode(typeof(T));
        return typeCode switch
        {
            TypeCode.Boolean => Convert<T, bool>(value.ToBoolean(CultureInfo.InvariantCulture)),
            TypeCode.Byte => Convert<T, byte>(value.ToByte(CultureInfo.InvariantCulture)),
            TypeCode.Char => Convert<T, char>(value.ToChar(CultureInfo.InvariantCulture)),
            TypeCode.DateTime => Convert<T, DateTime>(value.ToDateTime(CultureInfo.InvariantCulture)),
            TypeCode.Decimal => Convert<T, decimal>(value.ToDecimal(CultureInfo.InvariantCulture)),
            TypeCode.Double => Convert<T, double>(value.ToDouble(CultureInfo.InvariantCulture)),
            TypeCode.Int16 => Convert<T, short>(value.ToInt16(CultureInfo.InvariantCulture)),
            TypeCode.Int32 => Convert<T, int>(value.ToInt32(CultureInfo.InvariantCulture)),
            TypeCode.Int64 => Convert<T, long>(value.ToInt64(CultureInfo.InvariantCulture)),
            TypeCode.SByte => Convert<T, sbyte>(value.ToSByte(CultureInfo.InvariantCulture)),
            TypeCode.Single => Convert<T, float>(value.ToSingle(CultureInfo.InvariantCulture)),
            TypeCode.UInt16 => Convert<T, ushort>(value.ToUInt16(CultureInfo.InvariantCulture)),
            TypeCode.UInt32 => Convert<T, uint>(value.ToUInt32(CultureInfo.InvariantCulture)),
            TypeCode.UInt64 => Convert<T, ulong>(value.ToUInt64(CultureInfo.InvariantCulture)),
            TypeCode.String => Convert<T, string>(value.ToString(CultureInfo.InvariantCulture)),
            TypeCode.Object => ConvertObject<T>(value),
            _ => default,
        };
    }
}

#if DEBUG
[Obsolete("use Convert2", true)]
public static partial class ConvertibleHelper
{
    /// <inheritdoc cref="Convert2.Convert{TOut, TIn}(TIn)"/>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static TOut Convert<TOut, TIn>(TIn value) => Convert2.Convert<TOut, TIn>(value);

    /// <inheritdoc cref="Convert2.Convert{T}(IConvertible?)"/>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    [RequiresUnreferencedCode(Serializable.SerializationUnreferencedCodeMessage)]
    [RequiresDynamicCode(Serializable.SerializationRequiresDynamicCodeMessage)]
    public static T? Convert<T>(IConvertible? value) where T : notnull => Convert2.Convert<T>(value);
}
#endif
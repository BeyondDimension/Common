using Expression = System.Linq.Expressions.Expression;

namespace System;

/// <inheritdoc cref="System.Convert"/>
public static partial class Convert2
{
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
    public static T? Convert<T>(IConvertible? value) where T : notnull
    {
        if (value == default)
            return default;
        var typeCode = Type.GetTypeCode(typeof(T));
#pragma warning disable IL3050 // Calling members annotated with 'RequiresDynamicCodeAttribute' may break functionality when AOT compiling.
#pragma warning disable IL2026 // Members annotated with 'RequiresUnreferencedCodeAttribute' require dynamic access otherwise can break functionality when trimming application code
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
#pragma warning restore IL2026 // Members annotated with 'RequiresUnreferencedCodeAttribute' require dynamic access otherwise can break functionality when trimming application code
#pragma warning restore IL3050 // Calling members annotated with 'RequiresDynamicCodeAttribute' may break functionality when AOT compiling.
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
#pragma warning disable IDE0079 // 请删除不必要的忽略
#pragma warning disable IDE1006 // 命名样式
#pragma warning disable CS8981 // 该类型名称仅包含小写 ascii 字符。此类名称可能会成为该语言的保留值。
#pragma warning disable SA1313 // Parameter names should begin with lower-case letter
#pragma warning disable SA1600 // Elements should be documented

namespace System;

/// <summary>
/// This class represents the void return type
/// </summary>
public readonly struct nil : IConvertible
{
    /// <summary>
    /// typeof(nil).FullName
    /// </summary>
    public const string FullName = "System.nil";

    // https://github.com/dotnet/runtime/blob/v7.0.11/src/libraries/System.Private.CoreLib/src/System/Void.cs

    readonly TypeCode IConvertible.GetTypeCode() => TypeCode.Empty;

    readonly bool IConvertible.ToBoolean(IFormatProvider? provider) => default;

    public static implicit operator bool(nil _) => default;

    readonly byte IConvertible.ToByte(IFormatProvider? provider) => default;

    public static implicit operator byte(nil _) => default;

    readonly char IConvertible.ToChar(IFormatProvider? provider) => default;

    public static implicit operator char(nil _) => default;

    readonly DateTime IConvertible.ToDateTime(IFormatProvider? provider) => default;

    public static implicit operator DateTime(nil _) => default;

    readonly decimal IConvertible.ToDecimal(IFormatProvider? provider) => default;

    public static implicit operator decimal(nil _) => default;

    readonly double IConvertible.ToDouble(IFormatProvider? provider) => default;

    public static implicit operator double(nil _) => default;

    readonly short IConvertible.ToInt16(IFormatProvider? provider) => default;

    public static implicit operator short(nil _) => default;

    readonly int IConvertible.ToInt32(IFormatProvider? provider) => default;

    public static implicit operator int(nil _) => default;

    readonly long IConvertible.ToInt64(IFormatProvider? provider) => default;

    public static implicit operator long(nil _) => default;

    readonly sbyte IConvertible.ToSByte(IFormatProvider? provider) => default;

    public static implicit operator sbyte(nil _) => default;

    readonly float IConvertible.ToSingle(IFormatProvider? provider) => default;

    public static implicit operator float(nil _) => default;

    readonly string IConvertible.ToString(IFormatProvider? provider) => default!;

    public static implicit operator string?(nil _) => default;

    readonly object IConvertible.ToType(Type conversionType, IFormatProvider? provider) => default!;

    readonly ushort IConvertible.ToUInt16(IFormatProvider? provider) => default;

    public static implicit operator ushort(nil _) => default;

    readonly uint IConvertible.ToUInt32(IFormatProvider? provider) => default;

    public static implicit operator uint(nil _) => default;

    readonly ulong IConvertible.ToUInt64(IFormatProvider? provider) => default;

    public static implicit operator ulong(nil _) => default;

    public override string ToString() => nameof(nil);
}
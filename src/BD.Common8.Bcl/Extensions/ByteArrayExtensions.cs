namespace System.Extensions;

/// <summary>
/// 提供对 byte[]/ReadOnlySpan&lt;byte&gt; 类型的扩展函数
/// </summary>
public static partial class ByteArrayExtensions
{
    /// <summary>
    /// 将 byte[] 转换为 sbyte[]
    /// </summary>
    /// <param name="buffer"></param>
    /// <returns></returns>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static sbyte[] ToSByteArray(this byte[] buffer)
    {
        ReadOnlySpan<byte> buffer_ = buffer;
        return buffer_.ToSByteArray();
    }

    /// <summary>
    /// 将 ReadOnlySpan&lt;sbyte&gt; 转换为 sbyte[]
    /// </summary>
    /// <param name="buffer"></param>
    /// <returns></returns>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static sbyte[] ToSByteArray(this ReadOnlySpan<byte> buffer)
        => MemoryMarshal.Cast<byte, sbyte>(buffer).ToArray();

    /// <summary>
    /// 将 sbyte[] 转换为 byte[]
    /// </summary>
    /// <param name="buffer"></param>
    /// <returns></returns>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static byte[] ToByteArray(this sbyte[] buffer)
    {
        ReadOnlySpan<sbyte> buffer_ = buffer;
        return buffer_.ToByteArray();
    }

    /// <summary>
    /// 将 ReadOnlySpan&lt;sbyte&gt; 转换为 byte[]
    /// </summary>
    /// <param name="buffer"></param>
    /// <returns></returns>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static byte[] ToByteArray(this ReadOnlySpan<sbyte> buffer)
        => MemoryMarshal.Cast<sbyte, byte>(buffer).ToArray();

    /// <summary>
    /// 将 byte[] 转换为 HexString
    /// </summary>
    /// <param name="inArray"></param>
    /// <param name="isLower"></param>
    /// <returns></returns>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static string ToHexString(this byte[] inArray, bool isLower = false)
    {
#if HEXMATE
        return HexMate.Convert.ToHexString(inArray, isLower ? HexMate.HexFormattingOptions.Lowercase : HexMate.HexFormattingOptions.None);
#elif NET5_0_OR_GREATER
        return isLower ? Convert.ToHexString(inArray).ToLowerInvariant() : Convert.ToHexString(inArray);
#else
        StringBuilder b = new();
        foreach (var item in inArray)
        {
            b.Append(item.ToString(isLower ? "x2" : "X2"));
        }
        return b.ToString();
#endif
    }

    /// <summary>
    /// 将 byte[] 转换为 HexString
    /// </summary>
    /// <param name="bytes"></param>
    /// <param name="isLower"></param>
    /// <returns></returns>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static string ToHexString(this ReadOnlySpan<byte> bytes, bool isLower = false)
#if NET5_0_OR_GREATER
        => isLower ? Convert.ToHexString(bytes).ToLowerInvariant() : Convert.ToHexString(bytes);
#else
    {
        StringBuilder b = new();
        foreach (var item in bytes)
        {
            b.Append(item.ToString(isLower ? "x2" : "X2"));
        }
        return b.ToString();
    }
#endif

    /// <summary>
    /// 将 byte[] 转换为 HexString
    /// </summary>
    /// <param name="inArray"></param>
    /// <param name="offset"></param>
    /// <param name="length"></param>
    /// <param name="isLower"></param>
    /// <returns></returns>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static string ToHexString(this byte[] inArray, int offset, int length, bool isLower = false)
#if NET5_0_OR_GREATER
        => isLower ? Convert.ToHexString(inArray, offset, length).ToLowerInvariant() : Convert.ToHexString(inArray, offset, length);
#else
    {
        StringBuilder b = new();
        for (int i = offset; i < length; i++)
        {
            b.Append(inArray[i].ToString(isLower ? "x2" : "X2"));
        }
        return b.ToString();
    }
#endif
}
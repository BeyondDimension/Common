namespace System.Text;

/// <summary>
/// Base64 编码扩展
/// </summary>
public static partial class Base64Extensions
{
    /// <summary>
    /// Base64编码(ByteArray → String)
    /// </summary>
    /// <param name="inArray"></param>
    /// <param name="options"></param>
    /// <returns></returns>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static string Base64Encode(this byte[] inArray, Base64FormattingOptions options = Base64FormattingOptions.None)
    {
#if NET7_0_OR_GREATER
        ReadOnlySpan<byte> inArray_ = inArray;
        return Base64Encode(inArray_, options);
#else
        return Convert.ToBase64String(inArray, options);
#endif
    }

    /// <summary>
    /// Base64编码(ByteArray → String)
    /// </summary>
    /// <param name="inArray"></param>
    /// <param name="options"></param>
    /// <returns></returns>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static string? Base64Encode_Nullable(this byte[]? inArray, Base64FormattingOptions options = Base64FormattingOptions.None)
    {
        if (inArray == default) return default;
        return Base64Encode(inArray, options);
    }

    /// <summary>
    /// Base64编码(String → String)
    /// </summary>
    /// <param name="s"></param>
    /// <param name="options"></param>
    /// <returns></returns>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static string Base64Encode(this string s, Base64FormattingOptions options = Base64FormattingOptions.None)
    {
        var inArray = Encoding.UTF8.GetBytes(s);
        return Base64Encode(inArray, options);
    }

    /// <summary>
    /// Base64编码(String → String)
    /// </summary>
    /// <param name="s"></param>
    /// <param name="options"></param>
    /// <returns></returns>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static string? Base64Encode_Nullable(this string? s, Base64FormattingOptions options = Base64FormattingOptions.None)
    {
        if (s == default) return default;
        return Base64Encode(s, options);
    }

    /// <summary>
    /// Base64解码(String → ByteArray)
    /// </summary>
    /// <param name="s"></param>
    /// <param name="options"></param>
    /// <returns></returns>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static byte[]? Base64DecodeToByteArray_Nullable(this string? s, Base64FormattingOptions options = Base64FormattingOptions.None)
    {
        if (s == default) return default;
#if NET7_0_OR_GREATER
        return Base64DecodeToByteArray(s, options);
#else
        return Convert.FromBase64String(s);
#endif
    }

    /// <summary>
    /// Base64解码(String → String)
    /// </summary>
    /// <param name="s"></param>
    /// <param name="options"></param>
    /// <returns></returns>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static string Base64Decode(this string s, Base64FormattingOptions options = Base64FormattingOptions.None)
    {
#if NET7_0_OR_GREATER
        var bytes = Base64DecodeToByteArray(s, options);
#else
        var bytes = Convert.FromBase64String(s);
#endif
        return Encoding.UTF8.GetString(bytes);
    }

    /// <summary>
    /// Base64解码(String → String)
    /// </summary>
    /// <param name="s"></param>
    /// <param name="options"></param>
    /// <returns></returns>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static string? Base64Decode_Nullable(this string? s, Base64FormattingOptions options = Base64FormattingOptions.None)
    {
        if (s == default) return default;
        return Base64Decode(s, options);
    }
}
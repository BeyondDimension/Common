namespace System.Text;

/// <summary>
/// Base64Url 扩展
/// </summary>
public static partial class Base64UrlExtensions
{
    /// <summary>
    /// Base64Url编码(ByteArray → String)
    /// </summary>
    /// <param name="inArray"></param>
    /// <returns></returns>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static string Base64UrlEncode(this byte[] inArray)
    {
#if NET7_0_OR_GREATER
        ReadOnlySpan<byte> inArray_ = inArray;
        return Base64UrlEncode(inArray_);
#else
        var buffer = new char[GetArraySizeRequiredToEncode(inArray.Length)];
        var numBase64Chars = Base64UrlEncode(inArray, 0, buffer, outputOffset: 0, count: inArray.Length);
        return new string(buffer, startIndex: 0, length: numBase64Chars);
#endif
    }

#if !NET7_0_OR_GREATER
    /// <summary>
    /// Get the minimum output <c>char[]</c> size required for encoding <paramref name="count"/>
    /// <see cref="byte"/>s with the <see cref="Base64UrlEncode(byte[], int, char[], int, int)"/> method.
    /// </summary>
    /// <param name="count">The number of characters to encode.</param>
    /// <returns>
    /// The minimum output <c>char[]</c> size required for encoding <paramref name="count"/> <see cref="byte"/>s.
    /// </returns>
    static int GetArraySizeRequiredToEncode(int count)
    {
        var numWholeOrPartialInputBlocks = checked(count + 2) / 3;
        return checked(numWholeOrPartialInputBlocks * 4);
    }

    /// <summary>
    /// Encodes <paramref name="input"/> using base64url encoding.
    /// </summary>
    /// <param name="input">The binary input to encode.</param>
    /// <param name="offset">The offset into <paramref name="input"/> at which to begin encoding.</param>
    /// <param name="output">
    /// Buffer to receive the base64url-encoded form of <paramref name="input"/>. Array must be large enough to
    /// hold <paramref name="outputOffset"/> characters and the full base64-encoded form of
    /// <paramref name="input"/>, including padding characters.
    /// </param>
    /// <param name="outputOffset">
    /// The offset into <paramref name="output"/> at which to begin writing the base64url-encoded form of
    /// <paramref name="input"/>.
    /// </param>
    /// <param name="count">The number of <c>byte</c>s from <paramref name="input"/> to encode.</param>
    /// <returns>
    /// The number of characters written to <paramref name="output"/>, less any padding characters.
    /// </returns>
    static int Base64UrlEncode(byte[] input, int offset, char[] output, int outputOffset, int count)
    {
        var arraySizeRequired = GetArraySizeRequiredToEncode(count);
        if (output.Length - outputOffset < arraySizeRequired)
        {
            throw new ArgumentException(
                string.Format(
                    CultureInfo.CurrentCulture,
                    WebEncoders_InvalidCountOffsetOrLength,
                    nameof(count),
                    nameof(outputOffset),
                    nameof(output)),
                nameof(count));
        }

        // Special-case empty input.
        if (count == 0)
        {
            return 0;
        }

        // Use base64url encoding with no padding characters. See RFC 4648, Sec. 5.

        // Start with default Base64 encoding.
        var numBase64Chars = Convert.ToBase64CharArray(input, offset, count, output, outputOffset);

        // Fix up '+' -> '-' and '/' -> '_'. Drop padding characters.
        for (var i = outputOffset; i - outputOffset < numBase64Chars; i++)
        {
            var ch = output[i];
            if (ch == '+')
            {
                output[i] = '-';
            }
            else if (ch == '/')
            {
                output[i] = '_';
            }
            else if (ch == '=')
            {
                // We've reached a padding character; truncate the remainder.
                return i - outputOffset;
            }
        }

        return numBase64Chars;
    }
#endif

    /// <summary>
    /// Base64Url编码(ByteArray → String)
    /// </summary>
    /// <param name="inArray"></param>
    /// <returns></returns>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static string? Base64UrlEncode_Nullable(this byte[]? inArray)
    {
        if (inArray == default) return default;
        return Base64UrlEncode(inArray);
    }

    /// <summary>
    /// Base64Url编码(String → String)
    /// </summary>
    /// <param name="s"></param>
    /// <returns></returns>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static string Base64UrlEncode(this string s)
    {
        var inArray = Encoding.UTF8.GetBytes(s);
        return Base64UrlEncode(inArray);
    }

    /// <summary>
    /// Base64Url编码(String → String)
    /// </summary>
    /// <param name="s"></param>
    /// <returns></returns>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static string? Base64UrlEncode_Nullable(this string? s)
    {
        if (s == default) return default;
        return Base64UrlEncode(s);
    }

    /// <summary>
    /// Base64Url解码(String → ByteArray)
    /// </summary>
    /// <param name="s"></param>
    /// <returns></returns>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static byte[] Base64UrlDecodeToByteArray(this string s)
    {
        var encoded = s.AsSpan();
        return Base64UrlDecodeToByteArray(encoded);
    }

    /// <summary>
    /// Base64Url解码(String → ByteArray)
    /// </summary>
    /// <param name="s"></param>
    /// <returns></returns>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static byte[]? Base64UrlDecodeToByteArray_Nullable(this string? s)
    {
        if (s == default) return default;
        return Base64UrlDecodeToByteArray(s);
    }

    /// <summary>
    /// Base64Url解码(String → String)
    /// </summary>
    /// <param name="s"></param>
    /// <returns></returns>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static string Base64UrlDecode(this string s)
    {
        var bytes = Base64UrlDecodeToByteArray(s);
        return Encoding.UTF8.GetString(bytes);
    }

    /// <summary>
    /// Base64Url解码(String → String)
    /// </summary>
    /// <param name="s"></param>
    /// <returns></returns>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static string? Base64UrlDecode_Nullable(this string? s)
    {
        if (s == default) return default;
        return Base64UrlDecode(s);
    }
}
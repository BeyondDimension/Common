#if NET7_0_OR_GREATER
using Base64 = gfoidl.Base64.Base64;
#endif

// 使用 https://github.com/gfoidl/Base64 实现

// ReSharper disable once CheckNamespace
namespace System.Text;

partial class Base64Extensions
{
    /// <summary>
    /// Base64编码(ReadOnlySpan&lt;Byte&gt; → String)
    /// </summary>
    /// <param name="inArray"></param>
    /// <param name="options"></param>
    /// <returns></returns>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static string Base64Encode(this ReadOnlySpan<byte> inArray, Base64FormattingOptions options = Base64FormattingOptions.None)
#if NET7_0_OR_GREATER
        => options switch
        {
            Base64FormattingOptions.InsertLineBreaks => Convert.ToBase64String(inArray, Base64FormattingOptions.InsertLineBreaks),
            Base64FormattingOptions.None => Base64.Default.Encode(inArray),
            _ => throw new ArgumentOutOfRangeException(nameof(options)),
        };
#else
    {
        return Convert.ToBase64String(inArray.ToArray(), options);
    }
#endif

    /// <summary>
    /// Base64解码(ReadOnlySpan&lt;Char&gt; → ByteArray)
    /// </summary>
    /// <param name="encoded"></param>
    /// <returns></returns>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static byte[] Base64DecodeToByteArray(this ReadOnlySpan<char> encoded)
#if NET7_0_OR_GREATER
        => Base64.Default.Decode(encoded);
#else
    {
        var inArray = encoded.ToArray();
        return Convert.FromBase64CharArray(inArray, 0, inArray.Length);
    }
#endif

    /// <summary>
    /// Base64解码(String → ByteArray)
    /// </summary>
    /// <param name="s"></param>
    /// <param name="options"></param>
    /// <returns></returns>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static byte[] Base64DecodeToByteArray(this string s, Base64FormattingOptions options = Base64FormattingOptions.None)
    {
#if NET7_0_OR_GREATER
        switch (options)
        {
            case Base64FormattingOptions.InsertLineBreaks:
                return Convert.FromBase64String(s);
            case Base64FormattingOptions.None:
                var encoded = s.AsSpan();
                return Base64DecodeToByteArray(encoded);
            default:
                throw new ArgumentOutOfRangeException(nameof(options));
        }
#else
        return Convert.FromBase64String(s);
#endif
    }
}

partial class Base64UrlExtensions
{
    /// <summary>
    /// Base64Url编码(ReadOnlySpan&lt;Byte&gt; → String)
    /// </summary>
    /// <param name="inArray"></param>
    /// <returns></returns>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static string Base64UrlEncode(this ReadOnlySpan<byte> inArray)
#if NET7_0_OR_GREATER
        => Base64.Url.Encode(inArray);
#else
        => Base64UrlEncode(inArray.ToArray());
#endif

    /// <summary>
    /// Base64Url解码(ReadOnlySpan&lt;Char&gt; → ByteArray)
    /// </summary>
    /// <param name="encoded"></param>
    /// <returns></returns>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static byte[] Base64UrlDecodeToByteArray(this ReadOnlySpan<char> encoded)
#if NET7_0_OR_GREATER
        => Base64.Url.Decode(encoded);
#else
    { // Create array large enough for the Base64 characters, not just shorter Base64-URL-encoded form.
        var buffer = new char[GetArraySizeRequiredToDecode(encoded.Length)];
        return Base64UrlDecode(encoded, 0, buffer, bufferOffset: 0, count: encoded.Length);
    }

    /// <summary>
    /// Gets the minimum <c>char[]</c> size required for decoding of <paramref name="count"/> characters
    /// with the Base64UrlDecode(string, int, char[], int, int) method.
    /// </summary>
    /// <param name="count">The number of characters to decode.</param>
    /// <returns>
    /// The minimum <c>char[]</c> size required for decoding  of <paramref name="count"/> characters.
    /// </returns>
    static int GetArraySizeRequiredToDecode(int count)
    {
        if (count == 0)
        {
            return 0;
        }

        var numPaddingCharsToAdd = GetNumBase64PaddingCharsToAddForDecode(count);

        return checked(count + numPaddingCharsToAdd);
    }

    /// <summary>
    /// Invalid {0}, {1} or {2} length.
    /// </summary>
    internal const string WebEncoders_InvalidCountOffsetOrLength = "Invalid {0}, {1} or {2} length.";

    /// <summary>
    /// Malformed input: {0} is an invalid input length.
    /// </summary>
    internal const string WebEncoders_MalformedInput = "Malformed input: {0} is an invalid input length.";

    static int GetNumBase64PaddingCharsToAddForDecode(int inputLength)
    {
        return (inputLength % 4) switch
        {
            0 => 0,
            2 => 2,
            3 => 1,
            _ => throw new FormatException(
                                    string.Format(
                                        CultureInfo.CurrentCulture,
                                        WebEncoders_MalformedInput,
                                        inputLength)),
        };
    }

    /// <summary>
    /// Decodes a base64url-encoded <paramref name="input"/> into a <c>byte[]</c>.
    /// </summary>
    /// <param name="input">A string containing the base64url-encoded input to decode.</param>
    /// <param name="offset">The position in <paramref name="input"/> at which decoding should begin.</param>
    /// <param name="buffer">
    /// Scratch buffer to hold the <see cref="char"/>s to decode. Array must be large enough to hold
    /// <paramref name="bufferOffset"/> and <paramref name="count"/> characters as well as Base64 padding
    /// characters. Content is not preserved.
    /// </param>
    /// <param name="bufferOffset">
    /// The offset into <paramref name="buffer"/> at which to begin writing the <see cref="char"/>s to decode.
    /// </param>
    /// <param name="count">The number of characters in <paramref name="input"/> to decode.</param>
    /// <returns>The base64url-decoded form of the <paramref name="input"/>.</returns>
    /// <remarks>
    /// The input must not contain any whitespace or padding characters.
    /// Throws <see cref="FormatException"/> if the input is malformed.
    /// </remarks>
    public static byte[] Base64UrlDecode(ReadOnlySpan<char> input, int offset, char[] buffer, int bufferOffset, int count)
    {
        if (count == 0)
        {
            return Array.Empty<byte>();
        }

        // Assumption: input is base64url encoded without padding and contains no whitespace.
        var paddingCharsToAdd = GetNumBase64PaddingCharsToAddForDecode(count);
        var arraySizeRequired = checked(count + paddingCharsToAdd);
        Debug.Assert(arraySizeRequired % 4 == 0, "Invariant: Array length must be a multiple of 4.");

        if (buffer.Length - bufferOffset < arraySizeRequired)
        {
            throw new ArgumentException(
                string.Format(
                    CultureInfo.CurrentCulture,
                    WebEncoders_InvalidCountOffsetOrLength,
                    nameof(count),
                    nameof(bufferOffset),
                    nameof(input)),
                nameof(count));
        }

        // Copy input into buffer, fixing up '-' -> '+' and '_' -> '/'.
        var i = bufferOffset;
        for (var j = offset; i - bufferOffset < count; i++, j++)
        {
            var ch = input[j];
            if (ch == '-')
            {
                buffer[i] = '+';
            }
            else if (ch == '_')
            {
                buffer[i] = '/';
            }
            else
            {
                buffer[i] = ch;
            }
        }

        // Add the padding characters back.
        for (; paddingCharsToAdd > 0; i++, paddingCharsToAdd--)
        {
            buffer[i] = '=';
        }

        // Decode.
        // If the caller provided invalid base64 chars, they'll be caught here.
        return Convert.FromBase64CharArray(buffer, bufferOffset, arraySizeRequired);
    }
#endif
}
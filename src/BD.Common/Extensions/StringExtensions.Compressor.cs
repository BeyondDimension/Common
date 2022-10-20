// ReSharper disable once CheckNamespace
namespace System;

public static partial class StringExtensions
{
    /// <summary>
    /// Compresses the string.
    /// </summary>
    /// <param name="text">The text.</param>
    /// <returns></returns>
    [return: NotNullIfNotNull("text")]
    public static string? CompressString(this string? text)
    {
        if (string.IsNullOrEmpty(text)) return null;
        byte[] buffer = Encoding.UTF8.GetBytes(text);
        var gZipBuffer = buffer.CompressByteArray();
        return Convert.ToBase64String(gZipBuffer);
    }

    /// <inheritdoc cref="CompressString(string?)"/>
    [return: NotNullIfNotNull("text")]
    public static string? CompressStringByBrotli(this string? text)
    {
        if (string.IsNullOrEmpty(text)) return null;
        byte[] buffer = Encoding.UTF8.GetBytes(text);
        var gZipBuffer = buffer.CompressByteArrayByBrotli();
        return Convert.ToBase64String(gZipBuffer);
    }

    /// <summary>
    /// Decompresses the string.
    /// </summary>
    /// <param name="compressedText">The compressed text.</param>
    /// <returns></returns>
    [return: NotNullIfNotNull("compressedText")]
    public static string? DecompressString(this string? compressedText)
    {
        if (string.IsNullOrEmpty(compressedText)) return null;
        byte[] gZipBuffer = Convert.FromBase64String(compressedText);
        var buffer = gZipBuffer.DecompressByteArray();
        return Encoding.UTF8.GetString(buffer);
    }

    /// <inheritdoc cref="DecompressString(string?)"/>
    [return: NotNullIfNotNull("compressedText")]
    public static string? DecompressStringByBrotli(this string? compressedText)
    {
        if (string.IsNullOrEmpty(compressedText)) return null;
        byte[] gZipBuffer = Convert.FromBase64String(compressedText);
        var buffer = gZipBuffer.DecompressByteArrayByBrotli();
        return Encoding.UTF8.GetString(buffer);
    }
}
using System.Runtime.InteropServices;

// ReSharper disable once CheckNamespace
namespace System;

public static class ByteArrayExtensions
{
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static sbyte[] ToSByteArray(this byte[] buffer)
    {
        ReadOnlySpan<byte> buffer_ = buffer;
        return buffer_.ToSByteArray();
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static sbyte[] ToSByteArray(this ReadOnlySpan<byte> buffer)
        => MemoryMarshal.Cast<byte, sbyte>(buffer).ToArray();

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static byte[] ToByteArray(this sbyte[] buffer)
    {
        ReadOnlySpan<sbyte> buffer_ = buffer;
        return buffer_.ToByteArray();
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static byte[] ToByteArray(this ReadOnlySpan<sbyte> buffer)
        => MemoryMarshal.Cast<sbyte, byte>(buffer).ToArray();

    [return: NotNullIfNotNull("buffer")]
    static byte[]? CompressByteArray(this byte[]? buffer, Func<Stream, Stream> func)
    {
        if (buffer == null) return null;

        using var memoryStream = new MemoryStream();
        using (var stream = func(memoryStream))
        {
            stream.Write(buffer, 0, buffer.Length);
        }

        memoryStream.Position = 0;

        var compressedData = new byte[memoryStream.Length];
        memoryStream.Read(compressedData, 0, compressedData.Length);

        var gZipBuffer = new byte[compressedData.Length + 4];
        Buffer.BlockCopy(compressedData, 0, gZipBuffer, 4, compressedData.Length);
        Buffer.BlockCopy(BitConverter.GetBytes(buffer.Length), 0, gZipBuffer, 0, 4);

        return gZipBuffer;
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    [return: NotNullIfNotNull("buffer")]
    public static byte[]? CompressByteArray(this byte[]? buffer) => buffer.CompressByteArray(s => new GZipStream(s, CompressionMode.Compress, true));

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    [return: NotNullIfNotNull("buffer")]
    public static byte[]? CompressByteArrayByBrotli(this byte[]? buffer) => buffer.CompressByteArray(s => new BrotliStream(s, CompressionMode.Compress, true));

    [return: NotNullIfNotNull("value")]
    static byte[]? DecompressByteArray(this byte[]? value, Func<Stream, Stream> func)
    {
        if (value == null) return null;

        using var memoryStream = new MemoryStream();
        int dataLength = BitConverter.ToInt32(value, 0);
        memoryStream.Write(value, 4, value.Length - 4);

        var buffer = new byte[dataLength];

        memoryStream.Position = 0;
        using (var gZipStream = func(memoryStream))
        {
            gZipStream.Read(buffer, 0, buffer.Length);
        }

        return buffer;
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    [return: NotNullIfNotNull("value")]
    public static byte[]? DecompressByteArray(this byte[]? value) => value.DecompressByteArray(s => new GZipStream(s, CompressionMode.Decompress));

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    [return: NotNullIfNotNull("value")]
    public static byte[]? DecompressByteArrayByBrotli(this byte[]? value) => value.DecompressByteArray(s => new BrotliStream(s, CompressionMode.Decompress));

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static string ToHexString(this byte[] inArray/*, bool isLower = false*/)
    {
#if HEXMATE
        return HexMate.Convert.ToHexString(inArray, isLower ? HexMate.HexFormattingOptions.Lowercase : HexMate.HexFormattingOptions.None);
#elif NET5_0_OR_GREATER
        return /*isLower ? Convert.ToHexString(inArray).ToLowerInvariant() :*/ Convert.ToHexString(inArray);
#else
        return string.Concat(Array.ConvertAll(inArray, x => x.ToString(isLower ? "x2" : "X2")));
#endif
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static string ToHexString(this ReadOnlySpan<byte> bytes) => Convert.ToHexString(bytes);

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static string ToHexString(this byte[] inArray, int offset, int length) => Convert.ToHexString(inArray, offset, length);
}
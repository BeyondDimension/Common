namespace System.Extensions;

public static partial class ByteArrayExtensions // Compression
{
    /// <summary>
    /// 使用 GZip 压缩 byte[]
    /// </summary>
    /// <param name="buffer"></param>
    /// <param name="compressionLevel"></param>
    /// <returns></returns>
    [return: NotNullIfNotNull(nameof(buffer))]
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static byte[]? CompressByteArray(this byte[]? buffer, CompressionLevel? compressionLevel = null)
        => buffer.CompressByteArray(s => compressionLevel.HasValue ? new GZipStream(s, compressionLevel.Value, true) : new GZipStream(s, CompressionMode.Compress, true));

    /// <summary>
    /// 使用 GZip 压缩 byte[]
    /// </summary>
    /// <param name="buffer"></param>
    /// <param name="compressionLevel"></param>
    /// <param name="cancellationToken"></param>
    /// <returns></returns>
    [return: NotNullIfNotNull(nameof(buffer))]
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static ValueTask<byte[]?> CompressByteArrayAsync(this byte[]? buffer, CompressionLevel? compressionLevel = null, CancellationToken cancellationToken = default)
        => buffer.CompressByteArrayAsync(s => compressionLevel.HasValue ? new GZipStream(s, compressionLevel.Value, true) : new GZipStream(s, CompressionMode.Compress, true), cancellationToken);

    /// <summary>
    /// 使用 GZip 解压 byte[]
    /// </summary>
    /// <param name="value"></param>
    /// <returns></returns>
    [return: NotNullIfNotNull(nameof(value))]
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static byte[]? DecompressByteArray(this byte[]? value) => value.DecompressByteArray(s => new GZipStream(s, CompressionMode.Decompress));

    /// <summary>
    /// 使用 GZip 解压 byte[]
    /// </summary>
    /// <param name="value"></param>
    /// <param name="cancellationToken"></param>
    /// <returns></returns>
    [return: NotNullIfNotNull(nameof(value))]
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static ValueTask<byte[]?> DecompressByteArrayAsync(this byte[]? value, CancellationToken cancellationToken = default) => value.DecompressByteArrayAsync(s => new GZipStream(s, CompressionMode.Decompress), cancellationToken);

#if NETCOREAPP2_1_OR_GREATER || NETSTANDARD2_1_OR_GREATER
    /// <summary>
    /// 使用 Brotli 压缩 byte[]
    /// </summary>
    /// <param name="buffer"></param>
    /// <param name="compressionLevel"></param>
    /// <returns></returns>
    [return: NotNullIfNotNull(nameof(buffer))]
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static byte[]? CompressByteArrayByBrotli(this byte[]? buffer, CompressionLevel? compressionLevel = null)
        => buffer.CompressByteArray(s => compressionLevel.HasValue ? new BrotliStream(s, compressionLevel.Value, true) : new BrotliStream(s, CompressionMode.Compress, true));

    /// <summary>
    /// 使用 Brotli 压缩 byte[]
    /// </summary>
    /// <param name="buffer"></param>
    /// <param name="compressionLevel"></param>
    /// <param name="cancellationToken"></param>
    /// <returns></returns>
    [return: NotNullIfNotNull(nameof(buffer))]
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static ValueTask<byte[]?> CompressByteArrayByBrotliAsync(this byte[]? buffer, CompressionLevel? compressionLevel = null, CancellationToken cancellationToken = default)
        => buffer.CompressByteArrayAsync(s => compressionLevel.HasValue ? new BrotliStream(s, compressionLevel.Value, true) : new BrotliStream(s, CompressionMode.Compress, true), cancellationToken);

    /// <summary>
    /// 使用 Brotli 解压 byte[]
    /// </summary>
    /// <param name="value"></param>
    /// <returns></returns>
    [return: NotNullIfNotNull(nameof(value))]
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static byte[]? DecompressByteArrayByBrotli(this byte[]? value) => value.DecompressByteArray(s => new BrotliStream(s, CompressionMode.Decompress));

    /// <summary>
    /// 使用 Brotli 解压 byte[]
    /// </summary>
    /// <param name="value"></param>
    /// <param name="cancellationToken"></param>
    /// <returns></returns>
    [return: NotNullIfNotNull(nameof(value))]
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static ValueTask<byte[]?> DecompressByteArrayByBrotliAsync(this byte[]? value, CancellationToken cancellationToken = default) => value.DecompressByteArrayAsync(s => new BrotliStream(s, CompressionMode.Decompress), cancellationToken);
#endif

    static byte[]? CompressByteArray(
        this byte[]? buffer,
        Func<Stream, Stream> func)
    {
        if (buffer == null)
            return null;

        using var memoryStream = new MemoryStream();
        using (var stream = func(memoryStream))
            stream.Write(buffer, 0, buffer.Length);

        memoryStream.Position = 0;

        var compressedData = new byte[memoryStream.Length];
        memoryStream.Read(compressedData, 0, compressedData.Length);

        var bytes = new byte[compressedData.Length + 4];
        Buffer.BlockCopy(compressedData, 0, bytes, 4, compressedData.Length);
        Buffer.BlockCopy(BitConverter.GetBytes(buffer.Length), 0, bytes, 0, 4);
        return bytes;
    }

    static async ValueTask<byte[]?> CompressByteArrayAsync(
        this byte[]? buffer,
        Func<Stream, Stream> func,
        CancellationToken cancellationToken = default)
    {
        if (buffer == null)
            return null;

        using var memoryStream = new MemoryStream();
        using (var stream = func(memoryStream))
            await stream.WriteAsync(buffer, cancellationToken);

        memoryStream.Position = 0;

        var compressedData = new byte[memoryStream.Length];
        await memoryStream.ReadAsync(compressedData, cancellationToken);

        var bytes = new byte[compressedData.Length + 4];
        Buffer.BlockCopy(compressedData, 0, bytes, 4, compressedData.Length);
        Buffer.BlockCopy(BitConverter.GetBytes(buffer.Length), 0, bytes, 0, 4);
        return bytes;
    }

    [return: NotNullIfNotNull(nameof(value))]
    static byte[]? DecompressByteArray(
        this byte[]? value,
        Func<Stream, Stream> func)
    {
        if (value == null)
            return null;

        using var memoryStream = new MemoryStream();
        var dataLength = BitConverter.ToInt32(value, 0);
        memoryStream.Write(value, 4, value.Length - 4);

        var buffer = new byte[dataLength];

        memoryStream.Position = 0;
        using (var compressStream = func(memoryStream))
            compressStream.Read(buffer, 0, buffer.Length);

        return buffer;
    }

    [return: NotNullIfNotNull(nameof(value))]
    static async ValueTask<byte[]?> DecompressByteArrayAsync(
        this byte[]? value,
        Func<Stream, Stream> func,
        CancellationToken cancellationToken = default)
    {
        if (value == null)
            return null;

        using var memoryStream = new MemoryStream();
        var dataLength = BitConverter.ToInt32(value, 0);
#if NETSTANDARD2_1_OR_GREATER || NETCOREAPP2_1_OR_GREATER
        await memoryStream.WriteAsync(value.AsMemory(4, value.Length - 4), cancellationToken);
#else
        await memoryStream.WriteAsync(value, 4, value.Length - 4, cancellationToken);
#endif

        var buffer = new byte[dataLength];

        memoryStream.Position = 0;
        using (var compressStream = func(memoryStream))
            await compressStream.ReadAsync(buffer, cancellationToken);

        return buffer;
    }
}

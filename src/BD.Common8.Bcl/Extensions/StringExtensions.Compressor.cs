namespace System.Extensions;

/// <summary>
/// 提供对 <see cref="string"/> 类型的扩展函数
/// </summary>
public static partial class StringExtensions
{
    /// <summary>
    /// 使用 GZip 压缩 <see cref="string"/>
    /// </summary>
    /// <param name="text">The text.</param>
    /// <param name="compressionLevel"></param>
    /// <returns></returns>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static string CompressString(this string? text, CompressionLevel? compressionLevel = null)
    {
        if (string.IsNullOrEmpty(text))
            return string.Empty;
        byte[] buffer = Encoding.UTF8.GetBytes(text);
        var compressBuffer = buffer.CompressByteArray(compressionLevel);
        return compressBuffer.Base64Encode();
    }

    /// <summary>
    /// 使用 GZip 压缩 <see cref="string"/>
    /// </summary>
    /// <param name="text"></param>
    /// <param name="compressionLevel"></param>
    /// <param name="cancellationToken"></param>
    /// <returns></returns>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static async ValueTask<string> CompressStringAsync(this string? text, CompressionLevel? compressionLevel = null, CancellationToken cancellationToken = default)
    {
        if (string.IsNullOrEmpty(text))
            return string.Empty;
        byte[] buffer = Encoding.UTF8.GetBytes(text);
        var compressBuffer = await buffer.CompressByteArrayAsync(compressionLevel, cancellationToken);
        return compressBuffer!.Base64Encode();
    }

    /// <summary>
    /// 使用 GZip 解压 <see cref="string"/>
    /// </summary>
    /// <param name="compressedText">The compressed text.</param>
    /// <returns></returns>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static string DecompressString(this string? compressedText)
    {
        if (string.IsNullOrEmpty(compressedText))
            return string.Empty;
        byte[] compressBuffer = compressedText!.Base64DecodeToByteArray();
        var buffer = compressBuffer.DecompressByteArray();
        return Encoding.UTF8.GetString(buffer);
    }

    /// <summary>
    /// 使用 GZip 解压 <see cref="string"/>
    /// </summary>
    /// <param name="compressedText">The compressed text.</param>
    /// <param name="cancellationToken"></param>
    /// <returns></returns>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static async ValueTask<string> DecompressStringAsync(this string? compressedText, CancellationToken cancellationToken = default)
    {
        if (string.IsNullOrEmpty(compressedText))
            return string.Empty;
        byte[] compressBuffer = compressedText!.Base64DecodeToByteArray();
        var buffer = await compressBuffer.DecompressByteArrayAsync(cancellationToken);
        return Encoding.UTF8.GetString(buffer!);
    }

#if NETCOREAPP2_1_OR_GREATER || NETSTANDARD2_1_OR_GREATER
    /// <summary>
    /// 使用 Brotli 压缩 <see cref="string"/>
    /// </summary>
    /// <param name="text"></param>
    /// <param name="compressionLevel"></param>
    /// <returns></returns>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static string CompressStringByBrotli(this string? text, CompressionLevel? compressionLevel = null)
    {
        if (string.IsNullOrEmpty(text))
            return string.Empty;
        byte[] buffer = Encoding.UTF8.GetBytes(text);
        var compressBuffer = buffer.CompressByteArrayByBrotli(compressionLevel);
        return compressBuffer.Base64Encode();
    }

    /// <summary>
    /// 使用 Brotli 压缩 <see cref="string"/>
    /// </summary>
    /// <param name="text"></param>
    /// <param name="compressionLevel"></param>
    /// <param name="cancellationToken"></param>
    /// <returns></returns>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static async ValueTask<string> CompressStringByBrotliAsync(this string? text, CompressionLevel? compressionLevel = null, CancellationToken cancellationToken = default)
    {
        if (string.IsNullOrEmpty(text))
            return string.Empty;
        byte[] buffer = Encoding.UTF8.GetBytes(text);
        var compressBuffer = await buffer.CompressByteArrayByBrotliAsync(compressionLevel, cancellationToken);
        return compressBuffer!.Base64Encode();
    }

    /// <summary>
    /// 使用 Brotli 解压 <see cref="string"/>
    /// </summary>
    /// <param name="compressedText"></param>
    /// <returns></returns>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static string DecompressStringByBrotli(this string? compressedText)
    {
        if (string.IsNullOrEmpty(compressedText))
            return string.Empty;
        byte[] compressBuffer = compressedText.Base64DecodeToByteArray();
        var buffer = compressBuffer.DecompressByteArrayByBrotli();
        return Encoding.UTF8.GetString(buffer);
    }

    /// <summary>
    /// 使用 Brotli 解压 <see cref="string"/>
    /// </summary>
    /// <param name="compressedText"></param>
    /// <param name="cancellationToken"></param>
    /// <returns></returns>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static async ValueTask<string> DecompressStringByBrotliAsync(this string? compressedText, CancellationToken cancellationToken = default)
    {
        if (string.IsNullOrEmpty(compressedText))
            return string.Empty;
        byte[] compressBuffer = compressedText.Base64DecodeToByteArray();
        var buffer = await compressBuffer!.DecompressByteArrayByBrotliAsync(cancellationToken);
        return Encoding.UTF8.GetString(buffer!);
    }
#endif
}
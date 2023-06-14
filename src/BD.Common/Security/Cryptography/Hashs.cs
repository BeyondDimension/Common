namespace System.Security.Cryptography;

/// <summary>
/// 哈希算法
/// </summary>
public static partial class Hashs
{
    internal const bool def_hash_str_is_lower = true;

    internal delegate byte[] ComputeHashByteArrayDelegate(byte[] buffer);

    internal delegate byte[] ComputeHashReadOnlySpanByteDelegate(ReadOnlySpan<byte> buffer);

    internal delegate byte[] ComputeHashStreamDelegate(Stream stream);

    internal delegate ValueTask<byte[]> ComputeHashStreamAsyncDelegate(Stream stream, CancellationToken cancellationToken);

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    internal static string ToString(byte[] bytes, bool isLower = def_hash_str_is_lower) => string.Join(null, bytes.Select(x => x.ToString($"{(isLower ? "x" : "X")}2"))
#if NET35
        .ToArray()
#endif
        );

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    internal static byte[] ComputeHash<T>(byte[] buffer, T hashAlgorithm) where T : HashAlgorithm
    {
        var bytes = hashAlgorithm.ComputeHash(buffer);
        hashAlgorithm.Dispose();
        return bytes;
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    internal static byte[] ComputeHash<T>(Stream inputStream, T hashAlgorithm) where T : HashAlgorithm
    {
        var bytes = hashAlgorithm.ComputeHash(inputStream);
        hashAlgorithm.Dispose();
        return bytes;
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    internal static string ComputeHashString<T>(byte[] buffer, T hashAlgorithm, bool isLower = def_hash_str_is_lower) where T : HashAlgorithm
    {
        var bytes = ComputeHash(buffer, hashAlgorithm);
        return ToString(bytes, isLower);
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    internal static string ComputeHashString(byte[] buffer, ComputeHashByteArrayDelegate @delegate, bool isLower = def_hash_str_is_lower)
    {
        var bytes = @delegate(buffer);
        return ToString(bytes, isLower);
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    internal static string ComputeHashString(ReadOnlySpan<byte> buffer, ComputeHashReadOnlySpanByteDelegate @delegate, bool isLower = def_hash_str_is_lower)
    {
        var bytes = @delegate(buffer);
        return ToString(bytes, isLower);
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    internal static string ComputeHashString<T>(Stream inputStream, T hashAlgorithm, bool isLower = def_hash_str_is_lower) where T : HashAlgorithm
    {
        var bytes = ComputeHash(inputStream, hashAlgorithm);
        return ToString(bytes, isLower);
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    internal static string ComputeHashString(Stream inputStream, ComputeHashStreamDelegate @delegate, bool isLower = def_hash_str_is_lower)
    {
        var bytes = @delegate(inputStream);
        return ToString(bytes, isLower);
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    internal static async ValueTask<string> ComputeHashStringAsync(Stream inputStream, ComputeHashStreamAsyncDelegate @delegate, bool isLower = def_hash_str_is_lower, CancellationToken cancellationToken = default)
    {
        var bytes = await @delegate(inputStream, cancellationToken);
        return ToString(bytes, isLower);
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    internal static string ComputeHashString<T>(string str, T hashAlgorithm, bool isLower = def_hash_str_is_lower) where T : HashAlgorithm
    {
        var bytes = Encoding.UTF8.GetBytes(str);
        return ComputeHashString(bytes, hashAlgorithm, isLower);
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    internal static string ComputeHashString(string str, ComputeHashByteArrayDelegate @delegate, bool isLower = def_hash_str_is_lower)
    {
        var bytes = Encoding.UTF8.GetBytes(str);
        return ComputeHashString(bytes, @delegate, isLower);
    }

    public static partial class String
    {
        public static partial class Lengths { }
    }

    public static partial class ByteArray
    {

    }
}
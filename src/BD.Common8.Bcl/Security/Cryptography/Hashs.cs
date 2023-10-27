namespace System.Security.Cryptography;

/// <summary>
/// 哈希算法
/// </summary>
public static partial class Hashs
{
    /// <summary>
    /// 默认哈希字符串的大小写形式为小写
    /// </summary>
    internal const bool def_hash_str_is_lower = true;

    /// <summary>
    /// 定义计算字节数组哈希值的委托类型
    /// </summary>
    internal delegate byte[] ComputeHashByteArrayDelegate(byte[] buffer);

    /// <summary>
    /// 定义计算只读字节数组哈希值的委托类型
    /// </summary>
    /// <param name="buffer"></param>
    /// <returns></returns>
    internal delegate byte[] ComputeHashReadOnlySpanByteDelegate(ReadOnlySpan<byte> buffer);

    /// <summary>
    /// 定义计算流哈希值的委托类型
    /// </summary>
    internal delegate byte[] ComputeHashStreamDelegate(Stream stream);

    /// <summary>
    ///  定义异步计算流哈希值的委托类型
    /// </summary>
    internal delegate ValueTask<byte[]> ComputeHashStreamAsyncDelegate(Stream stream, CancellationToken cancellationToken);

    /// <summary>
    /// 将字节数组转换为十六进制字符串形式
    /// </summary>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    internal static string ToHexString(byte[] bytes, bool isLower = def_hash_str_is_lower) =>
#if NET35
        string.Join(null, bytes.Select(x => x.ToString($"{(isLower ? "x" : "X")}2")).ToArray());
#else
        bytes.ToHexString(isLower);
#endif

    /// <summary>
    /// 计算字节数组的哈希值
    /// </summary>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    internal static byte[] ComputeHash<T>(byte[] buffer, T hashAlgorithm) where T : HashAlgorithm
    {
        var bytes = hashAlgorithm.ComputeHash(buffer);
        hashAlgorithm.Dispose();
        return bytes;
    }

    /// <summary>
    /// 计算流的哈希值
    /// </summary>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    internal static byte[] ComputeHash<T>(Stream inputStream, T hashAlgorithm) where T : HashAlgorithm
    {
        var bytes = hashAlgorithm.ComputeHash(inputStream);
        hashAlgorithm.Dispose();
        return bytes;
    }

    /// <summary>
    /// 根据指定的哈希算法类型，计算字节数组的哈希值，并返回十六进制字符串形式
    /// </summary>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    internal static string ComputeHashString<T>(byte[] buffer, T hashAlgorithm, bool isLower = def_hash_str_is_lower) where T : HashAlgorithm
    {
        var bytes = ComputeHash(buffer, hashAlgorithm);
        return ToHexString(bytes, isLower);
    }

    /// <summary>
    /// 根据指定的委托方法，计算字节数组的哈希值，并返回十六进制字符串形式
    /// </summary>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    internal static string ComputeHashString(byte[] buffer, ComputeHashByteArrayDelegate @delegate, bool isLower = def_hash_str_is_lower)
    {
        var bytes = @delegate(buffer);
        return ToHexString(bytes, isLower);
    }

    /// <summary>
    /// 根据指定的委托方法，计算只读字节数组的哈希值，并返回十六进制字符串形式
    /// </summary>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    internal static string ComputeHashString(ReadOnlySpan<byte> buffer, ComputeHashReadOnlySpanByteDelegate @delegate, bool isLower = def_hash_str_is_lower)
    {
        var bytes = @delegate(buffer);
        return ToHexString(bytes, isLower);
    }

    /// <summary>
    /// 根据指定的哈希算法类型，计算输入流哈希值，并返回十六进制字符串形式
    /// </summary>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    internal static string ComputeHashString<T>(Stream inputStream, T hashAlgorithm, bool isLower = def_hash_str_is_lower) where T : HashAlgorithm
    {
        var bytes = ComputeHash(inputStream, hashAlgorithm);
        return ToHexString(bytes, isLower);
    }

    /// <summary>
    /// 根据指定的委托方法，计算输入流哈希值，并返回十六进制字符串形式
    /// </summary>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    internal static string ComputeHashString(Stream inputStream, ComputeHashStreamDelegate @delegate, bool isLower = def_hash_str_is_lower)
    {
        var bytes = @delegate(inputStream);
        return ToHexString(bytes, isLower);
    }

    /// <summary>
    /// 异步方式根据指定的委托方法，计算输入流哈希值，并返回十六进制字符串形式
    /// </summary>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    internal static async ValueTask<string> ComputeHashStringAsync(Stream inputStream, ComputeHashStreamAsyncDelegate @delegate, bool isLower = def_hash_str_is_lower, CancellationToken cancellationToken = default)
    {
        var bytes = await @delegate(inputStream, cancellationToken);
        return ToHexString(bytes, isLower);
    }

    /// <summary>
    /// 根据指定的哈希算法类型，计算字符串哈希值，并返回十六进制字符串形式
    /// </summary>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    internal static string ComputeHashString<T>(string str, T hashAlgorithm, bool isLower = def_hash_str_is_lower) where T : HashAlgorithm
    {
        var bytes = Encoding.UTF8.GetBytes(str);
        return ComputeHashString(bytes, hashAlgorithm, isLower);
    }

    /// <summary>
    /// 根据指定的委托方法，计算字符串哈希值，并返回十六进制字符串形式
    /// </summary>
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
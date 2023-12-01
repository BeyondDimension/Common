namespace System.Security.Cryptography;

partial class Hashs
{
    partial class String
    {
        partial class Lengths
        {
            /// <summary>
            /// SHA512 算法的哈希长度
            /// </summary>
            public const int SHA512 = 128;
        }

        /// <summary>
        /// 计算 SHA512 值
        /// </summary>
        /// <param name="text"></param>
        /// <param name="isLower"></param>
        /// <returns></returns>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static string SHA512(string text, bool isLower = def_hash_str_is_lower) => ComputeHashString(text, Cryptography.SHA512.HashData, isLower);

        /// <summary>
        /// 计算 SHA512 值
        /// </summary>
        /// <param name="buffer"></param>
        /// <param name="isLower"></param>
        /// <returns></returns>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static string SHA512(byte[] buffer, bool isLower = def_hash_str_is_lower) => ComputeHashString(buffer, Cryptography.SHA512.HashData, isLower);

        /// <summary>
        /// 计算 SHA512 值
        /// </summary>
        /// <param name="inputStream"></param>
        /// <param name="isLower"></param>
        /// <returns></returns>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static string SHA512(Stream inputStream, bool isLower = def_hash_str_is_lower) => ComputeHashString(inputStream, Cryptography.SHA512.HashData, isLower);

        /// <summary>
        /// 计算 SHA512 值
        /// </summary>
        /// <param name="inputStream"></param>
        /// <param name="isLower"></param>
        /// <param name="cancellationToken"></param>
        /// <returns></returns>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static ValueTask<string> SHA512Async(Stream inputStream, bool isLower = def_hash_str_is_lower, CancellationToken cancellationToken = default) => ComputeHashStringAsync(inputStream, Cryptography.SHA512.HashDataAsync, isLower, cancellationToken);
    }

    partial class ByteArray
    {
        /// <summary>
        /// 计算指定字节数组的 SHA512 哈希值
        /// </summary>
        /// <param name="buffer">要计算哈希值的字节数组</param>
        /// <returns>字节数组的 SHA512 哈希值</returns>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static byte[] SHA512(byte[] buffer) => Cryptography.SHA512.HashData(buffer);

        /// <summary>
        /// 计算指定只读字节数组的 SHA512 哈希值
        /// </summary>
        /// <param name="buffer">要计算哈希值的只读字节数组</param>
        /// <returns>字节数组的 SHA512 哈希值</returns>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static byte[] SHA512(ReadOnlySpan<byte> buffer) => Cryptography.SHA512.HashData(buffer);

        /// <summary>
        /// 计算指定流的 SHA512 哈希值
        /// </summary>
        /// <param name="inputStream">要计算哈希值的流</param>
        /// <returns>字节数组的 SHA512 哈希值</returns>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static byte[] SHA512(Stream inputStream) => Cryptography.SHA512.HashData(inputStream);

        /// <summary>
        /// 异步计算指定流的 SHA512 哈希值
        /// </summary>
        /// <param name="inputStream">要计算哈希值的流</param>
        /// <param name="cancellationToken">取消操作的令牌</param>
        /// <returns>表示异步任务，结果为字节数组的 SHA512 哈希值</returns>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static ValueTask<byte[]> SHA512Async(Stream inputStream, CancellationToken cancellationToken = default) => Cryptography.SHA512.HashDataAsync(inputStream, cancellationToken);
    }
}
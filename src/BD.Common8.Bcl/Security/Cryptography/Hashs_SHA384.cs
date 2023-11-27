namespace System.Security.Cryptography;

partial class Hashs
{
    partial class String
    {
        partial class Lengths
        {
            /// <summary>
            /// SHA384 算法的哈希长度
            /// </summary>
            public const int SHA384 = 96;
        }

        /// <summary>
        /// 计算 SHA384 值
        /// </summary>
        /// <param name="text"></param>
        /// <param name="isLower"></param>
        /// <returns></returns>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static string SHA384(string text, bool isLower = def_hash_str_is_lower) => ComputeHashString(text, Cryptography.SHA384.HashData, isLower);

        /// <summary>
        /// 计算 SHA384 值
        /// </summary>
        /// <param name="buffer"></param>
        /// <param name="isLower"></param>
        /// <returns></returns>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static string SHA384(byte[] buffer, bool isLower = def_hash_str_is_lower) => ComputeHashString(buffer, Cryptography.SHA384.HashData, isLower);

        /// <summary>
        /// 计算 SHA384 值
        /// </summary>
        /// <param name="inputStream"></param>
        /// <param name="isLower"></param>
        /// <returns></returns>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static string SHA384(Stream inputStream, bool isLower = def_hash_str_is_lower) => ComputeHashString(inputStream, Cryptography.SHA384.HashData, isLower);

        /// <summary>
        /// 计算 SHA384 值
        /// </summary>
        /// <param name="inputStream"></param>
        /// <param name="isLower"></param>
        /// <param name="cancellationToken"></param>
        /// <returns></returns>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static ValueTask<string> SHA384Async(Stream inputStream, bool isLower = def_hash_str_is_lower, CancellationToken cancellationToken = default) => ComputeHashStringAsync(inputStream, Cryptography.SHA384.HashDataAsync, isLower, cancellationToken);
    }

    partial class ByteArray
    {
        /// <summary>
        /// 计算指定字节数组的 SHA384 哈希值
        /// </summary>
        /// <param name="buffer">要计算哈希值的字节数组</param>
        /// <returns>表示字节数组的 SHA384 哈希值的字节数组</returns>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static byte[] SHA384(byte[] buffer) => Cryptography.SHA384.HashData(buffer);

        /// <summary>
        /// 计算指定流的 SHA384 哈希值
        /// </summary>
        /// <param name="inputStream">要计算哈希值的流</param>
        /// <returns>表示流的 SHA384 哈希值的字节数组</returns>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static byte[] SHA384(Stream inputStream) => Cryptography.SHA384.HashData(inputStream);

        /// <summary>
        /// 异步计算指定流的 SHA384 哈希值
        /// </summary>
        /// <param name="inputStream">要计算哈希值的流</param>
        /// <param name="cancellationToken">取消操作令牌</param>
        /// <returns>表示异步操作的任务对象，包含流的 SHA384 哈希值的字节数组</returns>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static ValueTask<byte[]> SHA384Async(Stream inputStream, CancellationToken cancellationToken = default) => Cryptography.SHA384.HashDataAsync(inputStream, cancellationToken);
    }
}
namespace System.Security.Cryptography;

partial class Hashs
{
    partial class String
    {
        partial class Lengths
        {
            /// <summary>
            /// SHA1 算法的哈希长度
            /// </summary>
            public const int SHA1 = 40;
        }

        /// <summary>
        /// 计算 SHA1 值
        /// </summary>
        /// <param name="text"></param>
        /// <param name="isLower"></param>
        /// <returns></returns>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static string SHA1(string text, bool isLower = def_hash_str_is_lower) => ComputeHashString(text, Cryptography.SHA1.HashData, isLower);

        /// <summary>
        /// 计算 SHA1 值
        /// </summary>
        /// <param name="buffer"></param>
        /// <param name="isLower"></param>
        /// <returns></returns>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static string SHA1(byte[] buffer, bool isLower = def_hash_str_is_lower) => ComputeHashString(buffer, Cryptography.SHA1.HashData, isLower);

        /// <summary>
        /// 计算 SHA1 值
        /// </summary>
        /// <param name="inputStream"></param>
        /// <param name="isLower"></param>
        /// <returns></returns>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static string SHA1(Stream inputStream, bool isLower = def_hash_str_is_lower) => ComputeHashString(inputStream, Cryptography.SHA1.HashData, isLower);

        /// <summary>
        /// 计算 SHA1 值
        /// </summary>
        /// <param name="inputStream"></param>
        /// <param name="isLower"></param>
        /// <param name="cancellationToken"></param>
        /// <returns></returns>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static ValueTask<string> SHA1Async(Stream inputStream, bool isLower = def_hash_str_is_lower, CancellationToken cancellationToken = default) => ComputeHashStringAsync(inputStream, Cryptography.SHA1.HashDataAsync, isLower, cancellationToken);
    }

    partial class ByteArray
    {
        /// <summary>
        /// 计算指定字节数组的 SHA1 哈希值
        /// </summary>
        /// <param name="buffer">要计算哈希值的字节数组</param>
        /// <returns>计算得到的 SHA1 哈希值</returns>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static byte[] SHA1(byte[] buffer) => Cryptography.SHA1.HashData(buffer);

        /// <summary>
        /// 计算指定只读字节数组的 SHA1 哈希值
        /// </summary>
        /// <param name="buffer">要计算哈希值的只读字节数组</param>
        /// <returns>计算得到的 SHA1 哈希值</returns>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static byte[] SHA1(ReadOnlySpan<byte> buffer) => Cryptography.SHA1.HashData(buffer);

        /// <summary>
        /// 计算从指定流中读取的数据的 SHA1 哈希值
        /// </summary>
        /// <param name="inputStream">要读取数据的流</param>
        /// <returns>计算得到的 SHA1 哈希值</returns>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static byte[] SHA1(Stream inputStream) => Cryptography.SHA1.HashData(inputStream);

        /// <summary>
        /// 异步计算从指定流中读取的数据的 SHA1 哈希值
        /// </summary>
        /// <param name="inputStream">要读取数据的流</param>
        /// <param name="cancellationToken">用于取消操作的取消标记</param>
        /// <returns>计算得到的 SHA1 哈希值的异步任务</returns>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static ValueTask<byte[]> SHA1Async(Stream inputStream, CancellationToken cancellationToken = default) => Cryptography.SHA1.HashDataAsync(inputStream, cancellationToken);
    }
}
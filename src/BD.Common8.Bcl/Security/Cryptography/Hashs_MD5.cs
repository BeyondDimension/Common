namespace System.Security.Cryptography;

partial class Hashs
{
    partial class String
    {
        partial class Lengths
        {
            /// <summary>
            /// MD5 算法的哈希长度
            /// </summary>
            public const int MD5 = 32;
        }

        /// <summary>
        /// 计算 MD5 值
        /// </summary>
        /// <param name="text"></param>
        /// <param name="isLower"></param>
        /// <returns></returns>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static string MD5(string text, bool isLower = def_hash_str_is_lower) => ComputeHashString(text, Cryptography.MD5.HashData, isLower);

        /// <summary>
        /// 计算 MD5 值
        /// </summary>
        /// <param name="buffer"></param>
        /// <param name="isLower"></param>
        /// <returns></returns>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static string MD5(byte[] buffer, bool isLower = def_hash_str_is_lower) => ComputeHashString(buffer, Cryptography.MD5.HashData, isLower);

        /// <summary>
        /// 计算 MD5 值
        /// </summary>
        /// <param name="buffer"></param>
        /// <param name="isLower"></param>
        /// <returns></returns>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static string MD5(ReadOnlySpan<byte> buffer, bool isLower = def_hash_str_is_lower) => ComputeHashString(buffer, Cryptography.MD5.HashData, isLower);

        /// <summary>
        /// 计算 MD5 值
        /// </summary>
        /// <param name="inputStream"></param>
        /// <param name="isLower"></param>
        /// <returns></returns>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static string MD5(Stream inputStream, bool isLower = def_hash_str_is_lower) => ComputeHashString(inputStream, Cryptography.MD5.HashData, isLower);

        /// <summary>
        /// 计算 MD5 值
        /// </summary>
        /// <param name="inputStream"></param>
        /// <param name="isLower"></param>
        /// <param name="cancellationToken"></param>
        /// <returns></returns>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static ValueTask<string> MD5Async(Stream inputStream, bool isLower = def_hash_str_is_lower, CancellationToken cancellationToken = default) => ComputeHashStringAsync(inputStream, Cryptography.MD5.HashDataAsync, isLower, cancellationToken);
    }

    partial class ByteArray
    {
        /// <summary>
        /// 计算字节数组的 MD5 哈希值并返回哈希结果
        /// </summary>
        /// <param name="buffer">要计算哈希值的字节数组</param>
        /// <returns>计算得到的哈希值</returns>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static byte[] MD5(byte[] buffer) => Cryptography.MD5.HashData(buffer);

        /// <summary>
        /// 计算只读字节数组的 MD5 哈希值并返回哈希结果
        /// </summary>
        /// <param name="buffer">要计算哈希值的只读字节数组</param>
        /// <returns>计算得到的哈希值</returns>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static byte[] MD5(ReadOnlySpan<byte> buffer) => Cryptography.MD5.HashData(buffer);

        /// <summary>
        /// 计算流的 MD5 哈希值并返回哈希结果
        /// </summary>
        /// <param name="inputStream">要计算哈希值的流</param>
        /// <returns>计算得到的哈希值</returns>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static byte[] MD5(Stream inputStream) => Cryptography.MD5.HashData(inputStream);

        /// <summary>
        /// 异步计算流的 MD5 哈希值并返回哈希结果
        /// </summary>
        /// <param name="inputStream">要异步计算哈希值的流</param>
        /// <param name="cancellationToken">取消操作的令牌</param>
        /// <returns>计算得到的哈希值的异步任务</returns>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static ValueTask<byte[]> MD5Async(Stream inputStream, CancellationToken cancellationToken = default) => Cryptography.MD5.HashDataAsync(inputStream, cancellationToken);
    }
}
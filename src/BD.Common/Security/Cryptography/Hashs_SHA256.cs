namespace System.Security.Cryptography;

partial class Hashs
{
    partial class String
    {
        partial class Lengths
        {
            public const int SHA256 = 64;
        }

        /// <summary>
        /// 计算 SHA256 值
        /// </summary>
        /// <param name="text"></param>
        /// <param name="isLower"></param>
        /// <returns></returns>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static string SHA256(string text, bool isLower = def_hash_str_is_lower) => ComputeHashString(text, Cryptography.SHA256.HashData, isLower);

        /// <summary>
        /// 计算 SHA256 值
        /// </summary>
        /// <param name="buffer"></param>
        /// <param name="isLower"></param>
        /// <returns></returns>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static string SHA256(byte[] buffer, bool isLower = def_hash_str_is_lower) => ComputeHashString(buffer, Cryptography.SHA256.HashData, isLower);

        /// <summary>
        /// 计算 SHA256 值
        /// </summary>
        /// <param name="inputStream"></param>
        /// <param name="isLower"></param>
        /// <returns></returns>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static string SHA256(Stream inputStream, bool isLower = def_hash_str_is_lower) => ComputeHashString(inputStream, Cryptography.SHA256.HashData, isLower);

        /// <summary>
        /// 计算 SHA256 值
        /// </summary>
        /// <param name="inputStream"></param>
        /// <param name="isLower"></param>
        /// <param name="cancellationToken"></param>
        /// <returns></returns>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static ValueTask<string> SHA256Async(Stream inputStream, bool isLower = def_hash_str_is_lower, CancellationToken cancellationToken = default) => ComputeHashStringAsync(inputStream, Cryptography.SHA256.HashDataAsync, isLower, cancellationToken);
    }

    partial class ByteArray
    {
        /// <summary>
        /// 计算 SHA256 值
        /// </summary>
        /// <param name="buffer"></param>
        /// <returns></returns>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static byte[] SHA256(byte[] buffer) => Cryptography.SHA256.HashData(buffer);

        /// <summary>
        /// 计算 SHA256 值
        /// </summary>
        /// <param name="buffer"></param>
        /// <returns></returns>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static byte[] SHA256(ReadOnlySpan<byte> buffer) => Cryptography.SHA256.HashData(buffer);

        /// <summary>
        /// 计算 SHA256 值
        /// </summary>
        /// <param name="inputStream"></param>
        /// <returns></returns>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static byte[] SHA256(Stream inputStream) => Cryptography.SHA256.HashData(inputStream);

        /// <summary>
        /// 计算 SHA256 值
        /// </summary>
        /// <param name="inputStream"></param>
        /// <param name="cancellationToken"></param>
        /// <returns></returns>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static ValueTask<byte[]> SHA256Async(Stream inputStream, CancellationToken cancellationToken = default) => Cryptography.SHA256.HashDataAsync(inputStream, cancellationToken);
    }
}
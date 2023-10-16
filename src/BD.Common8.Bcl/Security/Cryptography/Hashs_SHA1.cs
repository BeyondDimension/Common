namespace System.Security.Cryptography;

#pragma warning disable SA1600 // Elements should be documented

partial class Hashs
{
    partial class String
    {
        partial class Lengths
        {
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
        /// 计算 SHA1 值
        /// </summary>
        /// <param name="buffer"></param>
        /// <returns></returns>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static byte[] SHA1(byte[] buffer) => Cryptography.SHA1.HashData(buffer);

        /// <summary>
        /// 计算 SHA1 值
        /// </summary>
        /// <param name="buffer"></param>
        /// <returns></returns>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static byte[] SHA1(ReadOnlySpan<byte> buffer) => Cryptography.SHA1.HashData(buffer);

        /// <summary>
        /// 计算 SHA1 值
        /// </summary>
        /// <param name="inputStream"></param>
        /// <returns></returns>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static byte[] SHA1(Stream inputStream) => Cryptography.SHA1.HashData(inputStream);

        /// <summary>
        /// 计算 SHA1 值
        /// </summary>
        /// <param name="inputStream"></param>
        /// <param name="cancellationToken"></param>
        /// <returns></returns>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static ValueTask<byte[]> SHA1Async(Stream inputStream, CancellationToken cancellationToken = default) => Cryptography.SHA1.HashDataAsync(inputStream, cancellationToken);
    }
}
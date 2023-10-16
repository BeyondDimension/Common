namespace System.Security.Cryptography;

#pragma warning disable SA1600 // Elements should be documented

partial class Hashs
{
    partial class String
    {
        partial class Lengths
        {
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
        /// 计算 MD5 值
        /// </summary>
        /// <param name="buffer"></param>
        /// <returns></returns>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static byte[] MD5(byte[] buffer) => Cryptography.MD5.HashData(buffer);

        /// <summary>
        /// 计算 MD5 值
        /// </summary>
        /// <param name="buffer"></param>
        /// <returns></returns>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static byte[] MD5(ReadOnlySpan<byte> buffer) => Cryptography.MD5.HashData(buffer);

        /// <summary>
        /// 计算 MD5 值
        /// </summary>
        /// <param name="inputStream"></param>
        /// <returns></returns>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static byte[] MD5(Stream inputStream) => Cryptography.MD5.HashData(inputStream);

        /// <summary>
        /// 计算 MD5 值
        /// </summary>
        /// <param name="inputStream"></param>
        /// <param name="cancellationToken"></param>
        /// <returns></returns>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static ValueTask<byte[]> MD5Async(Stream inputStream, CancellationToken cancellationToken = default) => Cryptography.MD5.HashDataAsync(inputStream, cancellationToken);
    }
}
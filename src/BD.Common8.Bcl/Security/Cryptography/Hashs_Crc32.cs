using Force.Crc32;

namespace System.Security.Cryptography;

partial class Hashs
{
    /// <summary>
    /// 创建一个 Crc32 算法实例
    /// </summary>
    /// <returns><see cref="Crc32Algorithm"/> 实例</returns>
    static Crc32Algorithm CreateCrc32()
    {
        return new Crc32Algorithm();
    }

    partial class String
    {
        partial class Lengths
        {
            /// <summary>
            /// Crc32 算法的哈希长度
            /// </summary>
            public const int Crc32 = 8;
        }

        /// <summary>
        /// 计算 Crc32 值
        /// </summary>
        /// <param name="text"></param>
        /// <param name="isLower"></param>
        /// <returns></returns>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static string Crc32(string text, bool isLower = def_hash_str_is_lower) => ComputeHashString(text, CreateCrc32(), isLower);

        /// <summary>
        /// 计算 Crc32 值
        /// </summary>
        /// <param name="buffer"></param>
        /// <param name="isLower"></param>
        /// <returns></returns>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static string Crc32(byte[] buffer, bool isLower = def_hash_str_is_lower) => ComputeHashString(buffer, CreateCrc32(), isLower);

        /// <summary>
        /// 计算 Crc32 值
        /// </summary>
        /// <param name="buffer"></param>
        /// <param name="isLower"></param>
        /// <returns></returns>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static string Crc32(ReadOnlySpan<byte> buffer, bool isLower = def_hash_str_is_lower) => ComputeHashString(buffer.ToArray(), CreateCrc32(), isLower);

        /// <summary>
        /// 计算 Crc32 值
        /// </summary>
        /// <param name="inputStream"></param>
        /// <param name="isLower"></param>
        /// <returns></returns>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static string Crc32(Stream inputStream, bool isLower = def_hash_str_is_lower) => ComputeHashString(inputStream, CreateCrc32(), isLower);
    }

    partial class ByteArray
    {
        /// <summary>
        /// 计算字节数组的 Crc32 哈希值并返回哈希结果
        /// </summary>
        /// <param name="buffer">要计算哈希值的字节数组</param>
        /// <returns>计算得到的 MD5 哈希值</returns>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static byte[] Crc32(byte[] buffer) => ComputeHash(buffer, CreateCrc32());

        /// <summary>
        /// 计算只读字节数组的 Crc32 哈希值并返回哈希结果
        /// </summary>
        /// <param name="buffer">要计算哈希值的字节数组的只读跨度</param>
        /// <returns>计算得到的哈希结果的字节数组</returns>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static byte[] Crc32(ReadOnlySpan<byte> buffer) => ComputeHash(buffer.ToArray(), CreateCrc32());

        /// <summary>
        /// 计算流的 Crc32 哈希值并返回哈希结果
        /// </summary>
        /// <param name="inputStream">要计算哈希值的流</param>
        /// <returns>计算得到的 MD5 哈希值</returns>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static byte[] Crc32(Stream inputStream) => ComputeHash(inputStream, CreateCrc32());
    }
}
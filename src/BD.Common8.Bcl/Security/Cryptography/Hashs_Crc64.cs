using HashAlg_ = System.IO.Hashing.Crc64;

namespace System.Security.Cryptography;

partial class Hashs
{
    partial class String
    {
        partial class Lengths
        {
            /// <summary>
            /// Crc64 算法的哈希长度
            /// </summary>
            public const int Crc64 = 16;
        }

        /// <summary>
        /// 计算 Crc64 值
        /// </summary>
        /// <param name="text"></param>
        /// <param name="isLower"></param>
        /// <returns></returns>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static string Crc64(string text, bool isLower = def_hash_str_is_lower)
        {
            var bytes = Encoding.UTF8.GetBytes(text);
            return Crc64(bytes, isLower);
        }

        /// <summary>
        /// 计算 Crc64 值
        /// </summary>
        /// <param name="buffer"></param>
        /// <param name="isLower"></param>
        /// <returns></returns>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static string Crc64(byte[] buffer, bool isLower = def_hash_str_is_lower)
        {
            var result = HashAlg_.Hash(buffer);
            result.AsSpan().Reverse();
            return result.ToHexString(isLower);
        }

        /// <summary>
        /// 计算 Crc64 值
        /// </summary>
        /// <param name="buffer"></param>
        /// <param name="isLower"></param>
        /// <returns></returns>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static string Crc64(ReadOnlySpan<byte> buffer, bool isLower = def_hash_str_is_lower)
        {
            var result = HashAlg_.Hash(buffer);
            result.AsSpan().Reverse();
            return result.ToHexString(isLower);
        }

        /// <summary>
        /// 计算 Crc64 值
        /// </summary>
        /// <param name="inputStream"></param>
        /// <param name="isLower"></param>
        /// <returns></returns>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static string Crc64(Stream inputStream, bool isLower = def_hash_str_is_lower)
        {
            var hash = new HashAlg_();
            hash.Append(inputStream);
            var result = hash.GetCurrentHash();
            result.AsSpan().Reverse();
            return result.ToHexString(isLower);
        }
    }

    partial class ByteArray
    {
        partial class Lengths
        {
            /// <summary>
            /// Crc64 算法的哈希长度
            /// </summary>
            public const int Crc64 = 8;
        }

        /// <summary>
        /// 计算字节数组的 Crc64 哈希值并返回哈希结果
        /// </summary>
        /// <param name="buffer">要计算哈希值的字节数组</param>
        /// <returns>计算得到的哈希值</returns>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static byte[] Crc64(byte[] buffer)
        {
            var result = HashAlg_.Hash(buffer);
            result.AsSpan().Reverse();
            return result;
        }

        /// <summary>
        /// 计算只读字节数组的 Crc64 哈希值并返回哈希结果
        /// </summary>
        /// <param name="buffer">要计算哈希值的字节数组的只读跨度</param>
        /// <returns>计算得到的哈希结果的字节数组</returns>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static byte[] Crc64(ReadOnlySpan<byte> buffer)
        {
            var result = HashAlg_.Hash(buffer);
            result.AsSpan().Reverse();
            return result;
        }

        /// <summary>
        /// 计算流的 Crc64 哈希值并返回哈希结果
        /// </summary>
        /// <param name="inputStream">要计算哈希值的流</param>
        /// <returns>计算得到的哈希值</returns>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static byte[] Crc64(Stream inputStream)
        {
            var hash = new HashAlg_();
            hash.Append(inputStream);
            var result = hash.GetCurrentHash();
            result.AsSpan().Reverse();
            return result;
        }
    }
}
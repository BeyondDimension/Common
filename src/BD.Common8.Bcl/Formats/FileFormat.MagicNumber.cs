namespace System.Formats;

partial class FileFormat
{
    /// <summary>
    /// 文件头标识(幻数)
    /// </summary>
    public static class MagicNumber
    {
        /// <summary>
        /// 对 MagicNumber 进行比较，以确定两个序列是否相等，如果某一个序列为 <see langword="null"/> 则返回 <see langword="false"/>
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="first"></param>
        /// <param name="second"></param>
        /// <param name="equatable"></param>
        /// <returns></returns>
        static bool SequenceEqual<T>(
            IReadOnlyList<byte> first,
            IReadOnlyCollection<T> second,
            Func<byte, T, bool> equatable)
        {
            if (first == null || second == null) return false;
            if (first.Count < second.Count) return false;
            return !second.Where((t, i) => !equatable(first[i], t)).Any();
        }

        static bool SequenceEqual<T>(
            ReadOnlyMemory<byte> first,
            IReadOnlyCollection<T> second,
            Func<byte, T, bool> equatable)
        {
            if (second == null) return false;
            if (first.Length < second.Count) return false;
            return !second.Where((t, i) => !equatable(first.Span[i], t)).Any();
        }

        static bool Equals(byte left, byte right) => left.Equals(right);

        static bool Equals(byte left, byte? right) => !right.HasValue || left.Equals(right);

        /// <inheritdoc cref="SequenceEqual{T}(IReadOnlyList{byte}, IReadOnlyCollection{T}, Func{byte, T, bool})"/>
        public static bool SequenceEqual(IReadOnlyList<byte> first, IReadOnlyCollection<byte?> second) => SequenceEqual(first, second, Equals);

        /// <inheritdoc cref="SequenceEqual{T}(IReadOnlyList{byte}, IReadOnlyCollection{T}, Func{byte, T, bool})"/>
        public static bool SequenceEqual(IReadOnlyList<byte> first, IReadOnlyCollection<byte> second) => SequenceEqual(first, second, Equals);

        /// <summary>
        /// 判断 <see langword="first"/> 与 <see langword="seconds"/> 集合中的任意一个是否相等
        /// </summary>
        public static bool SequenceEqual(IReadOnlyList<byte> first, IEnumerable<IReadOnlyCollection<byte>> seconds)
            => seconds.Any(second => SequenceEqual(first, second));

        /// <summary>
        /// 判断 <see langword="first"/> 与 <see langword="seconds"/> 可包含空值集合中的任意一个是否相等
        /// </summary>
        public static bool SequenceEqual(IReadOnlyList<byte> first, IEnumerable<IReadOnlyCollection<byte?>> seconds)
            => seconds.Any(second => SequenceEqual(first, second));

        /// <inheritdoc cref="SequenceEqual{T}(ReadOnlyMemory{byte}, IReadOnlyCollection{T}, Func{byte, T, bool})"/>
        public static bool SequenceEqual(ReadOnlyMemory<byte> first, IReadOnlyCollection<byte?> second) => SequenceEqual(first, second, Equals);

        /// <inheritdoc cref="SequenceEqual{T}(ReadOnlyMemory{byte}, IReadOnlyCollection{T}, Func{byte, T, bool})"/>
        public static bool SequenceEqual(ReadOnlyMemory<byte> first, IReadOnlyCollection<byte> second) => SequenceEqual(first, second, Equals);

        /// <summary>
        /// 判断 <see langword="first"/> 与 <see langword="seconds"/> 集合中的任意一个是否相等
        /// </summary>
        public static bool SequenceEqual(ReadOnlyMemory<byte> first, IEnumerable<IReadOnlyCollection<byte>> seconds)
            => seconds.Any(second => SequenceEqual(first, second));

        /// <summary>
        /// 判断 <see langword="first"/> 与 <see langword="seconds"/> 可包含空值集合中的任意一个是否相等
        /// </summary>
        public static bool SequenceEqual(ReadOnlyMemory<byte> first, IEnumerable<IReadOnlyCollection<byte?>> seconds)
            => seconds.Any(second => SequenceEqual(first, second));

        /// <summary>
        /// 从流中读取文件头部字节缓冲区
        /// </summary>
        public static byte[] ReadHeaderBuffer(Stream stream, int length, bool resetPosition = true)
        {
            var currentPosition = stream.Position;
            if (currentPosition != 0) stream.Position = 0;
            byte[] result;
            if (stream.Length < length)
            {
                var buffer = new byte[stream.Length];
                stream.Read(buffer, 0, buffer.Length);
                result = buffer;
            }
            else
            {
                var buffer = new byte[length];
                stream.Read(buffer, 0, buffer.Length);
                result = buffer;
            }
            if (resetPosition) stream.Position = currentPosition;
            return result;
        }

        /// <summary>
        /// 获取幻数长度
        /// </summary>
        public static int GetLength(object magicNumber)
        {
            if (magicNumber is IReadOnlyCollection<byte> byteArray)
            {
                return byteArray.Count;
            }
            else if (magicNumber is IReadOnlyCollection<byte?> byteArrayNullable)
            {
                return byteArrayNullable.Count;
            }
            else if (magicNumber is IEnumerable<IReadOnlyCollection<byte>> byteArrayArray)
            {
                return byteArrayArray.Max(x => x.Count);
            }
            else if (magicNumber is IEnumerable<IReadOnlyCollection<byte?>> byteArrayArrayNullable)
            {
                return byteArrayArrayNullable.Max(x => x.Count);
            }
            else
            {
                throw new ArgumentOutOfRangeException(nameof(magicNumber), "type mismatch.");
            }
        }

        /// <summary>
        /// 用于判断给定的幻数是否与缓冲区或流中的数据相匹配
        /// </summary>
        /// <param name="magicNumber"></param>
        /// <param name="buffer"></param>
        /// <param name="stream"></param>
        /// <returns>如果幻数与缓冲区或流中的数据相匹配，则为 <see langword="true"/>；否则为 <see langword="false"/></returns>
        /// <exception cref="ArgumentNullException"></exception>
        public static bool Match(object magicNumber, IReadOnlyList<byte>? buffer, Stream? stream)
        {
            bool result;
            if (magicNumber is IReadOnlyCollection<byte> byteArray)
            {
                if (buffer == null)
                {
                    if (stream == null) goto null_ex;
                    buffer = ReadHeaderBuffer(stream, byteArray.Count);
                }
                result = SequenceEqual(buffer, byteArray);
            }
            else if (magicNumber is IReadOnlyCollection<byte?> byteArrayNullable)
            {
                if (buffer == null)
                {
                    if (stream == null) goto null_ex;
                    buffer = ReadHeaderBuffer(stream, byteArrayNullable.Count);
                }
                result = SequenceEqual(buffer, byteArrayNullable);
            }
            else if (magicNumber is IEnumerable<IReadOnlyCollection<byte>> byteArrayArray)
            {
                if (buffer == null)
                {
                    if (stream == null) goto null_ex;
                    buffer = ReadHeaderBuffer(stream, byteArrayArray.Max(x => x.Count));
                }
                result = SequenceEqual(buffer, byteArrayArray);
            }
            else if (magicNumber is IEnumerable<IReadOnlyCollection<byte?>> byteArrayArrayNullable)
            {
                if (buffer == null)
                {
                    if (stream == null) goto null_ex;
                    buffer = ReadHeaderBuffer(stream, byteArrayArrayNullable.Max(x => x.Count));
                }
                result = SequenceEqual(buffer, byteArrayArrayNullable);
            }
            else
            {
                result = false;
            }
            return result;
        null_ex: throw new ArgumentNullException(nameof(stream), "stream or buffer one must be non null.");
        }

        /// <summary>
        /// 判断给定的幻数是否与缓冲区或流中的数据相匹配
        /// </summary>
        /// <param name="magicNumber"></param>
        /// <param name="buffer"></param>
        /// <param name="stream"></param>
        /// <returns>如果幻数与缓冲区或流中的数据相匹配，则为 <see langword="true"/>；否则为 <see langword="false"/></returns>
        /// <exception cref="ArgumentNullException"></exception>
        public static bool Match(object magicNumber, ReadOnlyMemory<byte>? buffer, Stream? stream)
        {
            bool result;
            if (magicNumber is IReadOnlyCollection<byte> byteArray)
            {
                if (!buffer.HasValue)
                {
                    if (stream == null) goto null_ex;
                    buffer = ReadHeaderBuffer(stream, byteArray.Count);
                }
                result = SequenceEqual(buffer.Value, byteArray);
            }
            else if (magicNumber is IReadOnlyCollection<byte?> byteArrayNullable)
            {
                if (!buffer.HasValue)
                {
                    if (stream == null) goto null_ex;
                    buffer = ReadHeaderBuffer(stream, byteArrayNullable.Count);
                }
                result = SequenceEqual(buffer.Value, byteArrayNullable);
            }
            else if (magicNumber is IEnumerable<IReadOnlyCollection<byte>> byteArrayArray)
            {
                if (!buffer.HasValue)
                {
                    if (stream == null) goto null_ex;
                    buffer = ReadHeaderBuffer(stream, byteArrayArray.Max(x => x.Count));
                }
                result = SequenceEqual(buffer.Value, byteArrayArray);
            }
            else if (magicNumber is IEnumerable<IReadOnlyCollection<byte?>> byteArrayArrayNullable)
            {
                if (!buffer.HasValue)
                {
                    if (stream == null) goto null_ex;
                    buffer = ReadHeaderBuffer(stream, byteArrayArrayNullable.Max(x => x.Count));
                }
                result = SequenceEqual(buffer.Value, byteArrayArrayNullable);
            }
            else
            {
                result = false;
            }
            return result;
        null_ex: throw new ArgumentNullException(null, message: "stream or buffer one must be non null.");
        }
    }
}
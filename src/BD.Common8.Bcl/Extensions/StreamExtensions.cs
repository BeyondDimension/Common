namespace System.Extensions;

/// <summary>
/// 提供对 <see cref="Stream"/> 类型的扩展函数
/// </summary>
public static partial class StreamExtensions
{
    #region Read

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static char ReadChar(this Stream stream)
    {
        Span<byte> data = stackalloc byte[sizeof(char)];
        stream.Read(data);
        return BitConverter.ToChar(data);
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static short ReadInt16(this Stream stream)
    {
        Span<byte> data = stackalloc byte[sizeof(short)];
        stream.Read(data);
        return BitConverter.ToInt16(data);
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static int ReadInt32(this Stream stream)
    {
        Span<byte> data = stackalloc byte[sizeof(int)];
        stream.Read(data);
        return BitConverter.ToInt32(data);
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static long ReadInt64(this Stream stream)
    {
        Span<byte> data = stackalloc byte[sizeof(long)];
        stream.Read(data);
        return BitConverter.ToInt64(data);
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static ushort ReadUInt16(this Stream stream)
    {
        Span<byte> data = stackalloc byte[sizeof(ushort)];
        stream.Read(data);
        return BitConverter.ToUInt16(data);
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static uint ReadUInt32(this Stream stream)
    {
        Span<byte> data = stackalloc byte[sizeof(uint)];
        stream.Read(data);
        return BitConverter.ToUInt32(data);
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static ulong ReadUInt64(this Stream stream)
    {
        Span<byte> data = stackalloc byte[sizeof(ulong)];
        stream.Read(data);
        return BitConverter.ToUInt64(data);
    }

    /// <summary>
    /// 从流中读取一个 <see cref="byte"/>
    /// </summary>
    /// <param name="stream"></param>
    /// <returns></returns>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static byte ReadValueU8(this Stream stream)
    {
        var readByte = stream.ReadByte();
        if (readByte == -1)
            ThrowHelper.ThrowArgumentOutOfRangeException(readByte);
        return unchecked((byte)readByte);
    }

    /// <summary>
    /// 从流中读取一个 <see cref="int"/>
    /// </summary>
    /// <param name="stream"></param>
    /// <returns></returns>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static int ReadValueS32(this Stream stream)
    {
        Span<byte> data = stackalloc byte[sizeof(int)];
        stream.Read(data);
        return BitConverter.ToInt32(data);
    }

    /// <summary>
    /// 从流中读取一个 <see cref="uint"/>
    /// </summary>
    /// <param name="stream"></param>
    /// <returns></returns>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static uint ReadValueU32(this Stream stream)
    {
        Span<byte> data = stackalloc byte[sizeof(uint)];
        stream.Read(data);
        return BitConverter.ToUInt32(data);
    }

    /// <summary>
    /// 从流中读取一个 <see cref="ulong"/>
    /// </summary>
    /// <param name="stream"></param>
    /// <returns></returns>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static ulong ReadValueU64(this Stream stream)
    {
        Span<byte> data = stackalloc byte[sizeof(ulong)];
        stream.Read(data);
        return BitConverter.ToUInt64(data);
    }

    /// <summary>
    /// 从流中读取一个 <see cref="float"/>
    /// </summary>
    /// <param name="stream"></param>
    /// <returns></returns>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static float ReadValueF32(this Stream stream)
    {
        Span<byte> data = stackalloc byte[sizeof(float)];
        stream.Read(data);
        return BitConverter.ToSingle(data);
    }

    /// <summary>
    /// 从流中读取一个 <see cref="string"/>
    /// </summary>
    /// <param name="stream"></param>
    /// <param name="encoding"></param>
    /// <param name="end"></param>
    /// <returns></returns>
    internal static string ReadStringInternalDynamic(this Stream stream, Encoding encoding, char end)
    {
        int characterSize = encoding.GetByteCount("e");
        Debug.Assert(characterSize == 1 || characterSize == 2 || characterSize == 4);
        string characterEnd = end.ToString(CultureInfo.InvariantCulture);

        int i = 0;
        var data = new byte[128 * characterSize];

        while (true)
        {
            if (i + characterSize > data.Length)
            {
                Array.Resize(ref data, data.Length + (128 * characterSize));
            }

            stream.Read(data, i, characterSize);
            //Debug.Assert(read == characterSize);

            if (encoding.GetString(data, i, characterSize) == characterEnd)
            {
                break;
            }

            i += characterSize;
        }

        if (i == 0)
        {
            return "";
        }

        return encoding.GetString(data, 0, i);
    }

    /// <summary>
    /// 从流中读取一个 <see cref="string"/>，使用 <see cref="Encoding.ASCII"/> 编码
    /// </summary>
    /// <param name="stream"></param>
    /// <returns></returns>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static string ReadStringAscii(this Stream stream) => stream.ReadStringInternalDynamic(Encoding.ASCII, '\0');

    /// <summary>
    /// 从流中读取一个 <see cref="string"/>，使用 <see cref="Encoding.UTF8"/> 编码
    /// </summary>
    /// <param name="stream"></param>
    /// <returns></returns>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static string ReadStringUnicode(this Stream stream) => stream.ReadStringInternalDynamic(Encoding.UTF8, '\0');

    #endregion

    const int DefaultBufferSize = 1024;

    /// <summary>
    /// 将流转换为 <see cref="StreamWriter"/>
    /// </summary>
    /// <param name="stream"></param>
    /// <param name="encoding"></param>
    /// <param name="bufferSize"></param>
    /// <param name="leaveOpen"></param>
    /// <returns></returns>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static StreamWriter GetWriter(this Stream stream, Encoding? encoding = null, int bufferSize = -1, bool leaveOpen = false)
    {
        try
        {
            // https://docs.microsoft.com/zh-cn/dotnet/api/system.io.streamwriter.-ctor?view=net-5.0#System_IO_StreamWriter__ctor_System_IO_Stream_System_Text_Encoding_System_Int32_System_Boolean_
            // https://github.com/dotnet/corefx/blob/master/src/Common/src/CoreLib/System/IO/StreamWriter.cs#L94
            return new(stream, encoding, bufferSize, leaveOpen);
        }
        catch (Exception e) when (e is ArgumentNullException || e is ArgumentOutOfRangeException)
        {
            encoding ??= Encoding2.UTF8NoBOM;
            if (bufferSize == -1)
            {
                bufferSize = DefaultBufferSize;
            }
            return new(stream, encoding, bufferSize, leaveOpen);
        }
    }

    #region Write

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static void WriteByte(this Stream stream, int position, byte value)
    {
        var prevPosition = stream.Position;

        stream.Position = position;
        stream.WriteByte(value);
        stream.Position = prevPosition;
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static void WriteBytes(this Stream stream, int position, ReadOnlySpan<byte> buffer)
    {
        var prevPosition = stream.Position;

        stream.Position = position;
        stream.Write(buffer);
        stream.Position = prevPosition;
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static void WriteInt16(this Stream stream, short value)
    {
        Span<byte> data = stackalloc byte[sizeof(short)];
        BitConverter.TryWriteBytes(data, value);
        stream.Write(data);
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static void WriteInt16(this Stream stream, int position, short value)
    {
        Span<byte> data = stackalloc byte[sizeof(short)];
        BitConverter.TryWriteBytes(data, value);
        stream.WriteBytes(position, data);
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static void WriteInt32(this Stream stream, int value)
    {
        Span<byte> data = stackalloc byte[sizeof(int)];
        BitConverter.TryWriteBytes(data, value);
        stream.Write(data);
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static void WriteInt32(this Stream stream, int position, int value)
    {
        Span<byte> data = stackalloc byte[sizeof(int)];
        BitConverter.TryWriteBytes(data, value);
        stream.WriteBytes(position, data);
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static void WriteInt64(this Stream stream, long value)
    {
        Span<byte> data = stackalloc byte[sizeof(long)];
        BitConverter.TryWriteBytes(data, value);
        stream.Write(data);
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static void WriteInt64(this Stream stream, int position, long value)
    {
        Span<byte> data = stackalloc byte[sizeof(long)];
        BitConverter.TryWriteBytes(data, value);
        stream.WriteBytes(position, data);
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static void WriteUInt16(this Stream stream, ushort value)
    {
        Span<byte> data = stackalloc byte[sizeof(ushort)];
        BitConverter.TryWriteBytes(data, value);
        stream.Write(data);
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static void WriteUInt16(this Stream stream, int position, ushort value)
    {
        Span<byte> data = stackalloc byte[sizeof(ushort)];
        BitConverter.TryWriteBytes(data, value);
        stream.WriteBytes(position, data);
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static void WriteUInt32(this Stream stream, uint value)
    {
        Span<byte> data = stackalloc byte[sizeof(uint)];
        BitConverter.TryWriteBytes(data, value);
        stream.Write(data);
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static void WriteUInt32(this Stream stream, int position, uint value)
    {
        Span<byte> data = stackalloc byte[sizeof(uint)];
        BitConverter.TryWriteBytes(data, value);
        stream.WriteBytes(position, data);
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static void WriteUInt64(this Stream stream, ulong value)
    {
        Span<byte> data = stackalloc byte[sizeof(ulong)];
        BitConverter.TryWriteBytes(data, value);
        stream.Write(data);
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static void WriteUInt64(this Stream stream, int position, ulong value)
    {
        Span<byte> data = stackalloc byte[sizeof(ulong)];
        BitConverter.TryWriteBytes(data, value);
        stream.WriteBytes(position, data);
    }

    #endregion

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static IEnumerable<byte> ToEnumerable(this Stream stream)
        => new StreamEnumerable(stream);
}

file sealed class StreamEnumerable : IEnumerable<byte>, IEnumerator<byte>
{
    readonly Stream stream;

    public StreamEnumerable(Stream stream)
    {
        this.stream = stream;
        stream.Position = 0;
    }

    public byte Current { get; private set; }

    object IEnumerator.Current => Current;

    public void Dispose()
    {
    }

    public IEnumerator<byte> GetEnumerator() => this;

    public bool MoveNext()
    {
        var result = stream.ReadByte();
        if (result == -1)
        {
            return false;
        }
        else
        {
            Current = unchecked((byte)result);
            return true;
        }
    }

    public void Reset()
    {
        stream.Position = 0;
    }

    IEnumerator IEnumerable.GetEnumerator() => this;
}
namespace System.Extensions;

/// <summary>
/// 提供对 <see cref="Stream"/> 类型的扩展函数
/// </summary>
public static partial class StreamExtensions
{
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
        return (byte)readByte;
    }

    /// <summary>
    /// 从流中读取一个 <see cref="int"/>
    /// </summary>
    /// <param name="stream"></param>
    /// <returns></returns>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static int ReadValueS32(this Stream stream)
    {
        var data = new byte[4];
        int read = stream.Read(data, 0, 4);
        Debug.Assert(read == 4);
        return BitConverter.ToInt32(data, 0);
    }

    /// <summary>
    /// 从流中读取一个 <see cref="uint"/>
    /// </summary>
    /// <param name="stream"></param>
    /// <returns></returns>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static uint ReadValueU32(this Stream stream)
    {
        var data = new byte[4];
        int read = stream.Read(data, 0, 4);
        Debug.Assert(read == 4);
        return BitConverter.ToUInt32(data, 0);
    }

    /// <summary>
    /// 从流中读取一个 <see cref="ulong"/>
    /// </summary>
    /// <param name="stream"></param>
    /// <returns></returns>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static ulong ReadValueU64(this Stream stream)
    {
        var data = new byte[8];
        int read = stream.Read(data, 0, 8);
        Debug.Assert(read == 8);
        return BitConverter.ToUInt64(data, 0);
    }

    /// <summary>
    /// 从流中读取一个 <see cref="float"/>
    /// </summary>
    /// <param name="stream"></param>
    /// <returns></returns>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static float ReadValueF32(this Stream stream)
    {
        var data = new byte[4];
        int read = stream.Read(data, 0, 4);
        Debug.Assert(read == 4);
        return BitConverter.ToSingle(data, 0);
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

            int read = stream.Read(data, i, characterSize);
            Debug.Assert(read == characterSize);

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
}
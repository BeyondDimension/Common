namespace System.Extensions;

public static partial class StreamExtensions // Write
{
#if NETSTANDARD2_0 || NETFRAMEWORK
    /// <summary>
    /// 向当前流中写入字节序列，并将此流中的当前位置提升写入的字节数
    /// </summary>
    /// <param name="stream"></param>
    /// <param name="bytes"></param>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static void Write(this Stream stream, byte[] bytes) => stream.Write(bytes, 0, bytes.Length);

    /// <summary>
    /// 向当前流中写入字节序列，并将此流中的当前位置提升写入的字节数
    /// </summary>
    /// <param name="stream"></param>
    /// <param name="bytes"></param>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static void Write(this Stream stream, ReadOnlySpan<byte> bytes)
    {
        for (int i = 0; i < bytes.Length; i++)
        {
            stream.WriteByte(bytes[i]);
        }
    }
#endif
}
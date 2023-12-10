namespace System.Extensions;

public static partial class CookieExtensions
{
    /// <summary>
    /// 将 <see cref="CookieContainer"/> 数据写入到流中
    /// </summary>
    /// <param name="cookieContainer"></param>
    /// <param name="stream"></param>
    public static void WriteTo(this CookieContainer? cookieContainer, Stream stream)
    {
        var bufferWriter = new ArrayBufferWriter<byte>();
        using var state = MemoryPackWriterOptionalStatePool.Rent(MemoryPackSerializerOptions.Default);
        var writer = new MemoryPackWriter<ArrayBufferWriter<byte>>(ref bufferWriter, state);
        IMemoryPackFormatter<CookieContainer?> f = CookieFormatter.Default;
        f.Serialize(ref writer, ref cookieContainer);
        stream.Write(bufferWriter.WrittenSpan);
        bufferWriter.Clear();
        stream.Flush();
    }

    /// <summary>
    /// 将 <see cref="CookieContainer"/> 数据转换为二进制内容
    /// </summary>
    /// <param name="cookieContainer"></param>
    /// <returns></returns>
    public static byte[] ToByteArray(this CookieContainer? cookieContainer)
    {
        var bufferWriter = new ArrayBufferWriter<byte>();
        using var state = MemoryPackWriterOptionalStatePool.Rent(MemoryPackSerializerOptions.Default);
        var writer = new MemoryPackWriter<ArrayBufferWriter<byte>>(ref bufferWriter, state);
        IMemoryPackFormatter<CookieContainer?> f = CookieFormatter.Default;
        f.Serialize(ref writer, ref cookieContainer);
        var bytes = bufferWriter.WrittenSpan.ToArray();
        bufferWriter.Clear();
        return bytes;
    }

    /// <summary>
    /// 将 <see cref="CookieCollection"/> 数据写入到流中
    /// </summary>
    /// <param name="cookieCollection"></param>
    /// <param name="stream"></param>
    public static void WriteTo(this CookieCollection? cookieCollection, Stream stream)
    {
        var bufferWriter = new ArrayBufferWriter<byte>();
        using var state = MemoryPackWriterOptionalStatePool.Rent(MemoryPackSerializerOptions.Default);
        var writer = new MemoryPackWriter<ArrayBufferWriter<byte>>(ref bufferWriter, state);
        IMemoryPackFormatter<CookieCollection?> f = CookieFormatter.Default;
        f.Serialize(ref writer, ref cookieCollection);
        stream.Write(bufferWriter.WrittenSpan);
        bufferWriter.Clear();
        stream.Flush();
    }

    /// <summary>
    /// 将 <see cref="CookieCollection"/> 数据转换为二进制内容
    /// </summary>
    /// <param name="cookieCollection"></param>
    /// <returns></returns>
    public static byte[] ToByteArray(this CookieCollection? cookieCollection)
    {
        var bufferWriter = new ArrayBufferWriter<byte>();
        using var state = MemoryPackWriterOptionalStatePool.Rent(MemoryPackSerializerOptions.Default);
        var writer = new MemoryPackWriter<ArrayBufferWriter<byte>>(ref bufferWriter, state);
        IMemoryPackFormatter<CookieCollection?> f = CookieFormatter.Default;
        f.Serialize(ref writer, ref cookieCollection);
        var bytes = bufferWriter.WrittenSpan.ToArray();
        bufferWriter.Clear();
        return bytes;
    }

    /// <summary>
    /// 将 <see cref="Cookie"/> 数据写入到流中
    /// </summary>
    /// <param name="cookie"></param>
    /// <param name="stream"></param>
    public static void WriteTo(this Cookie? cookie, Stream stream)
    {
        var bufferWriter = new ArrayBufferWriter<byte>();
        using var state = MemoryPackWriterOptionalStatePool.Rent(MemoryPackSerializerOptions.Default);
        var writer = new MemoryPackWriter<ArrayBufferWriter<byte>>(ref bufferWriter, state);
        IMemoryPackFormatter<Cookie?> f = CookieFormatter.Default;
        f.Serialize(ref writer, ref cookie);
        stream.Write(bufferWriter.WrittenSpan);
        bufferWriter.Clear();
        stream.Flush();
    }

    /// <summary>
    /// 将 <see cref="Cookie"/> 数据转换为二进制内容
    /// </summary>
    /// <param name="cookie"></param>
    /// <returns></returns>
    public static byte[] ToByteArray(this Cookie? cookie)
    {
        var bufferWriter = new ArrayBufferWriter<byte>();
        using var state = MemoryPackWriterOptionalStatePool.Rent(MemoryPackSerializerOptions.Default);
        var writer = new MemoryPackWriter<ArrayBufferWriter<byte>>(ref bufferWriter, state);
        IMemoryPackFormatter<Cookie?> f = CookieFormatter.Default;
        f.Serialize(ref writer, ref cookie);
        var bytes = bufferWriter.WrittenSpan.ToArray();
        bufferWriter.Clear();
        return bytes;
    }
}

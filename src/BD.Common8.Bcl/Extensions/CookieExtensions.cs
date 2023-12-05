namespace System.Extensions;

public static partial class CookieExtensions
{
    /// <summary>
    /// 将 <see cref="CookieContainer"/> 数据写入到流中
    /// </summary>
    /// <param name="container"></param>
    /// <param name="stream"></param>
    public static void WriteTo(this CookieContainer? container, Stream stream)
    {
        var bufferWriter = new ArrayBufferWriter<byte>();
        using var state = MemoryPackWriterOptionalStatePool.Rent(MemoryPackSerializerOptions.Default);
        var writer = new MemoryPackWriter<ArrayBufferWriter<byte>>(ref bufferWriter, state);
        IMemoryPackFormatter<CookieContainer?> f = CookieFormatter.Default;
        f.Serialize(ref writer, ref container);
        stream.Write(bufferWriter.WrittenSpan);
        bufferWriter.Clear();
        stream.Flush();
    }

    /// <summary>
    /// 将 <see cref="CookieContainer"/> 数据转换为二进制内容
    /// </summary>
    /// <param name="container"></param>
    /// <returns></returns>
    public static byte[] ToByteArray(this CookieContainer? container)
    {
        var bufferWriter = new ArrayBufferWriter<byte>();
        using var state = MemoryPackWriterOptionalStatePool.Rent(MemoryPackSerializerOptions.Default);
        var writer = new MemoryPackWriter<ArrayBufferWriter<byte>>(ref bufferWriter, state);
        IMemoryPackFormatter<CookieContainer?> f = CookieFormatter.Default;
        f.Serialize(ref writer, ref container);
        var bytes = bufferWriter.WrittenSpan.ToArray();
        bufferWriter.Clear();
        return bytes;
    }

    /// <summary>
    /// 将 <see cref="CookieCollection"/> 数据写入到流中
    /// </summary>
    /// <param name="container"></param>
    /// <param name="stream"></param>
    public static void WriteTo(this CookieCollection? container, Stream stream)
    {
        var bufferWriter = new ArrayBufferWriter<byte>();
        using var state = MemoryPackWriterOptionalStatePool.Rent(MemoryPackSerializerOptions.Default);
        var writer = new MemoryPackWriter<ArrayBufferWriter<byte>>(ref bufferWriter, state);
        IMemoryPackFormatter<CookieCollection?> f = CookieFormatter.Default;
        f.Serialize(ref writer, ref container);
        stream.Write(bufferWriter.WrittenSpan);
        bufferWriter.Clear();
        stream.Flush();
    }

    /// <summary>
    /// 将 <see cref="CookieCollection"/> 数据转换为二进制内容
    /// </summary>
    /// <param name="container"></param>
    /// <returns></returns>
    public static byte[] ToByteArray(this CookieCollection? container)
    {
        var bufferWriter = new ArrayBufferWriter<byte>();
        using var state = MemoryPackWriterOptionalStatePool.Rent(MemoryPackSerializerOptions.Default);
        var writer = new MemoryPackWriter<ArrayBufferWriter<byte>>(ref bufferWriter, state);
        IMemoryPackFormatter<CookieCollection?> f = CookieFormatter.Default;
        f.Serialize(ref writer, ref container);
        var bytes = bufferWriter.WrittenSpan.ToArray();
        bufferWriter.Clear();
        return bytes;
    }

    /// <summary>
    /// 将 <see cref="Cookie"/> 数据写入到流中
    /// </summary>
    /// <param name="container"></param>
    /// <param name="stream"></param>
    public static void WriteTo(this Cookie? container, Stream stream)
    {
        var bufferWriter = new ArrayBufferWriter<byte>();
        using var state = MemoryPackWriterOptionalStatePool.Rent(MemoryPackSerializerOptions.Default);
        var writer = new MemoryPackWriter<ArrayBufferWriter<byte>>(ref bufferWriter, state);
        IMemoryPackFormatter<Cookie?> f = CookieFormatter.Default;
        f.Serialize(ref writer, ref container);
        stream.Write(bufferWriter.WrittenSpan);
        bufferWriter.Clear();
        stream.Flush();
    }

    /// <summary>
    /// 将 <see cref="Cookie"/> 数据转换为二进制内容
    /// </summary>
    /// <param name="container"></param>
    /// <returns></returns>
    public static byte[] ToByteArray(this Cookie? container)
    {
        var bufferWriter = new ArrayBufferWriter<byte>();
        using var state = MemoryPackWriterOptionalStatePool.Rent(MemoryPackSerializerOptions.Default);
        var writer = new MemoryPackWriter<ArrayBufferWriter<byte>>(ref bufferWriter, state);
        IMemoryPackFormatter<Cookie?> f = CookieFormatter.Default;
        f.Serialize(ref writer, ref container);
        var bytes = bufferWriter.WrittenSpan.ToArray();
        bufferWriter.Clear();
        return bytes;
    }
}

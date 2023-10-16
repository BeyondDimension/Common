namespace System.Extensions;

public static partial class StreamExtensions // Write
{
    /// <summary>
    /// 向当前流中写入字符串，默认使用 <see cref="Encoding.UTF8"/> 编码，并将此流中的当前位置提升写入的字节数
    /// </summary>
    /// <param name="stream"></param>
    /// <param name="s"></param>
    /// <param name="encoding"></param>
    [Obsolete("如果字符串为常量或模板，应使用 Write(\"\"u8) 或 WriteFormat(\"\"u8, object?[] args)，否则使用 WriteUtf16StrToUtf8OrCustom 替代此函数", true)]
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static void Write(this Stream stream, string s, Encoding? encoding = null)
    {
        encoding ??= Encoding.UTF8;
        stream.Write(encoding.GetBytes(s));
    }

    /// <summary>
    /// 向当前流中写入字符串，默认使用 <see cref="Encoding.UTF8"/> 编码，并将此流中的当前位置提升写入的字节数
    /// <para>注意：编码转换存在一定开销</para>
    /// </summary>
    /// <param name="stream"></param>
    /// <param name="s"></param>
    /// <param name="encoding"></param>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static void WriteUtf16StrToUtf8OrCustom(this Stream stream, string s, Encoding? encoding = null)
    {
        encoding ??= Encoding.UTF8;
        stream.Write(encoding.GetBytes(s));
    }

    /// <summary>
    /// 向当前流中写入字节序列，并将此流中的当前位置提升写入的字节数
    /// </summary>
    /// <param name="stream"></param>
    /// <param name="bytes"></param>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static void Write(this Stream stream, IEnumerable<byte> bytes)
    {
        foreach (byte @byte in bytes)
        {
            stream.WriteByte(@byte);
        }
    }

    /// <summary>
    /// 向当前流中写入 <see cref="object"/>，并将此流中的当前位置提升写入的字节数
    /// <para>仅支持以下类型或值</para>
    /// <list type="bullet">
    /// <item><see langword="null"/></item>
    /// <item><see cref="byte"/></item>
    /// <item>byte[]</item>
    /// <item>Memory&lt;byte&gt;</item>
    /// <item>ReadOnlyMemory&lt;byte&gt;</item>
    /// <item>IEnumerable&lt;byte&gt;</item>
    /// <item><see cref="object.ToString"/> + <see cref="Encoding.UTF8"/></item>
    /// </list>
    /// </summary>
    /// <param name="stream"></param>
    /// <param name="obj"></param>
    public static void WriteObject(this Stream stream, object? obj)
    {
        if (obj is null)
        {
        }
        else if (obj is byte[] bytes)
        {
            stream.Write(bytes);
        }
        else if (obj is byte @byte)
        {
            stream.WriteByte(@byte);
        }
        else if (obj is Memory<byte> memory2)
        {
            stream.Write(memory2.Span);
        }
        else if (obj is ReadOnlyMemory<byte> memory)
        {
            stream.Write(memory.Span);
        }
        else if (obj is IEnumerable<byte> enumerable)
        {
            stream.Write(enumerable);
        }
        else
        {
            var arg_str = obj.ToString();
            if (arg_str is not null)
            {
                bytes = Encoding.UTF8.GetBytes(arg_str);
                stream.Write(bytes);
            }
        }
    }
}
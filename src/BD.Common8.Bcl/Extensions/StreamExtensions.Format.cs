namespace System.Extensions;

public static partial class StreamExtensions // Format
{
    /// <summary>
    /// '{'
    /// </summary>
    public const byte curlyBracketLeft = 123;

    /// <summary>
    /// '}'
    /// </summary>
    public const byte curlyBracketRight = 125;

    /// <summary>
    /// '/r' CR
    /// </summary>
    public const byte newLineCR = 13;

    /// <summary>
    /// '/n' LF
    /// </summary>
    public const byte newLineLF = 10;

    /// <summary>
    /// 将 Utf8String 像 <see cref="string.Format(string, object[])"/> 一样格式化并写入流中
    /// <para>已知问题：模板字符串中不能含有 { 字符</para>
    /// </summary>
    /// <param name="stream"></param>
    /// <param name="utf8String"></param>
    /// <param name="args"></param>
    public static void WriteFormat(this Stream stream,
        ReadOnlySpan<byte> utf8String,
        params object?[] args)
    {
        if (args == null || args.Length == 0)
        {
            stream.Write(utf8String);
            return;
        }

        int index_l_brace, index_r_brace;
        index_l_brace = utf8String.IndexOf(curlyBracketLeft);
        if (index_l_brace >= 0) // 查找左大括号
        {
            var index_l_brace_add_1 = index_l_brace + 1;
            if (index_l_brace_add_1 < utf8String.Length)
            {
                if (utf8String[index_l_brace_add_1] == curlyBracketLeft) // 两个左大括号，转义不为参数
                {
                    stream.Write(utf8String[..index_l_brace]);
                    stream.WriteFormat(utf8String[(index_l_brace_add_1 + 1)..], args);
                    return;
                }
                else
                {
                    index_r_brace = utf8String[index_l_brace_add_1..].IndexOf(curlyBracketRight);
                    if (index_r_brace >= 0)
                    {
                        var args_index_bytes = utf8String.Slice(index_l_brace_add_1, index_r_brace);
                        var args_index = Encoding.UTF8.GetString(args_index_bytes
#if !(NETSTANDARD2_1_OR_GREATER || NETCOREAPP2_1_OR_GREATER)
                            .ToArray()
#endif
                            );
                        if (int.TryParse(args_index, out var args_index_int) && args_index_int >= 0 && args_index_int < args.Length)
                        {
                            stream.Write(utf8String[..index_l_brace]);
                            var arg = args[args_index_int];
                            stream.WriteObject(arg);
                            stream.WriteFormat(utf8String[(index_l_brace_add_1 + index_r_brace + 1)..], args);
                            return;
                        }
                    }
                }
            }
        }
        else
        {
            index_r_brace = utf8String.IndexOf(curlyBracketRight);
            if (index_r_brace >= 0) // 查找右大括号
            {
                var index_r_brace_add_1 = index_r_brace + 1;
                if (index_r_brace_add_1 < utf8String.Length)
                {
                    if (utf8String[index_r_brace_add_1] == curlyBracketRight) // 两个右大括号，转义不为参数
                    {
                        stream.Write(utf8String[..index_r_brace]);
                        stream.WriteFormat(utf8String[(index_r_brace_add_1 + 1)..], args);
                        return;
                    }
                }
            }
        }

        stream.Write(utf8String);
    }

    /// <summary>
    /// 向 <see cref="Stream"/> 写入换行符
    /// </summary>
    /// <param name="stream"></param>
    /// <param name="isCRLF"></param>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static void WriteNewLine(this Stream stream, bool isCRLF = true)
    {
        if (isCRLF)
            stream.WriteByte(newLineCR);
        stream.WriteByte(newLineLF);
    }

    /// <summary>
    /// 向 <see cref="Stream"/> 写入左大括号
    /// </summary>
    /// <param name="stream"></param>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static void WriteCurlyBracketLeft(this Stream stream) => stream.WriteByte(curlyBracketLeft);

    /// <summary>
    /// 向 <see cref="Stream"/> 写入右大括号
    /// </summary>
    /// <param name="stream"></param>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static void WriteCurlyBracketRight(this Stream stream) => stream.WriteByte(curlyBracketRight);

    /// <summary>
    /// 向 <see cref="Stream"/> 写入 UTF-8 BOM 头
    /// </summary>
    /// <param name="stream"></param>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static void WriteU8Preamble(this Stream stream)
    {
        stream.Write([0xEF, 0xBB, 0xBF]); // https://github.com/dotnet/runtime/blob/v8.0.6/src/libraries/System.Private.CoreLib/src/System/Text/UTF8Encoding.cs#L866
    }
}
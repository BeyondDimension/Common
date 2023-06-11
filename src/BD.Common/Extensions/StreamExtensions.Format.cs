// ReSharper disable once CheckNamespace
namespace System;

public static partial class StreamExtensions
{
    const byte l_brace = 123; // '{'
    const byte r_brace = 125; // '}'

    /// <summary>
    /// 将 Utf8String 像 <see cref="string.Format(string, object[])"/> 一样格式化并写入流中
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
        index_l_brace = utf8String.IndexOf(l_brace);
        if (index_l_brace >= 0) // 查找左大括号
        {
            var index_l_brace_add_1 = index_l_brace + 1;
            if (index_l_brace_add_1 < utf8String.Length)
            {
                if (utf8String[index_l_brace_add_1] == l_brace) // 两个左大括号，转义不为参数
                {
                    stream.Write(utf8String[..index_l_brace]);
                    stream.WriteFormat(utf8String[(index_l_brace_add_1 + 1)..], args);
                    return;
                }
                else
                {
                    index_r_brace = utf8String[index_l_brace_add_1..].IndexOf(r_brace);
                    if (index_r_brace >= 0)
                    {
                        //var index_r_brace_ = index_r_brace + index_l_brace_add_1;
                        //var index_r_brace_add_1 = index_r_brace_ + 1;
                        //if (index_r_brace_add_1 < utf8String.Length)
                        //{
                        //    if (utf8String[index_r_brace_add_1] == r_brace) // 两个右大括号，转义不为参数
                        //    {
                        //        stream.WriteByte(l_brace);
                        //        stream.Write(utf8String[..index_r_brace]);
                        //        stream.WriteByte(r_brace);
                        //        stream.WriteFormat(utf8String[(index_r_brace_add_1 + 1)..], args);
                        //        return;
                        //    }
                        //}

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
                            if (arg == null)
                            {

                            }
                            else if (arg is byte[] bytes)
                            {
                                stream.Write(bytes);
                            }
                            else if (arg is byte @byte)
                            {
                                stream.WriteByte(@byte);
                            }
                            else if (arg is ReadOnlyMemory<byte> memory)
                            {
                                stream.Write(memory.Span);
                            }
                            else if (arg is IEnumerable<byte> enumerable)
                            {
                                stream.Write(enumerable);
                            }
                            else
                            {
                                var arg_str = arg.ToString();
                                if (arg_str != null)
                                {
                                    bytes = Encoding.UTF8.GetBytes(arg_str);
                                    stream.Write(bytes);
                                }
                            }
                            stream.WriteFormat(utf8String[(index_l_brace_add_1 + index_r_brace + 1)..], args);
                            return;
                        }
                    }
                }
            }
        }
        else
        {
            index_r_brace = utf8String.IndexOf(r_brace);
            if (index_r_brace >= 0) // 查找右大括号
            {
                var index_r_brace_add_1 = index_r_brace + 1;
                if (index_r_brace_add_1 < utf8String.Length)
                {
                    if (utf8String[index_r_brace_add_1] == r_brace) // 两个右大括号，转义不为参数
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

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static void WriteNewLine(this Stream stream) =>
        stream.Write("""


            """u8);
}
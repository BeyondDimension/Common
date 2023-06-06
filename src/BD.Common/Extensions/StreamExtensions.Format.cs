// ReSharper disable once CheckNamespace
namespace System;

public static partial class StreamExtensions
{
    const byte l_brace = 123;
    const byte r_brace = 125;

    public static void WriteFormat(this Stream stream,
        ReadOnlySpan<byte> utf8String,
        params object?[] args)
    {
        var index_l_brace = utf8String.IndexOf(l_brace);
        if (index_l_brace >= 0)
        {
            var index_l_brace_add_1 = index_l_brace + 1;
            if (index_l_brace_add_1 < utf8String.Length)
            {
                if (utf8String[index_l_brace_add_1] == l_brace)
                {
                    stream.Write(utf8String[..index_l_brace_add_1]);
                    stream.WriteFormat(utf8String[index_l_brace_add_1..], args);
                    return;
                }
                else
                {

                }
            }
        }

        stream.Write(utf8String);
    }
}
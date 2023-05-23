using System.Runtime.CompilerServices;
using System.Text;

// ReSharper disable once CheckNamespace
namespace System;

public static class StreamExtensions
{
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static void Write(this Stream stream, string s)
    {
        stream.Write(Encoding.UTF8.GetBytes(s));
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static byte[] ToByteArray(this Stream stream)
    {
        if (stream is MemoryStream ms) return ms.ToArray();

        //try
        //{
        //    var len = stream.Length;
        //    var bytes = new byte[len];
        //    stream.Seek(0, SeekOrigin.Begin);
        //    stream.Read(bytes, 0, bytes.Length);
        //    stream.Seek(0, SeekOrigin.Begin);
        //    return bytes;
        //}
        //catch
        //{
        using var ms2 = new MemoryStream();
        stream.CopyTo(ms2);
        return ms2.ToArray();
        //}
    }
}
// ReSharper disable once CheckNamespace
namespace System;

public static partial class StreamExtensions
{
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static void Write(this Stream stream, string s, Encoding? encoding = null)
    {
        encoding ??= Encoding.UTF8;
        stream.Write(encoding.GetBytes(s));
    }

#if NETSTANDARD2_0
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static void Write(this Stream stream, byte[] bytes)
    {
        stream.Write(bytes, 0, bytes.Length);
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static void Write(this Stream stream, ReadOnlySpan<byte> bytes)
    {
        foreach (byte @byte in bytes)
        {
            stream.WriteByte(@byte);
        }
    }
#endif

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static void Write(this Stream stream, IEnumerable<byte> bytes)
    {
        foreach (byte @byte in bytes)
        {
            stream.WriteByte(@byte);
        }
    }
}
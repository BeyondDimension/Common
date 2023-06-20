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

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static void WriteObject(this Stream stream, object? obj)
    {
        if (obj == null)
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
            if (arg_str != null)
            {
                bytes = Encoding.UTF8.GetBytes(arg_str);
                stream.Write(bytes);
            }
        }
    }
}
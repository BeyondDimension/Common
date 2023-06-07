// ReSharper disable once CheckNamespace
namespace System;

public static partial class StreamExtensions
{
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

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static async ValueTask<byte[]> ToByteArrayAsync(this Stream stream, CancellationToken cancellationToken = default)
    {
        if (stream is MemoryStream ms)
            return ms.ToArray();

        //try
        //{
        //    var len = stream.Length;
        //    var bytes = new byte[len];
        //    stream.Seek(0, SeekOrigin.Begin);
        //    await stream.ReadAsync(bytes, 0, bytes.Length, cancellationToken);
        //    stream.Seek(0, SeekOrigin.Begin);
        //    return bytes;
        //}
        //catch
        //{
        using var ms2 = new MemoryStream();
#if NETSTANDARD2_1_OR_GREATER || NETCOREAPP2_1_OR_GREATER
        await stream.CopyToAsync(ms2, cancellationToken);
#else
        cancellationToken.ThrowIfCancellationRequested();
        await stream.CopyToAsync(ms2);
#endif
        return ms2.ToArray();
        //}
    }
}
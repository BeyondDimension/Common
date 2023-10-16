namespace System.Extensions;

public static partial class StreamExtensions // ToByteArray
{
    /// <summary>
    /// 将流内容写入字节数组，而与 <see cref="Stream.Position"/> 属性无关（调用此函数之前无需设置 Position = 0）
    /// </summary>
    /// <param name="stream"></param>
    /// <returns></returns>
    public static byte[] ToByteArray(this Stream stream)
    {
        if (stream is MemoryStream memoryStream_)
            return memoryStream_.ToArray();

        long? position = null;
        try
        {
            try
            {
                position = stream.Position;
            }
            catch
            {
            }

            if (position.HasValue && position.Value != 0)
            {
                try
                {
                    stream.Position = 0;
                }
                catch
                {
                }
            }

            using var memoryStream = new MemoryStream();
            stream.CopyTo(memoryStream);
            return memoryStream.ToArray();
        }
        finally
        {
            if (position.HasValue)
            {
                try
                {
                    stream.Position = position.Value;
                }
                catch
                {
                }
            }
        }
    }

    /// <summary>
    /// 将流内容写入字节数组，而与 <see cref="Stream.Position"/> 属性无关（调用此函数之前无需设置 Position = 0）
    /// </summary>
    /// <param name="stream"></param>
    /// <param name="cancellationToken"></param>
    /// <returns></returns>
    public static async ValueTask<byte[]> ToByteArrayAsync(this Stream stream, CancellationToken cancellationToken = default)
    {
        if (stream is MemoryStream memoryStream_)
            return memoryStream_.ToArray();

        long? position = null;
        try
        {
            try
            {
                position = stream.Position;
            }
            catch
            {
            }

            if (position.HasValue && position.Value != 0)
            {
                try
                {
                    stream.Position = 0;
                }
                catch
                {
                }
            }

            using var memoryStream = new MemoryStream();
#if NETSTANDARD2_1_OR_GREATER || NETCOREAPP2_1_OR_GREATER
            await stream.CopyToAsync(memoryStream, cancellationToken);
#else
            cancellationToken.ThrowIfCancellationRequested();
            await stream.CopyToAsync(memoryStream);
#endif
            return memoryStream.ToArray();
        }
        finally
        {
            if (position.HasValue)
            {
                try
                {
                    stream.Position = position.Value;
                }
                catch
                {
                }
            }
        }
    }
}
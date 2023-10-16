#if NETSTANDARD2_0 || NETFRAMEWORK
namespace System.Extensions;

static partial class StreamExtensions // Async | byte[] | ReadOnlyMemory<byte>
{
    /// <summary>
    /// 将字节的序列异步写入当前流，将该流中的当前位置向前移动写入的字节数，并监视取消请求。
    /// </summary>
    /// <param name="stream"></param>
    /// <param name="buffer">字节数组。</param>
    /// <param name="cancellationToken">要监视取消请求的标记。 默认值是 <see cref="CancellationToken.None"/>。</param>
    /// <returns></returns>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static async ValueTask WriteAsync(
        this Stream stream,
        byte[] buffer,
        CancellationToken cancellationToken = default)
    {
        await stream.WriteAsync(buffer, 0, buffer.Length, cancellationToken);
    }

    /// <summary>
    /// 从当前流异步读取字节的序列，将流中的位置提升读取的字节数，并监视取消请求。
    /// </summary>
    /// <param name="stream"></param>
    /// <param name="buffer">要将数据写入的字节数组。</param>
    /// <param name="cancellationToken">要监视取消请求的标记。 默认值是 <see cref="CancellationToken.None"/>。</param>
    /// <returns></returns>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static async ValueTask<int> ReadAsync(
        this Stream stream,
        byte[] buffer,
        CancellationToken cancellationToken = default)
    {
        var result = await stream.ReadAsync(buffer, 0, buffer.Length, cancellationToken);
        return result;
    }
}
#endif
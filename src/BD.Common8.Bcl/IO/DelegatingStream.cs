// https://github.com/dotnetcore/FastGithub/blob/2.1.4/FastGithub.FlowAnalyze/DelegatingStream.cs
// https://referencesource.microsoft.com/#System.ServiceModel/System/ServiceModel/Channels/DelegatingStream.cs

namespace System.IO;

/// <summary>
/// 用于对 Stream 类进行扩展和包装
/// </summary>
public abstract class DelegatingStream(Stream inner) : Stream
{
    /// <summary>
    /// 内部流对象
    /// </summary>
    protected Stream Inner { get; } = inner;

    /// <summary>
    /// 是否可读
    /// </summary>
    public override bool CanRead => Inner.CanRead;

    /// <summary>
    /// 是否可查找
    /// </summary>
    public override bool CanSeek => Inner.CanSeek;

    /// <summary>
    /// 是否可写
    /// </summary>
    public override bool CanWrite => Inner.CanWrite;

    /// <summary>
    /// 流的长度
    /// </summary>
    public override long Length => Inner.Length;

    /// <summary>
    /// 当前位置
    /// </summary>
    public override long Position
    {
        get => Inner.Position;
        set => Inner.Position = value;
    }

    /// <summary>
    /// 刷新流
    /// </summary>
    public override void Flush() => Inner.Flush();

    /// <summary>
    /// 异步刷新流
    /// </summary>
    /// <returns></returns>
    public override Task FlushAsync(CancellationToken cancellationToken) => Inner.FlushAsync(cancellationToken);

    /// <summary>
    /// 读取指定数量的字节到缓冲区中
    /// </summary>
    public override int Read(byte[] buffer, int offset, int count) => Inner.Read(buffer, offset, count);

#if NETCOREAPP || NETSTANDARD2_1_OR_GREATER
    /// <summary>
    /// 读取数据到指定的内存区域
    /// </summary>
    public override int Read(Span<byte> destination) => Inner.Read(destination);
#endif

    /// <summary>
    /// 异步读取指定数量的字节到缓冲区中
    /// </summary>
    public override Task<int> ReadAsync(byte[] buffer, int offset, int count, CancellationToken cancellationToken) => Inner.ReadAsync(buffer, offset, count, cancellationToken);

#if NETCOREAPP || NETSTANDARD2_1_OR_GREATER
    /// <summary>
    /// 异步读取数据到指定的内存区域
    /// </summary>
    public override ValueTask<int> ReadAsync(Memory<byte> destination, CancellationToken cancellationToken = default) => Inner.ReadAsync(destination, cancellationToken);
#endif

    /// <summary>
    /// 定位到指定的偏移位置
    /// </summary>
    public override long Seek(long offset, SeekOrigin origin) => Inner.Seek(offset, origin);

    /// <summary>
    /// 设置流的长度
    /// </summary>
    public override void SetLength(long value) => Inner.SetLength(value);

    /// <summary>
    /// 将指定数量的字节从缓冲区写入到流中
    /// </summary>
    public override void Write(byte[] buffer, int offset, int count) => Inner.Write(buffer, offset, count);

#if NETCOREAPP || NETSTANDARD2_1_OR_GREATER
    /// <summary>
    /// 写入指定的内存区域数据
    /// </summary>
    public override void Write(ReadOnlySpan<byte> source) => Inner.Write(source);
#endif

    /// <summary>
    /// 异步将指定数量的字节从缓冲区写入到流中
    /// </summary>
    public override Task WriteAsync(byte[] buffer, int offset, int count, CancellationToken cancellationToken) => Inner.WriteAsync(buffer, offset, count, cancellationToken);

#if NETCOREAPP || NETSTANDARD2_1_OR_GREATER
    /// <summary>
    /// 异步写入指定的内存区域数据
    /// </summary>
    public override ValueTask WriteAsync(ReadOnlyMemory<byte> source, CancellationToken cancellationToken = default) => Inner.WriteAsync(source, cancellationToken);
#endif

    /// <summary>
    /// 开始异步读取操作，将字节读取到缓冲区中
    /// </summary>
    public override IAsyncResult BeginRead(byte[] buffer, int offset, int count, AsyncCallback? callback, object? state) => TaskToApm.Begin(ReadAsync(buffer, offset, count), callback, state);

    /// <summary>
    /// 结束异步读取操作，并返回字节读取的数量
    /// </summary>
    public override int EndRead(IAsyncResult asyncResult) => TaskToApm.End<int>(asyncResult);

    /// <summary>
    /// 开始异步写入操作，将缓冲区中的字节写入到流中
    /// </summary>
    public override IAsyncResult BeginWrite(byte[] buffer, int offset, int count, AsyncCallback? callback, object? state) => TaskToApm.Begin(WriteAsync(buffer, offset, count), callback, state);

    /// <summary>
    /// 结束异步写入操作
    /// </summary>
    public override void EndWrite(IAsyncResult asyncResult) => TaskToApm.End(asyncResult);
}
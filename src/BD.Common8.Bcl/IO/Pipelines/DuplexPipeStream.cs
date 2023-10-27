// https://github.com/dotnetcore/FastGithub/blob/2.1.4/FastGithub.FlowAnalyze/DuplexPipeStreamExtensions.cs

#if NET6_0_OR_GREATER
namespace System.IO.Pipelines;

/// <summary>
/// 使用双工管道 <see cref="IDuplexPipe"/> 实现流的类
/// </summary>
public class DuplexPipeStream(IDuplexPipe duplexPipe, bool throwOnCancelled = false) : Stream
{
    /// <summary>
    /// 输入管道的读取器
    /// </summary>
    readonly PipeReader input = duplexPipe.Input;

    /// <summary>
    /// 输出管道的写入器
    /// </summary>
    readonly PipeWriter output = duplexPipe.Output;

    /// <summary>
    /// 取消操作时是否抛出异常
    /// </summary>
    readonly bool throwOnCancelled = throwOnCancelled;

    /// <summary>
    /// 是否取消调用
    /// </summary>
    volatile bool cancelCalled;

    /// <summary>
    /// 取消当前待处理的读取操作
    /// </summary>
    public void CancelPendingRead()
    {
        cancelCalled = true;
        input.CancelPendingRead();
    }

    /// <summary>
    /// 是可读取数据，始终返回 <see langword="true"/>
    /// </summary>
    public override bool CanRead => true;

    /// <summary>
    /// 是否可读取数据，始终返回 <see langword="false"/>
    /// </summary>
    public override bool CanSeek => false;

    /// <summary>
    /// 是否可查找，始终返回 <see langword="true"/>
    /// </summary>
    public override bool CanWrite => true;

    /// <summary>
    /// 不支持获取流的长度，抛 <see cref="NotSupportedException"/> 异常
    /// </summary>
    public override long Length
    {
        get
        {
            throw new NotSupportedException();
        }
    }

    /// <summary>
    ///  不支持获取或设置当前流中的位置，抛 <see cref="NotSupportedException"/> 异常
    /// </summary>
    public override long Position
    {
        get
        {
            throw new NotSupportedException();
        }

        set
        {
            throw new NotSupportedException();
        }
    }

    /// <summary>
    /// 不支持设置当前流中的位置
    /// </summary>
    /// <exception cref="NotSupportedException"></exception>
    public override long Seek(long offset, SeekOrigin origin)
    {
        throw new NotSupportedException();
    }

    /// <summary>
    /// 不支持设置当前流的长度
    /// </summary>
    /// <exception cref="NotSupportedException"></exception>
    public override void SetLength(long value)
    {
        throw new NotSupportedException();
    }

    /// <summary>
    /// 读取指定数量的字节到缓冲区中
    /// </summary>
    public override int Read(byte[] buffer, int offset, int count)
    {
        var vt = ReadAsyncInternal(new Memory<byte>(buffer, offset, count), default);
        return vt.IsCompleted ?
            vt.Result :
            vt.AsTask().GetAwaiter().GetResult();
    }

    /// <summary>
    /// 异步从流中读取指定数量的字节到缓冲区中
    /// </summary>
    public override Task<int> ReadAsync(byte[] buffer, int offset, int count, CancellationToken cancellationToken = default)
    {
        return ReadAsyncInternal(new Memory<byte>(buffer, offset, count), cancellationToken).AsTask();
    }

    /// <summary>
    /// 异步从流中读取字节到内存中
    /// </summary>
    public override ValueTask<int> ReadAsync(Memory<byte> destination, CancellationToken cancellationToken = default)
    {
        return ReadAsyncInternal(destination, cancellationToken);
    }

    /// <summary>
    /// 将指定数量的字节从缓冲区写入到流中
    /// </summary>
    public override void Write(byte[] buffer, int offset, int count)
    {
        WriteAsync(buffer, offset, count).GetAwaiter().GetResult();
    }

    /// <summary>
    /// 异步从指定数量的字节从缓冲区写入到流中
    /// </summary>
    public override async Task WriteAsync(byte[]? buffer, int offset, int count, CancellationToken cancellationToken)
    {
        await output.WriteAsync(buffer.AsMemory(offset, count), cancellationToken);
    }

    /// <summary>
    /// 异步将字节从只读内存中写入到流中
    /// </summary>
    public override async ValueTask WriteAsync(ReadOnlyMemory<byte> source, CancellationToken cancellationToken = default)
    {
        await output.WriteAsync(source, cancellationToken);
    }

    /// <summary>
    /// 刷新缓冲数据
    /// </summary>
    public override void Flush()
    {
        FlushAsync(CancellationToken.None).GetAwaiter().GetResult();
    }

    /// <summary>
    /// 异步刷新缓冲数据
    /// </summary>
    public override async Task FlushAsync(CancellationToken cancellationToken)
    {
        await output.FlushAsync(cancellationToken);
    }

    [AsyncMethodBuilder(typeof(PoolingAsyncValueTaskMethodBuilder<>))]
    private async ValueTask<int> ReadAsyncInternal(Memory<byte> destination, CancellationToken cancellationToken)
    {
        while (true)
        {
            var result = await input.ReadAsync(cancellationToken);
            var readableBuffer = result.Buffer;
            try
            {
                if (throwOnCancelled && result.IsCanceled && cancelCalled)
                {
                    // Reset the bool
                    cancelCalled = false;
                    throw new OperationCanceledException();
                }

                if (!readableBuffer.IsEmpty)
                {
                    // buffer.Count is int
                    var count = (int)Math.Min(readableBuffer.Length, destination.Length);
                    readableBuffer = readableBuffer.Slice(0, count);
                    readableBuffer.CopyTo(destination.Span);
                    return count;
                }

                if (result.IsCompleted)
                    return 0;
            }
            finally
            {
                input.AdvanceTo(readableBuffer.End, readableBuffer.End);
            }
        }
    }

    /// <summary>
    /// 开始异步读取操作，将字节读取到缓冲区中
    /// </summary>
    public override IAsyncResult BeginRead(byte[] buffer, int offset, int count, AsyncCallback? callback, object? state)
    {
        return TaskToApm.Begin(ReadAsync(buffer, offset, count), callback, state);
    }

    /// <summary>
    /// 结束异步读取操作，并返回字节读取的数量
    /// </summary>
    public override int EndRead(IAsyncResult asyncResult)
    {
        return TaskToApm.End<int>(asyncResult);
    }

    /// <summary>
    /// 开始异步写入操作，将缓冲区中的字节写入到流中
    /// </summary>
    public override IAsyncResult BeginWrite(byte[] buffer, int offset, int count, AsyncCallback? callback, object? state)
    {
        return TaskToApm.Begin(WriteAsync(buffer, offset, count), callback, state);
    }

    /// <summary>
    /// 结束异步写入操作
    /// </summary>
    public override void EndWrite(IAsyncResult asyncResult)
    {
        TaskToApm.End(asyncResult);
    }
}
#endif
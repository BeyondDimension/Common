// https://github.com/dotnetcore/FastGithub/blob/2.1.4/FastGithub.FlowAnalyze/DelegatingDuplexPipe.cs

#if NET6_0_OR_GREATER
namespace System.IO.Pipelines;

/// <summary>
/// 提供对双工管道的委托方法的封装
/// </summary>
/// <typeparam name="TDelegatingStream">委托流的类型参数</typeparam>
public class DelegatingDuplexPipe<TDelegatingStream> : IDuplexPipe, IAsyncDisposable where TDelegatingStream : DelegatingStream
{
    bool disposed;
    readonly object syncRoot = new();

    /// <summary>
    /// 获取用于从管道中读取数据的 <see cref="PipeReader"/> 对象
    /// </summary>
    public PipeReader Input { get; }

    /// <summary>
    /// 获取用于向管道中写入数据的 <see cref="PipeWriter"/> 对象
    /// </summary>
    public PipeWriter Output { get; }

    /// <summary>
    /// 使用给定的双工管道和委托流函数构造实例
    /// </summary>
    public DelegatingDuplexPipe(IDuplexPipe duplexPipe, Func<Stream, TDelegatingStream> delegatingStreamFactory) : this(duplexPipe, new StreamPipeReaderOptions(leaveOpen: true), new StreamPipeWriterOptions(leaveOpen: true), delegatingStreamFactory)
    {
    }

    /// <summary>
    /// 使用给定的双工管道，读取选项，写入选项和委托流函数构造实例
    /// </summary>
    public DelegatingDuplexPipe(IDuplexPipe duplexPipe, StreamPipeReaderOptions readerOptions, StreamPipeWriterOptions writerOptions, Func<Stream, TDelegatingStream> delegatingStreamFactory)
    {
        var delegatingStream = delegatingStreamFactory(duplexPipe.AsStream());
        Input = PipeReader.Create(delegatingStream, readerOptions);
        Output = PipeWriter.Create(delegatingStream, writerOptions);
    }

    /// <summary>
    /// 异步释放资源
    /// </summary>
    public virtual async ValueTask DisposeAsync()
    {
        lock (syncRoot)
        {
            if (disposed == true)
            {
                return;
            }
            disposed = true;
        }

        await Input.CompleteAsync();
        await Output.CompleteAsync();

        GC.SuppressFinalize(this);
    }
}
#endif
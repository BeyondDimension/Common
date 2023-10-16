// https://github.com/dotnetcore/FastGithub/blob/2.1.4/FastGithub.FlowAnalyze/DelegatingStream.cs
// https://referencesource.microsoft.com/#System.ServiceModel/System/ServiceModel/Channels/DelegatingStream.cs

namespace System.IO;

#pragma warning disable SA1600 // Elements should be documented

public abstract class DelegatingStream(Stream inner) : Stream
{
    protected Stream Inner { get; } = inner;

    public override bool CanRead => Inner.CanRead;

    public override bool CanSeek => Inner.CanSeek;

    public override bool CanWrite => Inner.CanWrite;

    public override long Length => Inner.Length;

    public override long Position
    {
        get => Inner.Position;
        set => Inner.Position = value;
    }

    public override void Flush() => Inner.Flush();

    public override Task FlushAsync(CancellationToken cancellationToken) => Inner.FlushAsync(cancellationToken);

    public override int Read(byte[] buffer, int offset, int count) => Inner.Read(buffer, offset, count);

#if NETCOREAPP || NETSTANDARD2_1_OR_GREATER
    public override int Read(Span<byte> destination) => Inner.Read(destination);
#endif

    public override Task<int> ReadAsync(byte[] buffer, int offset, int count, CancellationToken cancellationToken) => Inner.ReadAsync(buffer, offset, count, cancellationToken);

#if NETCOREAPP || NETSTANDARD2_1_OR_GREATER
    public override ValueTask<int> ReadAsync(Memory<byte> destination, CancellationToken cancellationToken = default) => Inner.ReadAsync(destination, cancellationToken);
#endif

    public override long Seek(long offset, SeekOrigin origin) => Inner.Seek(offset, origin);

    public override void SetLength(long value) => Inner.SetLength(value);

    public override void Write(byte[] buffer, int offset, int count) => Inner.Write(buffer, offset, count);

#if NETCOREAPP || NETSTANDARD2_1_OR_GREATER
    public override void Write(ReadOnlySpan<byte> source) => Inner.Write(source);
#endif

    public override Task WriteAsync(byte[] buffer, int offset, int count, CancellationToken cancellationToken) => Inner.WriteAsync(buffer, offset, count, cancellationToken);

#if NETCOREAPP || NETSTANDARD2_1_OR_GREATER
    public override ValueTask WriteAsync(ReadOnlyMemory<byte> source, CancellationToken cancellationToken = default) => Inner.WriteAsync(source, cancellationToken);
#endif

    public override IAsyncResult BeginRead(byte[] buffer, int offset, int count, AsyncCallback? callback, object? state) => TaskToApm.Begin(ReadAsync(buffer, offset, count), callback, state);

    public override int EndRead(IAsyncResult asyncResult) => TaskToApm.End<int>(asyncResult);

    public override IAsyncResult BeginWrite(byte[] buffer, int offset, int count, AsyncCallback? callback, object? state) => TaskToApm.Begin(WriteAsync(buffer, offset, count), callback, state);

    public override void EndWrite(IAsyncResult asyncResult) => TaskToApm.End(asyncResult);
}
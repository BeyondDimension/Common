// ReSharper disable once CheckNamespace
namespace System.IO.Pipelines;

public static class DuplexPipeStreamExtensions
{
    public static Stream AsStream(this IDuplexPipe duplexPipe, bool throwOnCancelled = false)
        => new DuplexPipeStream(duplexPipe, throwOnCancelled);
}
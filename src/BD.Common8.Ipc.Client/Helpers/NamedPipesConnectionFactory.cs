namespace BD.Common8.Ipc.Client.Helpers;
#pragma warning disable SA1600 // Elements should be documented
#pragma warning disable SA1313 // Parameter names should begin with lower-case letter

/// <summary>
/// https://learn.microsoft.com/zh-cn/aspnet/core/grpc/interprocess-namedpipes?view=aspnetcore-8.0#client-configuration
/// </summary>
/// <param name="pipeName"></param>
/// <param name="serverName"></param>
sealed class NamedPipesConnectionFactory(string pipeName, string serverName = ".")
{
    public async ValueTask<Stream> ConnectAsync(SocketsHttpConnectionContext _,
        CancellationToken cancellationToken = default)
    {
        var clientStream = new NamedPipeClientStream(
            serverName: serverName,
            pipeName: pipeName,
            direction: PipeDirection.InOut,
            options: PipeOptions.WriteThrough | PipeOptions.Asynchronous,
            impersonationLevel: TokenImpersonationLevel.Anonymous);

        try
        {
            await clientStream.ConnectAsync(cancellationToken).ConfigureAwait(false);
            return clientStream;
        }
        catch
        {
            clientStream.Dispose();
            throw;
        }
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static Func<SocketsHttpConnectionContext, CancellationToken, ValueTask<Stream>> GetConnectCallback(string pipeName, string serverName = ".")
    {
        var connectionFactory = new NamedPipesConnectionFactory(pipeName, serverName);
        return connectionFactory.ConnectAsync;
    }
}
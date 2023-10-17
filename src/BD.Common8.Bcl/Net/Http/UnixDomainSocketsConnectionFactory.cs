namespace System.Net.Http;

#pragma warning disable SA1600 // Elements should be documented
#pragma warning disable SA1313 // Parameter names should begin with lower-case letter

/// <summary>
/// https://learn.microsoft.com/zh-cn/aspnet/core/grpc/interprocess-uds?view=aspnetcore-8.0#client-configuration
/// </summary>
/// <param name="endPoint"></param>
public sealed class UnixDomainSocketsConnectionFactory(EndPoint endPoint)
{
    public async ValueTask<Stream> ConnectAsync(SocketsHttpConnectionContext _,
        CancellationToken cancellationToken = default)
    {
        var socket = new Socket(AddressFamily.Unix, SocketType.Stream, ProtocolType.Unspecified);

        try
        {
            await socket.ConnectAsync(endPoint, cancellationToken).ConfigureAwait(false);
            return new NetworkStream(socket, true);
        }
        catch
        {
            socket.Dispose();
            throw;
        }
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static Func<SocketsHttpConnectionContext, CancellationToken, ValueTask<Stream>> GetConnectCallback(string socketPath)
    {
        var udsEndPoint = new UnixDomainSocketEndPoint(socketPath);
        var connectionFactory = new UnixDomainSocketsConnectionFactory(udsEndPoint);
        return connectionFactory.ConnectAsync;
    }
}
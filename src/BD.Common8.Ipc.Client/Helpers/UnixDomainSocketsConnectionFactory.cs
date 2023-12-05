namespace BD.Common8.Ipc.Helpers;

#pragma warning disable SA1313 // Parameter names should begin with lower-case letter

/// <summary>
/// 提供了对 Unix 域套接字连接的封装
/// <para>https://learn.microsoft.com/zh-cn/aspnet/core/grpc/interprocess-uds?view=aspnetcore-8.0#client-configuration</para>
/// </summary>
/// <param name="endPoint"></param>
sealed class UnixDomainSocketsConnectionFactory(EndPoint endPoint)
{
    /// <summary>
    /// 异步连接到 Unix 域套接字，并返回一个网络流 Stream 对象
    /// </summary>
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

    /// <summary>
    /// 用于获取连接回调函数
    /// </summary>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static Func<SocketsHttpConnectionContext, CancellationToken, ValueTask<Stream>> GetConnectCallback(string socketPath)
    {
        var udsEndPoint = new UnixDomainSocketEndPoint(socketPath);
        var connectionFactory = new UnixDomainSocketsConnectionFactory(udsEndPoint);
        return connectionFactory.ConnectAsync;
    }
}
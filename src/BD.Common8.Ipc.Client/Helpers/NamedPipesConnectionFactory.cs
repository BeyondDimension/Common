namespace BD.Common8.Ipc.Client.Helpers;
#pragma warning disable SA1313 // Parameter names should begin with lower-case letter

/// <summary>
/// 提供通过命名管道连接到服务器的功能
/// https://learn.microsoft.com/zh-cn/aspnet/core/grpc/interprocess-namedpipes?view=aspnetcore-8.0#client-configuration
/// </summary>
sealed class NamedPipesConnectionFactory(string pipeName, string serverName = ".")
{
    /// <summary>
    /// 使用指定的命名管道名称和服务器名称作为参数，异步连接到命名管道，并返回命名管道的流
    /// </summary>
    /// <param name="_"></param>
    /// <param name="cancellationToken"></param>
    /// <returns></returns>
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

    /// <summary>
    /// 获取连接到命名管道的回调函数
    /// </summary>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static Func<SocketsHttpConnectionContext, CancellationToken, ValueTask<Stream>> GetConnectCallback(string pipeName, string serverName = ".")
    {
        var connectionFactory = new NamedPipesConnectionFactory(pipeName, serverName);
        return connectionFactory.ConnectAsync;
    }
}
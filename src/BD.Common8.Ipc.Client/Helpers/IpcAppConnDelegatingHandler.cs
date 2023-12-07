using System.Security.AccessControl;

namespace BD.Common8.Ipc.Helpers;

#pragma warning disable SA1600 // Elements should be documented

public sealed class IpcAppConnDelegatingHandler : DelegatingHandler
{
    volatile bool _disposed;

    public bool Disposed => _disposed;

    readonly ConcurrentDictionary<int, IDisposable>? disposables;
    readonly IpcAppConnectionStringType connectionStringType;
    readonly UnixDomainSocketEndPoint? udsEndPoint;
    readonly string? pipeName;

    public new SocketsHttpHandler InnerHandler { get; }

    public string BaseAddress { get; } = "https://localhost";

    public IpcAppConnDelegatingHandler(
        IpcAppConnectionString connectionString,
        SocketsHttpHandler innerHandler)
    {
        connectionStringType = connectionString.Type;
        switch (connectionStringType)
        {
            case IpcAppConnectionStringType.Https:
                BaseAddress = $"https://localhost:{connectionString.Int32Value}";
                break;
            case IpcAppConnectionStringType.UnixSocket:
                disposables = [];
                var socketPath = connectionString.StringValue;
                udsEndPoint = new UnixDomainSocketEndPoint(socketPath.ThrowIsNull());
                innerHandler.ConnectCallback = UnixSocketConnectAsync;
                break;
            case IpcAppConnectionStringType.NamedPipe:
                disposables = [];
                pipeName = connectionString.StringValue;
                innerHandler.ConnectCallback = NamedPipeConnectAsync;
                break;
            default:
                throw ThrowHelper.GetArgumentOutOfRangeException(connectionStringType);
        }
        InnerHandler = innerHandler;
        base.InnerHandler = innerHandler;
    }

    /// <summary>
    /// 提供了对 Unix 域套接字连接的封装
    /// <para>https://learn.microsoft.com/zh-cn/aspnet/core/grpc/interprocess-uds?view=aspnetcore-8.0#client-configuration</para>
    /// <para>异步连接到 Unix 域套接字，并返回一个网络流 Stream 对象</para>
    /// </summary>
    /// <param name="context"></param>
    /// <param name="cancellationToken"></param>
    /// <returns></returns>
    async ValueTask<Stream> UnixSocketConnectAsync(
        SocketsHttpConnectionContext context,
        CancellationToken cancellationToken = default)
    {
        var socket = new Socket(AddressFamily.Unix, SocketType.Stream, ProtocolType.Unspecified);

        try
        {
            disposables.ThrowIsNull().TryAdd(socket.GetHashCode(), socket);
#if DEBUG
            Console.WriteLine($"正在连接到 UnixSocket，udsEndPoint：{udsEndPoint}");
#endif
            await socket.ConnectAsync(udsEndPoint.ThrowIsNull(), cancellationToken).ConfigureAwait(false);
            return new NetworkStream(socket, true);
        }
#if DEBUG
        catch (Exception e)
#else
        catch
#endif
        {
#if DEBUG
            //Console.WriteLine($"连接到 UnixSocket 出现错误，udsEndPoint：{udsEndPoint}，e：{e}");
#endif
            disposables.ThrowIsNull().TryRemove(socket.GetHashCode(), out var _);
            socket.Dispose();
            throw;
        }
    }

    /// <summary>
    /// 提供通过命名管道连接到服务器的功能
    /// <para>https://learn.microsoft.com/zh-cn/aspnet/core/grpc/interprocess-namedpipes?view=aspnetcore-8.0#client-configuration</para>
    /// <para>使用指定的命名管道名称和服务器名称作为参数，异步连接到命名管道，并返回命名管道的流</para>
    /// </summary>
    /// <param name="context"></param>
    /// <param name="cancellationToken"></param>
    /// <returns></returns>
    async ValueTask<Stream> NamedPipeConnectAsync(
        SocketsHttpConnectionContext context,
        CancellationToken cancellationToken = default)
    {
        var clientStream = new NamedPipeClientStream(
            serverName: ".",
            pipeName: pipeName.ThrowIsNull(),
            direction: PipeDirection.InOut,
            options: PipeOptions.WriteThrough | PipeOptions.Asynchronous,
            impersonationLevel: TokenImpersonationLevel.Anonymous);

        try
        {
            disposables.ThrowIsNull().TryAdd(clientStream.GetHashCode(), clientStream);
#if DEBUG
            Console.WriteLine($"正在连接到 NamedPipeClientStream，pipeName：{pipeName}");
#endif
            await clientStream.ConnectAsync(cancellationToken).ConfigureAwait(false);
            return clientStream;
        }
#if DEBUG
        catch (Exception e)
#else
        catch
#endif
        {
#if DEBUG
            //Console.WriteLine($"连接到 NamedPipeClientStream 出现错误，pipeName：{pipeName}，e：{e}");
#endif
            disposables.ThrowIsNull().TryRemove(clientStream.GetHashCode(), out var _);
            clientStream.Dispose();
            throw;
        }
    }

    protected override void Dispose(bool disposing)
    {
        if (disposing && !_disposed)
        {
            _disposed = true;
            if (disposables != null)
            {
                foreach (var item in disposables.Values.ToArray())
                {
                    item?.Dispose();
                }
                disposables.Clear();
            }
        }
        base.Dispose(disposing);
    }
}
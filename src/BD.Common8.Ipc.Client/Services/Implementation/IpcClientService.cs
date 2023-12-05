using HttpTransportType = Microsoft.AspNetCore.Http.Connections.HttpTransportType;

namespace BD.Common8.Ipc.Services.Implementation;

#pragma warning disable SA1600 // Elements should be documented

/// <summary>
/// Ipc 客户端连接服务实现
/// </summary>
/// <param name="connectionString"></param>
public class IpcClientService(IpcAppConnectionString connectionString) :
    WebApiClientService(Log.Factory.CreateLogger<IpcClientService>(), null!),
    IIpcClientService, IAsyncDisposable
{
    SocketsHttpHandler? httpHandler;
    HttpClient? httpClient;
    SocketsHttpHandler? hubConnHandler;
    HubConnection? hubConnection;

    /// <inheritdoc cref="IpcAppConnectionString"/>
    protected readonly IpcAppConnectionString connectionString = connectionString;

    protected sealed override HttpClient CreateClient()
    {
        if (httpClient == null)
        {
            (httpClient, httpHandler) = IpcAppConnectionStringHelper.GetHttpClient(connectionString);
            ConfigureSocketsHttpHandler(httpHandler);
        }
        return httpClient;
    }

    /// <summary>
    /// 异步获取 SignalR Hub 连接
    /// </summary>
    /// <returns></returns>
    protected async ValueTask<HubConnection> GetHubConnAsync()
    {
        if (hubConnection != null)
            return hubConnection;
        var @lock = new AsyncExclusiveLock();
        using (await @lock.AcquireLockAsync(CancellationToken.None))
        {
            (var baseAddress, hubConnHandler) =
                IpcAppConnectionStringHelper.GetHttpMessageHandler(connectionString);
            ConfigureSocketsHttpHandler(hubConnHandler);

            var builder = new HubConnectionBuilder()
                .WithUrl(baseAddress, opt =>
                {
                    opt.Transports = HttpTransportType.WebSockets;
                    opt.HttpMessageHandlerFactory = (oldHandler) =>
                    {
                        oldHandler.Dispose(); // 传过来的 Handler 丢弃，使用根据连接字符串解析的
                        return hubConnHandler;
                    };
                    opt.WebSocketConfiguration = static o =>
                    {
                        o.HttpVersion = HttpVersion.Version20;
                    };
                    opt.Url = new Uri($"{baseAddress}/{IpcAppConnectionStringHelper.HubName}");
                })
                .WithAutomaticReconnect();

            hubConnection = builder.Build();
            hubConnection.Reconnected += HubConnection_Reconnected;
            hubConnection.Reconnecting += HubConnection_Reconnecting;

            using var cts = new CancellationTokenSource();
            cts.CancelAfter(TimeSpan.FromSeconds(IpcAppConnectionStringHelper.TimeoutFromSeconds));
            await hubConnection.StartAsync(cts.Token);
        }
        return hubConnection;
    }

    /// <summary>
    /// 配置 <see cref="SocketsHttpHandler"/>
    /// </summary>
    /// <param name="handler"></param>
    protected virtual void ConfigureSocketsHttpHandler(SocketsHttpHandler handler)
    {
    }

    /// <inheritdoc cref="HubConnection.Reconnecting"/>
    protected virtual Task HubConnection_Reconnecting(Exception? arg)
    {
        return Task.CompletedTask;
    }

    /// <inheritdoc cref="HubConnection.Reconnected"/>
    protected virtual Task HubConnection_Reconnected(string? arg)
    {
        return Task.CompletedTask;
    }

    public async ValueTask DisposeAsync()
    {
        await DisposeAsyncCore().ConfigureAwait(false);
        GC.SuppressFinalize(this);
    }

    protected virtual async ValueTask DisposeAsyncCore()
    {
        hubConnHandler?.Dispose();
        httpClient?.Dispose();
        if (hubConnection is not null)
        {
            await hubConnection.DisposeAsync().ConfigureAwait(false);
        }

        hubConnHandler = null;
        httpClient = null;
        hubConnection = null;
    }
}

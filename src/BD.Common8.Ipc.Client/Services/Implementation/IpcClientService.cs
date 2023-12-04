using DotNext.Threading;
using Microsoft.AspNetCore.SignalR.Client;
using HttpTransportType = Microsoft.AspNetCore.Http.Connections.HttpTransportType;

namespace BD.Common8.Ipc.Client.Services.Implementation;

#pragma warning disable SA1600 // Elements should be documented

/// <summary>
/// Ipc 客户端连接服务实现
/// </summary>
/// <param name="connectionString"></param>
public class IpcClientService(IpcAppConnectionString connectionString) : IIpcClientService, IAsyncDisposable
{
    /// <inheritdoc cref="ILogger"/>
    protected readonly ILogger logger = Log.Factory.CreateLogger<IpcClientService>();

    /// <inheritdoc/>
    ILogger Log.I.Logger => logger;

    HttpClient? httpClient;
    HttpMessageHandler? hubConnHandler;
    HubConnection? hubConnection;

    /// <inheritdoc cref="IpcAppConnectionString"/>
    protected readonly IpcAppConnectionString connectionString = connectionString;

    /// <inheritdoc/>
    public HttpClient HttpClient
        => httpClient ??= IpcAppConnectionStringHelper.GetHttpClient(connectionString);

    /// <inheritdoc/>
    public async ValueTask<HubConnection> GetHubConnAsync()
    {
        if (hubConnection != null)
            return hubConnection;
        var @lock = new AsyncExclusiveLock();
        using (await @lock.AcquireLockAsync(CancellationToken.None))
        {
            (string baseAddress, HttpMessageHandler handler) = IpcAppConnectionStringHelper.GetHttpMessageHandler(connectionString);
            hubConnHandler = handler;

            var builder = new HubConnectionBuilder()
                .WithUrl(baseAddress, opt =>
                {
                    opt.Transports = HttpTransportType.WebSockets;
                    opt.HttpMessageHandlerFactory = (oldHandler) =>
                    {
                        oldHandler.Dispose(); // 传过来的 Handler 丢弃，使用根据连接字符串解析的
                        return handler;
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

            var cts = new CancellationTokenSource();
            cts.CancelAfter(TimeSpan.FromSeconds(IpcAppConnectionStringHelper.TimeoutFromSeconds));
            await hubConnection.StartAsync(cts.Token);
        }
        return hubConnection;
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

using HttpTransportType = Microsoft.AspNetCore.Http.Connections.HttpTransportType;

namespace BD.Common8.Ipc.Services.Implementation;

#pragma warning disable SA1600 // Elements should be documented

/// <summary>
/// Ipc 客户端连接服务实现
/// </summary>
/// <param name="connectionString"></param>
public partial class IpcClientService(IpcAppConnectionString connectionString) :
    WebApiClientService(Log.Factory.CreateLogger<IpcClientService>(), null!),
    IIpcClientService, IAsyncDisposable
{
    IpcAppConnDelegatingHandler? httpHandler;
    HttpClient? httpClient;
    IpcAppConnDelegatingHandler? hubConnHandler;
    protected HubConnection? hubConnection;

    /// <inheritdoc cref="IpcAppConnectionString"/>
    protected readonly IpcAppConnectionString connectionString = connectionString;

    /// <inheritdoc/>
    protected sealed override HttpClient CreateClient()
    {
        if (httpClient == null)
        {
            (httpClient, httpHandler) = IpcAppConnectionStringHelper.GetHttpClient(connectionString);
            ConfigureSocketsHttpHandler(httpHandler.InnerHandler);
        }
        return httpClient;
    }

    /// <inheritdoc cref="IpcAppConnectionStringHelper.HubName"/>
    protected virtual string HubName => IpcAppConnectionStringHelper.HubName;

    AsyncExclusiveLock lock_GetHubConnAsync = new();
    AsyncExclusiveLock lock_TryStartAsync = new();

    /// <summary>
    /// 异步获取 SignalR Hub 连接
    /// </summary>
    /// <returns></returns>
    protected async ValueTask<HubConnection> GetHubConnAsync()
    {
        if (hubConnection != null)
        {
            if (hubConnection.State == HubConnectionState.Disconnected)
            {
                await TryStartAsync();
            }
            return hubConnection;
        }
        using (await lock_GetHubConnAsync.AcquireLockAsync(CancellationToken.None))
        {
            hubConnHandler = IpcAppConnectionStringHelper.GetHttpMessageHandler(connectionString);
            ConfigureSocketsHttpHandler(hubConnHandler.InnerHandler);

            var builder = new HubConnectionBuilder()
                //.WithServerTimeout()
                .WithUrl(hubConnHandler.BaseAddress, opt =>
                {
                    opt.Transports = HttpTransportType.WebSockets;
                    opt.HttpMessageHandlerFactory = (oldHandler) =>
                    {
                        oldHandler.Dispose(); // 传过来的 Handler 丢弃，使用根据连接字符串解析的
                        if (hubConnHandler.Disposed)
                        {
#if DEBUG
                            Console.WriteLine("已重新创建 HubConnDelegatingHandler");
#endif
                            hubConnHandler = IpcAppConnectionStringHelper.GetHttpMessageHandler(connectionString);
                            ConfigureSocketsHttpHandler(hubConnHandler.InnerHandler);
                        }
                        return hubConnHandler;
                    };
                    opt.WebSocketConfiguration = static o =>
                    {
                        o.HttpVersion = HttpVersion.Version20;
                    };
                    string GetHubUrl()
                    {
                        if (HubName.StartsWith('/'))
                            return $"{hubConnHandler.BaseAddress}{HubName}";
                        else
                            return $"{hubConnHandler.BaseAddress}/{HubName}";
                    }
                    opt.Url = new Uri(GetHubUrl());
                })
                .WithAutomaticReconnect();

            builder.AddJsonProtocol(ConfigureJsonHubProtocolOptions);

            ConfigureHubConnectionBuilder(builder);

            hubConnection = builder.Build();
            hubConnection.Reconnected += HubConnection_Reconnected;
            hubConnection.Reconnecting += HubConnection_Reconnecting;
            OnBuildHubConnection(hubConnection);

            await TryStartAsync();
        }
        return hubConnection;
    }

    protected virtual void OnBuildHubConnection(HubConnection connection)
    {
    }

    async Task TryStartAsync()
    {
        if (hubConnection == null)
            return;

        using (await lock_TryStartAsync.AcquireLockAsync(CancellationToken.None))
        {
            try
            {
                using var cts = new CancellationTokenSource();
                cts.CancelAfter(TimeSpan.FromSeconds(IpcAppConnectionStringHelper.TimeoutFromSeconds));
                await hubConnection.StartAsync(cts.Token);
            }
            catch (OperationCanceledException)
            {
            }
        }
    }

    protected virtual void ConfigureHubConnectionBuilder(IHubConnectionBuilder builder)
    {
    }

    protected virtual void ConfigureJsonHubProtocolOptions(JsonHubProtocolOptions options)
    {
        JsonSerializerOptions.CopyTypeInfoResolverChainTo(options.PayloadSerializerOptions);
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
        if (lock_GetHubConnAsync is not null)
        {
            await lock_GetHubConnAsync.DisposeAsync().ConfigureAwait(false);
            lock_GetHubConnAsync = null!;
        }
        if (lock_TryStartAsync is not null)
        {
            await lock_TryStartAsync.DisposeAsync().ConfigureAwait(false);
            lock_TryStartAsync = null!;
        }

        hubConnHandler = null;
        httpClient = null;
        hubConnection = null;
    }

    /// <summary>
    /// 当请求出现错误时
    /// </summary>
    /// <typeparam name="TResponseBody"></typeparam>
    /// <param name="ex"></param>
    /// <param name="hubConnection"></param>
    /// <param name="callerMemberName"></param>
    /// <returns></returns>
    protected virtual TResponseBody? OnError<[DynamicallyAccessedMembers(DynamicallyAccessedMemberTypes.All)] TResponseBody>(
        Exception ex,
        HubConnection? hubConnection,
        [CallerMemberName] string callerMemberName = "") where TResponseBody : notnull
    {
        if (EnableLogOnError)
        {
            logger.LogError(ex,
                $"{{callerMemberName}} fail, connId: {{connectionId}}.",
                callerMemberName,
                hubConnection?.ConnectionId);
        }

        var typeResponseBody = typeof(TResponseBody);
        if (typeResponseBody == typeof(nil))
            return default;

        ApiRspCode apiRspCode = default;
        if (hubConnection == null || hubConnection.State != HubConnectionState.Connected)
        {
            apiRspCode = ApiRspCode.Timeout;
        }

        var apiRspBase = GetApiRspBase<TResponseBody>(typeResponseBody);
        if (apiRspBase != null)
        {
            apiRspBase.Code = apiRspCode == default ? GetApiRspCodeByClientException(ex) : apiRspCode;
            apiRspBase.ClientException = ex;
            apiRspBase.InternalMessage = apiRspBase.GetMessage();
            return (TResponseBody?)(object)apiRspBase;
        }

        return default;
    }
}
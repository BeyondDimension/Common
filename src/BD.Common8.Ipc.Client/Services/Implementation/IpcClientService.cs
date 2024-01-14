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
    ConcurrentDictionary<string, HubConnection> hubConnections = [];

    /// <inheritdoc cref="IpcAppConnectionString"/>
    protected readonly IpcAppConnectionString connectionString = connectionString;

    /// <inheritdoc/>
    protected sealed override HttpClient CreateClient()
    {
        if (httpClient == null)
        {
            (httpClient, httpHandler) =
                IpcAppConnectionStringHelper.GetHttpClient(connectionString);
            httpClient.DefaultRequestHeaders.Authorization =
                AuthenticationHeaderValue.Parse(GetAccessToken());
            ConfigureSocketsHttpHandler(httpHandler.InnerHandler);
        }
        return httpClient;
    }

    /// <inheritdoc cref="IpcAppConnectionString.HubUrl"/>
    protected virtual string HubUrl => IpcAppConnectionString.HubUrl;

    AsyncExclusiveLock lock_GetHubConnAsync = new();
    AsyncExclusiveLock lock_TryStartAsync = new();

    string? _AccessToken;

    /// <summary>
    /// 获取持有者令牌身份验证
    /// <para>https://learn.microsoft.com/zh-cn/aspnet/core/signalr/authn-and-authz?view=aspnetcore-8.0#bearer-token-authentication</para>
    /// </summary>
    /// <returns></returns>
    protected virtual string GetAccessToken()
    {
        _AccessToken ??= connectionString.GetAccessToken();
        return _AccessToken;
    }

    Task<string?> GetAccessTokenAsync()
    {
        var accessToken = GetAccessToken();
        return Task.FromResult(accessToken)!;
    }

    protected virtual bool UseMemoryPack => false;

    /// <summary>
    /// 异步获取 SignalR Hub 连接
    /// </summary>
    /// <returns></returns>
    protected async ValueTask<HubConnection> GetHubConnAsync(string? hubUrl = null)
    {
        hubUrl ??= HubUrl;

        if (hubConnections.TryGetValue(hubUrl, out var hubConnection))
        {
            if (hubConnection.State == HubConnectionState.Disconnected)
            {
                await TryStartAsync(hubConnection);
            }
            return hubConnection;
        }

        using (await lock_GetHubConnAsync.AcquireLockAsync(CancellationToken.None))
        {
            hubConnHandler = IpcAppConnectionStringHelper.GetHttpMessageHandler(connectionString);
            ConfigureSocketsHttpHandler(hubConnHandler.InnerHandler);

            string GetHubUrl()
            {
                if (hubUrl.StartsWith('/'))
                    return $"{hubConnHandler.BaseAddress}{hubUrl}";
                else
                    return $"{hubConnHandler.BaseAddress}/{hubUrl}";
            }
            var url = new Uri(GetHubUrl());
            var builder = new HubConnectionBuilder()
                .WithUrl(url, HttpTransportType.WebSockets, opt =>
                {
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
                    opt.AccessTokenProvider = GetAccessTokenAsync; // 授权头不正确将返回超时
                })
                .WithAutomaticReconnect();

            if (UseMemoryPack)
            {
                builder.Services.TryAddEnumerable(
                    ServiceDescriptor.Singleton<IHubProtocol, MemoryPackHubProtocol>());
            }
            else
            {
                builder.AddJsonProtocol(ConfigureJsonHubProtocolOptions);
            }

            ConfigureHubConnectionBuilder(builder);

            hubConnection = builder.Build();
            hubConnection.Reconnected += HubConnection_Reconnected;
            hubConnection.Reconnecting += HubConnection_Reconnecting;
            OnBuildHubConnection(hubConnection);
            hubConnections.TryAdd(hubUrl, hubConnection);

            await TryStartAsync(hubConnection);
        }
        return hubConnection;
    }

    protected virtual void OnBuildHubConnection(HubConnection connection)
    {
    }

    async Task TryStartAsync(HubConnection hubConnection)
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
        options.PayloadSerializerOptions = Serializable.CreateOptions(options.PayloadSerializerOptions);
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

    /// <inheritdoc/>
    public async ValueTask DisposeAsync()
    {
        await DisposeAsyncCore().ConfigureAwait(false);
        GC.SuppressFinalize(this);
    }

    protected virtual async ValueTask DisposeAsyncCore()
    {
        hubConnHandler?.Dispose();
        httpClient?.Dispose();

        foreach (var hubConnection in hubConnections.Values)
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
        hubConnections = null!;
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
        //if (EnableLogOnError)
        //{
        //    logger.LogError(ex,
        //        $"{{callerMemberName}} fail, connId: {{connectionId}}.",
        //        callerMemberName,
        //        hubConnection?.ConnectionId);
        //}

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
            return (TResponseBody?)(object)apiRspBase;
        }

        return default;
    }
}
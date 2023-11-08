namespace BD.Common.Services.Implementation.SmsSender;

public abstract class SmsSenderBase : ISmsSender
{
    public abstract string Channel { get; }

    public abstract bool SupportCheck { get; }

    public abstract Task<ICheckSmsResult> CheckSmsAsync(string number, string message, CancellationToken cancellationToken);

    public abstract Task<ISendSmsResult> SendSmsAsync(string number, string message, ushort type, CancellationToken cancellationToken);

    /// <summary>
    /// 生成随机短信验证码值，某些平台可能提供了随机生成，可以重写该函数替换
    /// </summary>
    /// <param name="length"></param>
    /// <returns></returns>
    public virtual string GenerateRandomNum(int length)
    {
        return Random2.GenerateRandomNum((ushort)length).ToString();
    }

    public static IServiceCollection Add<TSmsSender, TSmsSettings>(
        IServiceCollection services)
        where TSmsSender : class, ISmsSender
        where TSmsSettings : class, ISmsSettings
    {
        services.AddHttpClient<TSmsSender>();
        services.AddScoped<DebugSmsSenderProvider>();
        services.AddScoped<ISmsSender, SmsSenderWrapper<TSmsSender, TSmsSettings>>();
        return services;
    }

    internal static IServiceCollection Add<TSmsSettings>(
        IServiceCollection services,
        string? name)
        where TSmsSettings : class, ISmsSettings => name switch
        {
            null => services.AddScoped<ISmsSender, DebugSmsSenderProvider>(),
            nameof(ISmsSettings.SmsOptions._21VianetBlueCloud)
                => Add<Channels._21VianetBlueCloud.SenderProviderInvoker<TSmsSettings>, TSmsSettings>(services),
            nameof(ISmsSettings.SmsOptions.AlibabaCloud)
                => Add<Channels.AlibabaCloud.SenderProviderInvoker<TSmsSettings>, TSmsSettings>(services),
            nameof(ISmsSettings.SmsOptions.NetEaseCloud)
                => Add<Channels.NetEaseCloud.SenderProviderInvoker<TSmsSettings>, TSmsSettings>(services),
            nameof(ISmsSettings.SmsOptions.HuaweiCloud)
                => Add<Channels.HuaweiCloud.SenderProviderInvoker<TSmsSettings>, TSmsSettings>(services),
            _ => throw new ArgumentOutOfRangeException(nameof(name), name, null),
        };

#if __HAVE_N_JSON__
    readonly JsonSerializer jsonSerializer = new();
#endif

    protected virtual StringContent GetJsonContent<T>(T inputValue)
    {
#if __HAVE_N_JSON__
        var jsonStr = JsonConvert.SerializeObject(inputValue);
#endif
#if !__NOT_HAVE_S_JSON__
#pragma warning disable IL2026 // Members annotated with 'RequiresUnreferencedCodeAttribute' require dynamic access otherwise can break functionality when trimming application code
        var jsonStr = JsonSerializer.Serialize(inputValue, JsonSerializerCompatOptions.Default);
#pragma warning restore IL2026 // Members annotated with 'RequiresUnreferencedCodeAttribute' require dynamic access otherwise can break functionality when trimming application code
#endif
        return new StringContent(jsonStr, Encoding.UTF8, MediaTypeNames.JSON);
    }

    protected virtual async Task<T?> ReadFromJsonAsync<T>(HttpContent content, CancellationToken cancellationToken)
    {
#if __HAVE_N_JSON__
        using var stream = await content.ReadAsStreamAsync(cancellationToken).ConfigureAwait(false);
        using var reader = new StreamReader(stream, Encoding.UTF8);
        using var jsonStr = new JsonTextReader(reader);
        var jsonObj = jsonSerializer.Deserialize<T>(jsonStr);
#endif
#if !__NOT_HAVE_S_JSON__
#pragma warning disable IL2026 // Members annotated with 'RequiresUnreferencedCodeAttribute' require dynamic access otherwise can break functionality when trimming application code
        var jsonObj = await content.ReadFromJsonAsync<T>(JsonSerializerCompatOptions.Default, cancellationToken);
#pragma warning restore IL2026 // Members annotated with 'RequiresUnreferencedCodeAttribute' require dynamic access otherwise can break functionality when trimming application code
#endif
        return jsonObj;
    }
}
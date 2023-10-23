namespace BD.Common8.SmsSender.Services.Implementation.SmsSender;

#pragma warning disable SA1600 // Elements should be documented

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
        return Random2.GenerateRandomNum(length).ToString();
    }

    public static IServiceCollection Add<[DynamicallyAccessedMembers(DynamicallyAccessedMemberTypes.PublicConstructors)] TSmsSender, TSmsSettings>(
        IServiceCollection services)
        where TSmsSender : class, ISmsSender
        where TSmsSettings : class, ISmsSettings
    {
        IClientHttpClientFactory.AddHttpClient<TSmsSender>(services);
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
            _ => throw new ArgumentOutOfRangeException(nameof(name), name, null),
        };

    protected virtual HttpContent GetJsonContent<T>(T inputValue, JsonTypeInfo<T> jsonTypeInfo)
    {
        var content = JsonContent.Create(inputValue, jsonTypeInfo);
        return content;
    }

    protected virtual async Task<T?> ReadFromJsonAsync<T>(HttpContent content, JsonTypeInfo<T> jsonTypeInfo, CancellationToken cancellationToken)
    {
        var jsonObj = await content.ReadFromJsonAsync(jsonTypeInfo, cancellationToken);
        return jsonObj;
    }
}

[JsonSerializable(typeof(Channels._21VianetBlueCloud.SmsSenderProvider.RequestData))]
[JsonSerializable(typeof(SendSms21VianetBlueCloudResult))]
[JsonSerializable(typeof(SendSmsAlibabaCloudResult))]
[JsonSerializable(typeof(SendSmsNetEaseCloudResult))]
[JsonSerializable(typeof(NetEaseCloudResult))]
sealed partial class SmsSenderJsonSerializerContext : JsonSerializerContext
{
    static SmsSenderJsonSerializerContext()
    {
        s_defaultOptions.AllowTrailingCommas = true;
        s_defaultOptions.Encoder = JavaScriptEncoder.UnsafeRelaxedJsonEscaping;
    }
}
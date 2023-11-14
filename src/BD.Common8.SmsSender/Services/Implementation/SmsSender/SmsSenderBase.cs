namespace BD.Common8.SmsSender.Services.Implementation.SmsSender;

/// <summary>
/// Sms 发送基类
/// </summary>
public abstract class SmsSenderBase : ISmsSender
{
    /// <inheritdoc/>
    public abstract string Channel { get; }

    /// <inheritdoc/>
    public abstract bool SupportCheck { get; }

    /// <inheritdoc/>
    public abstract Task<ICheckSmsResult> CheckSmsAsync(string number, string message, CancellationToken cancellationToken);

    /// <inheritdoc/>
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

    /// <summary>
    /// 依赖注入，添加短信发送服务
    /// </summary>
    /// <typeparam name="TSmsSender"></typeparam>
    /// <typeparam name="TSmsSettings"></typeparam>
    /// <param name="services"></param>
    /// <returns></returns>
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

    /// <summary>
    /// 依赖注入，根据短信渠道添加短信发送服务
    /// </summary>
    /// <typeparam name="TSmsSettings"></typeparam>
    /// <param name="services"></param>
    /// <param name="name"></param>
    /// <returns></returns>
    /// <exception cref="ArgumentOutOfRangeException"></exception>
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

    /// <summary>
    /// 获取 JSON 格式的 HttpContent
    /// </summary>
    /// <typeparam name="T"></typeparam>
    /// <param name="inputValue"></param>
    /// <param name="jsonTypeInfo"></param>
    /// <returns></returns>
    protected virtual HttpContent GetJsonContent<T>(T inputValue, JsonTypeInfo<T> jsonTypeInfo)
    {
        var content = JsonContent.Create(inputValue, jsonTypeInfo);
        return content;
    }

    /// <summary>
    /// 从 HttpContent 中读取 JSON 数据
    /// </summary>
    /// <typeparam name="T"></typeparam>
    /// <param name="content"></param>
    /// <param name="jsonTypeInfo"></param>
    /// <param name="cancellationToken"></param>
    /// <returns></returns>
    protected virtual async Task<T?> ReadFromJsonAsync<T>(HttpContent content, JsonTypeInfo<T> jsonTypeInfo, CancellationToken cancellationToken)
    {
        var jsonObj = await content.ReadFromJsonAsync(jsonTypeInfo, cancellationToken);
        return jsonObj;
    }
}

/// <summary>
/// 修改这些默认选项，可以控制相关类型序列化
/// </summary>
[JsonSerializable(typeof(Channels._21VianetBlueCloud.SmsSenderProvider.RequestData))]
[JsonSerializable(typeof(SendSms21VianetBlueCloudResult))]
[JsonSerializable(typeof(SendSmsAlibabaCloudResult))]
[JsonSerializable(typeof(SendSmsNetEaseCloudResult))]
[JsonSerializable(typeof(NetEaseCloudResult))]
[JsonSerializable(typeof(SendHuaweiCloudResult))]
sealed partial class SmsSenderJsonSerializerContext : JsonSerializerContext
{
    static SmsSenderJsonSerializerContext()
    {
        s_defaultOptions.AllowTrailingCommas = true;
        s_defaultOptions.Encoder = JavaScriptEncoder.UnsafeRelaxedJsonEscaping;
    }
}
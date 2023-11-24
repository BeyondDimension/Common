namespace BD.Common8.SmsSender.Services.Implementation.SmsSender.Channels._21VianetBlueCloud;

/// <summary>
/// 蓝云短信服务发件人提供调用程序
/// </summary>
/// <typeparam name="TSmsSettings"></typeparam>
internal sealed class SenderProviderInvoker<[DynamicallyAccessedMembers(DynamicallyAccessedMemberTypes.PublicParameterlessConstructor)] TSmsSettings>(ILogger<SenderProviderInvoker<TSmsSettings>> logger,
    IOptions<TSmsSettings> settings,
    HttpClient httpClient) : SmsSenderProvider(logger,
        settings.Value?.SmsOptions?._21VianetBlueCloud,
        httpClient)
    where TSmsSettings : class, ISmsSettings
{
}
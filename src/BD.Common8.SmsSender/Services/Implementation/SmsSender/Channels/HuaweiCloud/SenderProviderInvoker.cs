namespace BD.Common8.SmsSender.Services.Implementation.SmsSender.Channels.HuaweiCloud;

/// <summary>
/// 华为云短信服务发件人提供调用程序
/// </summary>
/// <typeparam name="TSmsSettings"></typeparam>
internal sealed class SenderProviderInvoker<[DynamicallyAccessedMembers(DynamicallyAccessedMemberTypes.PublicParameterlessConstructor)] TSmsSettings>(ILogger<SenderProviderInvoker<TSmsSettings>> logger,
    IOptions<TSmsSettings> settings,
    HttpClient httpClient) : SmsSenderProvider(logger,
        settings.Value?.SmsOptions?.HuaweiCloud,
        httpClient)
    where TSmsSettings : class, ISmsSettings
{
}

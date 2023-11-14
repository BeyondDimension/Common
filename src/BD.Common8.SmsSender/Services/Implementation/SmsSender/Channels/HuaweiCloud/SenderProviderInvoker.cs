namespace BD.Common8.SmsSender.Services.Implementation.SmsSender.Channels.HuaweiCloud;

#pragma warning disable SA1600 // Elements should be documented
internal sealed class SenderProviderInvoker<[DynamicallyAccessedMembers(DynamicallyAccessedMemberTypes.PublicParameterlessConstructor)] TSmsSettings>(ILogger<SenderProviderInvoker<TSmsSettings>> logger,
    IOptions<TSmsSettings> settings,
    HttpClient httpClient) : SmsSenderProvider(logger,
        settings.Value?.SmsOptions?.HuaweiCloud,
        httpClient)
    where TSmsSettings : class, ISmsSettings
{
}

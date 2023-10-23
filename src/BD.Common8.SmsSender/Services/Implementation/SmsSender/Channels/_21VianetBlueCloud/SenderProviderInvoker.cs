namespace BD.Common8.SmsSender.Services.Implementation.SmsSender.Channels._21VianetBlueCloud;

#pragma warning disable SA1600 // Elements should be documented

internal sealed class SenderProviderInvoker<[DynamicallyAccessedMembers(DynamicallyAccessedMemberTypes.PublicParameterlessConstructor)] TSmsSettings>(ILogger<SenderProviderInvoker<TSmsSettings>> logger,
    IOptions<TSmsSettings> settings,
    HttpClient httpClient) : SmsSenderProvider(logger,
        settings.Value?.SmsOptions?._21VianetBlueCloud,
        httpClient)
    where TSmsSettings : class, ISmsSettings
{
}
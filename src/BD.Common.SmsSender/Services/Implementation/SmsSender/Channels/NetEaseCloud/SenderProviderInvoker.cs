namespace BD.Common.Services.Implementation.SmsSender.Channels.NetEaseCloud;

internal sealed class SenderProviderInvoker<TSmsSettings> : SmsSenderProvider
    where TSmsSettings : class, ISmsSettings
{
#pragma warning disable IL2091 // Target generic argument does not satisfy 'DynamicallyAccessedMembersAttribute' in target method or type. The generic parameter of the source method or type does not have matching annotations.
    public SenderProviderInvoker(ILogger<SenderProviderInvoker<TSmsSettings>> logger, IOptions<TSmsSettings> settings, HttpClient httpClient) : base(logger, settings.Value?.SmsOptions?.NetEaseCloud, httpClient)
#pragma warning restore IL2091 // Target generic argument does not satisfy 'DynamicallyAccessedMembersAttribute' in target method or type. The generic parameter of the source method or type does not have matching annotations.
    {
    }
}
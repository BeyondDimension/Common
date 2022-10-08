using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;

namespace BD.Common.Services.Implementation.SmsSender.Channels._21VianetBlueCloud;

internal sealed class SenderProviderInvoker<TSmsSettings> : SmsSenderProvider
    where TSmsSettings : class, ISmsSettings
{
    public SenderProviderInvoker(ILogger<SenderProviderInvoker<TSmsSettings>> logger,
#pragma warning disable IL2091 // Target generic argument does not satisfy 'DynamicallyAccessedMembersAttribute' in target method or type. The generic parameter of the source method or type does not have matching annotations.
        IOptions<TSmsSettings> settings,
#pragma warning restore IL2091 // Target generic argument does not satisfy 'DynamicallyAccessedMembersAttribute' in target method or type. The generic parameter of the source method or type does not have matching annotations.
        HttpClient httpClient) : base(logger,
            settings.Value?.SmsOptions?._21VianetBlueCloud,
            httpClient)
    {
    }
}
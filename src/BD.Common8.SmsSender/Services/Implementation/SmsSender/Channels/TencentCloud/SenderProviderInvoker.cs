using BD.Common8.SmsSender.Models.SmsSender.Abstractions;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using System.Diagnostics.CodeAnalysis;

namespace BD.Common8.SmsSender.Services.Implementation.SmsSender.Channels.TencentCloud;

/// <summary>
/// 阿里云短信服务发件人提供调用程序
/// </summary>
/// <typeparam name="TSmsSettings"></typeparam>
internal sealed class SenderProviderInvoker<[DynamicallyAccessedMembers(DynamicallyAccessedMemberTypes.PublicParameterlessConstructor)] TSmsSettings>(ILogger<SenderProviderInvoker<TSmsSettings>> logger, IOptions<TSmsSettings> settings, HttpClient httpClient) : SmsSenderProvider(logger, settings.Value?.SmsOptions?.TencentCloud, httpClient)
    where TSmsSettings : class, ISmsSettings
{
}

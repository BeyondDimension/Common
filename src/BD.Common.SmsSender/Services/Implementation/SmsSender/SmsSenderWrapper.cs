using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Options;

namespace BD.Common.Services.Implementation.SmsSender;

internal sealed class SmsSenderWrapper<TSmsSender, TSmsSettings> : ISmsSender
    where TSmsSender : ISmsSender
    where TSmsSettings : class, ISmsSettings
{
    readonly ISmsSender smsSender;

    public SmsSenderWrapper(
      IHostEnvironment hostingEnvironment,
#pragma warning disable IL2091 // Target generic argument does not satisfy 'DynamicallyAccessedMembersAttribute' in target method or type. The generic parameter of the source method or type does not have matching annotations.
      IOptions<TSmsSettings> options,
#pragma warning restore IL2091 // Target generic argument does not satisfy 'DynamicallyAccessedMembersAttribute' in target method or type. The generic parameter of the source method or type does not have matching annotations.
      TSmsSender smsSender,
      DebugSmsSenderProvider debug)
    {
        this.smsSender = smsSender;

        var settings = options.Value;

        if (settings != null)
            if (settings.SmsOptions == null || (settings.UseDebugSmsSender.HasValue && settings.UseDebugSmsSender.Value) || (!settings.UseDebugSmsSender.HasValue
                && hostingEnvironment.IsDevelopment()))
                this.smsSender = debug;
    }

    public string Channel => smsSender.Channel;

    public bool SupportCheck => smsSender.SupportCheck;

    public Task<ICheckSmsResult> CheckSmsAsync(string number, string message, CancellationToken cancellationToken) => smsSender.CheckSmsAsync(number, message, cancellationToken);

    public Task<ISendSmsResult> SendSmsAsync(string number, string message, ushort type, CancellationToken cancellationToken) => smsSender.SendSmsAsync(number, message, type, cancellationToken);

    public string GenerateRandomNum(int length) => smsSender.GenerateRandomNum(length);
}
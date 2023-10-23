namespace BD.Common8.SmsSender.Services.Implementation.SmsSender;

#pragma warning disable SA1600 // Elements should be documented

internal sealed class SmsSenderWrapper<TSmsSender, [DynamicallyAccessedMembers(DynamicallyAccessedMemberTypes.PublicParameterlessConstructor)] TSmsSettings> : ISmsSender
    where TSmsSender : ISmsSender
    where TSmsSettings : class, ISmsSettings
{
    readonly ISmsSender smsSender;

    public SmsSenderWrapper(
      IHostEnvironment hostingEnvironment,
      IOptions<TSmsSettings> options,
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
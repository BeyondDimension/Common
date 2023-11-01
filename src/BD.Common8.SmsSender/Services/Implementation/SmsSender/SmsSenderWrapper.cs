namespace BD.Common8.SmsSender.Services.Implementation.SmsSender;

/// <summary>
/// 提供一个短信发送器
/// </summary>
/// <typeparam name="TSmsSender"></typeparam>
/// <typeparam name="TSmsSettings"></typeparam>
internal sealed class SmsSenderWrapper<TSmsSender, [DynamicallyAccessedMembers(DynamicallyAccessedMemberTypes.PublicParameterlessConstructor)] TSmsSettings> : ISmsSender
    where TSmsSender : ISmsSender
    where TSmsSettings : class, ISmsSettings
{
    readonly ISmsSender smsSender;

    /// <summary>
    /// 初始化 SmsSenderWrapper 实例
    /// </summary>
    /// <param name="hostingEnvironment"></param>
    /// <param name="options"></param>
    /// <param name="smsSender"></param>
    /// <param name="debug"></param>
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

    /// <inheritdoc/>
    public string Channel => smsSender.Channel;

    /// <inheritdoc/>
    public bool SupportCheck => smsSender.SupportCheck;

    /// <inheritdoc/>
    public Task<ICheckSmsResult> CheckSmsAsync(string number, string message, CancellationToken cancellationToken) => smsSender.CheckSmsAsync(number, message, cancellationToken);

    /// <inheritdoc/>
    public Task<ISendSmsResult> SendSmsAsync(string number, string message, ushort type, CancellationToken cancellationToken) => smsSender.SendSmsAsync(number, message, type, cancellationToken);

    /// <inheritdoc/>
    public string GenerateRandomNum(int length) => smsSender.GenerateRandomNum(length);
}
namespace BD.Common8.SmsSender.Services.Implementation.SmsSender;

/// <summary>
/// 调试模式下可使用此实现，验证码全输 6
/// </summary>
public sealed class DebugSmsSenderProvider : ISmsSender
{
    /// <summary>
    /// 当前类名字符串
    /// </summary>
    public const string Name = nameof(DebugSmsSenderProvider);

    /// <inheritdoc/>
    public string Channel => Name;

    /// <inheritdoc/>
    public bool SupportCheck => true;

    /// <inheritdoc/>
    public Task<ICheckSmsResult> CheckSmsAsync(string number, string message, CancellationToken cancellationToken) => Task.FromResult<ICheckSmsResult>(new CheckSmsResult
    {
        IsSuccess = true,
        IsCheckSuccess = true,
    });

    /// <inheritdoc/>
    public Task<ISendSmsResult> SendSmsAsync(string number, string message, ushort type, CancellationToken cancellationToken) => Task.FromResult<ISendSmsResult>(new SendSmsResult
    {
        IsSuccess = true,
    });

    /// <inheritdoc/>
    public string GenerateRandomNum(int length) => string.Join(null, new char[length].Select(x => '6'));
}
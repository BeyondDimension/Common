namespace BD.Common8.SmsSender.Services.Implementation.SmsSender;

#pragma warning disable SA1600 // Elements should be documented

/// <summary>
/// 调试模式下可使用此实现，验证码全输 6
/// </summary>
public sealed class DebugSmsSenderProvider : ISmsSender
{
    public const string Name = nameof(DebugSmsSenderProvider);

    public string Channel => Name;

    public bool SupportCheck => true;

    public Task<ICheckSmsResult> CheckSmsAsync(string number, string message, CancellationToken cancellationToken) => Task.FromResult<ICheckSmsResult>(new CheckSmsResult
    {
        IsSuccess = true,
        IsCheckSuccess = true,
    });

    public Task<ISendSmsResult> SendSmsAsync(string number, string message, ushort type, CancellationToken cancellationToken) => Task.FromResult<ISendSmsResult>(new SendSmsResult
    {
        IsSuccess = true,
    });

    public string GenerateRandomNum(int length) => string.Join(null, new char[length].Select(x => '6'));
}
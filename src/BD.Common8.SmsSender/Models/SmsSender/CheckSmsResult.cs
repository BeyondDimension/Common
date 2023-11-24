namespace BD.Common8.SmsSender.Models.SmsSender;

/// <summary>
/// 验证短信结果的类
/// </summary>
public sealed class CheckSmsResult : SmsResult<CheckSmsResult>, ICheckSmsResult
{
    /// <inheritdoc/>
    public bool IsCheckSuccess { get; set; }
}

/// <summary>
/// 验证短信结果的类，带有泛型参数
/// </summary>
/// <typeparam name="TResult">结果的泛型参数类型，必须实现 <see cref="IJsonModel"/> 接口 </typeparam>
public class CheckSmsResult<TResult> : SmsResult<TResult, CheckSmsResult<TResult>>, ICheckSmsResult
  where TResult : IJsonModel
{
    /// <inheritdoc/>
    public bool IsCheckSuccess { get; set; }
}
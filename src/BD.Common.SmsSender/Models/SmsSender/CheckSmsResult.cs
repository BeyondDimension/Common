namespace BD.Common.Models.SmsSender;

public sealed class CheckSmsResult : SmsResult<CheckSmsResult>, ICheckSmsResult
{
    public bool IsCheckSuccess { get; set; }
}

public class CheckSmsResult<TResult> : SmsResult<TResult, CheckSmsResult<TResult>>, ICheckSmsResult
  where TResult : IJsonModel
{
    public bool IsCheckSuccess { get; set; }
}
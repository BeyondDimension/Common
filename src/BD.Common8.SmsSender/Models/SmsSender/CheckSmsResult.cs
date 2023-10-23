namespace BD.Common8.SmsSender.Models.SmsSender;

#pragma warning disable SA1600 // Elements should be documented

public sealed class CheckSmsResult : SmsResult<CheckSmsResult>, ICheckSmsResult
{
    public bool IsCheckSuccess { get; set; }
}

public class CheckSmsResult<TResult> : SmsResult<TResult, CheckSmsResult<TResult>>, ICheckSmsResult
  where TResult : IJsonModel
{
    public bool IsCheckSuccess { get; set; }
}
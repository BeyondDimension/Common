namespace BD.Common.Models.SmsSender;

public sealed class SendSmsResult : SmsResult<SendSmsResult>, ISendSmsResult
{
}

public class SendSmsResult<TResult> : SmsResult<TResult, SendSmsResult<TResult>>, ISendSmsResult
  where TResult : IJsonModel
{
}
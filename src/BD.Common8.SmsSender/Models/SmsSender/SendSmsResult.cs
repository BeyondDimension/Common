namespace BD.Common8.SmsSender.Models.SmsSender;

#pragma warning disable SA1600 // Elements should be documented

public sealed class SendSmsResult : SmsResult<SendSmsResult>, ISendSmsResult
{
}

public class SendSmsResult<TResult> : SmsResult<TResult, SendSmsResult<TResult>>, ISendSmsResult
  where TResult : IJsonModel
{
}
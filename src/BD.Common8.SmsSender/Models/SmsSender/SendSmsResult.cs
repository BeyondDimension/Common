using BD.Common8.Models.Abstractions;
using BD.Common8.SmsSender.Models.SmsSender.Abstractions;
using System.Text.Json.Serialization;

namespace BD.Common8.SmsSender.Models.SmsSender;

/// <summary>
/// 发送短信的结果类
/// </summary>
public sealed class SendSmsResult : SmsResult<SendSmsResult>, ISendSmsResult, IJsonSerializerContext
{
    static JsonSerializerContext IJsonSerializerContext.Default => global::BD.Common8.SmsSender.Services.Implementation.SmsSender.SmsSenderJsonSerializerContext.Default;
}

/// <summary>
/// 发送短信的结果类，带有泛型参数
/// </summary>
/// <typeparam name="TResult">结果的泛型参数类型，必须实现 <see cref="IJsonModel"/> 接口</typeparam>
public class SendSmsResult<TResult> : SmsResult<TResult, SendSmsResult<TResult>>, ISendSmsResult, IJsonSerializerContext
  where TResult : IJsonModel, IJsonSerializerContext
{
    static JsonSerializerContext IJsonSerializerContext.Default => global::BD.Common8.SmsSender.Services.Implementation.SmsSender.SmsSenderJsonSerializerContext.Default;
}
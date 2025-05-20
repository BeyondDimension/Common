using BD.Common8.Models.Abstractions;
using BD.Common8.SmsSender.Models.SmsSender.Abstractions;
using BD.Common8.SmsSender.Services.Implementation.SmsSender;
using System.Diagnostics;

namespace BD.Common8.SmsSender.Models.SmsSender.Channels.NetEaseCloud;

/// <summary>
/// 提供网易云 API 请求的返回结果
/// </summary>
/// <typeparam name="T"></typeparam>
public class NetEaseCloudResult<T> : JsonModel<T>, ISmsSubResult where T : NetEaseCloudResult<T>
{
    /// <summary>
    /// 网易云发送 Sms 响应代码
    /// </summary>
    [global::System.Text.Json.Serialization.JsonPropertyName(code)]
    public SendSmsNetEaseCloudResponseCode Code { get; set; }

    /// <summary>
    /// 响应代码的字段名称
    /// </summary>
    protected const string code = nameof(code);

    /// <summary>
    /// 消息字段名称
    /// </summary>
    protected const string msg = nameof(msg);

    /// <summary>
    /// 返回结果的对象名称
    /// </summary>
    protected const string obj = nameof(obj);

    /// <summary>
    /// 判断操作是否成功
    /// </summary>
    public virtual bool IsOK() => Code == SendSmsNetEaseCloudResponseCode.操作成功;

    /// <summary>
    /// 判断短信验证是否失败
    /// </summary>
    public virtual bool IsCheckSmsFail() => Code == SendSmsNetEaseCloudResponseCode.验证失败;

    /// <summary>
    /// 返回包含 Code 属性值的信息
    /// </summary>
    /// <returns></returns>
    protected virtual string? GetRecord() => $"code: {Code}";

    /// <inheritdoc />
    string? ISmsSubResult.GetRecord() => GetRecord();
}

/// <summary>
/// 继承 <see cref="NetEaseCloudResult{T}"/>
/// </summary>
[DebuggerDisplay("{DebuggerDisplay(),nq}")]
public sealed class NetEaseCloudResult : NetEaseCloudResult<NetEaseCloudResult>
{
#pragma warning disable IL2026 // Members annotated with 'RequiresUnreferencedCodeAttribute' require dynamic access otherwise can break functionality when trimming application code
#pragma warning disable IL3050 // Calling members annotated with 'RequiresDynamicCodeAttribute' may break functionality when AOT compiling.
    string DebuggerDisplay() => global::System.Text.Json.JsonSerializer.Serialize(this, SmsSenderJsonSerializerContext.Default.Options);
#pragma warning restore IL3050 // Calling members annotated with 'RequiresDynamicCodeAttribute' may break functionality when AOT compiling.
#pragma warning restore IL2026 // Members annotated with 'RequiresUnreferencedCodeAttribute' require dynamic access otherwise can break functionality when trimming application code
}
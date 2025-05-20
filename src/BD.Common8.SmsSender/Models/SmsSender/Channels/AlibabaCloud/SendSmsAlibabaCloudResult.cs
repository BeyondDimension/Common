using BD.Common8.SmsSender.Models.SmsSender.Abstractions;
using BD.Common8.SmsSender.Services.Implementation.SmsSender;
using System.Diagnostics;

namespace BD.Common8.SmsSender.Models.SmsSender.Channels.AlibabaCloud;

/// <summary>
/// 阿里云短信发送结果
/// </summary>
[DebuggerDisplay("{DebuggerDisplay(),nq}")]
public sealed class SendSmsAlibabaCloudResult : AlibabaCloudResult<SendSmsAlibabaCloudResult>, ISmsSubResult
{
#pragma warning disable IL2026 // Members annotated with 'RequiresUnreferencedCodeAttribute' require dynamic access otherwise can break functionality when trimming application code
#pragma warning disable IL3050 // Calling members annotated with 'RequiresDynamicCodeAttribute' may break functionality when AOT compiling.
    string DebuggerDisplay() => global::System.Text.Json.JsonSerializer.Serialize(this, SmsSenderJsonSerializerContext.Default.Options);
#pragma warning restore IL3050 // Calling members annotated with 'RequiresDynamicCodeAttribute' may break functionality when AOT compiling.
#pragma warning restore IL2026 // Members annotated with 'RequiresUnreferencedCodeAttribute' require dynamic access otherwise can break functionality when trimming application code

    /// <summary>
    /// 消息内容
    /// </summary>
    public string? Message { get; set; }

    /// <summary>
    /// 唯一标识符
    /// </summary>
    public string? RequestId { get; set; }

    /// <summary>
    /// 业务标识符
    /// </summary>
    public string? BizId { get; set; }

    /// <summary>
    /// 返回包含了状态、消息内容、 唯一标识符、业务标识符的信息
    /// </summary>
    string? ISmsSubResult.GetRecord()
        => $"code: {Code}, message: {Message}, requestId: {RequestId}, bizId: {BizId}";
}
using BD.Common8.SmsSender.Services.Implementation.SmsSender;
using System.Diagnostics;

namespace BD.Common8.SmsSender.Models.SmsSender.Channels.HuaweiCloud;

/// <summary>
/// 华为云短信发送结果
/// </summary>
[DebuggerDisplay("{DebuggerDisplay(),nq}")]
public class SendHuaweiCloudResult : HuaweiCloudResult<SendHuaweiCloudResult>
{
#pragma warning disable IL2026 // Members annotated with 'RequiresUnreferencedCodeAttribute' require dynamic access otherwise can break functionality when trimming application code
#pragma warning disable IL3050 // Calling members annotated with 'RequiresDynamicCodeAttribute' may break functionality when AOT compiling.
    string DebuggerDisplay() => global::System.Text.Json.JsonSerializer.Serialize(this, SmsSenderJsonSerializerContext.Default.Options);
#pragma warning restore IL3050 // Calling members annotated with 'RequiresDynamicCodeAttribute' may break functionality when AOT compiling.
#pragma warning restore IL2026 // Members annotated with 'RequiresUnreferencedCodeAttribute' require dynamic access otherwise can break functionality when trimming application code

    /// <summary>
    /// 短信发送结果详情信息
    /// </summary>
    [global::System.Text.Json.Serialization.JsonPropertyName("result")]
    public List<SendResult>? Result { get; set; }
}

/// <summary>
/// 华为云短信结果
/// </summary>
[DebuggerDisplay("{DebuggerDisplay(),nq}")]
public class SendResult
{
#pragma warning disable IL2026 // Members annotated with 'RequiresUnreferencedCodeAttribute' require dynamic access otherwise can break functionality when trimming application code
#pragma warning disable IL3050 // Calling members annotated with 'RequiresDynamicCodeAttribute' may break functionality when AOT compiling.
    string DebuggerDisplay() => global::System.Text.Json.JsonSerializer.Serialize(this, SmsSenderJsonSerializerContext.Default.Options);
#pragma warning restore IL3050 // Calling members annotated with 'RequiresDynamicCodeAttribute' may break functionality when AOT compiling.
#pragma warning restore IL2026 // Members annotated with 'RequiresUnreferencedCodeAttribute' require dynamic access otherwise can break functionality when trimming application code

    /// <summary>
    /// 消息 MsgId
    /// </summary>
    [global::System.Text.Json.Serialization.JsonPropertyName("smsMsgId")]
    public string? SmsMsgId { get; set; }

    /// <summary>
    /// 发送时间
    /// </summary>
    [global::System.Text.Json.Serialization.JsonPropertyName("createTime")]
    public string? CreateTime { get; set; }
}

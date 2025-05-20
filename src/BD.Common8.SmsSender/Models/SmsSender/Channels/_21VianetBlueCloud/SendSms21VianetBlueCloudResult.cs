using BD.Common8.Models.Abstractions;
using BD.Common8.SmsSender.Models.SmsSender.Abstractions;
using BD.Common8.SmsSender.Services.Implementation.SmsSender;
using System.Diagnostics;

namespace BD.Common8.SmsSender.Models.SmsSender.Channels._21VianetBlueCloud;

/// <summary>
/// https://bcssstorage.blob.core.chinacloudapi.cn/docs/CCS/%E8%93%9D%E4%BA%91%E7%94%A8%E6%88%B7%E8%BF%9E%E6%8E%A5%E6%9C%8D%E5%8A%A1%E6%8A%80%E6%9C%AF%E6%96%87%E6%A1%A3(%E7%9F%AD%E4%BF%A1%2B%E7%99%BB%E5%BD%95).pdf
/// </summary>
[DebuggerDisplay("{DebuggerDisplay(),nq}")]
public class SendSms21VianetBlueCloudResult : JsonModel<SendSms21VianetBlueCloudResult>, ISmsSubResult
{
#pragma warning disable IL2026 // Members annotated with 'RequiresUnreferencedCodeAttribute' require dynamic access otherwise can break functionality when trimming application code
#pragma warning disable IL3050 // Calling members annotated with 'RequiresDynamicCodeAttribute' may break functionality when AOT compiling.
    string DebuggerDisplay() => global::System.Text.Json.JsonSerializer.Serialize(this, SmsSenderJsonSerializerContext.Default.Options);
#pragma warning restore IL3050 // Calling members annotated with 'RequiresDynamicCodeAttribute' may break functionality when AOT compiling.
#pragma warning restore IL2026 // Members annotated with 'RequiresUnreferencedCodeAttribute' require dynamic access otherwise can break functionality when trimming application code

    /// <summary>
    /// 短信发送的 ID，用于后续查询
    /// </summary>
    [global::System.Text.Json.Serialization.JsonPropertyName("messageId")]
    public string? MessageId { get; set; }

    /// <summary>
    /// 发送时间
    /// </summary>
    [global::System.Text.Json.Serialization.JsonPropertyName("sendTime")]
    public string? SendTime { get; set; }

    /// <summary>
    /// 返回包含了短信发送的 MessageId 和发送时间 SendTime 的信息
    /// </summary>
    string? ISmsSubResult.GetRecord() => $"messageId: {MessageId}, sendTime: {SendTime}";
}
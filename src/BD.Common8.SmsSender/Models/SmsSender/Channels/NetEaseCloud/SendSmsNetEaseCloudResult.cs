using BD.Common8.SmsSender.Services.Implementation.SmsSender;
using System.Diagnostics;

namespace BD.Common8.SmsSender.Models.SmsSender.Channels.NetEaseCloud;

/// <summary>
/// 网易云短信发送结果
/// </summary>
[DebuggerDisplay("{DebuggerDisplay(),nq}")]
public sealed class SendSmsNetEaseCloudResult : NetEaseCloudResult<SendSmsNetEaseCloudResult>
{
#pragma warning disable IL2026 // Members annotated with 'RequiresUnreferencedCodeAttribute' require dynamic access otherwise can break functionality when trimming application code
#pragma warning disable IL3050 // Calling members annotated with 'RequiresDynamicCodeAttribute' may break functionality when AOT compiling.
    string DebuggerDisplay() => global::System.Text.Json.JsonSerializer.Serialize(this, SmsSenderJsonSerializerContext.Default.Options);
#pragma warning restore IL3050 // Calling members annotated with 'RequiresDynamicCodeAttribute' may break functionality when AOT compiling.
#pragma warning restore IL2026 // Members annotated with 'RequiresUnreferencedCodeAttribute' require dynamic access otherwise can break functionality when trimming application code

    /// <summary>
    /// 短信发送结果的消息内容
    /// </summary>
    [global::System.Text.Json.Serialization.JsonPropertyName(msg)]
    public string? Msg { get; set; }

    /// <summary>
    /// 短信发送结果的对象信息
    /// </summary>
    [global::System.Text.Json.Serialization.JsonPropertyName(obj)]
    public string? Obj { get; set; }

    /// <summary>
    /// 返回当前实例的详细记录
    /// </summary>
    protected override string? GetRecord() => $"code: {Code}, msg: {Msg}, obj: {Obj}";
}
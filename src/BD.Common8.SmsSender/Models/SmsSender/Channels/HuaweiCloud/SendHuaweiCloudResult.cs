namespace BD.Common8.SmsSender.Models.SmsSender.Channels.HuaweiCloud;

/// <summary>
/// 华为云短信发送结果
/// </summary>
public class SendHuaweiCloudResult : HuaweiCloudResult<SendHuaweiCloudResult>
{
    /// <summary>
    /// 短信发送结果详情信息
    /// </summary>
    [SystemTextJsonProperty("result")]
    public List<SendResult>? Result { get; set; }
}

/// <summary>
/// 华为云短信结果
/// </summary>
public class SendResult
{
    /// <summary>
    /// 消息 MsgId
    /// </summary>
    [SystemTextJsonProperty("smsMsgId")]
    public string? SmsMsgId { get; set; }

    /// <summary>
    /// 发送时间
    /// </summary>
    [SystemTextJsonProperty("createTime")]
    public string? CreateTime { get; set; }
}

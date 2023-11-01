namespace BD.Common8.SmsSender.Models.SmsSender.Channels.AlibabaCloud;

/// <summary>
/// 阿里云短信发送结果
/// </summary>
public sealed class SendSmsAlibabaCloudResult : AlibabaCloudResult<SendSmsAlibabaCloudResult>, ISmsSubResult
{
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
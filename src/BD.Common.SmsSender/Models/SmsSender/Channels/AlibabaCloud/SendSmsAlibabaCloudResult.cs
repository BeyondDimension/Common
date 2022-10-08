namespace BD.Common.Models.SmsSender.Channels.AlibabaCloud;

public sealed class SendSmsAlibabaCloudResult : AlibabaCloudResult<SendSmsAlibabaCloudResult>, ISmsSubResult
{
    public string? Message { get; set; }

    public string? RequestId { get; set; }

    public string? BizId { get; set; }

    string? ISmsSubResult.GetRecord()
        => $"code: {Code}, message: {Message}, requestId: {RequestId}, bizId: {BizId}";
}
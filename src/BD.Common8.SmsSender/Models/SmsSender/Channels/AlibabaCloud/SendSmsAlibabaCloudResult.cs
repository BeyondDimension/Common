namespace BD.Common8.SmsSender.Models.SmsSender.Channels.AlibabaCloud;

#pragma warning disable SA1600 // Elements should be documented

public sealed class SendSmsAlibabaCloudResult : AlibabaCloudResult<SendSmsAlibabaCloudResult>, ISmsSubResult
{
    public string? Message { get; set; }

    public string? RequestId { get; set; }

    public string? BizId { get; set; }

    string? ISmsSubResult.GetRecord()
        => $"code: {Code}, message: {Message}, requestId: {RequestId}, bizId: {BizId}";
}
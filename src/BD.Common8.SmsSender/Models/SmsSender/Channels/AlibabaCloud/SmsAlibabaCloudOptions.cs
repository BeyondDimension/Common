namespace BD.Common8.SmsSender.Models.SmsSender.Channels.AlibabaCloud;

#pragma warning disable SA1600 // Elements should be documented

public class SmsAlibabaCloudOptions : ChannelSmsOptions
{
    /// <summary>
    /// 平台分配的 AppKey
    /// </summary>
    public string? AccessKeyId { get; set; }

    /// <summary>
    /// 平台分配的 AppSecret
    /// </summary>
    public string? AccessKeySecret { get; set; }

    /// <summary>
    /// 短信内容签名，正式环境后申请 App 名称签名，此处填写 App 名称
    /// </summary>
    public string? SignName { get; set; }

    public SmsOptionsTemplateId<string>[]? Templates { get; set; }

    /// <summary>
    /// (默认)短信模板 Id，发送国际/港澳台消息时，请使用国际/港澳台短信模版
    /// </summary>
    public string? DefaultTemplate { get; set; }

    public override bool IsValid()
    {
        return base.IsValid() && !string.IsNullOrWhiteSpace(AccessKeyId) &&
            !string.IsNullOrWhiteSpace(AccessKeySecret) &&
            !string.IsNullOrWhiteSpace(SignName) &&
            (Templates.Any_Nullable() || DefaultTemplate != default);
    }
}
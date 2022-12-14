namespace BD.Common.Models.SmsSender.Channels.NetEaseCloud;

public class SmsNetEaseCloudOptions : ChannelSmsOptions
{
    /// <summary>
    /// 开发者平台分配的appkey
    /// </summary>
    public string? AppKey { get; set; }

    /// <summary>
    /// 开发者平台分配的appSecret
    /// </summary>
    public string? AppSecret { get; set; }

    /// <summary>
    /// 开发者平台分配的模板标志
    /// </summary>
    public SmsOptionsTemplateId<int>[]? Templates { get; set; }

    public int? DefaultTemplate { get; set; }

    public override bool IsValid()
    {
        return base.IsValid() && !string.IsNullOrWhiteSpace(AppKey) &&
            !string.IsNullOrWhiteSpace(AppSecret) &&
            (Templates.Any_Nullable() || DefaultTemplate != default);
    }
}
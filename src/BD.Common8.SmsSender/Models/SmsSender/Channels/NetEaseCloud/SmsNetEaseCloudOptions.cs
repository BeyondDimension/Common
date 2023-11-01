namespace BD.Common8.SmsSender.Models.SmsSender.Channels.NetEaseCloud;

/// <summary>
/// 提供给网易云短信配置选项
/// </summary>
public class SmsNetEaseCloudOptions : ChannelSmsOptions
{
    /// <summary>
    /// 开发者平台分配的 AppKey
    /// </summary>
    public string? AppKey { get; set; }

    /// <summary>
    /// 开发者平台分配的 AppSecret
    /// </summary>
    public string? AppSecret { get; set; }

    /// <summary>
    /// 开发者平台分配的模板标志
    /// </summary>
    public SmsOptionsTemplateId<int>[]? Templates { get; set; }

    /// <summary>
    /// 默认模板
    /// </summary>
    public int? DefaultTemplate { get; set; }

    /// <summary>
    /// 验证选项是否有效
    /// </summary>
    public override bool IsValid()
    {
        return base.IsValid() && !string.IsNullOrWhiteSpace(AppKey) &&
            !string.IsNullOrWhiteSpace(AppSecret) &&
            (Templates.Any_Nullable() || DefaultTemplate != default);
    }
}
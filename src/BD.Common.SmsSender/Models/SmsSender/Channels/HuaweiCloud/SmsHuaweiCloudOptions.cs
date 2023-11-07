namespace BD.Common.Models.SmsSender.Channels.HuaWeiCloud;

public class SmsHuaWeiCloudOptions : ChannelSmsOptions
{
    /// <summary>
    /// APP 接入地址(在控制台"应用管理"页面获取)+ 接口访问 URI
    /// </summary>
    public string? ApiAddress { get; set; }

    public string? AppKey { get; set; }

    public string? AppSecret { get; set; }

    /// <summary>
    /// 签名名称
    /// </summary>
    public string? Signature { get; set; }

    /// <summary>
    /// 国内短信签名通道号
    /// </summary>
    public string? Sender { get; set; }

    /// <summary>
    /// 选填,短信状态报告接收地址,推荐使用域名,为空或者不填表示不接收状态报告
    /// </summary>
    public string? StatusCallBack { get; set; } = "";

    public SmsOptionsTemplateId<string>[]? Templates { get; set; }

    public string? DefaultTemplate { get; set; }

    public override bool IsValid()
    {
        return base.IsValid() && !string.IsNullOrWhiteSpace(AppKey) &&
            !string.IsNullOrWhiteSpace(AppSecret) &&
            !string.IsNullOrWhiteSpace(ApiAddress) &&
            !string.IsNullOrWhiteSpace(Sender) &&
            (Templates.Any_Nullable() || DefaultTemplate != default);
    }
}

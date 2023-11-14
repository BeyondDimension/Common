namespace BD.Common8.SmsSender.Models.SmsSender.Channels.HuaweiCloud;

/// <summary>
/// 提供给华为云短信配置选项
/// </summary>
public class SmsHuaweiCloudOptions : ChannelSmsOptions
{
    /// <summary>
    /// APP 接入地址(在控制台"应用管理"页面获取)+ 接口访问 URI
    /// </summary>
    public string? ApiAddress { get; set; }

    /// <summary>
    /// 华为云平台 AppKey
    /// </summary>
    public string? AppKey { get; set; }

    /// <summary>
    /// 华为云平台 AppSecret
    /// </summary>
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
    /// 选填，短信状态报告接收地址，推荐使用域名，为空或者不填表示不接收状态报告
    /// </summary>
    public string? StatusCallBack { get; set; } = "";

    /// <summary>
    /// 模板列表
    /// </summary>
    public SmsOptionsTemplateId<string>[]? Templates { get; set; }

    /// <summary>
    /// 默认模板
    /// </summary>
    public string? DefaultTemplate { get; set; }

    /// <summary>
    /// 验证配置
    /// </summary>
    /// <returns></returns>
    public override bool IsValid()
    {
        return base.IsValid() && !string.IsNullOrWhiteSpace(AppKey) &&
            !string.IsNullOrWhiteSpace(AppSecret) &&
            !string.IsNullOrWhiteSpace(ApiAddress) &&
            !string.IsNullOrWhiteSpace(Sender) &&
            (Templates.Any_Nullable() || DefaultTemplate != default);
    }
}

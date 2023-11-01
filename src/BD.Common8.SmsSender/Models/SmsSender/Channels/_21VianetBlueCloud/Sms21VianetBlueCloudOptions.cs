namespace BD.Common8.SmsSender.Models.SmsSender.Channels._21VianetBlueCloud;

/// <summary>
/// 提供给蓝云短信服务的配置选项
/// </summary>
public class Sms21VianetBlueCloudOptions : ChannelSmsOptions
{
    /// <summary>
    /// ccs account name
    /// </summary>
    public string? Account { get; set; }

    /// <summary>
    /// 密钥名称
    /// </summary>
    public string? KeyName { get; set; }

    /// <summary>
    /// 密钥
    /// </summary>
    public string? KeyValue { get; set; }

    /// <summary>
    /// 下发扩展码，两位纯数字
    /// </summary>
    public string ExtendCode { get; set; } = "08";

    /// <summary>
    /// 短信验证码值在模板中的变量名
    /// </summary>
    public string CodeTemplateKeyName { get; set; } = "code";

    /// <summary>
    /// 默认模板
    /// </summary>
    public string? DefaultTemplate { get; set; }

    /// <summary>
    /// 开发者平台分配的模板标志
    /// </summary>
    public SmsOptionsTemplateId<string>[]? Templates { get; set; }

    /// <summary>
    /// 检验选项是否有效
    /// </summary>
    public override bool IsValid()
    {
        return base.IsValid() && !string.IsNullOrWhiteSpace(Account) &&
            !string.IsNullOrWhiteSpace(ExtendCode) &&
            !string.IsNullOrWhiteSpace(KeyName) &&
            !string.IsNullOrWhiteSpace(KeyValue) &&
            (Templates.Any_Nullable() || DefaultTemplate != default);
    }
}
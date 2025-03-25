namespace BD.Common8.SmsSender.Models.SmsSender.Channels.TencentCloud;

public class SmsTencentCloudOptions : ChannelSmsOptions
{
    /// <summary>
    /// 平台分配的 SecretId
    /// </summary>
    public string? SecretId { get; set; }

    /// <summary>
    /// 平台分配的 SecretKey
    /// </summary>
    public string? SecretKey { get; set; }

    /// <summary>
    /// 短信 SdkAppId，在 短信控制台 添加应用后生成的实际 SdkAppId，示例如1400006666。
    /// 示例值：1400006666
    /// </summary>
    public string? SmsSdkAppId { get; set; }

    /// <summary>
    /// 发送国内短信该参数必填，且需填写签名内容而非签名ID。
    /// 发送国际/港澳台短信该参数非必填
    /// </summary>
    public string? SignName { get; set; }

    /// <summary>
    /// 短信模板列表
    /// </summary>
    public SmsOptionsTemplateId<string>[]? Templates { get; set; }

    /// <summary>
    /// (默认)短信模板 Id，发送国际/港澳台消息时，请使用国际/港澳台短信模版
    /// </summary>
    public string? DefaultTemplate { get; set; }

    /// <summary>
    /// 验证配置是否有效的方法，检查必要的属性不为空，并且短信模板列表不为空或默认模板不为默认值
    /// </summary>
    /// <returns></returns>
    public override bool IsValid()
    {
        return base.IsValid() && !string.IsNullOrWhiteSpace(SecretId) &&
            !string.IsNullOrWhiteSpace(SecretKey) &&
            !string.IsNullOrWhiteSpace(SmsSdkAppId) &&
            !string.IsNullOrWhiteSpace(SignName) &&
            (Templates.Any_Nullable() || DefaultTemplate != default);
    }
}

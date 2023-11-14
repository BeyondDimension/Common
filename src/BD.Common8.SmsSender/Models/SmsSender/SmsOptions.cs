namespace BD.Common8.SmsSender.Models.SmsSender;

/// <summary>
/// 提供短信相关配置选项
/// </summary>
public class SmsOptions
{
    /// <summary>
    /// 阿里云短信配置选项
    /// </summary>
    public SmsAlibabaCloudOptions? AlibabaCloud { get; set; }

    /// <summary>
    /// 网易云信短信配置选项
    /// </summary>
    public SmsNetEaseCloudOptions? NetEaseCloud { get; set; }

    /// <summary>
    /// 蓝云短信配置选项
    /// </summary>
    public Sms21VianetBlueCloudOptions? _21VianetBlueCloud { get; set; }

    /// <summary>
    /// 华为云短信配置
    /// </summary>
    public SmsHuaweiCloudOptions? HuaweiCloud { get; set; }

    /// <summary>
    /// 获取默认的短信提供商名称
    /// </summary>
    public static string? GetDefaultProviderName(SmsOptions? options)
    {
        if (options != null)
        {
            if (options._21VianetBlueCloud.HasValue())
                return nameof(_21VianetBlueCloud);
            if (options.AlibabaCloud.HasValue())
                return nameof(AlibabaCloud);
            if (options.NetEaseCloud.HasValue())
                return nameof(NetEaseCloud);
            if (options.HuaweiCloud.HasValue())
                return nameof(HuaweiCloud);
        }
        return null;
    }
}
namespace BD.Common8.SmsSender.Models.SmsSender;

#pragma warning disable SA1600 // Elements should be documented

public class SmsOptions
{
    public SmsAlibabaCloudOptions? AlibabaCloud { get; set; }

    public SmsNetEaseCloudOptions? NetEaseCloud { get; set; }

    public Sms21VianetBlueCloudOptions? _21VianetBlueCloud { get; set; }

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
        }
        return null;
    }
}
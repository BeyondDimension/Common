using BD.Common.Models.SmsSender.Channels._21VianetBlueCloud;
using BD.Common.Models.SmsSender.Channels.AlibabaCloud;
using BD.Common.Models.SmsSender.Channels.HuaweiCloud;
using BD.Common.Models.SmsSender.Channels.NetEaseCloud;

namespace BD.Common.Models.SmsSender;

public class SmsOptions
{
    public SmsAlibabaCloudOptions? AlibabaCloud { get; set; }

    public SmsNetEaseCloudOptions? NetEaseCloud { get; set; }

#pragma warning disable IDE1006 // 命名样式
    public Sms21VianetBlueCloudOptions? _21VianetBlueCloud { get; set; }
#pragma warning restore IDE1006 // 命名样式

    public SmsHuaweiCloudOptions? HuaweiCloud { get; set; }

    public static string? GetDefaultProviderName(SmsOptions? options)
    {
        if (options != null)
        {
            if (options._21VianetBlueCloud.HasValue()) return nameof(_21VianetBlueCloud);
            if (options.AlibabaCloud.HasValue()) return nameof(AlibabaCloud);
            if (options.NetEaseCloud.HasValue()) return nameof(NetEaseCloud);
            if (options.HuaweiCloud.HasValue()) return nameof(HuaweiCloud);
        }
        return null;
    }
}
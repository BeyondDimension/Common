// https://bcssstorage.blob.core.chinacloudapi.cn/docs/CCS/%E8%93%9D%E4%BA%91%E7%94%A8%E6%88%B7%E8%BF%9E%E6%8E%A5%E6%9C%8D%E5%8A%A1%E6%8A%80%E6%9C%AF%E6%96%87%E6%A1%A3(%E7%9F%AD%E4%BF%A1%2B%E7%99%BB%E5%BD%95).pdf

namespace BD.Common.Models.SmsSender.Channels._21VianetBlueCloud;

public class SendSms21VianetBlueCloudResult : JsonModel<SendSms21VianetBlueCloudResult>, ISmsSubResult
{
    /// <summary>
    /// 短信发送的 ID，用于后续查询
    /// </summary>
#if __HAVE_N_JSON__
    [N_JsonProperty("messageId")]
#endif
#if !__NOT_HAVE_S_JSON__
    [S_JsonProperty("messageId")]
#endif
    public string? MessageId { get; set; }

    /// <summary>
    /// 发送时间
    /// </summary>
#if __HAVE_N_JSON__
    [N_JsonProperty("sendTime")]
#endif
#if !__NOT_HAVE_S_JSON__
    [S_JsonProperty("sendTime")]
#endif
    public string? SendTime { get; set; }

    string? ISmsSubResult.GetRecord() => $"messageId: {MessageId}, sendTime: {SendTime}";
}
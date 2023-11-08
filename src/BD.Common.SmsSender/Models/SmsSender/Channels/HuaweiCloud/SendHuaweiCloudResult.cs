namespace BD.Common.Models.SmsSender.Channels.HuaweiCloud;

public class SendHuaweiCloudResult : HuaweiCloudResult<SendHuaweiCloudResult>
{
#if __HAVE_N_JSON__
    [N_JsonProperty("result")]
#endif
#if !__NOT_HAVE_S_JSON__
    [S_JsonProperty("result")]
#endif
    public List<SendResult>? Result { get; set; }
}

public class SendResult
{
#if __HAVE_N_JSON__
    [N_JsonProperty("smsMsgId")]
#endif
#if !__NOT_HAVE_S_JSON__
    [S_JsonProperty("smsMsgId")]
#endif
    public string? SmsMsgId { get; set; }

#if __HAVE_N_JSON__
    [N_JsonProperty("createTime")]
#endif
#if !__NOT_HAVE_S_JSON__
    [S_JsonProperty("createTime")]
#endif
    public string? CreateTime { get; set; }
}

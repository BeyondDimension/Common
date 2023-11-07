namespace BD.Common.Models.SmsSender.Channels.HuaWeiCloud;

public class SendHuaWeiCloudResult : HuaWeiCloudResult<SendHuaWeiCloudResult>, ISmsSubResult
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

    string? ISmsSubResult.GetRecord() => $"smsMsgId: {SmsMsgId}, createTime: {CreateTime}";
}

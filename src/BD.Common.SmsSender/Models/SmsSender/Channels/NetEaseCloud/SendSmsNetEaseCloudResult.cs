namespace BD.Common.Models.SmsSender.Channels.NetEaseCloud;

public sealed class SendSmsNetEaseCloudResult : NetEaseCloudResult<SendSmsNetEaseCloudResult>
{
#if __HAVE_N_JSON__
    [N_JsonProperty(msg)]
#endif
#if !__NOT_HAVE_S_JSON__
    [S_JsonProperty(msg)]
#endif
    public string? Msg { get; set; }

#if __HAVE_N_JSON__
    [N_JsonProperty(obj)]
#endif
#if !__NOT_HAVE_S_JSON__
    [S_JsonProperty(obj)]
#endif
    public string? Obj { get; set; }

    protected override string? GetRecord() => $"code: {Code}, msg: {Msg}, obj: {Obj}";
}
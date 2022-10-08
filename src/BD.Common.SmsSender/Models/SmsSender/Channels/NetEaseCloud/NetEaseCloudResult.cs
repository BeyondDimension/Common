namespace BD.Common.Models.SmsSender.Channels.NetEaseCloud;

public class NetEaseCloudResult<T> : JsonModel<T>, ISmsSubResult where T : NetEaseCloudResult<T>
{
#if __HAVE_N_JSON__
    [N_JsonProperty(code)]
#endif
#if !__NOT_HAVE_S_JSON__
    [S_JsonProperty(code)]
#endif
    public SendSmsNetEaseCloudResponseCode Code { get; set; }

    protected const string code = nameof(code);
    protected const string msg = nameof(msg);
    protected const string obj = nameof(obj);

    public virtual bool IsOK() => Code == SendSmsNetEaseCloudResponseCode.操作成功;

    public virtual bool IsCheckSmsFail() => Code == SendSmsNetEaseCloudResponseCode.验证失败;

    protected virtual string? GetRecord() => $"code: {Code}";

    string? ISmsSubResult.GetRecord() => GetRecord();
}

public sealed class NetEaseCloudResult : NetEaseCloudResult<NetEaseCloudResult>
{
}
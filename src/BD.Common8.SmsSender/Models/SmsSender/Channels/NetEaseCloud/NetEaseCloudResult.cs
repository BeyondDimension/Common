namespace BD.Common8.SmsSender.Models.SmsSender.Channels.NetEaseCloud;

#pragma warning disable SA1600 // Elements should be documented

public class NetEaseCloudResult<T> : JsonModel<T>, ISmsSubResult where T : NetEaseCloudResult<T>
{
    [SystemTextJsonProperty(code)]
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
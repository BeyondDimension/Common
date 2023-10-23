namespace BD.Common8.SmsSender.Models.SmsSender.Channels.NetEaseCloud;

#pragma warning disable SA1600 // Elements should be documented

public sealed class SendSmsNetEaseCloudResult : NetEaseCloudResult<SendSmsNetEaseCloudResult>
{
    [SystemTextJsonProperty(msg)]
    public string? Msg { get; set; }

    [SystemTextJsonProperty(obj)]
    public string? Obj { get; set; }

    protected override string? GetRecord() => $"code: {Code}, msg: {Msg}, obj: {Obj}";
}
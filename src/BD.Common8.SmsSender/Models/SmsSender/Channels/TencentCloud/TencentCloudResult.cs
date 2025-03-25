namespace BD.Common8.SmsSender.Models.SmsSender.Channels.TencentCloud;

public class TencentCloudResult<T> : JsonModel, ISmsSubResult where T : JsonModel, ITencentCloud
{
    [SystemTextJsonProperty("Response")]
    [NewtonsoftJsonProperty("Response")]
    public required T Response { get; set; }

    /// <summary>
    /// 返回包含了状态、消息内容、 唯一标识符、业务标识符的信息
    /// </summary>
    string? ISmsSubResult.GetRecord()
        => $"requestId: {Response.RequestId}";
}

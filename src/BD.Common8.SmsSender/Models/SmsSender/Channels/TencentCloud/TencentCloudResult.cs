using BD.Common8.Models.Abstractions;
using BD.Common8.SmsSender.Models.SmsSender.Abstractions;
using System.Text.Json.Serialization;

namespace BD.Common8.SmsSender.Models.SmsSender.Channels.TencentCloud;

public class TencentCloudResult<T> : JsonModel<TencentCloudResult<T>>, ISmsSubResult, IJsonSerializerContext where T : ITencentCloud
{
    static JsonSerializerContext IJsonSerializerContext.Default => global::BD.Common8.SmsSender.Services.Implementation.SmsSender.SmsSenderJsonSerializerContext.Default;

    [global::System.Text.Json.Serialization.JsonPropertyName("Response")]
    [global::Newtonsoft.Json.JsonProperty("Response")]
    public required T Response { get; set; }

    /// <summary>
    /// 返回包含了状态、消息内容、 唯一标识符、业务标识符的信息
    /// </summary>
    string? ISmsSubResult.GetRecord()
        => $"requestId: {Response.RequestId}";
}

using BD.Common8.Models.Abstractions;

namespace BD.Common8.SmsSender.Models.SmsSender.Channels.TencentCloud;

public class SendSmsTencentCloudResult : JsonModel, ITencentCloud
{
    /// <summary>
    /// 短信发送状态。
    /// </summary>
    [global::System.Text.Json.Serialization.JsonPropertyName("SendStatusSet")]
    [global::Newtonsoft.Json.JsonProperty("SendStatusSet")]
    public required SendStatus[] SendStatusSet { get; set; }

    /// <summary>
    /// 唯一请求 ID，由服务端生成，每次请求都会返回（若请求因其他原因未能抵达服务端，则该次请求不会获得 RequestId）。定位问题时需要提供该次请求的 RequestId。
    /// </summary>
    [global::System.Text.Json.Serialization.JsonPropertyName("RequestId")]
    public required string RequestId { get; set; }

    public bool IsOk()
    {
        return SendStatusSet.All(x => x.Code == "Ok");
    }
}

public interface ITencentCloud
{
    string RequestId { get; set; }

    bool IsOk();
}

public class SendStatus
{
    /// <summary>
    /// 发送流水号。
    /// </summary>
    [global::System.Text.Json.Serialization.JsonPropertyName("SerialNo")]
    [global::Newtonsoft.Json.JsonProperty("SerialNo")]
    public required string SerialNo { get; set; }

    /// <summary>
    /// 手机号码，E.164标准，+[国家或地区码][手机号] ，示例如：+8618501234444， 其中前面有一个+号 ，86为国家码，18501234444为手机号。
    /// </summary>
    [global::System.Text.Json.Serialization.JsonPropertyName("PhoneNumber")]
    [global::Newtonsoft.Json.JsonProperty("PhoneNumber")]
    public required string PhoneNumber { get; set; }

    /// <summary>
    /// 计费条数，计费规则请查询 [计费策略](https://cloud.tencent.com/document/product/382/36135)。
    /// </summary>
    [global::System.Text.Json.Serialization.JsonPropertyName("Fee")]
    [global::Newtonsoft.Json.JsonProperty("Fee")]
    public ulong? Fee { get; set; }

    /// <summary>
    /// 用户 session 内容。
    /// </summary>
    [global::System.Text.Json.Serialization.JsonPropertyName("SessionContext")]
    [global::Newtonsoft.Json.JsonProperty("SessionContext")]
    public required string SessionContext { get; set; }

    /// <summary>
    /// 短信请求错误码，具体含义请参考 [错误码](https://cloud.tencent.com/document/api/382/55981#6.-.E9.94.99.E8.AF.AF.E7.A0.81)，发送成功返回 "Ok"。
    /// </summary>
    [global::System.Text.Json.Serialization.JsonPropertyName("Code")]
    [global::Newtonsoft.Json.JsonProperty("Code")]
    public required string Code { get; set; }

    /// <summary>
    /// 短信请求错误码描述。
    /// </summary>
    [global::System.Text.Json.Serialization.JsonPropertyName("Message")]
    [global::Newtonsoft.Json.JsonProperty("Message")]
    public required string Message { get; set; }

    /// <summary>
    /// 国家码或地区码，例如 CN、US 等，对于未识别出国家码或者地区码，默认返回 DEF，具体支持列表请参考 [国际/港澳台短信价格总览](https://cloud.tencent.com/document/product/382/18051)。
    /// </summary>
    [global::System.Text.Json.Serialization.JsonPropertyName("IsoCode")]
    [global::Newtonsoft.Json.JsonProperty("IsoCode")]
    public required string IsoCode { get; set; }
}


namespace BD.Common8.SmsSender.Models.SmsSender.Channels.TencentCloud;

public class SendSmsTencentCloudResult : JsonModel, ITencentCloud
{
    /// <summary>
    /// 短信发送状态。
    /// </summary>
    [SystemTextJsonProperty("SendStatusSet")]
    [NewtonsoftJsonProperty("SendStatusSet")]
    public required SendStatus[] SendStatusSet { get; set; }

    /// <summary>
    /// 唯一请求 ID，由服务端生成，每次请求都会返回（若请求因其他原因未能抵达服务端，则该次请求不会获得 RequestId）。定位问题时需要提供该次请求的 RequestId。
    /// </summary>
    [SystemTextJsonProperty("RequestId")]
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
    [SystemTextJsonProperty("SerialNo")]
    [NewtonsoftJsonProperty("SerialNo")]
    public required string SerialNo { get; set; }

    /// <summary>
    /// 手机号码，E.164标准，+[国家或地区码][手机号] ，示例如：+8618501234444， 其中前面有一个+号 ，86为国家码，18501234444为手机号。
    /// </summary>
    [SystemTextJsonProperty("PhoneNumber")]
    [NewtonsoftJsonProperty("PhoneNumber")]
    public required string PhoneNumber { get; set; }

    /// <summary>
    /// 计费条数，计费规则请查询 [计费策略](https://cloud.tencent.com/document/product/382/36135)。
    /// </summary>
    [SystemTextJsonProperty("Fee")]
    [NewtonsoftJsonProperty("Fee")]
    public ulong? Fee { get; set; }

    /// <summary>
    /// 用户 session 内容。
    /// </summary>
    [SystemTextJsonProperty("SessionContext")]
    [NewtonsoftJsonProperty("SessionContext")]
    public required string SessionContext { get; set; }

    /// <summary>
    /// 短信请求错误码，具体含义请参考 [错误码](https://cloud.tencent.com/document/api/382/55981#6.-.E9.94.99.E8.AF.AF.E7.A0.81)，发送成功返回 "Ok"。
    /// </summary>
    [SystemTextJsonProperty("Code")]
    [NewtonsoftJsonProperty("Code")]
    public required string Code { get; set; }

    /// <summary>
    /// 短信请求错误码描述。
    /// </summary>
    [SystemTextJsonProperty("Message")]
    [NewtonsoftJsonProperty("Message")]
    public required string Message { get; set; }

    /// <summary>
    /// 国家码或地区码，例如 CN、US 等，对于未识别出国家码或者地区码，默认返回 DEF，具体支持列表请参考 [国际/港澳台短信价格总览](https://cloud.tencent.com/document/product/382/18051)。
    /// </summary>
    [SystemTextJsonProperty("IsoCode")]
    [NewtonsoftJsonProperty("IsoCode")]
    public required string IsoCode { get; set; }
}


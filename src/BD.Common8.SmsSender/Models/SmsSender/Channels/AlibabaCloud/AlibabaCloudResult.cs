namespace BD.Common8.SmsSender.Models.SmsSender.Channels.AlibabaCloud;

/// <summary>
/// 提供阿里云 API 请求的返回结果
/// </summary>
/// <typeparam name="T"></typeparam>
public class AlibabaCloudResult<T> : JsonModel<T> where T : AlibabaCloudResult<T>
{
    /// <summary>
    /// 状态码
    /// https://help.aliyun.com/document_detail/55323.html
    /// </summary>
    public string? Code { get; set; }

    /// <summary>
    /// 判断结果是否为 "OK"
    /// </summary>
    /// <returns>如果结果为"OK"，返回 <see langword="true"/> ；否则返回 <see langword="false"/> </returns>
    public virtual bool IsOK() => Code?.Equals("OK", StringComparison.OrdinalIgnoreCase) ?? false;
}
namespace BD.Common8.SmsSender.Models.SmsSender.Channels.HuaweiCloud;

/// <summary>
/// 提供华为云 API 请求的返回结果
/// </summary>
/// <typeparam name="T"></typeparam>
public class HuaweiCloudResult<T> : JsonModel<T>, ISmsSubResult where T : HuaweiCloudResult<T>
{
    /// <summary>
    /// 状态码
    /// </summary>
    [SystemTextJsonProperty("code")]
    public string? Code { get; set; }

    /// <summary>
    /// 状态描述
    /// </summary>
    [SystemTextJsonProperty("description")]
    public string? Description { get; set; }

    /// <summary>
    /// 判断返回结果是否成功
    /// </summary>
    public virtual bool IsOK() => Description?.Equals("Success", StringComparison.OrdinalIgnoreCase) ?? false;

    /// <summary>
    /// 返回包含状态码，状态描述属性值的信息
    /// </summary>
    /// <returns></returns>
    protected virtual string? GetRecord() => $"code: {Code}, description {Description}";

    /// <inheritdoc />
    string? ISmsSubResult.GetRecord() => GetRecord();
}

namespace BD.Common8.SmsSender.Models.SmsSender.Channels.AlibabaCloud;

#pragma warning disable SA1600 // Elements should be documented

public class AlibabaCloudResult<T> : JsonModel<T> where T : AlibabaCloudResult<T>
{
    /// <summary>
    /// https://help.aliyun.com/document_detail/55323.html
    /// </summary>
    public string? Code { get; set; }

    public virtual bool IsOK() => Code?.Equals("OK", StringComparison.OrdinalIgnoreCase) ?? false;
}
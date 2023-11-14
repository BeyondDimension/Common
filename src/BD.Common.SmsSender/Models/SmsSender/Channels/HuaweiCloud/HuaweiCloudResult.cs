namespace BD.Common.Models.SmsSender.Channels.HuaweiCloud;

public class HuaweiCloudResult<T> : JsonModel<T>, ISmsSubResult where T : HuaweiCloudResult<T>
{
#if __HAVE_N_JSON__
    [N_JsonProperty("code")]
#endif
#if !__NOT_HAVE_S_JSON__
    [S_JsonProperty("code")]
#endif
    public string? Code { get; set; }

#if __HAVE_N_JSON__
    [N_JsonProperty("description")]
#endif
#if !__NOT_HAVE_S_JSON__
    [S_JsonProperty("description")]
#endif
    public string? Description { get; set; }

    public virtual bool IsOK() => Description?.Equals("Success", StringComparison.OrdinalIgnoreCase) ?? false;

    protected virtual string? GetRecord() => $"code: {Code}, description {Description}";

    string? ISmsSubResult.GetRecord() => GetRecord();
}

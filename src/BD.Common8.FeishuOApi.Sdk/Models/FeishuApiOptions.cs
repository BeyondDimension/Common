namespace BD.Common8.FeishuOApi.Sdk.Models;

/// <summary>
/// 飞书开放平台配置
/// </summary>
public sealed record class FeishuApiOptions
{
    /// <summary>
    /// 飞书 WebHook Id
    /// </summary>
    public string? HookId { get; set; }
}
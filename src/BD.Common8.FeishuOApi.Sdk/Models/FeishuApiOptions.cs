using System.Diagnostics;

namespace BD.Common8.FeishuOApi.Sdk.Models;

/// <summary>
/// 飞书开放平台配置
/// </summary>
[DebuggerDisplay("{DebuggerDisplay(),nq}")]
public sealed record class FeishuApiOptions
{
    string DebuggerDisplay() => $"HookId: {HookId}, ServerTag: {ServerTag}";

    /// <summary>
    /// 飞书 WebHook Id
    /// </summary>
    public string? HookId { get; set; }

    /// <summary>
    /// 服务标识
    /// </summary>
    public string? ServerTag { get; set; }
}
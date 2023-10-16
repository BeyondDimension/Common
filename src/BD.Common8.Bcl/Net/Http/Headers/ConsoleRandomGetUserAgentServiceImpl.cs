using static System.Net.Http.Headers.UserAgentConstants;

namespace System.Net.Http.Headers;

/// <summary>
/// 用于控制台程序的 <see cref="IRandomGetUserAgentService"/>
/// </summary>
public class ConsoleRandomGetUserAgentServiceImpl : IRandomGetUserAgentService
{
    /// <summary>
    /// User-Agent 字符串
    /// </summary>
    public string? UserAgent { get; set; }

    /// <summary>
    /// 提供用于随机选择的 User-Agent 字符串数组
    /// </summary>
    public virtual string[] UserAgents { get; } = [Win10EdgeLatest, Win10ChromeLatest];

    /// <inheritdoc/>
    public virtual string GetUserAgent()
    {
        UserAgent ??= GetRandomUserAgent(UserAgents);
        return UserAgent;
    }

    /// <summary>
    /// 刷新随机 User-Agent 字符串
    /// </summary>
    public virtual void RefreshUserAgent() => UserAgent = GetRandomUserAgent(UserAgents);
}

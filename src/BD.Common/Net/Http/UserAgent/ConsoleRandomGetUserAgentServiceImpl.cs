using static System.Net.Http.IRandomGetUserAgentService;

// ReSharper disable once CheckNamespace
namespace System.Net.Http;

public class ConsoleRandomGetUserAgentServiceImpl : IRandomGetUserAgentService
{
    public string? UserAgent { get; set; }

    public virtual string[] UserAgents { get; } = new[] { Win10EdgeLatest, Win10ChromeLatest };

    public virtual string GetUserAgent()
    {
        UserAgent ??= GetRandomUserAgent(UserAgents);
        return UserAgent;
    }

    public virtual void RefreshUserAgent() => UserAgent = GetRandomUserAgent(UserAgents);
}

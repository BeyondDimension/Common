using static System.Net.Http.IRandomGetUserAgentService;

// ReSharper disable once CheckNamespace
namespace BD.Common.Services.Implementation;

/// <summary>
/// 用于服务端的 UserAgent
/// </summary>
public class ServerGetUserAgentServiceImpl : IRandomGetUserAgentService
{
    readonly IHttpContextAccessor? accessor;
    readonly PuppeteerService? puppeteerService;

    public ServerGetUserAgentServiceImpl(IServiceProvider s)
    {
        if (s.GetService<IPuppeteerService>() is PuppeteerService puppeteerService)
        {
            this.puppeteerService = puppeteerService;
        }
        else
        {
            accessor = s.GetService<IHttpContextAccessor>();
        }
    }

    public virtual string[] UserAgents { get; } = new[] { Win10EdgeLatest, Win10ChromeLatest };

    public string GetUserAgent()
    {
        // 当使用 Puppeteer 时，统一使用浏览器的 UA
        if (puppeteerService != null) return puppeteerService.GetUserAgent();

        var ctx = accessor?.HttpContext;
        if (ctx != null)
        {
            // 生成一个随机的 UA 记录在 HttpContext 中，使单个请求使用同一个随机的值
            const string HttpContextItemKey = "RandomGetUserAgent";
            if (ctx.Items[HttpContextItemKey] is string value)
            {
                return value;
            }
            else
            {
                value = GetRandomUserAgent(UserAgents);
                ctx.Items[HttpContextItemKey] = value;
                return value;
            }
        }

        return GetRandomUserAgent(UserAgents);
    }

    static readonly string[] blacklist = new[] { "http-client", "Apache", "HttpClient" };

    public static bool IsMatchBlacklist(string userAgent)
    {
        foreach (var item in blacklist)
        {
            if (userAgent.Contains(item, StringComparison.OrdinalIgnoreCase))
            {
                return true;
            }
        }
        return false;
    }

    public static bool IsMatchBlacklist(HttpRequest request)
    {
        var userAgent = request.UserAgent();
        var r = string.IsNullOrWhiteSpace(userAgent) || IsMatchBlacklist(userAgent);
        return r;
    }
}

namespace BD.Common8.AspNetCore.Services.Implementation;

#pragma warning disable SA1600 // Elements should be documented

/// <summary>
/// 用于服务端的 UserAgent
/// </summary>
public class ServerGetUserAgentServiceImpl(IServiceProvider s) : IRandomGetUserAgentService
{
    readonly IHttpContextAccessor? accessor = s.GetService<IHttpContextAccessor>();

    public virtual string[] UserAgents { get; } = [UserAgentConstants.Win10EdgeLatest, UserAgentConstants.Win10ChromeLatest];

    public string GetUserAgent()
    {
        //// 当使用 Puppeteer 时，统一使用浏览器的 UA
        //if (puppeteerService != null) return puppeteerService.GetUserAgent();

        var ctx = accessor?.HttpContext;
        if (ctx != null)
        {
            // 生成一个随机的 UA 记录在 HttpContext 中，使单个请求使用同一个随机的值
            const string HttpContextItemKey = "RandomGetUserAgent";
            if (ctx.Items[HttpContextItemKey] is string value)
                return value;
            else
            {
                value = UserAgentConstants.GetRandomUserAgent(UserAgents);
                ctx.Items[HttpContextItemKey] = value;
                return value;
            }
        }

        return UserAgentConstants.GetRandomUserAgent(UserAgents);
    }

    static readonly string[] blacklist = ["http-client", "Apache", "HttpClient"];

    public static bool IsMatchBlacklist(string userAgent)
    {
        foreach (var item in blacklist)
            if (userAgent.Contains(item, StringComparison.OrdinalIgnoreCase))
                return true;
        return false;
    }

    public static bool IsMatchBlacklist(HttpRequest request)
    {
        var userAgent = request.UserAgent();
        var r = string.IsNullOrWhiteSpace(userAgent) || IsMatchBlacklist(userAgent);
        return r;
    }
}

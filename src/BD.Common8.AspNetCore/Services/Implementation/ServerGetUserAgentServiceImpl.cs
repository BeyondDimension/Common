namespace BD.Common8.AspNetCore.Services.Implementation;

/// <summary>
/// 用于服务端的 UserAgent
/// </summary>
public class ServerGetUserAgentServiceImpl(IServiceProvider s) : IRandomGetUserAgentService
{
    /// <summary>
    /// HTTP 上下文访问器，用于获取当前请求的上下文信息
    /// </summary>
    readonly IHttpContextAccessor? accessor = s.GetService<IHttpContextAccessor>();

    /// <summary>
    /// 可用的 UserAgent 列表
    /// </summary>
    public virtual string[] UserAgents { get; } = [UserAgentConstants.Win10EdgeLatest, UserAgentConstants.Win10ChromeLatest];

    /// <summary>
    /// 获取随机的 UserAgent
    /// </summary>
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

    /// <summary>
    /// 用户代理黑名单数组
    /// </summary>
    static readonly string[] blacklist = ["http-client", "Apache", "HttpClient"];

    /// <summary>
    /// 判断指定的 UserAgent 是否匹配黑名单，如果匹配黑名单返回 <see langword="true"/>；否则为 <see langword="false"/>
    /// </summary>
    public static bool IsMatchBlacklist(string userAgent)
    {
        foreach (var item in blacklist)
            if (userAgent.Contains(item, StringComparison.OrdinalIgnoreCase))
                return true;
        return false;
    }

    /// <summary>
    /// 判断指定的 HttpRequest 的 UserAgent 是否匹配黑名单，如果匹配黑名单返回 <see langword="true"/>；否则为 <see langword="false"/>
    /// </summary>
    public static bool IsMatchBlacklist(HttpRequest request)
    {
        var userAgent = request.UserAgent();
        var r = string.IsNullOrWhiteSpace(userAgent) || IsMatchBlacklist(userAgent);
        return r;
    }
}

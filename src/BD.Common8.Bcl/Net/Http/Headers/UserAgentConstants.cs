namespace System.Net.Http.Headers;

/// <summary>
/// User-Agent 首部包含了一个特征字符串，用来让网络协议的对端来识别发起请求的用户代理软件的应用类型、操作系统、软件开发商以及版本号。
/// </summary>
public static partial class UserAgentConstants
{
    /// <summary>
    /// 默认值
    /// </summary>
    public const string Default = "Mozilla/5.0 (Windows NT 10.0; Win64; x64) AppleWebKit/537.36 (KHTML, like Gecko) Chrome/107.0.0.0 Safari/537.36 Edg/107.0.1418.62";

    /// <summary>
    /// Microsoft Lumia 950
    /// </summary>
    public const string Lumia950 = "Mozilla/5.0 (Windows Phone 10.0; Android 4.2.1; Microsoft; Lumia 950) AppleWebKit/537.36 (KHTML, like Gecko) Chrome/46.0.2486.0 Mobile Safari/537.36 Edge/14.14263 Edg/107.0.0.0";

    /// <summary>
    /// iPhone X 2017
    /// </summary>
    public const string Apple_iPhoneX = "Mozilla/5.0 (iPhone; CPU iPhone OS 13_2_3 like Mac OS X) AppleWebKit/605.1.15 (KHTML, like Gecko) Version/13.0.3 Mobile/15E148 Safari/604.1 Edg/107.0.0.0";

    /// <summary>
    /// iPad Pro
    /// </summary>
    public const string Apple_iPad_Pro = "Mozilla/5.0 (iPad; CPU OS 11_0 like Mac OS X) AppleWebKit/604.1.34 (KHTML, like Gecko) Version/11.0 Mobile/15A5341f Safari/604.1 Edg/107.0.0.0";

    /// <summary>
    /// Samsung Galaxy S20 Ultra 2020
    /// </summary>
    public const string Samsung_Galaxy_S20_Ultra = "Mozilla/5.0 (Linux; Android 10; SM-G981B) AppleWebKit/537.36 (KHTML, like Gecko) Chrome/80.0.3987.162 Mobile Safari/537.36 Edg/107.0.0.0";

    /// <summary>
    /// Steam, Valve
    /// </summary>
    public const string Steam = "Mozilla/5.0 (Windows; U; Windows NT 10.0; en-US; Valve Steam GameOverlay/1669935987; ) AppleWebKit/537.36 (KHTML, like Gecko) Chrome/85.0.4183.121 Safari/537.36";

    /// <summary>
    /// Windows 10 Edge
    /// </summary>
    public const string Win10EdgeLatest = "Mozilla/5.0 (Windows NT 10.0; Win64; x64) AppleWebKit/537.36 (KHTML, like Gecko) Chrome/110.0.0.0 Safari/537.36 Edg/110.0.1587.69";

    /// <summary>
    /// Windows 10 Chrome
    /// </summary>
    public const string Win10ChromeLatest = "Mozilla/5.0 (Windows NT 10.0; Win64; x64) AppleWebKit/537.36 (KHTML, like Gecko) Chrome/111.0.0.0 Safari/537.36";

    /// <summary>
    /// 从数组中获取一个随机值
    /// </summary>
    /// <param name="userAgents"></param>
    /// <returns></returns>
    public static string GetRandomUserAgent(string[] userAgents)
        => userAgents[Random2.Next(userAgents.Length)];
}

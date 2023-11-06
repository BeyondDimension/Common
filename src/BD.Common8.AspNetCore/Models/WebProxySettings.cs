#if DEBUG
namespace BD.Common8.AspNetCore.Models;

/// <summary>
/// Web 代理设置
/// </summary>
[Obsolete("use System.Net.LocalClashWebProxy", true)]
public static partial class WebProxySettings
{
    /// <summary>
    /// 获取本地 Clash 代理
    /// </summary>
    /// <returns></returns>
    public static IWebProxy GetLocalClash() => new WebProxy(
        LocalClashAddress, LocalClashPort);

    /// <summary>
    /// 本地 Clash 代理地址和端口
    /// </summary>
    public const string LocalClash = $"{LocalClashAddress}:{LocalClashPortStr}";

    /// <summary>
    /// 本地 Clash 代理地址
    /// </summary>
    public const string LocalClashAddress = "socks5://127.0.0.1";

    /// <summary>
    /// 本地 Clash 代理端口
    /// </summary>
    public const int LocalClashPort = 7890;

    /// <summary>
    /// 本地 Clash 代理端口字符串
    /// </summary>
    public const string LocalClashPortStr = "7890";
}
#endif
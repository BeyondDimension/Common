#if DEBUG
namespace BD.Common8.AspNetCore.Models;

#pragma warning disable SA1600 // Elements should be documented

[Obsolete("use System.Net.LocalClashWebProxy", true)]
public static partial class WebProxySettings
{
    /// <summary>
    /// 获取本地 Clash 代理
    /// </summary>
    /// <returns></returns>
    public static IWebProxy GetLocalClash() => new WebProxy(
        LocalClashAddress, LocalClashPort);

    public const string LocalClash = $"{LocalClashAddress}:{LocalClashPortStr}";
    public const string LocalClashAddress = "socks5://127.0.0.1";

    public const int LocalClashPort = 7890;
    public const string LocalClashPortStr = "7890";
}
#endif
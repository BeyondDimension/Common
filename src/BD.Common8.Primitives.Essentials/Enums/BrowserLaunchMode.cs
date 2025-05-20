namespace BD.Common8.Essentials.Enums;

/// <summary>
/// 打开浏览器的启动类型
/// </summary>
public enum BrowserLaunchMode : byte
{
    /// <summary>
    /// 启动优化的系统浏览器，并留在你的应用程序内。(Chrome Custom Tabs 和 SFSafariViewController)
    /// </summary>
    SystemPreferred,

    /// <summary>
    /// 使用默认的外部启动器，在应用程序之外打开浏览器
    /// </summary>
    External,
}
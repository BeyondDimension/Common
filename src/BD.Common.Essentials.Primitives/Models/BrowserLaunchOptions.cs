namespace BD.Common.Models;

/// <summary>
/// 打开浏览器的可选设置
/// </summary>
public sealed class BrowserLaunchOptions
{
    /// <summary>
    /// 背景工具条的首选颜色
    /// </summary>
    public GdiPlusColor? PreferredToolbarColor { get; set; }

    /// <summary>
    /// 浏览器上控件的首选颜色
    /// </summary>
    public GdiPlusColor? PreferredControlColor { get; set; }

    /// <summary>
    /// 浏览器的启动类型
    /// </summary>
    public BrowserLaunchMode LaunchMode { get; set; } = BrowserLaunchMode.SystemPreferred;

    /// <summary>
    /// 标题显示的首选模式
    /// </summary>
    public BrowserTitleMode TitleMode { get; set; } = BrowserTitleMode.Default;

    /// <summary>
    /// 额外的启动标志，根据设备和启动模式可能生效，也可能不生效
    /// </summary>
    public BrowserLaunchFlags Flags { get; set; } = BrowserLaunchFlags.None;
}
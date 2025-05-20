using BD.Common8.Essentials.Enums;
using System.Drawing;

namespace BD.Common8.Essentials.Models.Abstractions;

/// <summary>
/// 打开浏览器的可选设置接口
/// </summary>
public interface IBrowserLaunchOptions
{
    /// <summary>
    /// 背景工具条的首选颜色
    /// </summary>
    Color? PreferredToolbarColor { get; set; }

    /// <summary>
    /// 浏览器上控件的首选颜色
    /// </summary>
    Color? PreferredControlColor { get; set; }

    /// <summary>
    /// 浏览器的启动类型的默认值
    /// </summary>
    const BrowserLaunchMode DefaultLaunchMode = BrowserLaunchMode.SystemPreferred;

    /// <summary>
    /// 浏览器的启动类型
    /// </summary>
    BrowserLaunchMode LaunchMode { get; set; }

    /// <summary>
    /// 标题显示的首选模式
    /// </summary>
    BrowserTitleMode TitleMode { get; set; }

    /// <summary>
    /// 额外的启动标志，根据设备和启动模式可能生效，也可能不生效
    /// </summary>
    BrowserLaunchFlags Flags { get; set; }
}

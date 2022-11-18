namespace BD.Common.Enums;

/// <summary>
/// 可以设置的额外标志，以控制浏览器的打开方式。
/// <para>这个枚举支持其成员值的位法组合</para>
/// </summary>
[Flags]
public enum BrowserLaunchFlags
{
    /// <summary>
    /// 没有额外的标志。这是默认的
    /// </summary>
    None = 0,

    /// <summary>
    /// 在 Android 上，如果有的话，在当前活动旁边启动新活动
    /// </summary>
    LaunchAdjacent = 1,

    /// <summary>
    /// 在 iOS 上，在支持的情况下，用系统首选的浏览器启动浏览器作为页面表
    /// </summary>
    PresentAsPageSheet = 2,

    /// <summary>
    /// 在 iOS 上，在支持的情况下，用系统首选的浏览器启动浏览器作为表单
    /// </summary>
    PresentAsFormSheet = 4,
}
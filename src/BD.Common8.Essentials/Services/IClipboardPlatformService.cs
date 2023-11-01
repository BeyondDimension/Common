namespace BD.Common8.Essentials.Services;

/// <summary>
/// 提供了对剪贴板进行操作的功能
/// </summary>
public interface IClipboardPlatformService
{
    /// <summary>
    /// 获取 <see cref="IClipboardPlatformService"/> 的实例
    /// </summary>
    static IClipboardPlatformService? Instance => Ioc.Get_Nullable<IClipboardPlatformService>();

    /// <summary>
    /// 将文本设置到剪贴板中
    /// </summary>
    ValueTask PlatformSetTextAsync(string text);

    /// <summary>
    /// 从剪贴板中获取文本
    /// </summary>
    ValueTask<string> PlatformGetTextAsync();

    /// <summary>
    /// 判断剪贴板是否包含文本
    /// </summary>
    ValueTask<bool> PlatformHasTextAsync();

    /// <summary>
    /// 剪贴板内容变化时触发的事件
    /// </summary>
    event EventHandler<EventArgs> ClipboardContentChanged;
}
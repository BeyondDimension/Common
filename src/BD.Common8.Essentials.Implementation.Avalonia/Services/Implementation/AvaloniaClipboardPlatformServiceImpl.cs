using BD.Common8.Essentials.Extensions;
using System.Runtime.CompilerServices;

namespace BD.Common8.Essentials.Services.Implementation;

/// <summary>
/// 提供 Avalonia 剪贴板服务实现
/// </summary>
sealed class AvaloniaClipboardPlatformServiceImpl : IClipboardPlatformService
{
    /// <summary>
    /// 当剪贴板内容更改时触发的事件
    /// </summary>
#pragma warning disable CS0067 // 从不使用事件“AvaloniaClipboardPlatformServiceImpl.ClipboardContentChanged”
    public event EventHandler<EventArgs>? ClipboardContentChanged;
#pragma warning restore CS0067 // 从不使用事件“AvaloniaClipboardPlatformServiceImpl.ClipboardContentChanged”

    /// <summary>
    /// 异步获取剪贴板中的文本内容
    /// </summary>
    public async Task<string?> PlatformGetTextAsync()
    {
        var topLevel = global::Avalonia.Application.Current.GetMainWindowOrActiveWindowOrMainView();
        if (topLevel != null)
        {
            var clipboard = topLevel.Clipboard;
            if (clipboard != null)
            {
                var result = await clipboard.GetTextAsync();
                return result ?? string.Empty;
            }
        }
        return string.Empty;
    }

    /// <summary>
    /// 异步检查剪贴板是否包含文本内容
    /// </summary>
    public bool PlatformHasText()
    {
        var result = PlatformGetTextAsync().GetAwaiter().GetResult();
        return !string.IsNullOrEmpty(result);
    }

    /// <summary>
    /// 将指定的文本设置到剪贴板中
    /// </summary>
    public Task PlatformSetTextAsync(string text)
        => OperatingSystem.IsLinux() ?
        PlatformSetTextLinuxAsync(text) :
        PlatformSetTextCoreAsync(text);

    /// <summary>
    /// 在 Linux 平台上将指定的文本设置到剪贴板中
    /// </summary>
    /// <param name="text"></param>
    /// <returns></returns>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    static Task PlatformSetTextLinuxAsync(string text)
    {
        var topLevel = global::Avalonia.Application.Current.GetMainWindowOrActiveWindowOrMainView();
        if (topLevel != null)
        {
            var clipboard = topLevel.Clipboard;
            // TODO: 不能用 await 等待 Linux 上不知啥原因导致卡死
            clipboard?.SetTextAsync(text);
        }
        return Task.CompletedTask;
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    static async Task PlatformSetTextCoreAsync(string text)
    {
        var topLevel = global::Avalonia.Application.Current.GetMainWindowOrActiveWindowOrMainView();
        if (topLevel != null)
        {
            var clipboard = topLevel.Clipboard;
            if (clipboard != null)
            {
                await clipboard.SetTextAsync(text);
            }
        }
    }
}

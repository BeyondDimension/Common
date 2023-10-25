namespace BD.Common8.Essentials.Services.Implementation;

#pragma warning disable SA1600 // Elements should be documented

sealed class AvaloniaClipboardPlatformServiceImpl : IClipboardPlatformService
{
#pragma warning disable CS0067 // 从不使用事件“AvaloniaClipboardPlatformServiceImpl.ClipboardContentChanged”
    public event EventHandler<EventArgs>? ClipboardContentChanged;
#pragma warning restore CS0067 // 从不使用事件“AvaloniaClipboardPlatformServiceImpl.ClipboardContentChanged”

    public async ValueTask<string> PlatformGetTextAsync()
    {
        var topLevel = AvaApplication.Current.GetMainWindowOrActiveWindowOrMainView();
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

    public async ValueTask<bool> PlatformHasTextAsync()
    {
        var result = await PlatformGetTextAsync();
        return !string.IsNullOrEmpty(result);
    }

    public ValueTask PlatformSetTextAsync(string text)
        => OperatingSystem.IsLinux() ?
        PlatformSetTextLinuxAsync(text) :
        PlatformSetTextCoreAsync(text);

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    static ValueTask PlatformSetTextLinuxAsync(string text)
    {
        var topLevel = AvaApplication.Current.GetMainWindowOrActiveWindowOrMainView();
        if (topLevel != null)
        {
            var clipboard = topLevel.Clipboard;
            // 不能用 await 等待 Linux 上不知啥原因导致卡死
            clipboard?.SetTextAsync(text);
        }
        return default;
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    static async ValueTask PlatformSetTextCoreAsync(string text)
    {
        var topLevel = AvaApplication.Current.GetMainWindowOrActiveWindowOrMainView();
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

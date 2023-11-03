namespace BD.Common8.Essentials.Helpers;

/// <summary>
/// 提供使用设备剪贴板的方法
/// <para>https://docs.microsoft.com/zh-cn/xamarin/essentials/clipboard</para>
/// <para>https://github.com/xamarin/Essentials/blob/main/Xamarin.Essentials/Clipboard/Clipboard.shared.cs</para>
/// </summary>
public static class Clipboard2
{
    /// <summary>
    /// 将剪贴板的内容设置为指定的文本
    /// </summary>
    /// <param name="text">要放在剪贴板上的文本</param>
    /// <returns><see cref="ValueTask"/> 具有异步操作当前状态的对象</returns>
    /// <remarks>此方法立即返回，并且在返回时不保证文本已在剪贴板上</remarks>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static async ValueTask SetTextAsync(string? text)
    {
        var clipboardPlatformService = IClipboardPlatformService.Instance;
        if (clipboardPlatformService != null)
        {
            await clipboardPlatformService.PlatformSetTextAsync(text ?? string.Empty);
        }
    }

    /// <summary>
    /// 将剪贴板的内容设置为指定的文本
    /// </summary>
    /// <param name="text">要放在剪贴板上的文本</param>
    /// <remarks>此方法会立即返回，并且不会保证此方法返回时文本已在剪贴板上</remarks>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static async void SetText(string? text) => await SetTextAsync(text);

    /// <summary>
    /// 返回剪贴板上的任何文本
    /// </summary>
    /// <returns>返回剪贴板上的文本内容，如果没有则返回 <see cref="string.Empty"/></returns>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static async ValueTask<string> GetTextAsync()
    {
        var clipboardPlatformService = IClipboardPlatformService.Instance;
        if (clipboardPlatformService != null)
        {
            return await clipboardPlatformService.PlatformGetTextAsync();
        }
        return string.Empty;
    }

#if DEBUG
    /// <summary>
    /// 检查剪贴板中是否包含文本
    /// </summary>
    [Obsolete("use HasTextAsync", true)]
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static bool HasText()
    {
        var clipboardPlatformService = IClipboardPlatformService.Instance;
        if (clipboardPlatformService != null)
        {
#pragma warning disable CA2012 // 正确使用 ValueTask
            var task = clipboardPlatformService.PlatformHasTextAsync();
            if (task.IsCompleted)
                return task.Result;
            return task.GetAwaiter().GetResult();
#pragma warning restore CA2012 // 正确使用 ValueTask
        }
        return default;
    }
#endif

    /// <summary>
    /// 获取一个值，该值指示剪贴板上是否有任何文本
    /// </summary>
    /// <returns></returns>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static async ValueTask<bool> HasTextAsync()
    {
        var clipboardPlatformService = IClipboardPlatformService.Instance;
        if (clipboardPlatformService != null)
        {
            return await clipboardPlatformService.PlatformHasTextAsync();
        }
        return default;
    }

    /// <summary>
    /// 剪贴板内容更改时发生
    /// </summary>
    public static event EventHandler<EventArgs>? ClipboardContentChanged
    {
        add
        {
            var clipboardPlatformService = IClipboardPlatformService.Instance;
            if (clipboardPlatformService != null)
            {
                clipboardPlatformService.ClipboardContentChanged += value;
            }
        }

        remove
        {
            var clipboardPlatformService = IClipboardPlatformService.Instance;
            if (clipboardPlatformService != null)
            {
                clipboardPlatformService.ClipboardContentChanged -= value;
            }
        }
    }
}
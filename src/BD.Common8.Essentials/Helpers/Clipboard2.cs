namespace BD.Common8.Essentials.Helpers;

#pragma warning disable SA1600 // Elements should be documented

/// <summary>
/// Provides a way to work with text on the device clipboard.
/// <para>https://docs.microsoft.com/zh-cn/xamarin/essentials/clipboard</para>
/// <para>https://github.com/xamarin/Essentials/blob/main/Xamarin.Essentials/Clipboard/Clipboard.shared.cs</para>
/// </summary>
public static class Clipboard2
{
    /// <summary>
    /// Sets the contents of the clipboard to be the specified text.
    /// </summary>
    /// <param name="text">The text to put on the clipboard.</param>
    /// <returns>A <see cref="ValueTask"/> object with the current status of the asynchronous operation.</returns>
    /// <remarks>This method returns immediately and does not guarentee that the text is on the clipboard by the time this method returns.</remarks>
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
    /// Sets the contents of the clipboard to be the specified text.
    /// </summary>
    /// <param name="text">The text to put on the clipboard.</param>
    /// <remarks>This method returns immediately and does not guarentee that the text is on the clipboard by the time this method returns.</remarks>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static async void SetText(string? text) => await SetTextAsync(text);

    /// <summary>
    /// Returns any text that is on the clipboard.
    /// </summary>
    /// <returns>Text content that is on the clipboard, or <see cref="string.Empty"/> if there is none.</returns>
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
    /// Gets a value indicating whether there is any text on the clipboard.
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
    /// Occurs when the clipboard content changes.
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
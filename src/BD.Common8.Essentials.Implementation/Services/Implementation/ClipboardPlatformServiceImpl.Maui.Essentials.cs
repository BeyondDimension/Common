#if !LINUX
namespace BD.Common8.Essentials.Services.Implementation;

partial class ClipboardPlatformServiceImpl
{
    /// <inheritdoc/>
    async Task IClipboardPlatformService.PlatformSetTextAsync(string text)
    {
        await global::Microsoft.Maui.ApplicationModel.DataTransfer.Clipboard.SetTextAsync(text);
    }

    /// <inheritdoc/>
    async Task<string?> IClipboardPlatformService.PlatformGetTextAsync()
    {
        var result = await global::Microsoft.Maui.ApplicationModel.DataTransfer.Clipboard.GetTextAsync();
        return result;
    }

    /// <inheritdoc/>
    bool IClipboardPlatformService.PlatformHasText()
    {
        var result = global::Microsoft.Maui.ApplicationModel.DataTransfer.Clipboard.HasText;
        return result;
    }

    /// <inheritdoc/>
    public event EventHandler<EventArgs> ClipboardContentChanged
    {
        add
        {
            global::Microsoft.Maui.ApplicationModel.DataTransfer.Clipboard.ClipboardContentChanged += value;
        }

        remove
        {
            global::Microsoft.Maui.ApplicationModel.DataTransfer.Clipboard.ClipboardContentChanged -= value;
        }
    }
}
#endif
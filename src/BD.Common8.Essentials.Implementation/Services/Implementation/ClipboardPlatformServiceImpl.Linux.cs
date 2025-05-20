#if LINUX
namespace BD.Common8.Essentials.Services.Implementation;

partial class ClipboardPlatformServiceImpl
{
    /// <inheritdoc/>
    Task IClipboardPlatformService.PlatformSetTextAsync(string text)
    {
        return Task.CompletedTask;
    }

    /// <inheritdoc/>
    Task<string?> IClipboardPlatformService.PlatformGetTextAsync()
    {
        return Task.FromResult((string?)null);
    }

    /// <inheritdoc/>
    bool IClipboardPlatformService.PlatformHasText()
    {
        return false;
    }

    /// <inheritdoc/>
    public event EventHandler<EventArgs> ClipboardContentChanged
    {
        add
        {
        }

        remove
        {
        }
    }
}
#endif
namespace BD.Common8.Essentials.Services;

#pragma warning disable SA1600 // Elements should be documented

public interface IClipboardPlatformService
{
    static IClipboardPlatformService? Instance => Ioc.Get_Nullable<IClipboardPlatformService>();

    ValueTask PlatformSetTextAsync(string text);

    ValueTask<string> PlatformGetTextAsync();

    ValueTask<bool> PlatformHasTextAsync();

    event EventHandler<EventArgs> ClipboardContentChanged;
}
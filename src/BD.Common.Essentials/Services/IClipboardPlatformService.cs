namespace BD.Common.Services;

public interface IClipboardPlatformService
{
    static IClipboardPlatformService Instance => Ioc.Get<IClipboardPlatformService>();

    Task PlatformSetTextAsync(string text);

    Task<string> PlatformGetTextAsync();

    bool PlatformHasText { get; }
}
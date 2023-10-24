namespace BD.Common8.Essentials.Helpers;

/// <summary>
/// https://github.com/dotnet/maui/blob/8.0.0-rc.2.9373/src/Essentials/src/Types/Shared/WebUtils.shared.cs
/// </summary>
static partial class WebUtils
{
#if IOS || MACOS || MACCATALYST
    /// <summary>
    /// <see cref="Uri"/> ConvertTo <see cref="NSUrl"/>
    /// </summary>
    /// <param name="uri"></param>
    /// <returns></returns>
    [SupportedOSPlatform("macos")]
    [SupportedOSPlatform("maccatalyst")]
    [SupportedOSPlatform("ios")]
    public static NSUrl GetNativeUrl(Uri uri)
    {
        try
        {
            return new NSUrl(uri.OriginalString);
        }
        catch (Exception ex)
        {
            Debug.WriteLine($"Unable to create NSUrl from Original string, trying Absolute URI: {ex.Message}");
            return new NSUrl(uri.AbsoluteUri);
        }
    }
#endif
}

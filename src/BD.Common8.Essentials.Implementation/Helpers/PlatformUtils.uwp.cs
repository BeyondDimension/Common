#if WINDOWS
namespace BD.Common8.Essentials.Helpers;

#pragma warning disable SA1600 // Elements should be documented

/// <summary>
/// https://github.com/dotnet/maui/blob/8.0.0-rc.2.9373/src/Essentials/src/Platform/PlatformUtils.uwp.cs
/// </summary>
static partial class PlatformUtils
{
    [SupportedOSPlatform("windows")]
    public const string AppManifestFilename = "AppxManifest.xml";
    [SupportedOSPlatform("windows")]
    public const string AppManifestXmlns = "http://schemas.microsoft.com/appx/manifest/foundation/windows10";
    [SupportedOSPlatform("windows")]
    public const string AppManifestUapXmlns = "http://schemas.microsoft.com/appx/manifest/uap/windows10";
}
#endif
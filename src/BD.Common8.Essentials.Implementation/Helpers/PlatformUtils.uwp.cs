#if WINDOWS
namespace BD.Common8.Essentials.Helpers;

/// <summary>
/// https://github.com/dotnet/maui/blob/8.0.0-rc.2.9373/src/Essentials/src/Platform/PlatformUtils.uwp.cs
/// </summary>
static partial class PlatformUtils
{
    /// <summary>
    /// 程序清单文件名
    /// </summary>
    [SupportedOSPlatform("windows")]
    public const string AppManifestFilename = "AppxManifest.xml";

    /// <summary>
    /// 程序清单命名空间
    /// </summary>
    [SupportedOSPlatform("windows")]
    public const string AppManifestXmlns = "http://schemas.microsoft.com/appx/manifest/foundation/windows10";

    /// <summary>
    /// 程序清单 UWP 命名空间
    /// </summary>
    [SupportedOSPlatform("windows")]
    public const string AppManifestUapXmlns = "http://schemas.microsoft.com/appx/manifest/uap/windows10";
}
#endif
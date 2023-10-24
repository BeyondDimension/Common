#if WINDOWS
using Windows.ApplicationModel;

namespace BD.Common8.Essentials.Helpers;

#pragma warning disable SA1600 // Elements should be documented

/// <summary>
/// https://github.com/dotnet/maui/blob/8.0.0-rc.2.9373/src/Essentials/src/FileSystem/FileSystemUtils.uwp.cs
/// </summary>
static partial class FileSystemUtils
{
    [SupportedOSPlatform("windows10.0.10240.0")]
    public static bool AppPackageFileExists(string filename)
    {
        var file = PlatformGetFullAppPackageFilePath(filename);
        return File.Exists(file);
    }

    [SupportedOSPlatform("windows10.0.10240.0")]
    public static string PlatformGetFullAppPackageFilePath(string filename)
    {
        ArgumentNullException.ThrowIfNull(filename);

        filename = NormalizePath(filename);

        string? root;
        if (AppInfoUtils.IsPackagedApp)
            root = Package.Current.InstalledLocation.Path;
        else
            root = AppContext.BaseDirectory;

        return Path.Combine(root, filename);
    }

    [SupportedOSPlatform("windows10.0.10240.0")]
    public static bool TryGetAppPackageFileUri(string filename, [NotNullWhen(true)] out string? uri)
    {
        var path = PlatformGetFullAppPackageFilePath(filename);

        if (File.Exists(path))
        {
            if (AppInfoUtils.IsPackagedApp)
                uri = $"ms-appx:///{filename.Replace('\\', '/')}";
            else
                uri = $"file:///{path.Replace('\\', '/')}";

            return true;
        }

        uri = null;
        return false;
    }
}
#endif
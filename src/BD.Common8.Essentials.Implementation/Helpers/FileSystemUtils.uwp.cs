#if WINDOWS
using Windows.ApplicationModel;

namespace BD.Common8.Essentials.Helpers;

/// <summary>
/// 提供与应用程序包文件操作相关的方法
/// <para> https://github.com/dotnet/maui/blob/8.0.0-rc.2.9373/src/Essentials/src/FileSystem/FileSystemUtils.uwp.cs </para>
/// </summary>
static partial class FileSystemUtils
{

    /// <summary>
    /// 判断指定的应用程序包文件是否存在
    /// </summary>
    [SupportedOSPlatform("windows10.0.10240.0")]
    public static bool AppPackageFileExists(string filename)
    {
        var file = PlatformGetFullAppPackageFilePath(filename);
        return File.Exists(file);
    }

    /// <summary>
    /// 获取完整的应用程序包文件路径
    /// </summary>
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

    /// <summary>
    /// 尝试获取应用程序包文件的 URI
    /// </summary>
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
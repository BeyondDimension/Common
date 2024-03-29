#if WINDOWS
using Windows.ApplicationModel;

namespace BD.Common8.Essentials.Helpers;

static partial class AppInfoUtils
{
    static readonly Lazy<bool> _isPackagedAppLazy = new(() =>
    {
        try
        {
            if (Package.Current != null)
                return true;
        }
        catch
        {
            // no-op
        }

        return false;
    });

    /// <summary>
    /// Gets if this app is a packaged app.
    /// </summary>
    public static bool IsPackagedApp => _isPackagedAppLazy.Value;

    /// <summary>
    /// Converts a <see cref="PackageVersion"/> object to a <see cref="Version"/> object.
    /// </summary>
    /// <param name="version">The <see cref="PackageVersion"/> to convert.</param>
    /// <returns>A new <see cref="Version"/> object with the version information of this app.</returns>
    public static Version ToVersion(this PackageVersion version) =>
        new(version.Major, version.Minor, version.Build, version.Revision);

    /// <summary>
    /// Gets the version information for this app.
    /// </summary>
    /// <param name="assembly">The assembly to retrieve the version information for.</param>
    /// <param name="name">The key that is used to retrieve the version information from the metadata.</param>
    /// <returns><see langword="null"/> if <paramref name="name"/> is <see langword="null"/> or empty, or no version information could be found with the value of <paramref name="name"/>.</returns>
    public static Version? GetAppInfoVersionValue(this Assembly assembly, string name)
    {
        if (assembly.GetAppInfoValue(name) is string value && !string.IsNullOrEmpty(value))
            return Version.Parse(value);

        return null;
    }

    /// <summary>
    /// Gets the app info from this apps' metadata.
    /// </summary>
    /// <param name="assembly">The assembly to retrieve the app info for.</param>
    /// <param name="name">The key of the metadata to be retrieved (e.g. PackageName, PublisherName or Name).</param>
    /// <returns>The value that corresponds to the given key in <paramref name="name"/>.</returns>
    public static string? GetAppInfoValue(this Assembly assembly, string name) =>
        assembly.GetMetadataAttributeValue("Microsoft.Maui.ApplicationModel.AppInfo." + name);

    /// <summary>
    /// Gets the value for a given key from the assembly metadata.
    /// </summary>
    /// <param name="assembly">The assembly to retrieve the information for.</param>
    /// <param name="key">The key of the metadata to be retrieved (e.g. PackageName, PublisherName or Name).</param>
    /// <returns>The value that corresponds to the given key in <paramref name="key"/>.</returns>
    public static string? GetMetadataAttributeValue(this Assembly assembly, string key)
    {
        foreach (var attr in assembly.GetCustomAttributes<AssemblyMetadataAttribute>())
        {
            if (attr.Key == key)
                return attr.Value;
        }

        return null;
    }
}
#endif
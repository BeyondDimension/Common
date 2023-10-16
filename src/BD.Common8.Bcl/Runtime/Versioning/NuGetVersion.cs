// https://github.com/NuGetArchive/NuGet.Versioning/blob/rc-preview1/src/NuGet.Versioning/NuGetVersion.cs
// https://github.com/NuGetArchive/NuGet.Versioning/blob/rc-preview1/src/NuGet.Versioning/NuGetVersionFactory.cs

#pragma warning disable IDE0079 // 请删除不必要的忽略
#pragma warning disable IDE0005 // 删除不必要的 using 指令
#pragma warning disable SA1209 // Using alias directives should be placed after other using directives
#pragma warning disable SA1211 // Using alias directives should be ordered alphabetically by alias name
#pragma warning disable SA1600 // Elements should be documented
#pragma warning disable IDE0290 // 使用主构造函数

namespace System.Runtime.Versioning;

/// <summary>
/// A hybrid implementation of SemVer that supports semantic versioning as described at http://semver.org while not strictly enforcing it to
/// allow older 4-digit versioning schemes to continue working.
/// </summary>
public partial class NuGetVersion : SemanticVersion
{
    protected readonly string? _originalString;

    /// <summary>
    /// Creates a NuGetVersion using NuGetVersion.Parse(string)
    /// </summary>
    /// <param name="version">Version string</param>
    public NuGetVersion(string version)
        : this(Parse(version))
    {
    }

    /// <summary>
    /// Creates a NuGetVersion from an existing NuGetVersion
    /// </summary>
    public NuGetVersion(NuGetVersion version)
        : this(version.Version, version.ReleaseLabels, version.Metadata, version.ToString())
    {
    }

    /// <summary>
    /// Creates a NuGetVersion from a .NET Version
    /// </summary>
    /// <param name="version">Version numbers</param>
    /// <param name="releaseLabel">Prerelease label</param>
    /// <param name="metadata">Build metadata</param>
    public NuGetVersion(Version version, string? releaseLabel = null, string? metadata = null)
        : this(version, ParseReleaseLabels(releaseLabel), metadata, GetLegacyString(version, ParseReleaseLabels(releaseLabel), metadata))
    {
    }

    /// <summary>
    /// Creates a NuGetVersion X.Y.Z
    /// </summary>
    /// <param name="major">X.y.z</param>
    /// <param name="minor">x.Y.z</param>
    /// <param name="patch">x.y.Z</param>
    public NuGetVersion(int major, int minor, int patch)
        : this(major, minor, patch, Enumerable.Empty<string>(), null)
    {
    }

    /// <summary>
    /// Creates a NuGetVersion X.Y.Z-alpha
    /// </summary>
    /// <param name="major">X.y.z</param>
    /// <param name="minor">x.Y.z</param>
    /// <param name="patch">x.y.Z</param>
    /// <param name="releaseLabel">Prerelease label</param>
    public NuGetVersion(int major, int minor, int patch, string? releaseLabel)
        : this(major, minor, patch, ParseReleaseLabels(releaseLabel), null)
    {
    }

    /// <summary>
    /// Creates a NuGetVersion X.Y.Z-alpha#build01
    /// </summary>
    /// <param name="major">X.y.z</param>
    /// <param name="minor">x.Y.z</param>
    /// <param name="patch">x.y.Z</param>
    /// <param name="releaseLabel">Prerelease label</param>
    /// <param name="metadata">Build metadata</param>
    public NuGetVersion(int major, int minor, int patch, string? releaseLabel, string? metadata)
        : this(major, minor, patch, ParseReleaseLabels(releaseLabel), metadata)
    {
    }

    /// <summary>
    /// Creates a NuGetVersion X.Y.Z-alpha.1.2#build01
    /// </summary>
    /// <param name="major">X.y.z</param>
    /// <param name="minor">x.Y.z</param>
    /// <param name="patch">x.y.Z</param>
    /// <param name="releaseLabels">Prerelease labels</param>
    /// <param name="metadata">Build metadata</param>
    public NuGetVersion(int major, int minor, int patch, IEnumerable<string>? releaseLabels, string? metadata)
        : this(new Version(major, minor, patch, 0), releaseLabels, metadata, null)
    {
    }

    /// <summary>
    /// Creates a NuGetVersion W.X.Y.Z
    /// </summary>
    /// <param name="major">W.x.y.z</param>
    /// <param name="minor">w.X.y.z</param>
    /// <param name="patch">w.x.Y.z</param>
    /// <param name="revision">w.x.y.Z</param>
    public NuGetVersion(int major, int minor, int patch, int revision)
        : this(major, minor, patch, revision, Enumerable.Empty<string>(), null)
    {
    }

    /// <summary>
    /// Creates a NuGetVersion W.X.Y.Z-alpha#build01
    /// </summary>
    /// <param name="major">W.x.y.z</param>
    /// <param name="minor">w.X.y.z</param>
    /// <param name="patch">w.x.Y.z</param>
    /// <param name="revision">w.x.y.Z</param>
    /// <param name="releaseLabel">Prerelease label</param>
    /// <param name="metadata">Build metadata</param>
    public NuGetVersion(int major, int minor, int patch, int revision, string? releaseLabel, string? metadata)
        : this(major, minor, patch, revision, ParseReleaseLabels(releaseLabel), metadata)
    {
    }

    /// <summary>
    /// Creates a NuGetVersion W.X.Y.Z-alpha.1#build01
    /// </summary>
    /// <param name="major">W.x.y.z</param>
    /// <param name="minor">w.X.y.z</param>
    /// <param name="patch">w.x.Y.z</param>
    /// <param name="revision">w.x.y.Z</param>
    /// <param name="releaseLabels">Prerelease labels</param>
    /// <param name="metadata">Build metadata</param>
    public NuGetVersion(int major, int minor, int patch, int revision, IEnumerable<string>? releaseLabels, string? metadata)
        : this(new Version(major, minor, patch, revision), releaseLabels, metadata, null)
    {
    }

    /// <summary>
    /// Creates a NuGetVersion from a .NET Version with additional release labels, build metadata, and a non-normalized version string.
    /// </summary>
    /// <param name="version">Version numbers</param>
    /// <param name="releaseLabels">prerelease labels</param>
    /// <param name="metadata">Build metadata</param>
    /// <param name="originalVersion">Non-normalized original version string</param>
    public NuGetVersion(Version version, IEnumerable<string>? releaseLabels, string? metadata, string? originalVersion)
        : base(version, releaseLabels, metadata)
    {
        _originalString = originalVersion;
    }

    /// <summary>
    /// Returns the version string.
    /// </summary>
    /// <remarks>This method includes legacy behavior. Use ToNormalizedString() instead.</remarks>
    public override string ToString()
    {
        if (string.IsNullOrEmpty(_originalString))
        {
            return ToNormalizedString();
        }

        return _originalString ?? string.Empty;
    }

    /// <summary>
    /// A System.Version representation of the version without metadata or release labels.
    /// </summary>
    public Version Version => _version;

    /// <summary>
    /// True if the NuGetVersion is using legacy behavior.
    /// </summary>
    public virtual bool IsLegacyVersion => Version.Revision > 0;

    /// <summary>
    /// Revision version R (x.y.z.R)
    /// </summary>
    public int Revision => _version.Revision;
}

partial class NuGetVersion
{
    /// <summary>
    /// https://github.com/NuGetArchive/NuGet.Versioning/blob/rc-preview1/src/NuGet.Versioning/Resources.resx#L61-L63
    /// </summary>
    protected const string Argument_Cannot_Be_Null_Or_Empty = "Value cannot be null or an empty string.";

    /// <summary>
    /// Creates a NuGetVersion from a string representing the semantic version.
    /// </summary>
    public static new NuGetVersion Parse(string value)
    {
        if (string.IsNullOrEmpty(value))
        {
            throw new ArgumentException(Argument_Cannot_Be_Null_Or_Empty, nameof(value));
        }

        if (!TryParse(value, out var ver))
        {
            throw new ArgumentException(string.Format(Invalidvalue, value), nameof(value));
        }

        return ver;
    }

    /// <summary>
    /// Parses a version string using loose semantic versioning rules that allows 2-4 version components followed by an optional special version.
    /// </summary>
    public static bool TryParse(string? value, [NotNullWhen(true)] out NuGetVersion? version)
    {
        version = null;

        if (value != null)
        {
            Version? systemVersion = null;

            // trim the value before passing it in since we not strict here
            var sections = ParseSections(value.Trim());

            // null indicates the string did not meet the rules
            if (sections != default && !string.IsNullOrEmpty(sections.versionString))
            {
                string versionPart = sections.versionString!;

                if (versionPart.IndexOf('.') < 0)
                {
                    // System.Version requires at least a 2 part version to parse.
                    versionPart += ".0";
                }

                if (Version.TryParse(versionPart, out systemVersion))
                {
                    // labels
                    if (sections.releaseLabels != null && !sections.releaseLabels.All(s => IsValidPart(s, false)))
                    {
                        return false;
                    }

                    // build metadata
                    if (sections.buildMetadata != null && !IsValid(sections.buildMetadata, true))
                    {
                        return false;
                    }

                    Version ver = NormalizeVersionValue(systemVersion);

                    string originalVersion = value;

                    if (originalVersion.IndexOf(' ') > -1)
                    {
                        originalVersion = value.Replace(" ", "");
                    }

                    version = new NuGetVersion(version: ver,
                                                releaseLabels: sections.releaseLabels,
                                                metadata: sections.buildMetadata ?? string.Empty,
                                                originalVersion: originalVersion);

                    return true;
                }
            }
        }

        return false;
    }

    /// <summary>
    /// Parses a version string using strict SemVer rules.
    /// </summary>
    public static bool TryParseStrict(string? value, [NotNullWhen(true)] out NuGetVersion? version)
    {
        version = null;

        if (SemanticVersion.TryParse(value, out var semVer))
        {
            version = new NuGetVersion(semVer.Major, semVer.Minor, semVer.Patch, 0, semVer.ReleaseLabels, semVer.Metadata);
            return true;
        }

        return false;
    }

    /// <summary>
    /// Creates a legacy version string using System.Version
    /// </summary>
    static string GetLegacyString(Version version, IEnumerable<string>? releaseLabels, string? metadata)
    {
        StringBuilder sb = new(version.ToString());

        if (releaseLabels != null)
        {
            sb.AppendFormat(CultureInfo.InvariantCulture, "-{0}", string.Join(".", releaseLabels));
        }

        if (!string.IsNullOrEmpty(metadata))
        {
            sb.AppendFormat(CultureInfo.InvariantCulture, "+{0}", metadata);
        }

        return sb.ToString();
    }

    static string[]? ParseReleaseLabels(string? releaseLabels)
    {
        if (!string.IsNullOrEmpty(releaseLabels))
        {
            return releaseLabels!.Split('.');
        }

        return null;
    }
}
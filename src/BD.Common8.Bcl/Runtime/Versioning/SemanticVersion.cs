// https://github.com/NuGetArchive/NuGet.Versioning/blob/rc-preview1/src/NuGet.Versioning/SemanticVersion.cs
// https://github.com/NuGetArchive/NuGet.Versioning/blob/rc-preview1/src/NuGet.Versioning/SemanticVersionBase.cs
// https://github.com/NuGetArchive/NuGet.Versioning/blob/rc-preview1/src/NuGet.Versioning/SemanticVersionFactory.cs
// https://github.com/NuGetArchive/NuGet.Versioning/blob/rc-preview1/src/NuGet.Versioning/VersionComparer.cs
// https://github.com/NuGetArchive/NuGet.Versioning/blob/rc-preview1/src/NuGet.Versioning/VersionComparison.cs
// https://github.com/NuGetArchive/NuGet.Versioning/blob/rc-preview1/src/NuGet.Versioning/VersionFormatter.cs
// https://github.com/NuGetArchive/NuGet.Versioning/blob/rc-preview1/src/NuGet.Versioning/HashCodeCombiner.cs

#pragma warning disable IDE0079 // 请删除不必要的忽略
#pragma warning disable IDE0005 // 删除不必要的 using 指令
#pragma warning disable SA1209 // Using alias directives should be placed after other using directives
#pragma warning disable SA1211 // Using alias directives should be ordered alphabetically by alias name
#pragma warning disable SA1600 // Elements should be documented

namespace System.Runtime.Versioning;

/// <summary>
/// A strict SemVer implementation
/// </summary>
public partial class SemanticVersion
{
    protected readonly IEnumerable<string>? _releaseLabels;
    protected readonly string? _metadata;
    protected readonly Version _version;

    /// <summary>
    /// Creates a SemanticVersion from an existing SemanticVersion
    /// </summary>
    public SemanticVersion(SemanticVersion version)
        : this(version.Major, version.Minor, version.Patch, version.ReleaseLabels, version.Metadata)
    {
    }

    /// <summary>
    /// Creates a SemanticVersion X.Y.Z
    /// </summary>
    /// <param name="major">X.y.z</param>
    /// <param name="minor">x.Y.z</param>
    /// <param name="patch">x.y.Z</param>
    public SemanticVersion(int major, int minor, int patch)
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
    public SemanticVersion(int major, int minor, int patch, string? releaseLabel)
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
    public SemanticVersion(int major, int minor, int patch, string? releaseLabel, string? metadata)
        : this(major, minor, patch, ParseReleaseLabels(releaseLabel), metadata)
    {
    }

    /// <summary>
    /// Creates a NuGetVersion X.Y.Z-alpha.1.2#build01
    /// </summary>
    /// <param name="major">X.y.z</param>
    /// <param name="minor">x.Y.z</param>
    /// <param name="patch">x.y.Z</param>
    /// <param name="releaseLabels">Release labels that have been split by the dot separator</param>
    /// <param name="metadata">Build metadata</param>
    public SemanticVersion(int major, int minor, int patch, IEnumerable<string>? releaseLabels, string? metadata)
        : this(new Version(major, minor, patch, 0), releaseLabels, metadata)
    {
    }

    protected SemanticVersion(Version version, string? releaseLabel = null, string? metadata = null)
        : this(version, ParseReleaseLabels(releaseLabel), metadata)
    {
    }

    protected SemanticVersion(int major, int minor, int patch, int revision, string? releaseLabel, string? metadata)
        : this(major, minor, patch, revision, ParseReleaseLabels(releaseLabel), metadata)
    {
    }

    protected SemanticVersion(int major, int minor, int patch, int revision, IEnumerable<string>? releaseLabels, string? metadata)
        : this(new Version(major, minor, patch, revision), releaseLabels, metadata)
    {
    }

    protected SemanticVersion(Version version, IEnumerable<string>? releaseLabels, string? metadata)
    {
#if NET6_0_OR_GREATER
        ArgumentNullException.ThrowIfNull(version);
#else
        version = version ?? throw new ArgumentNullException(nameof(version));
#endif

        _version = NormalizeVersionValue(version);
        _metadata = metadata;

        if (releaseLabels != null)
        {
            // enumerate the list
            _releaseLabels = releaseLabels.ToArray();
        }
    }

    /// <summary>
    /// Major version X (X.y.z)
    /// </summary>
    public int Major => _version.Major;

    /// <summary>
    /// Minor version Y (x.Y.z)
    /// </summary>
    public int Minor => _version.Minor;

    /// <summary>
    /// Patch version Z (x.y.Z)
    /// </summary>
    public int Patch => _version.Build;

    /// <summary>
    /// A collection of pre-release labels attached to the version.
    /// </summary>
    public IEnumerable<string> ReleaseLabels => _releaseLabels ?? Enumerable.Empty<string>();

    /// <summary>
    /// The full pre-release label for the version.
    /// </summary>
    public string Release
    {
        get
        {
            if (_releaseLabels != null)
            {
                return string.Join(".", _releaseLabels);
            }

            return string.Empty;
        }
    }

    /// <summary>
    /// True if pre-release labels exist for the version.
    /// </summary>
    public virtual bool IsPrerelease
    {
        get
        {
            if (ReleaseLabels != null)
            {
                var enumerator = ReleaseLabels.GetEnumerator();
                return enumerator.MoveNext() && !string.IsNullOrEmpty(enumerator.Current);
            }

            return false;
        }
    }

    /// <summary>
    /// True if metadata exists for the version.
    /// </summary>
    public virtual bool HasMetadata => !string.IsNullOrEmpty(Metadata);

    /// <summary>
    /// Build metadata attached to the version.
    /// </summary>
    public virtual string? Metadata => _metadata;
}

partial class SemanticVersion : IFormattable, IComparable, IComparable<SemanticVersion>, IEquatable<SemanticVersion>
{
    /// <summary>
    /// Gives a normalized representation of the version.
    /// </summary>
    public virtual string ToNormalizedString()
    {
        return ToString("N", new VersionFormatter());
    }

    public override string ToString()
    {
        return ToNormalizedString();
    }

    public virtual string ToString(string? format, IFormatProvider? formatProvider)
    {
        if (formatProvider == null || !TryFormatter(format, formatProvider, out var formattedString))
        {
            formattedString = ToString();
        }

        return formattedString ?? string.Empty;
    }

    protected bool TryFormatter(string? format, IFormatProvider? formatProvider, out string? formattedString)
    {
        bool formatted = false;
        formattedString = null;

        if (formatProvider != null)
        {
            if (formatProvider.GetFormat(GetType()) is ICustomFormatter formatter)
            {
                formatted = true;
                formattedString = formatter.Format(format, this, formatProvider);
            }
        }

        return formatted;
    }

    public override int GetHashCode()
    {
        return VersionComparer.Default.GetHashCode(this);
    }

    public virtual int CompareTo(object? obj)
    {
        return CompareTo(obj as SemanticVersion);
    }

    public virtual int CompareTo(SemanticVersion? other)
    {
        return CompareTo(other, VersionComparison.Default);
    }

    public override bool Equals(object? obj)
    {
        return Equals(obj as SemanticVersion);
    }

    public virtual bool Equals(SemanticVersion? other)
    {
        return Equals(other, VersionComparison.Default);
    }

    /// <summary>
    /// True if the VersionBase objects are equal based on the given comparison mode.
    /// </summary>
    public virtual bool Equals(SemanticVersion? other, VersionComparison versionComparison)
    {
        return CompareTo(other, versionComparison) == 0;
    }

    /// <summary>
    /// Compares NuGetVersion objects using the given comparison mode.
    /// </summary>
    public virtual int CompareTo(SemanticVersion? other, VersionComparison versionComparison)
    {
        VersionComparer comparer = new(versionComparison);
        return comparer.Compare(this, other);
    }

    public static bool operator ==(SemanticVersion? version1, SemanticVersion? version2) => Compare(version1, version2) == 0;

    public static bool operator !=(SemanticVersion? version1, SemanticVersion? version2) => Compare(version1, version2) != 0;

    public static bool operator <(SemanticVersion? version1, SemanticVersion? version2) => Compare(version1, version2) < 0;

    public static bool operator <=(SemanticVersion? version1, SemanticVersion? version2) => Compare(version1, version2) <= 0;

    public static bool operator >(SemanticVersion? version1, SemanticVersion? version2) => Compare(version1, version2) > 0;

    public static bool operator >=(SemanticVersion? version1, SemanticVersion? version2) => Compare(version1, version2) >= 0;

    static int Compare(SemanticVersion? version1, SemanticVersion? version2)
    {
        var comparer = new VersionComparer();
        return comparer.Compare(version1, version2);
    }
}

partial class SemanticVersion
{
    /// <summary>
    /// https://github.com/NuGetArchive/NuGet.Versioning/blob/rc-preview1/src/NuGet.Versioning/Resources.resx#L64-L66
    /// </summary>
    protected const string Invalidvalue = "'{0}' is not a valid version string.";

    /// <summary>
    /// Parses a SemVer string using strict SemVer rules.
    /// </summary>
    public static SemanticVersion Parse(string value)
    {
        if (!TryParse(value, out var ver))
        {
            throw new ArgumentException(string.Format(Invalidvalue, value), nameof(value));
        }

        return ver;
    }

    /// <summary>
    /// Parse a version string
    /// </summary>
    /// <returns>false if the version is not a strict semver</returns>
    public static bool TryParse(string? value, [NotNullWhen(true)] out SemanticVersion? version)
    {
        version = null;

        if (value != null)
        {
            var sections = ParseSections(value);

            // null indicates the string did not meet the rules
            if (sections != default && Version.TryParse(sections.versionString, out var systemVersion))
            {
                // validate the version string
                string[] parts = sections.versionString!.Split('.');

                if (parts.Length != 3)
                {
                    // versions must be 3 parts
                    return false;
                }

                foreach (var part in parts)
                {
                    if (!IsValidPart(part, false))
                    {
                        // leading zeros are not allowed
                        return false;
                    }
                }

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

                version = new SemanticVersion(version: ver,
                                            releaseLabels: sections.releaseLabels,
                                            metadata: sections.buildMetadata ?? string.Empty);

                return true;
            }
        }

        return false;
    }

    internal static bool IsLetterOrDigitOrDash(char c)
    {
        int x = c;

        // "ABCDEFGHIJKLMNOPQRSTUVWXYZabcdefghijklmnopqrstuvwxyz0123456789-"
        return (x >= 48 && x <= 57) || (x >= 65 && x <= 90) || (x >= 97 && x <= 122) || x == 45;
    }

    internal static bool IsValid(string s, bool allowLeadingZeros)
    {
        return s.Split('.').All(p => IsValidPart(p, allowLeadingZeros));
    }

    internal static bool IsValidPart(string s, bool allowLeadingZeros)
    {
        return IsValidPart(s.ToCharArray(), allowLeadingZeros);
    }

    internal static bool IsValidPart(char[] chars, bool allowLeadingZeros)
    {
        bool result = true;

        if (chars.Length == 0)
        {
            // empty labels are not allowed
            result = false;
        }

        // 0 is fine, but 00 is not.
        // 0A counts as an alpha numeric string where zeros are not counted
        if (!allowLeadingZeros && chars.Length > 1 && chars[0] == '0' && chars.All(c => char.IsDigit(c)))
        {
            // no leading zeros in labels allowed
            result = false;
        }
        else
        {
            result &= chars.All(c => IsLetterOrDigitOrDash(c));
        }

        return result;
    }

    /// <summary>
    /// Parse the version string into version/release/build
    /// The goal of this code is to take the most direct and optimized path
    /// to parsing and validating a semver. Regex would be much cleaner, but
    /// due to the number of versions created in NuGet Regex is too slow.
    /// </summary>
    internal static (string? versionString, string[]? releaseLabels, string? buildMetadata) ParseSections(string value)
    {
        string? versionString = null;
        string[]? releaseLabels = null;
        string? buildMetadata = null;

        int dashPos = -1;
        int plusPos = -1;

        char[] chars = value.ToCharArray();
        for (int i = 0; i < chars.Length; i++)
        {
            var end = i == chars.Length - 1;

            if (dashPos < 0)
            {
                if (end || chars[i] == '-' || chars[i] == '+')
                {
                    int endPos = i + (end ? 1 : 0);
                    versionString = value[..endPos];

                    dashPos = i;

                    if (chars[i] == '+')
                    {
                        plusPos = i;
                    }
                }
            }
            else if (plusPos < 0)
            {
                if (end || chars[i] == '+')
                {
                    int start = dashPos + 1;
                    int endPos = i + (end ? 1 : 0);
                    string releaseLabel = value[start..endPos];

                    releaseLabels = releaseLabel.Split('.');

                    plusPos = i;
                }
            }
            else if (end)
            {
                int start = plusPos + 1;
                int endPos = i + (end ? 1 : 0);
                buildMetadata = value[start..endPos];
            }
        }

        return new(versionString, releaseLabels, buildMetadata);
    }

    internal static Version NormalizeVersionValue(Version version)
    {
        Version normalized = version;

        if (version.Build < 0 || version.Revision < 0)
        {
            normalized = new Version(
                           version.Major,
                           version.Minor,
                           Math.Max(version.Build, 0),
                           Math.Max(version.Revision, 0));
        }

        return normalized;
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

partial class SemanticVersion
{
    /// <summary>
    /// IVersionComparer represents a version comparer capable of sorting and determining the equality of SemanticVersion objects.
    /// </summary>
    public interface IVersionComparer : IEqualityComparer<SemanticVersion?>, IComparer<SemanticVersion?>
    {
    }

    /// <summary>
    /// An IVersionComparer for NuGetVersion and NuGetVersion types.
    /// </summary>
    public sealed class VersionComparer : IVersionComparer
    {
        readonly VersionComparison _mode;

        /// <summary>
        /// Creates a VersionComparer using the default mode.
        /// </summary>
        public VersionComparer()
        {
            _mode = VersionComparison.Default;
        }

        /// <summary>
        /// Creates a VersionComparer that respects the given comparison mode.
        /// </summary>
        /// <param name="versionComparison">comparison mode</param>
        public VersionComparer(VersionComparison versionComparison)
        {
            _mode = versionComparison;
        }

        /// <summary>
        /// Determines if both versions are equal.
        /// </summary>
        public bool Equals(SemanticVersion? x, SemanticVersion? y)
        {
            return Compare(x, y) == 0;
        }

        /// <summary>
        /// Compares the given versions using the VersionComparison mode.
        /// </summary>
        public static int Compare(SemanticVersion? version1, SemanticVersion? version2, VersionComparison versionComparison)
        {
            var comparer = new VersionComparer(versionComparison);
            return comparer.Compare(version1, version2);
        }

        /// <summary>
        /// Gives a hash code based on the normalized version string.
        /// </summary>
        public int GetHashCode(SemanticVersion? version)
        {
            if (version is null)
            {
                return 0;
            }

            HashCodeCombiner combiner = new();

            combiner.AddObject(version.Major);
            combiner.AddObject(version.Minor);
            combiner.AddObject(version.Patch);

            var nuGetVersion = version as NuGetVersion;
            if (nuGetVersion != null && nuGetVersion.Revision > 0)
            {
                combiner.AddObject(nuGetVersion.Revision);
            }

            if (_mode == VersionComparison.Default || _mode == VersionComparison.VersionRelease || _mode == VersionComparison.VersionReleaseMetadata)
            {
                if (version.IsPrerelease)
                {
                    combiner.AddObject(version.Release.ToUpperInvariant());
                }
            }

            if (_mode == VersionComparison.VersionReleaseMetadata)
            {
                if (version.HasMetadata)
                {
                    combiner.AddObject(version.Metadata);
                }
            }

            return combiner.CombinedHash;
        }

        /// <summary>
        /// Compare versions.
        /// </summary>
        public int Compare(SemanticVersion? x, SemanticVersion? y)
        {
            if (ReferenceEquals(x, y))
            {
                return 0;
            }

            if (y is null)
            {
                return 1;
            }

            if (x is null)
            {
                return -1;
            }

            // compare version
            int result = x.Major.CompareTo(y.Major);
            if (result != 0)
                return result;

            result = x.Minor.CompareTo(y.Minor);
            if (result != 0)
                return result;

            result = x.Patch.CompareTo(y.Patch);
            if (result != 0)
                return result;

            var legacyX = x as NuGetVersion;
            var legacyY = y as NuGetVersion;

            result = CompareLegacyVersion(legacyX, legacyY);
            if (result != 0)
                return result;

            if (_mode != VersionComparison.Version)
            {
                // compare release labels
                if (x.IsPrerelease && !y.IsPrerelease)
                    return -1;

                if (!x.IsPrerelease && y.IsPrerelease)
                    return 1;

                if (x.IsPrerelease && y.IsPrerelease)
                {
                    result = CompareReleaseLabels(x.ReleaseLabels, y.ReleaseLabels);
                    if (result != 0)
                        return result;
                }

                // compare the metadata
                if (_mode == VersionComparison.VersionReleaseMetadata)
                {
                    result = StringComparer.OrdinalIgnoreCase.Compare(x.Metadata ?? string.Empty, y.Metadata ?? string.Empty);
                    if (result != 0)
                        return result;
                }
            }

            return 0;
        }

        /// <summary>
        /// Compares the 4th digit of the version number.
        /// </summary>
        static int CompareLegacyVersion(NuGetVersion? legacyX, NuGetVersion? legacyY)
        {
            int result = 0;

            // true if one has a 4th version number
            if (legacyX != null && legacyY != null)
            {
                result = legacyX.Version.CompareTo(legacyY.Version);
            }
            else if (legacyX != null && legacyX.Version.Revision > 0)
            {
                result = 1;
            }
            else if (legacyY != null && legacyY.Version.Revision > 0)
            {
                result = -1;
            }

            return result;
        }

        /// <summary>
        /// A default comparer that compares metadata as strings.
        /// </summary>
        public static readonly IVersionComparer Default = new VersionComparer(VersionComparison.Default);

        /// <summary>
        /// A comparer that uses only the version numbers.
        /// </summary>
        public static readonly IVersionComparer Version = new VersionComparer(VersionComparison.Version);

        /// <summary>
        /// Compares versions without comparing the metadata.
        /// </summary>
        public static readonly IVersionComparer VersionRelease = new VersionComparer(VersionComparison.VersionRelease);

        /// <summary>
        /// A version comparer that follows SemVer 2.0.0 rules.
        /// </summary>
        public static readonly IVersionComparer VersionReleaseMetadata = new VersionComparer(VersionComparison.VersionReleaseMetadata);

        /// <summary>
        /// Compares sets of release labels.
        /// </summary>
        static int CompareReleaseLabels(IEnumerable<string> version1, IEnumerable<string> version2)
        {
            int result = 0;

            IEnumerator<string> a = version1.GetEnumerator();
            IEnumerator<string> b = version2.GetEnumerator();

            bool aExists = a.MoveNext();
            bool bExists = b.MoveNext();

            while (aExists || bExists)
            {
                if (!aExists && bExists)
                    return -1;

                if (aExists && !bExists)
                    return 1;

                // compare the labels
                result = CompareRelease(a.Current, b.Current);

                if (result != 0)
                    return result;

                aExists = a.MoveNext();
                bExists = b.MoveNext();
            }

            return result;
        }

        /// <summary>
        /// Release labels are compared as numbers if they are numeric, otherwise they will be compared
        /// as strings.
        /// </summary>
        static int CompareRelease(string version1, string version2)
        {
            // check if the identifiers are numeric
            bool v1IsNumeric = int.TryParse(version1, out var version1Num);
            bool v2IsNumeric = int.TryParse(version2, out var version2Num);
            int result;
            // if both are numeric compare them as numbers
            if (v1IsNumeric && v2IsNumeric)
            {
                result = version1Num.CompareTo(version2Num);
            }
            else if (v1IsNumeric || v2IsNumeric)
            {
                // numeric labels come before alpha labels
                if (v1IsNumeric)
                {
                    result = -1;
                }
                else
                {
                    result = 1;
                }
            }
            else
            {
                // Ignoring 2.0.0 case sensitive compare. Everything will be compared case insensitively as 2.0.1 specifies.
                result = StringComparer.OrdinalIgnoreCase.Compare(version1, version2);
            }

            return result;
        }
    }

    /// <summary>
    /// Version comparison modes.
    /// </summary>
    public enum VersionComparison : byte
    {
        /// <summary>
        /// Semantic version 2.0.1-rc comparison with additional compares for extra NuGetVersion fields.
        /// </summary>
        Default = 0,

        /// <summary>
        /// Compares only the version numbers.
        /// </summary>
        Version = 1,

        /// <summary>
        /// Include Version number and Release labels in the compare.
        /// </summary>
        VersionRelease = 2,

        /// <summary>
        /// Include all metadata during the compare.
        /// </summary>
        VersionReleaseMetadata = 3,
    }

    public class VersionFormatter : IFormatProvider, ICustomFormatter
    {
        public string Format(string? format, object? arg, IFormatProvider? formatProvider)
        {
#if NET6_0_OR_GREATER
            ArgumentNullException.ThrowIfNull(arg);
#else
            arg = arg ?? throw new ArgumentNullException(nameof(arg));
#endif

            string? formatted = null;
            Type argType = arg.GetType();

            if (argType == typeof(IFormattable))
            {
                formatted = ((IFormattable)arg).ToString(format, formatProvider);
            }
            else if (!string.IsNullOrEmpty(format))
            {
                if (arg is SemanticVersion version)
                {
                    // single char identifiers
                    if (format!.Length == 1)
                    {
                        formatted = Format(format[0], version);
                    }
                    else
                    {
                        StringBuilder sb = new(format.Length);

                        for (int i = 0; i < format.Length; i++)
                        {
                            var s = Format(format[i], version);

                            if (s == null)
                            {
                                sb.Append(format[i]);
                            }
                            else
                            {
                                sb.Append(s);
                            }
                        }

                        formatted = sb.ToString();
                    }
                }
            }

            return formatted ?? string.Empty;
        }

        public object? GetFormat(Type? formatType)
        {
            if (formatType == typeof(ICustomFormatter)
                || formatType == typeof(NuGetVersion)
                || formatType == typeof(SemanticVersion))
            {
                return this;
            }

            return null;
        }

        static string GetNormalizedString(SemanticVersion version)
        {
            StringBuilder sb = new StringBuilder();

            sb.Append(Format('V', version));

            if (version.IsPrerelease)
            {
                sb.Append('-');
                sb.Append(version.Release);
            }

            if (version.HasMetadata)
            {
                sb.Append('+');
                sb.Append(version.Metadata);
            }

            return sb.ToString();
        }

        static string? Format(char c, SemanticVersion version)
        {
            string? s = null;
            switch (c)
            {
                case 'N':
                    s = GetNormalizedString(version);
                    break;
                case 'R':
                    s = version.Release;
                    break;
                case 'M':
                    s = version.Metadata;
                    break;
                case 'V':
                    s = FormatVersion(version);
                    break;
                case 'x':
                    s = string.Format(CultureInfo.InvariantCulture, "{0}", version.Major);
                    break;
                case 'y':
                    s = string.Format(CultureInfo.InvariantCulture, "{0}", version.Minor);
                    break;
                case 'z':
                    s = string.Format(CultureInfo.InvariantCulture, "{0}", version.Patch);
                    break;
                case 'r':
                    var nuGetVersion = version as NuGetVersion;
                    s = string.Format(CultureInfo.InvariantCulture, "{0}", nuGetVersion != null && nuGetVersion.IsLegacyVersion ? nuGetVersion.Version.Revision : 0);
                    break;
            }

            return s;
        }

        static string FormatVersion(SemanticVersion version)
        {
            var nuGetVersion = version as NuGetVersion;
            bool legacy = nuGetVersion != null && nuGetVersion.IsLegacyVersion;

            return string.Format(CultureInfo.InvariantCulture, "{0}.{1}.{2}{3}", version.Major, version.Minor, version.Patch,
                legacy ? string.Format(CultureInfo.InvariantCulture, ".{0}", nuGetVersion!.Version.Revision) : null);
        }
    }

    /// <summary>
    /// Hash code creator, based on the original NuGet hash code combiner/ASP hash code combiner implementations
    /// </summary>
    internal sealed class HashCodeCombiner
    {
        // seed from String.GetHashCode()
        const long Seed = 0x1505L;

        long _combinedHash;

        internal HashCodeCombiner()
        {
            _combinedHash = Seed;
        }

        internal int CombinedHash => _combinedHash.GetHashCode();

        internal void AddInt32(int i)
        {
            _combinedHash = ((_combinedHash << 5) + _combinedHash) ^ i;
        }

        internal void AddObject(int i)
        {
            AddInt32(i);
        }

        internal void AddObject(bool b)
        {
            AddInt32(b.GetHashCode());
        }

        internal void AddObject(object? o)
        {
            if (o != null)
            {
                AddInt32(o.GetHashCode());
            }
        }

        /// <summary>
        /// Create a unique hash code for the given set of items
        /// </summary>
        internal static int GetHashCode(params object[] objects)
        {
            HashCodeCombiner combiner = new();

            foreach (object obj in objects)
            {
                combiner.AddObject(obj);
            }

            return combiner.CombinedHash;
        }
    }
}
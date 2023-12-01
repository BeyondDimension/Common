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
namespace System.Runtime.Versioning;

/// <summary>
/// 严格的 SemVer 实现
/// </summary>
public partial class SemanticVersion
{
    /// <summary>
    /// 发布标签集合
    /// </summary>
    protected readonly IEnumerable<string>? _releaseLabels;

    /// <summary>
    /// 用于描述版本的附加信息
    /// </summary>
    protected readonly string? _metadata;

    /// <summary>
    /// 表示版本号的 <see cref="Version"/> 对象
    /// </summary>
    protected readonly Version _version;

    /// <summary>
    /// 初始化 <see cref="SemanticVersion"/> 类的新实例
    /// </summary>
    /// <param name="version"></param>
    public SemanticVersion(SemanticVersion version)
        : this(version.Major, version.Minor, version.Patch, version.ReleaseLabels, version.Metadata)
    {
    }

    /// <summary>
    /// 创建语义版本 X.Y.Z
    /// </summary>
    /// <param name="major">X.y.z</param>
    /// <param name="minor">x.Y.z</param>
    /// <param name="patch">x.y.Z</param>
    public SemanticVersion(int major, int minor, int patch)
        : this(major, minor, patch, Enumerable.Empty<string>(), null)
    {
    }

    /// <summary>
    /// 创建 NuGetVersion X.Y.Z-alpha
    /// </summary>
    /// <param name="major">X.y.z</param>
    /// <param name="minor">x.Y.z</param>
    /// <param name="patch">x.y.Z</param>
    /// <param name="releaseLabel">预发布标签</param>
    public SemanticVersion(int major, int minor, int patch, string? releaseLabel)
        : this(major, minor, patch, ParseReleaseLabels(releaseLabel), null)
    {
    }

    /// <summary>
    /// 创建 NuGetVersion X.Y.Z-alpha#build01
    /// </summary>
    /// <param name="major">X.y.z</param>
    /// <param name="minor">x.Y.z</param>
    /// <param name="patch">x.y.Z</param>
    /// <param name="releaseLabel">预发布标签</param>
    /// <param name="metadata">生成元数据</param>
    public SemanticVersion(int major, int minor, int patch, string? releaseLabel, string? metadata)
        : this(major, minor, patch, ParseReleaseLabels(releaseLabel), metadata)
    {
    }

    /// <summary>
    /// 创建 NuGetVersion X.Y.Z-alpha 1.2#build01
    /// </summary>
    /// <param name="major">X.y.z</param>
    /// <param name="minor">x.Y.z</param>
    /// <param name="patch">x.y.Z</param>
    /// <param name="releaseLabels">释放已由点分隔符拆分的标签</param>
    /// <param name="metadata">生成元数据</param>
    public SemanticVersion(int major, int minor, int patch, IEnumerable<string>? releaseLabels, string? metadata)
        : this(new Version(major, minor, patch, 0), releaseLabels, metadata)
    {
    }

    /// <summary>
    /// 初始化 <see cref="SemanticVersion"/> 类的新实例
    /// </summary>
    /// <param name="version"></param>
    /// <param name="releaseLabel"></param>
    /// <param name="metadata"></param>
    protected SemanticVersion(Version version, string? releaseLabel = null, string? metadata = null)
        : this(version, ParseReleaseLabels(releaseLabel), metadata)
    {
    }

    /// <summary>
    /// 初始化 <see cref="SemanticVersion"/> 类的新实例
    /// </summary>
    /// <param name="major"></param>
    /// <param name="minor"></param>
    /// <param name="patch"></param>
    /// <param name="revision"></param>
    /// <param name="releaseLabel"></param>
    /// <param name="metadata"></param>
    protected SemanticVersion(int major, int minor, int patch, int revision, string? releaseLabel, string? metadata)
        : this(major, minor, patch, revision, ParseReleaseLabels(releaseLabel), metadata)
    {
    }

    /// <summary>
    /// 初始化 <see cref="SemanticVersion"/> 类的新实例
    /// </summary>
    /// <param name="major"></param>
    /// <param name="minor"></param>
    /// <param name="patch"></param>
    /// <param name="revision"></param>
    /// <param name="releaseLabels"></param>
    /// <param name="metadata"></param>
    protected SemanticVersion(int major, int minor, int patch, int revision, IEnumerable<string>? releaseLabels, string? metadata)
        : this(new Version(major, minor, patch, revision), releaseLabels, metadata)
    {
    }

    /// <summary>
    /// 初始化 <see cref="SemanticVersion"/> 类的新实例
    /// </summary>
    /// <param name="version"></param>
    /// <param name="releaseLabels"></param>
    /// <param name="metadata"></param>
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
    /// 主要版本 X (X.y.z)
    /// </summary>
    public int Major => _version.Major;

    /// <summary>
    /// 次要版本 Y (x.Y.z)
    /// </summary>
    public int Minor => _version.Minor;

    /// <summary>
    /// 修补程序版本 Z (x.y.Z)
    /// </summary>
    public int Patch => _version.Build;

    /// <summary>
    /// 附加到版本的预发布标签的集合
    /// </summary>
    public IEnumerable<string> ReleaseLabels => _releaseLabels ?? Enumerable.Empty<string>();

    /// <summary>
    /// 该版本的完整预发布标签
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
    /// 如果版本存在预发布标签，则为 <see langword="true"/>
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
    /// 如果版本存在元数据，则为 <see langword="true"/>
    /// </summary>
    public virtual bool HasMetadata => !string.IsNullOrEmpty(Metadata);

    /// <summary>
    /// 生成附加到版本的元数据
    /// </summary>
    public virtual string? Metadata => _metadata;
}

partial class SemanticVersion : IFormattable, IComparable, IComparable<SemanticVersion>, IEquatable<SemanticVersion>
{
    /// <summary>
    /// 提供版本的规范化表示
    /// </summary>
    public virtual string ToNormalizedString()
    {
        return ToString("N", new VersionFormatter());
    }

    /// <inheritdoc cref="ToNormalizedString"/>
    public override string ToString()
    {
        return ToNormalizedString();
    }

    /// <summary>
    /// 将 SemanticVersion 对象转换为字符串
    /// </summary>
    /// <param name="format">指定格式字符串</param>
    /// <param name="formatProvider">提供自定义格式化的对象</param>
    /// <returns>格式化后的字符串表示</returns>
    public virtual string ToString(string? format, IFormatProvider? formatProvider)
    {
        if (formatProvider == null || !TryFormatter(format, formatProvider, out var formattedString))
        {
            formattedString = ToString();
        }

        return formattedString ?? string.Empty;
    }

    /// <summary>
    /// 尝试使用自定义格式化对象对 SemanticVersion 进行格式化
    /// </summary>
    /// <param name="format">指定格式字符串</param>
    /// <param name="formatProvider">提供自定义格式化的对象</param>
    /// <param name="formattedString">格式化后的字符串</param>
    /// <returns>如果成功格式化，则返回true；否则返回false</returns>
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

    /// <summary>
    /// 获取 <see cref="SemanticVersion"/> 对象的哈希码
    /// </summary>
    /// <returns>SemanticVersion 对象的哈希码</returns>
    public override int GetHashCode()
    {
        return VersionComparer.Default.GetHashCode(this);
    }

    /// <inheritdoc/>
    public virtual int CompareTo(object? obj)
    {
        return CompareTo(obj as SemanticVersion);
    }

    /// <inheritdoc/>
    public virtual int CompareTo(SemanticVersion? other)
    {
        return CompareTo(other, VersionComparison.Default);
    }

    /// <inheritdoc/>
    public override bool Equals(object? obj)
    {
        return Equals(obj as SemanticVersion);
    }

    /// <inheritdoc/>
    public virtual bool Equals(SemanticVersion? other)
    {
        return Equals(other, VersionComparison.Default);
    }

    /// <summary>
    /// 如果 <see cref="SemanticVersion"/> 对象基于给定的比较模式相等，则为 <see langword="true"/>
    /// </summary>
    public virtual bool Equals(SemanticVersion? other, VersionComparison versionComparison)
    {
        return CompareTo(other, versionComparison) == 0;
    }

    /// <summary>
    /// 使用给定的比较模式比较 <see cref="SemanticVersion"/> 对象
    /// </summary>
    /// <param name="other"></param>
    /// <param name="versionComparison"></param>
    /// <returns>返回比较结果的数值</returns>
    public virtual int CompareTo(SemanticVersion? other, VersionComparison versionComparison)
    {
        VersionComparer comparer = new(versionComparison);
        return comparer.Compare(this, other);
    }

    /// <summary>
    /// 判断两个语义化版本是否相等
    /// </summary>
    /// <param name="version1">要比较的第一个语义化版本</param>
    /// <param name="version2">要比较的第二个语义化版本</param>
    /// <returns>如果两个版本相等，则为 <see langword="true"/>；否则为 <see langword="false"/></returns>
    public static bool operator ==(SemanticVersion? version1, SemanticVersion? version2) => Compare(version1, version2) == 0;

    /// <summary>
    /// 判断两个语义化版本是否不相等
    /// </summary>
    /// <param name="version1">要比较的第一个语义化版本</param>
    /// <param name="version2">要比较的第二个语义化版本</param>
    /// <returns>如果两个版本不相等，则为 <see langword="true"/>；否则为 <see langword="false"/></returns>
    public static bool operator !=(SemanticVersion? version1, SemanticVersion? version2) => Compare(version1, version2) != 0;

    /// <summary>
    /// 判断第一个语义化版本是否小于第二个语义化版本
    /// </summary>
    /// <param name="version1">要比较的第一个语义化版本</param>
    /// <param name="version2">要比较的第二个语义化版本</param>
    /// <returns>如果第一个版本小于第二个版本，则为 <see langword="true"/>；否则为 <see langword="false"/></returns>
    public static bool operator <(SemanticVersion? version1, SemanticVersion? version2) => Compare(version1, version2) < 0;

    /// <summary>
    /// 判断第一个语义化版本是否小于等于第二个语义化版本
    /// </summary>
    /// <param name="version1">要比较的第一个语义化版本</param>
    /// <param name="version2">要比较的第二个语义化版本</param>
    /// <returns>如果第一个版本小于等于第二个版本，则为 <see langword="true"/>；否则为 <see langword="false"/></returns>
    public static bool operator <=(SemanticVersion? version1, SemanticVersion? version2) => Compare(version1, version2) <= 0;

    /// <summary>
    /// 判断第一个语义化版本是否大于第二个语义化版本
    /// </summary>
    /// <param name="version1">要比较的第一个语义化版本</param>
    /// <param name="version2">要比较的第二个语义化版本</param>
    /// <returns>如果第一个版本大于等于第二个版本，则为 <see langword="true"/>；否则为 <see langword="false"/></returns>
    public static bool operator >(SemanticVersion? version1, SemanticVersion? version2) => Compare(version1, version2) > 0;

    /// <summary>
    /// 判断第一个语义化版本是否大于等于第二个语义化版本
    /// </summary>
    /// <param name="version1">要比较的第一个语义化版本</param>
    /// <param name="version2">要比较的第二个语义化版本</param>
    /// <returns>如果第一个版本大于等于第二个版本，则为 <see langword="true"/>；否则为 <see langword="false"/></returns>
    public static bool operator >=(SemanticVersion? version1, SemanticVersion? version2) => Compare(version1, version2) >= 0;

    /// <summary>
    /// 比较两个语义化版本的大小
    /// </summary>
    /// <param name="version1">要比较的第一个语义化版本</param>
    /// <param name="version2">要比较的第二个语义化版本</param>
    /// <returns>如果第一个版本小于第二个版本，则为负数；如果两个版本相等，则为零；如果第一个版本大于第二个版本，则为正数</returns>
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
    /// 解析版本字符串，解析失败引发 <see cref="ArgumentException"/> 异常
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
    /// 尝试解析版本字符串
    /// </summary>
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

    /// <summary>
    /// 判断字符是否为字母数字或破折号
    /// </summary>
    /// <param name="c">要判断的字符</param>
    /// <returns><see langword="true"/> 表示是字母数字或破折号，<see langword="false"/> 表示不是</returns>
    internal static bool IsLetterOrDigitOrDash(char c)
    {
        int x = c;

        // "ABCDEFGHIJKLMNOPQRSTUVWXYZabcdefghijklmnopqrstuvwxyz0123456789-"
        return (x >= 48 && x <= 57) || (x >= 65 && x <= 90) || (x >= 97 && x <= 122) || x == 45;
    }

    /// <summary>
    /// 判断字符串是否为有效的语义版本
    /// </summary>
    /// <param name="s">要判断的字符串</param>
    /// <param name="allowLeadingZeros">是否允许以零开头的版本号部分</param>
    /// <returns><see langword="true"/> 表示是有效的语义版本，<see langword="false"/> 表示不是</returns>
    internal static bool IsValid(string s, bool allowLeadingZeros)
    {
        return s.Split('.').All(p => IsValidPart(p, allowLeadingZeros));
    }

    /// <summary>
    /// 判断字符串的版本号部分是否有效
    /// </summary>
    /// <param name="s">要判断的版本号部分</param>
    /// <param name="allowLeadingZeros">是否允许以零开头的版本号部分</param>
    /// <returns><see langword="true"/> 表示是有效的版本号部分，<see langword="false"/> 表示不是</returns>
    internal static bool IsValidPart(string s, bool allowLeadingZeros)
    {
        return IsValidPart(s.ToCharArray(), allowLeadingZeros);
    }

    /// <summary>
    /// 判断字符数组的版本号部分是否有效
    /// </summary>
    /// <param name="chars">要判断的字符数组</param>
    /// <param name="allowLeadingZeros">是否允许以零开头的版本号部分</param>
    /// <returns><see langword="true"/> 表示是有效的版本号部分，<see langword="false"/> 表示不是</returns>
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
    /// 将版本字符串解析为版本 /release/build
    /// 此代码的目标是采用最直接和优化的路径
    /// 解析和验证 semver，Regex 会更干净
    /// 但是由于在 NuGet-Regex 中创建的版本数量太慢
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

    /// <summary>
    /// 将版本号规范化
    /// </summary>
    /// <param name="version">待规范化的版本号</param>
    /// <returns>规范化后的版本号</returns>
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

    /// <summary>
    /// 解析发布标签
    /// </summary>
    /// <param name="releaseLabels">发布标签字符串</param>
    /// <returns>解析后的发布标签数组</returns>
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
    /// 表示能够对 <see cref="SemanticVersion"/> 对象进行排序并确定其相等性的版本比较器
    /// </summary>
    public interface IVersionComparer : IEqualityComparer<SemanticVersion?>, IComparer<SemanticVersion?>
    {
    }

    /// <summary>
    /// 版本比较器实现类
    /// </summary>
    public sealed class VersionComparer : IVersionComparer
    {
        readonly VersionComparison _mode;

        /// <summary>
        /// 初始化默认版本比较模式的 <see cref="VersionComparer"/> 的新实例
        /// </summary>
        public VersionComparer()
        {
            _mode = VersionComparison.Default;
        }

        /// <summary>
        /// 初始化给定版本比较模式的 <see cref="VersionComparer"/> 的新实例
        /// </summary>
        /// <param name="versionComparison">比较模式</param>
        public VersionComparer(VersionComparison versionComparison)
        {
            _mode = versionComparison;
        }

        /// <summary>
        /// 确定两个版本是否相等
        /// </summary>
        public bool Equals(SemanticVersion? x, SemanticVersion? y)
        {
            return Compare(x, y) == 0;
        }

        /// <summary>
        /// 使用 <see cref="VersionComparison"/> 模式比较给定的版本
        /// </summary>
        public static int Compare(SemanticVersion? version1, SemanticVersion? version2, VersionComparison versionComparison)
        {
            var comparer = new VersionComparer(versionComparison);
            return comparer.Compare(version1, version2);
        }

        /// <summary>
        /// 提供基于规范化版本字符串的哈希代码
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
        /// 比较版本
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
        /// 比较版本号的第四位数字
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
        /// 将元数据作为字符串进行比较的默认比较器
        /// </summary>
        public static readonly IVersionComparer Default = new VersionComparer(VersionComparison.Default);

        /// <summary>
        /// 仅使用版本号的比较器
        /// </summary>
        public static readonly IVersionComparer Version = new VersionComparer(VersionComparison.Version);

        /// <summary>
        /// 比较版本而不比较元数据
        /// </summary>
        public static readonly IVersionComparer VersionRelease = new VersionComparer(VersionComparison.VersionRelease);

        /// <summary>
        /// 遵循 SemVer 2.0.0 规则的版本比较器
        /// </summary>
        public static readonly IVersionComparer VersionReleaseMetadata = new VersionComparer(VersionComparison.VersionReleaseMetadata);

        /// <summary>
        /// 比较发布标签集
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
        /// 如果发布标签是数字，则将其作为数字进行比较，否则将进行比较
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
    /// 版本比较模式
    /// </summary>
    public enum VersionComparison : byte
    {
        /// <summary>
        /// 语义版本 2.0.1-rc 比较，对额外的 NuGetVersion 字段进行额外比较
        /// </summary>
        Default = 0,

        /// <summary>
        /// 仅比较版本号
        /// </summary>
        Version = 1,

        /// <summary>
        /// 在比较中包括版本号和发布标签
        /// </summary>
        VersionRelease = 2,

        /// <summary>
        /// 在比较期间包括所有元数据
        /// </summary>
        VersionReleaseMetadata = 3,
    }

    /// <summary>
    /// 版本格式化器，实现 <see cref="IFormatProvider"/> 和 <see cref="ICustomFormatter"/> 接口
    /// </summary>
    public class VersionFormatter : IFormatProvider, ICustomFormatter
    {
        /// <summary>
        /// 格式化指定对象
        /// </summary>
        /// <param name="format">格式字符串</param>
        /// <param name="arg">要格式化的对象</param>
        /// <param name="formatProvider">用于提供格式化信息的对象</param>
        /// <returns>格式化结果</returns>
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

        /// <summary>
        /// 获取提供指定类型格式化信息的对象
        /// </summary>
        /// <param name="formatType">要获取的格式化信息的类型</param>
        /// <returns>指定类型的格式化信息对象</returns>
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

        /// <summary>
        /// 获取规范化字符串
        /// </summary>
        /// <param name="version">语义版本对象</param>
        /// <returns>规范化的版本字符串</returns>
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

        /// <summary>
        /// 格式化版本字符串
        /// </summary>
        /// <param name="c">格式化字符</param>
        /// <param name="version">语义版本对象</param>
        /// <returns>格式化后的字符串</returns>
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

        /// <summary>
        /// 格式化完整版本字符串
        /// </summary>
        /// <param name="version">语义版本对象</param>
        /// <returns>格式化后的完整版本字符串</returns>
        static string FormatVersion(SemanticVersion version)
        {
            var nuGetVersion = version as NuGetVersion;
            bool legacy = nuGetVersion != null && nuGetVersion.IsLegacyVersion;

            return string.Format(CultureInfo.InvariantCulture, "{0}.{1}.{2}{3}", version.Major, version.Minor, version.Patch,
                legacy ? string.Format(CultureInfo.InvariantCulture, ".{0}", nuGetVersion!.Version.Revision) : null);
        }
    }

    /// <summary>
    /// 哈希代码创建者，基于原始 NuGet 哈希代码组合器 /ASP 哈希代码组合程序实现
    /// </summary>
    internal sealed class HashCodeCombiner
    {
        /// <summary>
        /// 字符串中的种子
        /// </summary>
        const long Seed = 0x1505L;

        /// <summary>
        /// 组合哈希码的存储变量
        /// </summary>
        long _combinedHash;

        /// <summary>
        /// 初始化 <see cref="HashCodeCombiner"/> 类的新实例
        /// </summary>
        internal HashCodeCombiner()
        {
            _combinedHash = Seed;
        }

        /// <summary>
        /// 返回 <see cref="_combinedHash"/> 的哈希代码
        /// </summary>
        internal int CombinedHash => _combinedHash.GetHashCode();

        /// <summary>
        /// 向组合哈希码中添加一个 32 位整数
        /// </summary>
        /// <param name="i">要添加的 32 位整数</param>
        internal void AddInt32(int i)
        {
            _combinedHash = ((_combinedHash << 5) + _combinedHash) ^ i;
        }

        /// <summary>
        /// 向组合哈希码中添加一个整数
        /// </summary>
        /// <param name="i">要添加的对象</param>
        internal void AddObject(int i)
        {
            AddInt32(i);
        }

        /// <summary>
        /// 向组合哈希码中添加一个布尔值，使用布尔值的哈希码进行组合
        /// </summary>
        /// <param name="b">要添加的布尔值</param>
        internal void AddObject(bool b)
        {
            AddInt32(b.GetHashCode());
        }

        /// <summary>
        /// 向组合哈希码中添加一个对象，使用该对象的哈希码进行组合
        /// </summary>
        /// <param name="o">要添加的对象</param>
        internal void AddObject(object? o)
        {
            if (o != null)
            {
                AddInt32(o.GetHashCode());
            }
        }

        /// <summary>
        /// 为给定的项目集创建唯一的哈希代码
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
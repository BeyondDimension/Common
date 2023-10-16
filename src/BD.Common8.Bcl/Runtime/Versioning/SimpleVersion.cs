// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// https://github.com/dotnet/msbuild/blob/v17.8.0-preview-23418-03/src/Build/Utilities/SimpleVersion.cs

#pragma warning disable IDE0079 // 请删除不必要的忽略
#pragma warning disable CA1512 // 使用 ArgumentOutOfRangeException 引发帮助程序
#pragma warning restore IDE0079 // 请删除不必要的忽略

namespace System.Runtime.Versioning;

/// <summary>
/// Simple replacement for System.Version used to implement version
/// comparison intrinic property functions.
///
/// Allows major version only (e.g. "3" is 3.0.0.0), ignores leading 'v'
/// (e.g. "v3.0" is 3.0.0.0).
///
/// Ignores semver prerelease and metadata portions (e.g. "1.0.0-preview+info"
/// is 1.0.0.0).
///
/// Treats unspecified components as 0 (e.g. x == x.0 == x.0.0 == x.0.0.0).
///
/// Ignores leading and trailing whitespace, but does not tolerate whitespace
/// between components, unlike System.Version.
///
/// Also unlike System.Version, '+' is ignored as semver metadata as described
/// above, not tolerated as positive sign of integer component.
/// </summary>
/// <remarks>
/// Tolerating leading 'v' allows using $(TargetFrameworkVersion) directly.
///
/// Ignoring semver portions allows, for example, checking >= major.minor
/// while still in development of that release.
///
/// Implemented as a struct to avoid heap allocation. Parsing is done
/// without heap allocation at all on .NET Core. However, on .NET Framework,
/// the integer component substrings are allocated as there is no int.Parse
/// on span there.
/// </remarks>
public readonly struct SimpleVersion : IEquatable<SimpleVersion>, IComparable<SimpleVersion>
{
    /// <summary>
    /// 获取当前 <see cref="SimpleVersion"/> 对象版本号的主要版本号部分的值。
    /// </summary>
    public readonly int Major;

    /// <summary>
    /// 获取当前 <see cref="SimpleVersion"/> 对象版本号的次要版本号部分的值。
    /// </summary>
    public readonly int Minor;

    /// <summary>
    /// 获取当前 <see cref="SimpleVersion"/> 对象版本号的内部版本号部分的值。
    /// </summary>
    public readonly int Build;

    /// <summary>
    /// 获取当前 <see cref="SimpleVersion"/> 对象版本号的修订号部分的值。
    /// </summary>
    public readonly int Revision;

    /// <summary>
    /// 使用指定的主版本号、次版本号、内部版本号和修订号初始化 <see cref="SimpleVersion"/> 类的新实例。
    /// </summary>
    /// <param name="major">主版本号。</param>
    /// <param name="minor">次版本号。</param>
    /// <param name="build">内部版本号。</param>
    /// <param name="revision">修订号。</param>
    /// <exception cref="ArgumentOutOfRangeException">major、minor、build 或 revision 小于零。</exception>
    public SimpleVersion(int major, int minor = 0, int build = 0, int revision = 0)
    {
        if (major < 0)
        {
            throw new ArgumentOutOfRangeException(nameof(major));
        }

        if (minor < 0)
        {
            throw new ArgumentOutOfRangeException(nameof(minor));
        }

        if (build < 0)
        {
            throw new ArgumentOutOfRangeException(nameof(build));
        }

        if (revision < 0)
        {
            throw new ArgumentOutOfRangeException(nameof(revision));
        }

        Major = major;
        Minor = minor;
        Build = build;
        Revision = revision;
    }

    /// <summary>
    /// 返回一个值，该值指示当前 <see cref="SimpleVersion"/> 对象和指定的 <see cref="SimpleVersion"/> 对象是否表示同一个值。
    /// </summary>
    /// <param name="other">要与当前的 <see cref="SimpleVersion"/> 对象进行比较的 <see cref="SimpleVersion"/> 对象，或者为 null。</param>
    /// <returns></returns>
    public bool Equals(SimpleVersion other)
    {
        return Major == other.Major &&
               Minor == other.Minor &&
               Build == other.Build &&
               Revision == other.Revision;
    }

    /// <summary>
    /// 将当前 <see cref="SimpleVersion"/> 对象与指定的 <see cref="SimpleVersion"/> 对象进行比较，并返回二者相对值的一个指示。
    /// </summary>
    /// <param name="other">要与当前的 <see cref="SimpleVersion"/> 对象进行比较的 <see cref="SimpleVersion"/> 对象，或者为 default。</param>
    /// <returns></returns>
    public int CompareTo(SimpleVersion other)
    {
        return Major != other.Major ? (Major > other.Major ? 1 : -1) :
               Minor != other.Minor ? (Minor > other.Minor ? 1 : -1) :
               Build != other.Build ? (Build > other.Build ? 1 : -1) :
               Revision != other.Revision ? (Revision > other.Revision ? 1 : -1) :
               0;
    }

    /// <summary>
    /// 返回一个值，该值指示当前 <see cref="SimpleVersion"/> 对象和指定的 <see cref="SimpleVersion"/> 对象是否表示同一个值。
    /// </summary>
    /// <param name="obj">要与当前的 <see cref="SimpleVersion"/> 对象进行比较的 <see cref="SimpleVersion"/> 对象，或者为 null。</param>
    /// <returns></returns>
    public override bool Equals(object? obj) => obj is SimpleVersion v && Equals(v);

    /// <summary>
    /// 返回当前 <see cref="SimpleVersion"/> 对象的哈希代码。
    /// </summary>
    /// <returns>32 位有符号整数哈希代码。</returns>
    public override int GetHashCode() => (Major, Minor, Build, Revision).GetHashCode();

    /// <summary>
    /// 将当前 <see cref="SimpleVersion"/> 对象的值转换为其等效的 <see cref="string"/> 表示形式。
    /// </summary>
    /// <returns>当前 <see cref="string"/> 对象的主要版本号、次要版本号、内部版本号和修订号部分的值的 <see cref="SimpleVersion"/> 表示形式（遵循下面所示格式）。 各部分之间由句点字符（“.”）分隔。 </returns>
    public override string ToString() =>
#if NETFRAMEWORK && !NET46_OR_GREATER
        $"{Major}.{Minor}.{Build}.{Revision}";
#else
        FormattableString.Invariant($"{Major}.{Minor}.{Build}.{Revision}");
#endif

    public static bool operator ==(SimpleVersion a, SimpleVersion b) => a.Equals(b);

    public static bool operator !=(SimpleVersion a, SimpleVersion b) => !a.Equals(b);

    public static bool operator <(SimpleVersion a, SimpleVersion b) => a.CompareTo(b) < 0;

    public static bool operator <=(SimpleVersion a, SimpleVersion b) => a.CompareTo(b) <= 0;

    public static bool operator >(SimpleVersion a, SimpleVersion b) => a.CompareTo(b) > 0;

    public static bool operator >=(SimpleVersion a, SimpleVersion b) => a.CompareTo(b) >= 0;

    /// <summary>
    /// 将版本号的字符串表示形式转换为等效的 <see cref="SimpleVersion"/> 对象。
    /// </summary>
    /// <param name="input">包含要转换的版本号的字符串。</param>
    /// <returns>一个等效于 input 参数中指定的版本号的对象。</returns>
    /// <exception cref="ArgumentNullException">input 为 null。</exception>
    public static SimpleVersion Parse(string input)
    {
#if NET6_0_OR_GREATER
        ArgumentNullException.ThrowIfNull(input);
#else
        input = input ?? throw new ArgumentNullException(nameof(input));
#endif

        var span = RemoveTrivia(input);

        int minor = 0, build = 0, revision = 0;

        if (ParseComponent(ref span, out int major) &&
            ParseComponent(ref span, out minor) &&
            ParseComponent(ref span, out build) &&
            ParseComponent(ref span, out revision))
        {
            // More than 4 components (too many dots)
            InvalidVersionFormat();
        }

        return new SimpleVersion(major, minor, build, revision);
    }

    static readonly char[] semverSeparators = ['-', '+'];

    static ReadOnlySpan<char> RemoveTrivia(string input)
    {
        // Ignore leading/trailing whitespace in input.
        ReadOnlySpan<char> span = input.AsSpan().Trim();

        // Ignore a leading "v".
        if (span.Length > 0 && (span[0] == 'v' || span[0] == 'V'))
        {
            span = span[1..];
        }

        // Ignore semver separator and anything after.
        int separatorIndex = span.IndexOfAny(semverSeparators);
        if (separatorIndex >= 0)
        {
            span = span[..separatorIndex];
        }

        return span;
    }

    static bool ParseComponent(ref ReadOnlySpan<char> span, out int value)
    {
        int dotIndex = span.IndexOf('.');
        if (dotIndex < 0)
        {
            value = ParseComponent(span);
            return false;
        }
        else
        {
            value = ParseComponent(span[..dotIndex]);
            span = span[(dotIndex + 1)..];
            return true;
        }
    }

    static int ParseComponent(ReadOnlySpan<char> span)
    {
#if NETFRAMEWORK || NETSTANDARD2_0
        // Cannot parse int from span on .NET Framework, so allocate the substring
        var spanOrString = span.ToString();
#else
        var spanOrString = span;
#endif

        if (!int.TryParse(spanOrString, NumberStyles.None, CultureInfo.InvariantCulture, out int value))
        {
            InvalidVersionFormat();
        }

        // Cannot parse as negative using NumberStyles.None. Also, +/- would have
        // been stripped as semver trivia earlier.
        Debug.Assert(value >= 0);

        return value;
    }

    [DoesNotReturn]
    static void InvalidVersionFormat()
    {
        // https://github.com/dotnet/msbuild/blob/v17.8.0-preview-23418-03/src/Build/Resources/Strings.resx#L1873-L1875
        throw new FormatException("Version string was not in a correct format.");
    }
}
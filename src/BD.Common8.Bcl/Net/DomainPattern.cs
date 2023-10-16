// https://github.com/dotnetcore/FastGithub/blob/2.1.4/FastGithub.Configuration/DomainPattern.cs

#if NETCOREAPP
namespace System.Net;

/// <summary>
/// 域名表达式
/// <para>* 表示除 . 之外任意 0 到多个字符</para>
/// </summary>
public sealed class DomainPattern : IComparable<DomainPattern>
{
    /// <summary>
    /// 通用分隔符
    /// </summary>
    public const char GeneralSeparator = ';';

    readonly ImmutableArray<Regex> regexs;
    readonly string domainPattern;

    /// <summary>
    /// 排序
    /// </summary>
    public long Order { get; init; }

    /// <summary>
    /// Initializes a new instance of the <see cref="DomainPattern"/> class.
    /// </summary>
    /// <param name="domainPattern"></param>
    public DomainPattern(string domainPattern)
    {
        if (string.IsNullOrWhiteSpace(domainPattern))
            throw new ArgumentNullException(nameof(domainPattern));

        this.domainPattern = domainPattern;

        var items = domainPattern.Split(GeneralSeparator, StringSplitOptions.RemoveEmptyEntries);

        regexs = items.Select(s =>
        {
            var isRegex = s.StartsWith('/');
            if (isRegex)
            {
                return new Regex(s[1..], RegexOptions.IgnoreCase);
            }
            else
            {
                var regexPattern = Regex.Escape(s).Replace(@"\*", @"[^\.]*");
                return new Regex(regexPattern, RegexOptions.IgnoreCase);
            }
        }).ToImmutableArray();
    }

    /// <summary>
    /// 与目标比较
    /// </summary>
    /// <param name="other"></param>
    /// <returns></returns>
    public int CompareTo(DomainPattern? other)
    {
        if (other is null)
        {
            return 1;
        }

        if (Order < other.Order)
        {
            return -1;
        }
        else if (Order > other.Order)
        {
            return 1;
        }

        var segmentsX = domainPattern.Split('.');
        var segmentsY = other.domainPattern.Split('.');
        var value = segmentsX.Length - segmentsY.Length;
        if (value != 0)
        {
            return value;
        }

        for (var i = segmentsX.Length - 1; i >= 0; i--)
        {
            var x = segmentsX[i];
            var y = segmentsY[i];

            value = Compare(x, y);
            if (value == 0)
            {
                continue;
            }
            return value;
        }

        return 0;
    }

    /// <summary>
    /// 比较两个分段
    /// </summary>
    /// <param name="x"></param>
    /// <param name="y"></param>
    /// <returns></returns>
    static int Compare(string x, string y)
    {
        var valueX = x.Replace('*', char.MaxValue);
        var valueY = y.Replace('*', char.MaxValue);
        return valueX.CompareTo(valueY);
    }

    /// <summary>
    /// 是否与指定域名匹配
    /// </summary>
    /// <param name="domain"></param>
    /// <returns></returns>
    public bool IsMatch(string domain) => regexs.Any(s => s.IsMatch(domain));

    /// <inheritdoc/>
    public override string ToString() => domainPattern;
}
#endif
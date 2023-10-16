// https://github.com/dotnetcore/FastGithub/blob/2.1.4/FastGithub.Configuration/TlsSniPattern.cs

#if NETCOREAPP2_0_OR_GREATER || NETSTANDARD2_1_OR_GREATER
namespace System.Net;

/// <summary>
/// SNI 自定义值表达式
/// <list type="bullet">
/// <item>@domain 变量表示取域名值</item>
/// <item>@ipadress 变量表示取 IP</item>
/// <item>@random 变量表示取随机值</item>
/// </list>
/// </summary>
/// <remarks>
/// SNI 自定义值表达式
/// </remarks>
/// <param name="value">表示式值</param>
public struct TlsSniPattern(string? value)
{
    readonly string? _value = value;

    /// <summary>
    /// 获取表示式值
    /// </summary>
    public readonly string Value => _value ?? string.Empty;

    /// <summary>
    /// @domain
    /// </summary>
    public const string DomainValue = "@domain";

    /// <summary>
    /// @ipaddress
    /// </summary>
    public const string IPAddressValue = "@ipaddress";

    /// <summary>
    /// @random
    /// </summary>
    public const string RandomValue = "@random";

    /// <summary>
    /// 无 SNI
    /// </summary>
    public static TlsSniPattern None { get; } = new TlsSniPattern(default);

    /// <summary>
    /// 域名 SNI
    /// </summary>
    public static TlsSniPattern Domain { get; } = new TlsSniPattern(DomainValue);

    /// <summary>
    /// IP 值的 SNI
    /// </summary>
    public static TlsSniPattern IPAddress { get; } = new TlsSniPattern(IPAddressValue);

    /// <summary>
    /// 随机值的 SNI
    /// </summary>
    public static TlsSniPattern Random { get; } = new TlsSniPattern(RandomValue);

    /// <summary>
    /// 更新域名
    /// </summary>
    /// <param name="domain"></param>
    /// <returns></returns>
    public readonly TlsSniPattern WithDomain(string domain)
    {
        if (string.IsNullOrEmpty(_value)) return None;
        var value = _value.Replace(DomainValue, domain,
            StringComparison.OrdinalIgnoreCase);
        return new TlsSniPattern(value);
    }

    /// <summary>
    /// 更新 IP 地址
    /// </summary>
    /// <param name="address"></param>
    /// <returns></returns>
    public readonly TlsSniPattern WithIPAddress(IPAddress address)
    {
        if (string.IsNullOrEmpty(_value)) return None;
        var value = _value!.Replace(IPAddressValue, address.ToString(),
            StringComparison.OrdinalIgnoreCase);
        return new TlsSniPattern(value);
    }

    /// <summary>
    /// 更新随机数
    /// </summary>
    /// <returns></returns>
    public readonly TlsSniPattern WithRandom()
    {
        if (string.IsNullOrEmpty(_value)) return None;
        var value = _value!.Replace(RandomValue,
#if NETCOREAPP3_0_OR_GREATER
            Environment.TickCount64.ToString(),
#else
            Environment.TickCount.ToString(),
#endif
            StringComparison.OrdinalIgnoreCase);
        return new TlsSniPattern(value);
    }

    /// <inheritdoc/>
    public override readonly string ToString() => Value;
}
#endif
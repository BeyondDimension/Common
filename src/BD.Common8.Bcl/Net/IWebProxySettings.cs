namespace System.Net;

/// <summary>
/// Web 代理的配置参数
/// </summary>
public interface IWebProxySettings
{
    /// <summary>
    /// Web 代理地址
    /// </summary>
    string? WebProxyAddress { get; }

    /// <summary>
    /// Web 代理端口
    /// </summary>
    int? WebProxyPort { get; }

    /// <summary>
    /// Web 代理的身份验证的用户名
    /// </summary>
    string? WebProxyUserName { get; }

    /// <summary>
    /// Web 代理的身份验证的密码
    /// </summary>
    string? WebProxyPassword { get; }
}

/// <inheritdoc cref="IWebProxySettings"/>
[DebuggerDisplay("{WebProxyAddress}:{WebProxyPort}")]
#if !(NETFRAMEWORK && !NET462_OR_GREATER) && !(NETSTANDARD && !NETSTANDARD2_0_OR_GREATER)
[MPObj]
#endif
#if !NETFRAMEWORK && !(NETSTANDARD && !NETSTANDARD2_1_OR_GREATER)
[MP2Obj(MP2SerializeLayout.Explicit)]
#endif
public partial record WebProxySettings : IWebProxySettings
{
    /// <inheritdoc/>
#if !(NETFRAMEWORK && !NET462_OR_GREATER) && !(NETSTANDARD && !NETSTANDARD2_0_OR_GREATER)
    [MPKey(0)]
#endif
#if !NETFRAMEWORK && !(NETSTANDARD && !NETSTANDARD2_1_OR_GREATER)
    [MP2Key(0)]
#endif
    public string? WebProxyAddress { get; set; }

    /// <inheritdoc/>
#if !(NETFRAMEWORK && !NET462_OR_GREATER) && !(NETSTANDARD && !NETSTANDARD2_0_OR_GREATER)
    [MPKey(1)]
#endif
#if !NETFRAMEWORK && !(NETSTANDARD && !NETSTANDARD2_1_OR_GREATER)
    [MP2Key(1)]
#endif
    public int? WebProxyPort { get; set; }

    /// <inheritdoc/>
#if !(NETFRAMEWORK && !NET462_OR_GREATER) && !(NETSTANDARD && !NETSTANDARD2_0_OR_GREATER)
    [MPKey(2)]
#endif
#if !NETFRAMEWORK && !(NETSTANDARD && !NETSTANDARD2_1_OR_GREATER)
    [MP2Key(2)]
#endif
    public string? WebProxyUserName { get; set; }

    /// <inheritdoc/>
#if !(NETFRAMEWORK && !NET462_OR_GREATER) && !(NETSTANDARD && !NETSTANDARD2_0_OR_GREATER)
    [MPKey(3)]
#endif
#if !NETFRAMEWORK && !(NETSTANDARD && !NETSTANDARD2_1_OR_GREATER)
    [MP2Key(3)]
#endif
    public string? WebProxyPassword { get; set; }
}

/// <summary>
/// 提供对 <see cref="IWebProxySettings"/> 类型的扩展函数
/// </summary>
public static partial class WebProxySettingsExtensions
{
    /// <summary>
    /// 根据配置参数创建 <see cref="IWebProxy"/> 实例
    /// </summary>
    /// <param name="settings"></param>
    /// <param name="bypassOnLocal"></param>
    /// <param name="bypassList"></param>
    /// <returns></returns>
    public static IWebProxy? GetWebProxy(this IWebProxySettings? settings,
        bool bypassOnLocal = true,
        [StringSyntax("Regex", new[] { RegexOptions.IgnoreCase | RegexOptions.CultureInvariant })] string[]? bypassList = null)
    {
        if (settings != null)
        {
            if (!string.IsNullOrWhiteSpace(settings?.WebProxyAddress))
            {
                ICredentials? credentials = null;
                if (!string.IsNullOrEmpty(settings.WebProxyUserName) && !string.IsNullOrEmpty(settings.WebProxyPassword))
                    credentials = new NetworkCredential(settings.WebProxyUserName, settings.WebProxyPassword);
                var url = $"{settings.WebProxyAddress}:{settings.WebProxyPort}";
                return new WebProxy(url, bypassOnLocal, bypassList, credentials);
            }
        }
        return default;
    }
}
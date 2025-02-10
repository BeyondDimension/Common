namespace System.Net;

/// <summary>
/// Web 代理的配置参数
/// </summary>
public interface IWebProxySettings
{
    /// <summary>
    /// Web 代理地址，与 <see cref="WebProxyHost"/> 和 <see cref="WebProxyPort"/> 互斥，优先级最高
    /// </summary>
    string? WebProxyAddress { get; }

    /// <summary>
    /// Web 代理 Host，当 <see cref="WebProxyAddress"/> 无效时且 <see cref="WebProxyPort"/> 有效时使用"/>
    /// </summary>
    string? WebProxyHost { get; }

    /// <summary>
    /// Web 代理端口，当 <see cref="WebProxyAddress"/> 无效时且 <see cref="WebProxyHost"/> 有效时使用
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

    /// <summary>
    /// Web 代理的身份验证的域名
    /// </summary>
    string? WebProxyNetworkCredentialDomain { get; }

    /// <summary>
    /// 是否跳过代理服务器而使用本地地址
    /// </summary>
    bool? WebProxyBypassProxyOnLocal { get; }
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

    /// <inheritdoc/>
#if !(NETFRAMEWORK && !NET462_OR_GREATER) && !(NETSTANDARD && !NETSTANDARD2_0_OR_GREATER)
    [MPKey(4)]
#endif
#if !NETFRAMEWORK && !(NETSTANDARD && !NETSTANDARD2_1_OR_GREATER)
    [MP2Key(4)]
#endif
    public string? WebProxyHost { get; set; }

    /// <inheritdoc/>
#if !(NETFRAMEWORK && !NET462_OR_GREATER) && !(NETSTANDARD && !NETSTANDARD2_0_OR_GREATER)
    [MPKey(5)]
#endif
#if !NETFRAMEWORK && !(NETSTANDARD && !NETSTANDARD2_1_OR_GREATER)
    [MP2Key(5)]
#endif
    public string? WebProxyNetworkCredentialDomain { get; }

    /// <inheritdoc/>
#if !(NETFRAMEWORK && !NET462_OR_GREATER) && !(NETSTANDARD && !NETSTANDARD2_0_OR_GREATER)
    [MPKey(6)]
#endif
#if !NETFRAMEWORK && !(NETSTANDARD && !NETSTANDARD2_1_OR_GREATER)
    [MP2Key(6)]
#endif
    public bool? WebProxyBypassProxyOnLocal { get; }
}

/// <summary>
/// 自定义代理构造的包装类，用于比较代理是否相等
/// </summary>
file sealed class WebProxyIdWrap : IWebProxy
{
    readonly WebProxy innerProxy;
    readonly string id;

    /// <summary>
    /// 自定义代理参数的唯一标识，用作比较值是否相等
    /// </summary>
    internal string Id => id;

    static void SetCredentials(WebProxy webProxy, string? userName, string? password, string? domain)
    {
        NetworkCredential? credential = null;
        if (!string.IsNullOrEmpty(userName))
        {
            if (string.IsNullOrWhiteSpace(domain))
            {
                credential = new(userName, password);
            }
            else
            {
                credential = new(userName, password, domain);
            }
        }
        if (credential != null)
        {
            webProxy.Credentials = credential;
        }
    }

    static string GetId(WebProxy webProxy, string? userName, string? password, string? domain)
    {
        var id =
$"""
Address: {webProxy.Address}
BypassProxyOnLocal: {webProxy.BypassProxyOnLocal}
UserName: {ToSecure(userName)}
Password: {ToSecure(password)}
Domain: {domain}
WebProxyIdWrap
""";
        return id;

        static string? ToSecure(string? s)
        {
            var r = Hashs.String.SHA384((s ?? "") + "WebProxyIdWrap.ToSecure");
#if DEBUG
            return
$"""
{s}
{r}
""";
#else
            return r;
#endif
        }
    }

    internal WebProxyIdWrap(string? userName, string? password, string? domain, string host, int port)
    {
        innerProxy = new(host, port);
        SetCredentials(innerProxy, userName, password, domain);
        id = GetId(innerProxy, userName, password, domain);
    }

    internal WebProxyIdWrap(string? userName, string? password, string? domain, string host, int port, bool bypassOnLocal)
    {
        innerProxy = new(host, port)
        {
            BypassProxyOnLocal = bypassOnLocal,
        };
        SetCredentials(innerProxy, userName, password, domain);
        id = GetId(innerProxy, userName, password, domain);
    }

    internal WebProxyIdWrap(string? userName, string? password, string? domain, string? address)
    {
        innerProxy = new(address);
        SetCredentials(innerProxy, userName, password, domain);
        id = GetId(innerProxy, userName, password, domain);
    }

    internal WebProxyIdWrap(string? userName, string? password, string? domain, string? address, bool bypassOnLocal)
    {
        innerProxy = new(address, bypassOnLocal);
        SetCredentials(innerProxy, userName, password, domain);
        id = GetId(innerProxy, userName, password, domain);
    }

    /// <inheritdoc/>
    ICredentials? IWebProxy.Credentials
    {
        get => innerProxy.Credentials;
        set => innerProxy.Credentials = value;
    }

    /// <inheritdoc/>
    Uri? IWebProxy.GetProxy(Uri destination) => innerProxy.GetProxy(destination);

    /// <inheritdoc/>
    bool IWebProxy.IsBypassed(Uri host) => innerProxy.IsBypassed(host);

    /// <inheritdoc/>
    public override string? ToString() => id;
}

/// <summary>
/// 提供对 <see cref="IWebProxySettings"/> 类型的扩展函数
/// </summary>
public static partial class WebProxySettingsExtensions
{
    /// <summary>
    /// 比较两个 <see cref="IWebProxy"/> 实例是否相等
    /// </summary>
    /// <param name="l"></param>
    /// <param name="r"></param>
    /// <returns></returns>
    public static bool IdEquals(
        this IWebProxy? l,
        IWebProxy? r)
    {
        if (l == null && r == null)
        {
            return true;
        }
        else if (l == null || r == null)
        {
            return false;
        }
        else if (l is WebProxyIdWrap l2 && r is WebProxyIdWrap r2)
        {
            return l2.Id == r2.Id;
        }
        else
        {
            return l.Equals(r);
        }
    }

    /// <summary>
    /// 根据配置参数创建 <see cref="IWebProxy"/> 实例，返回类型可通过 <see cref="IdEquals"/> 进行比较，当 <see cref="IWebProxySettings"/> 代理设置相同时，返回的不同实例比较将都相等
    /// </summary>
    /// <param name="settings"></param>
    /// <returns></returns>
    public static IWebProxy? GetWebProxy(
        this IWebProxySettings? settings)
    {
        WebProxyIdWrap? webProxy = null;
        if (settings != null)
        {
            if (!string.IsNullOrWhiteSpace(settings.WebProxyAddress))
            {
                if (settings.WebProxyBypassProxyOnLocal.HasValue)
                {
                    webProxy = new(settings.WebProxyUserName,
                        settings.WebProxyPassword,
                        settings.WebProxyNetworkCredentialDomain,
                        settings.WebProxyAddress,
                        settings.WebProxyBypassProxyOnLocal.Value);
                }
                else
                {
                    webProxy = new(settings.WebProxyUserName,
                        settings.WebProxyPassword,
                        settings.WebProxyNetworkCredentialDomain,
                        settings.WebProxyAddress);
                }
            }
            else if (!string.IsNullOrWhiteSpace(settings.WebProxyHost) && settings.WebProxyPort.HasValue)
            {
                if (settings.WebProxyBypassProxyOnLocal.HasValue)
                {
                    webProxy = new(settings.WebProxyUserName,
                        settings.WebProxyPassword,
                        settings.WebProxyNetworkCredentialDomain,
                        settings.WebProxyHost,
                        settings.WebProxyPort.Value,
                        settings.WebProxyBypassProxyOnLocal.Value);
                }
                else
                {
                    webProxy = new(settings.WebProxyUserName,
                        settings.WebProxyPassword,
                        settings.WebProxyNetworkCredentialDomain,
                        settings.WebProxyHost,
                        settings.WebProxyPort.Value);
                }
            }
        }
        return webProxy;
    }
}
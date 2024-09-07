namespace BD.Common8.AspNetCore.Models;

/// <summary>
/// appsettings.json | appsettings.Development.json 文件的设置项的抽象基类
/// </summary>
public abstract class BMAppSettings : IJWTAppSettings, INotUseForwardedHeaders, IViewsUrl
{
    /// <inheritdoc/>
    public bool NotUseForwardedHeaders { get; set; }

    /// <inheritdoc/>
    public string? ForwardedHeadersKnownProxies { get; set; }

    /// <inheritdoc/>
    public virtual List<IPAddress> GetForwardedHeadersKnownProxies() => INotUseForwardedHeaders.GetForwardedHeadersKnownProxies(ForwardedHeadersKnownProxies);

    /// <inheritdoc/>
    public abstract string SecretKey { get; set; }

    /// <inheritdoc/>
    public abstract string Issuer { get; set; }

    /// <inheritdoc/>
    public abstract string Audience { get; set; }

    /// <summary>
    /// 用于创建一个默认管理员账号的用户名
    /// </summary>
    public abstract string AdminUserName { get; set; }

    /// <summary>
    /// 用于创建一个默认管理员账号的密码
    /// </summary>
    public abstract string AdminPassword { get; set; }

    /// <summary>
    /// 配置允许跨域访问的 Web UI 地址
    /// </summary>
    public virtual string? ViewsUrl { get; set; }

    /// <summary>
    /// 初始化后台系统的哈希盐值
    /// </summary>
    public abstract string? InitSystemSecuritySalt { get; set; }

    /// <summary>
    /// 获取初始化后台系统的哈希盐值
    /// </summary>
    /// <returns></returns>
    public abstract string GetInitSystemSecuritySalt();

    /// <inheritdoc/>
    [IgnoreDataMember]
    [NewtonsoftJsonIgnore]
    [SystemTextJsonIgnore]
    public SigningCredentials? SigningCredentials { get; set; }

    /// <inheritdoc/>
    public virtual TimeSpan AccessExpiration { get; set; } = TimeSpan.FromDays(31);

    /// <inheritdoc/>
    public virtual TimeSpan RefreshExpiration { get; set; } = TimeSpan.FromDays(62);

    /// <inheritdoc/>
    [IgnoreDataMember]
    [NewtonsoftJsonIgnore]
    [SystemTextJsonIgnore]
    public bool UseCors { get; set; }

    /// <summary>
    /// 是否禁用 API 文档
    /// </summary>
    public bool DisabledApiDoc { get; set; }
}

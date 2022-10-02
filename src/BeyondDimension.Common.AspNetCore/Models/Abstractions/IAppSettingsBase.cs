// ReSharper disable once CheckNamespace
namespace BD.Common.Models.Abstractions;

#if !BLAZOR
public partial interface IAppSettingsBase : INotUseForwardedHeaders
{
    /// <summary>
    /// JWT密钥
    /// </summary>
    string SecretKey { get; set; }

    /// <summary>
    /// 发行者
    /// </summary>
    string Issuer { get; set; }

    /// <summary>
    /// 受众
    /// </summary>
    string Audience { get; set; }

    /// <summary>
    /// 过期时间
    /// </summary>
    TimeSpan AccessExpiration { get; set; }

    /// <summary>
    /// 刷新时间
    /// </summary>
    TimeSpan RefreshExpiration { get; set; }

    SigningCredentials? SigningCredentials { get; set; }
}

public partial interface INotUseForwardedHeaders
{
    /// <summary>
    /// 不启用反向代理
    /// </summary>
    bool NotUseForwardedHeaders { get; set; }
}
#endif
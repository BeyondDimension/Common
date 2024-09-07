namespace BD.Common8.AspNetCore.Models.Abstractions;

/// <summary>
/// appsettings.json | appsettings.Development.json 文件的设置项中提供 JWT 配置项的接口
/// </summary>
public interface IJWTAppSettings
{
    /// <summary>
    /// JWT 密钥
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

    /// <summary>
    /// 获取或设置签名凭证
    /// </summary>
    SigningCredentials? SigningCredentials { get; set; }
}

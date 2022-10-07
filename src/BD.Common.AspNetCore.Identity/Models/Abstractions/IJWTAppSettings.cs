namespace BD.Common.Models.Abstractions;

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

    SigningCredentials? SigningCredentials { get; set; }
}

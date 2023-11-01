namespace BD.Common8.AspNetCore.Models.Abstractions;

/// <summary>
/// 抽象类用于处理 JWT 配置
/// </summary>
public abstract class JWTAppSettings
{
    /// <summary>
    /// 密钥，用于生成和验证令牌
    /// </summary>
    public string SecretKey { get; set; } = "";

    /// <summary>
    /// 发行人，用于标识令牌的发行方
    /// </summary>
    public string Issuer { get; set; } = "";

    /// <summary>
    /// 受众，用于指定令牌的受众方
    /// </summary>
    public string Audience { get; set; } = "";

    /// <summary>
    /// 访问令牌过期时间间隔
    /// </summary>
    public TimeSpan AccessExpiration { get; set; }

    /// <summary>
    /// 刷新令牌过期时间间隔
    /// </summary>
    public TimeSpan RefreshExpiration { get; set; }

    /// <summary>
    /// 签名凭证，用于生成令牌的签名，忽略此属性以避免序列化时暴露敏感信息
    /// </summary>
    [IgnoreDataMember]
    [NewtonsoftJsonIgnore]
    [SystemTextJsonIgnore]
    public SigningCredentials? SigningCredentials { get; set; }
}

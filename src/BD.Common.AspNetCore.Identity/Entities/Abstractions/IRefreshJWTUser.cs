// ReSharper disable once CheckNamespace

namespace BD.Common.Entities.Abstractions;

/// <summary>
/// 用户的刷新 JWT 相关数据
/// </summary>
public interface IRefreshJWTUser
{
    /// <summary>
    /// 刷新 Token 值
    /// </summary>
    string? RefreshToken { get; set; }

    /// <summary>
    /// 刷新 Token 值有效期
    /// </summary>
    DateTimeOffset RefreshExpiration { get; set; }

    /// <summary>
    /// 禁止在此时间之前刷新 Token
    /// </summary>
    DateTimeOffset NotBefore { get; set; }
}

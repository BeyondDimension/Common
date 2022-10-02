// ReSharper disable once CheckNamespace

namespace BD.Common.Entities.Abstractions;

public interface IJWTUser
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
    /// 禁止在此时间之前刷新
    /// </summary>
    DateTimeOffset NotBefore { get; set; }
}

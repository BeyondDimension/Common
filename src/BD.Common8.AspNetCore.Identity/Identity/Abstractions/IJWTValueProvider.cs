namespace BD.Common8.AspNetCore.Identity.Abstractions;

/// <summary>
/// 适用于 To B 服务端(后台管理系统)的 JWT 值提供程序
/// </summary>
public interface IJWTValueProvider
{
    /// <summary>
    /// 生成 JWT 令牌
    /// </summary>
    Task<JWTEntity?> GenerateTokenAsync(Guid userId, IEnumerable<string>? roles, Action<List<Claim>>? aciton, CancellationToken cancellationToken = default);
    
    /// <summary>
    /// 密钥最小长度
    /// </summary>
    const int SecretKeyMinLength = 16;

    /// <summary>
    /// 获取安全密钥
    /// </summary>
    static SymmetricSecurityKey GetSecurityKey(string secretKey)
    {
        var data = Encoding.UTF8.GetBytes(secretKey);
        var temp = data.Length - SecretKeyMinLength;
        if (temp < 0) // 低于最低值，补位，填充 1
        {
            temp = Math.Abs(temp);
            data = data.Concat(new byte[temp].Select(x => (byte)'1')).ToArray();
        }
        var signingKey = new SymmetricSecurityKey(data);
        return signingKey;
    }
}

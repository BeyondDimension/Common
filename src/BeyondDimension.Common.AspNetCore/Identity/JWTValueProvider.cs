namespace BD.Common.Identity;

/// <summary>
/// JWT 值提供程序
/// </summary>
/// <typeparam name="TAppSettings"></typeparam>
/// <typeparam name="TAppDbContext"></typeparam>
/// <typeparam name="TUser"></typeparam>
public class JWTValueProvider<TAppSettings, TAppDbContext, TUser> : IJWTValueProvider
    where TAppSettings : AppSettingsBase, IAppSettingsBase
    where TAppDbContext : DbContext, IApplicationDbContext<TUser>
    where TUser : IdentityUser<Guid>, IJWTUser
{
    readonly TAppSettings options;
    readonly TAppDbContext db;
    static readonly RandomNumberGenerator _rng = RandomNumberGenerator.Create();

    public JWTValueProvider(IOptions<TAppSettings> options, TAppDbContext db)
    {
        this.options = options.Value;
        this.db = db;
    }

    public async Task<JWTEntity?> GenerateTokenAsync(Guid userId, IEnumerable<string>? roles, CancellationToken cancellationToken)
    {
        var now = DateTimeOffset.UtcNow;

        JwtSecurityTokenHandler handler = new();

        // Token 过期时间
        var expires = now.Add(options.AccessExpiration);

        var idString = userId.ToString();

        // https://github.com/dotnet/aspnetcore/blob/main/src/Identity/Extensions.Core/src/PasswordHasher.cs#L96
        var refresh_token = GenerateRefreshToken(idString);

        // 刷新 Token 过期时间
        var refresh_token_expires = expires.Add(options.RefreshExpiration);
        // 刷新 Token 必须在过期前 7 天后才能使用
        var refresh_not_before = expires.AddDays(-7);

        await AddOrUpdateTokenAsync(
            userId, refresh_token, refresh_token_expires, refresh_not_before,
            cancellationToken);

        var claims = new List<Claim>
        {
            new Claim(JwtRegisteredClaimNames.Sub, idString),
            new Claim(JwtRegisteredClaimNames.Iat, now.ToUnixTimeMilliseconds().ToString(), ClaimValueTypes.Integer64),
        };

        if (roles != null)
        {
            AddRolesToClaims(claims, roles);
        }

        var jwt = new JwtSecurityToken(
            issuer: options.Issuer,
            audience: options.Audience,
            claims: claims,
            notBefore: now.DateTime,
            expires: expires.DateTime,
            signingCredentials: options.SigningCredentials.ThrowIsNull(nameof(options.SigningCredentials)));

        var encodedJwt = handler.WriteToken(jwt);

        var response = new JWTEntity
        {
            ExpiresIn = expires,
            AccessToken = encodedJwt,
            RefreshToken = refresh_token,
        };

        return response;
    }

    static void AddRolesToClaims(List<Claim> claims, IEnumerable<string> roles)
    {
        foreach (var role in roles)
        {
            var roleClaim = new Claim(ClaimTypes.Role, role);
            claims.Add(roleClaim);
        }
    }

    static string GenerateRefreshToken(string password)
    {
        // https://github.com/dotnet/aspnetcore/blob/main/src/Identity/Extensions.Core/src/PasswordHasher.cs#L141
        const int saltSize = 32;
        const int iterCount = 10000;
        const KeyDerivationPrf prf = KeyDerivationPrf.HMACSHA256;
        const int numBytesRequested = 32;
        byte[] salt = new byte[saltSize];
        _rng.GetBytes(salt);
        byte[] subkey = KeyDerivation.Pbkdf2(password, salt, prf, iterCount, numBytesRequested);

        var outputBytes = new byte[13 + salt.Length + subkey.Length];
        outputBytes[0] = 0x01; // format marker
        WriteNetworkByteOrder(outputBytes, 1, (uint)prf);
        WriteNetworkByteOrder(outputBytes, 5, (uint)iterCount);
        WriteNetworkByteOrder(outputBytes, 9, (uint)saltSize);
        Buffer.BlockCopy(salt, 0, outputBytes, 13, salt.Length);
        Buffer.BlockCopy(subkey, 0, outputBytes, 13 + saltSize, subkey.Length);

        return outputBytes.Base64UrlEncode();
    }

    static void WriteNetworkByteOrder(byte[] buffer, int offset, uint value)
    {
        buffer[offset + 0] = (byte)(value >> 24);
        buffer[offset + 1] = (byte)(value >> 16);
        buffer[offset + 2] = (byte)(value >> 8);
        buffer[offset + 3] = (byte)(value >> 0);
    }

    /// <summary>
    /// 将 刷新 Token 添加或更新到数据库中，并返回 JwtId，这将触发
    /// </summary>
    /// <param name="userId"></param>
    /// <param name="refresh_token"></param>
    /// <param name="refresh_token_expires"></param>
    /// <param name="refresh_not_before"></param>
    /// <param name="cancellationToken"></param>
    /// <returns></returns>
    async Task AddOrUpdateTokenAsync(
        Guid userId,
        string refresh_token,
        DateTimeOffset refresh_token_expires,
        DateTimeOffset refresh_not_before,
        CancellationToken cancellationToken)
    {
        var user = await db.Users.FindAsync(keyValues: new object[] { userId }, cancellationToken);

        user.ThrowIsNull();

        user.RefreshToken = refresh_token;
        user.RefreshExpiration = refresh_token_expires;
        user.NotBefore = refresh_not_before;

        await db.SaveChangesAsync(cancellationToken);
    }
}

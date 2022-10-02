namespace BD.Common.Identity.Abstractions;

public interface IJWTValueProvider
{
    Task<JWTEntity?> GenerateTokenAsync(Guid userId, IEnumerable<string>? roles, CancellationToken cancellationToken = default);

    const int secretKeyMinLength = 16;

    public static SymmetricSecurityKey GetSecurityKey(string secretKey)
    {
        var data = Encoding.UTF8.GetBytes(secretKey);
        var temp = data.Length - secretKeyMinLength;
        if (temp < 0) // 低于最低值，补位，填充 1
        {
            temp = Math.Abs(temp);
            data = data.Concat(new byte[temp].Select(x => (byte)'1')).ToArray();
        }
        var signingKey = new SymmetricSecurityKey(data);
        return signingKey;
    }
}

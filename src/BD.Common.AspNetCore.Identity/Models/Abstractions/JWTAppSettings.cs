namespace BD.Common.Models.Abstractions;

public abstract class JWTAppSettings
{
    public string SecretKey { get; set; } = "";

    public string Issuer { get; set; } = "";

    public string Audience { get; set; } = "";

    public TimeSpan AccessExpiration { get; set; }

    public TimeSpan RefreshExpiration { get; set; }

    [IgnoreDataMember]
    [JsonIgnore]
    public SigningCredentials? SigningCredentials { get; set; }
}

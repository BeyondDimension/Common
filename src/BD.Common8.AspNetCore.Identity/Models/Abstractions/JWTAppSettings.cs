namespace BD.Common8.AspNetCore.Models.Abstractions;

#pragma warning disable SA1600 // Elements should be documented

public abstract class JWTAppSettings
{
    public string SecretKey { get; set; } = "";

    public string Issuer { get; set; } = "";

    public string Audience { get; set; } = "";

    public TimeSpan AccessExpiration { get; set; }

    public TimeSpan RefreshExpiration { get; set; }

    [IgnoreDataMember]
    [NewtonsoftJsonIgnore]
    [SystemTextJsonIgnore]
    public SigningCredentials? SigningCredentials { get; set; }
}

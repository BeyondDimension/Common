// ReSharper disable once CheckNamespace
namespace BD.Common.Entities.Abstractions;

public abstract class JWTUser : IdentityUser<Guid>, IJWTUser
{
    [StringLength(MaxLengths.Url)]
    public string? RefreshToken { get; set; }

    public DateTimeOffset RefreshExpiration { get; set; }

    public DateTimeOffset NotBefore { get; set; }
}

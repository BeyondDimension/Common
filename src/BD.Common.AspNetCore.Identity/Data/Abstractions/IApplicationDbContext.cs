namespace BD.Common.Data.Abstractions;

public interface IApplicationDbContext<TUser> where TUser : IdentityUser<Guid>, IJWTUser
{
    DbSet<TUser> Users { get; }
}

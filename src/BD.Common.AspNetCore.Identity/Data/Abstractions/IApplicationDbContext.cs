namespace BD.Common.Data.Abstractions;

public interface IApplicationDbContext<TUser> where TUser : class, IEntity<Guid>, IRefreshJWTUser
{
    DbSet<TUser> Users { get; }
}
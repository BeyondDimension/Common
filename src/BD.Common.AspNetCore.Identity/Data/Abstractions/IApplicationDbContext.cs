namespace BD.Common.Data.Abstractions;

public interface IApplicationDbContext<TUser> where TUser : class, IEntity<Guid>, IJWTUser
{
    DbSet<TUser> Users { get; }
}
namespace BD.Common8.AspNetCore.Data.Abstractions;

#pragma warning disable SA1600 // Elements should be documented

public interface IApplicationDbContext<[DynamicallyAccessedMembers(IEntity.DynamicallyAccessedMemberTypes)] TUser> where TUser : class, IEntity<Guid>, IRefreshJWTUser
{
    DbSet<TUser> Users { get; }
}
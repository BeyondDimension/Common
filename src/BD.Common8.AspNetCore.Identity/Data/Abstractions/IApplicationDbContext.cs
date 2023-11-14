namespace BD.Common8.AspNetCore.Data.Abstractions;

/// <summary>
/// 提供与用户相关的数据访问功能
/// </summary>
/// <typeparam name="TUser"></typeparam>
public interface IApplicationDbContext<[DynamicallyAccessedMembers(IEntity.DynamicallyAccessedMemberTypes)] TUser> where TUser : class, IEntity<Guid>, IRefreshJWTUser
{
    /// <summary>
    /// 获取用户实体集合的属性
    /// </summary>
    DbSet<TUser> Users { get; }
}
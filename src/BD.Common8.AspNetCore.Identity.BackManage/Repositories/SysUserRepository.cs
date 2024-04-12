namespace BD.Common8.AspNetCore.Repositories;

/// <summary>
/// <see cref="ISysUserRepository"/> 的实现类
/// </summary>
/// <typeparam name="TDbContext"></typeparam>
/// <remarks>
/// Initializes a new instance of the <see cref="SysUserRepository{TDbContext}"/> class.
/// </remarks>
/// <param name="dbContext"></param>
/// <param name="requestAbortedProvider"></param>
sealed class SysUserRepository<TDbContext>(
    TDbContext dbContext,
    IRequestAbortedProvider requestAbortedProvider) : Repository<TDbContext, SysUser, Guid>(dbContext, requestAbortedProvider), ISysUserRepository where TDbContext : DbContext, IApplicationDbContext
{
    //public static readonly Expression<Func<SysUser, SysUserTableItem>> ExpressionTable = x => new()
    //{
    //    Id = x.Id,
    //    UserName = x.UserName,
    //};

    public async Task<PagedModel<SysUserTableItem>> QueryAsync(
              string? userName,
              int current = IPagedModel.DefaultCurrent,
              int pageSize = IPagedModel.DefaultPageSize)
    {
        //var query = Entity.AsNoTrackingWithIdentityResolution();
        //if (!string.IsNullOrEmpty(name))
        //    query = query.Where(x => x.UserName.Contains(name));
        //var r = await query.OrderByDescending(x => x.CreationTime)
        //    .Select(ExpressionTable)
        //    .PagingAsync(current, pageSize, RequestAborted);
        //foreach (var item in r.DataSource)
        //{
        //    var roles = from ur in db.UserRoles.AsNoTrackingWithIdentityResolution()
        //                join rs in db.Roles.AsNoTrackingWithIdentityResolution() on ur.RoleId equals rs.Id
        //                where ur.UserId == item.Id
        //                select rs.Name;
        //    item.Role = await roles.ToArrayAsync();
        //}
        //return r;
        var role = from ur in db.UserRoles.AsNoTrackingWithIdentityResolution()
                   join r in db.Roles.AsNoTrackingWithIdentityResolution() on ur.RoleId equals r.Id
                   select new
                   {
                       ur.UserId,
                       r.Name,
                   };
        var query = from user in db.Users.AsNoTrackingWithIdentityResolution()
                    select new SysUserTableItem
                    {
                        Id = user.Id,
                        UserName = user.UserName,
                        LockoutEnabled = user.LockoutEnabled,
                        Roles = role.Where(x => x.UserId == user.Id).Select(x => x.Name).ToArray(),
                    };
        if (!string.IsNullOrEmpty(userName))
            query = query.Where(x => x.UserName.Contains(userName));
        return await query.PagingAsync(current, pageSize, RequestAborted);
    }
}

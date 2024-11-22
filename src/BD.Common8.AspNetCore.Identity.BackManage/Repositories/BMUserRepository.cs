namespace BD.Common8.AspNetCore.Repositories;

/// <summary>
/// <see cref="IBMUserRepository"/> 的实现类
/// </summary>
/// <typeparam name="TDbContext"></typeparam>
/// <remarks>
/// Initializes a new instance of the <see cref="BMUserRepository{TDbContext}"/> class.
/// </remarks>
/// <param name="dbContext"></param>
/// <param name="requestAbortedProvider"></param>
sealed class BMUserRepository<TDbContext>(
    TDbContext dbContext,
    IRequestAbortedProvider requestAbortedProvider) : Repository<TDbContext, BMUser, Guid>(dbContext, requestAbortedProvider), IBMUserRepository where TDbContext : DbContext, IBMDbContext
{
    //public static readonly Expression<Func<SysUser, SysUserTableItem>> ExpressionTable = x => new()
    //{
    //    Id = x.Id,
    //    UserName = x.UserName,
    //};

    public async Task<PagedModel<BMUserTableItem>> QueryAsync(
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
        //    var roles = from ur in db.SysUserRoles.AsNoTrackingWithIdentityResolution()
        //                join rs in db.SysRoles.AsNoTrackingWithIdentityResolution() on ur.RoleId equals rs.Id
        //                where ur.UserId == item.Id
        //                select rs.Name;
        //    item.Role = await roles.ToArrayAsync();
        //}
        //return r;
        var role = from ur in db.SysUserRoles.AsNoTrackingWithIdentityResolution()
                   join r in db.SysRoles.AsNoTrackingWithIdentityResolution() on ur.RoleId equals r.Id
                   select new
                   {
                       ur.UserId,
                       r.Name,
                   };
        var query = from user in db.SysUsers.AsNoTrackingWithIdentityResolution()
                    select new BMUserTableItem
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

namespace BD.Common8.AspNetCore.Repositories;

/// <summary>
/// <see cref="IBMRoleRepository"/> 的实现类
/// </summary>
/// <typeparam name="TDbContext"></typeparam>
/// <remarks>
/// Initializes a new instance of the <see cref="BMRoleRepository{TDbContext}"/> class.
/// </remarks>
/// <param name="dbContext"></param>
/// <param name="requestAbortedProvider"></param>
sealed class BMRoleRepository<TDbContext>(TDbContext dbContext, IRequestAbortedProvider requestAbortedProvider) : Repository<TDbContext, BMRole, Guid>(dbContext, requestAbortedProvider), IBMRoleRepository where TDbContext : DbContext, IBMDbContext
{
    public async Task<PagedModel<BMRoleModel>> QueryAsync(
            string? name,
            int current = IPagedModel.DefaultCurrent,
            int pageSize = IPagedModel.DefaultPageSize)
    {
        var query = Entity.AsNoTrackingWithIdentityResolution();
        if (!string.IsNullOrEmpty(name))
            query = query.Where(x => x.Name.Contains(name));
        var r = await query.OrderByDescending(x => x.CreationTime)
            .Select(BMRoleModel.Expression)
            .PagingAsync(current, pageSize, RequestAborted);
        return r;
    }

    public async Task<SelectItemModel<Guid>[]> GetSelectAsync(int takeCount = SelectItemModel.Count)
    {
        var query = Entity.AsNoTrackingWithIdentityResolution();
        var r = await query
            .Select(x => new SelectItemModel<Guid>
            {
                Id = x.Id,
                Title = x.Name,
            })
            .Take(takeCount).ToArrayAsync();
        return r;
    }

    public async Task<Guid[]> GetRoleMenus(Guid roleId, Guid? tenantId)
    {
        var query = db.Set<BMMenuButtonRole>()
                .AsNoTrackingWithIdentityResolution()
                .Where(x => x.RoleId == roleId);
        if (tenantId.HasValue)
            query = query.Where(x => x.TenantId == tenantId);
        var r = await query.Select(x => x.MenuId).Distinct().ToArrayAsync();
        return r;
    }
}

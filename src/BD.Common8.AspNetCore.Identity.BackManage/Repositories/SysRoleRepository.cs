namespace BD.Common8.AspNetCore.Repositories;

sealed class SysRoleRepository<TDbContext> : Repository<TDbContext, SysRole, Guid>, ISysRoleRepository where TDbContext : DbContext, IApplicationDbContext
{
    public SysRoleRepository(TDbContext dbContext, IRequestAbortedProvider requestAbortedProvider) : base(dbContext, requestAbortedProvider)
    {
    }

    public async Task<PagedModel<SysRoleModel>> QueryAsync(
            string? name,
            int current = IPagedModel.DefaultCurrent,
            int pageSize = IPagedModel.DefaultPageSize)
    {
        var query = Entity.AsNoTrackingWithIdentityResolution();
        if (!string.IsNullOrEmpty(name))
            query = query.Where(x => x.Name.Contains(name));
        var r = await query.OrderByDescending(x => x.CreationTime)
            .Select(SysRoleModel.Expression)
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
        var query = db.Set<SysMenuButtonRole>()
                .AsNoTrackingWithIdentityResolution()
                .Where(x => x.RoleId == roleId);
        if (tenantId.HasValue)
            query = query.Where(x => x.TenantId == tenantId);
        var r = await query.Select(x => x.MenuId).Distinct().ToArrayAsync();
        return r;
    }
}

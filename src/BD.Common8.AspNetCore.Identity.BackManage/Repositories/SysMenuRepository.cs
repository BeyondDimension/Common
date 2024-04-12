namespace BD.Common8.AspNetCore.Repositories;

using EditModel = BD.Common8.AspNetCore.Models.SysMenuEdit;

/// <summary>
/// <see cref="ISysMenuRepository"/> 的实现类
/// </summary>
/// <typeparam name="TDbContext"></typeparam>
/// <remarks>
/// Initializes a new instance of the <see cref="SysMenuRepository{TDbContext}"/> class.
/// </remarks>
/// <param name="mapper"></param>
/// <param name="dbContext"></param>
/// <param name="requestAbortedProvider"></param>
sealed class SysMenuRepository<TDbContext>(IMapper mapper, TDbContext dbContext, IRequestAbortedProvider requestAbortedProvider) : Repository<TDbContext, SysMenu, Guid>(dbContext, requestAbortedProvider), ISysMenuRepository where TDbContext : DbContext, IApplicationDbContext
{
    //public static readonly Expression<Func<SysMenu, SysMenuTreeItem>> ExpressionTree = x => new()
    //{
    //    Id = x.Id,
    //    ParentId = x.ParentId,
    //    Name = x.Name,
    //    Children = x.Children != null ? x.Children.Select(x => new SysMenuTreeItem
    //    {
    //        Id = x.Id,
    //        Name = x.Name,
    //        ParentId = x.ParentId
    //    }).ToArray() : null
    //};
    readonly IMapper mapper = mapper;
    static Comparison<SysMenu> menuComparisonByOrder = (a, b) => (int)(a.Order - b.Order);

    public async Task<SysMenuTreeItem[]> GetTreeAsync()
    {
        //var item = from menu in Entity.AsNoTrackingWithIdentityResolution()
        //           select new SysMenuTreeItem
        //           {
        //               Id = menu.Id,
        //               ParentId = menu.ParentId,
        //               Name = menu.Name,
        //               Children = Entity.AsNoTrackingWithIdentityResolution()
        //               .Where(x => x.ParentId == menus.Id).ToArray()
        //           };
        //var r = from menus in Entity.AsNoTrackingWithIdentityResolution()
        //        select new SysMenuTreeItem
        //        {
        //            Id = menus.Id,
        //            ParentId = menus.ParentId,
        //            Name = menus.Name,
        //            Children = item.Where(x => x.ParentId == menus.Id).ToArray()
        //        };
        //return await r.ToArrayAsync();
        //var query = Entity.AsNoTrackingWithIdentityResolution()
        //    .Include(x => x.Children)
        //    .Where(x => !x.ParentId.HasValue);
        //return await query
        //    .OrderBy(x => x.Order)
        //    .ProjectTo<SysMenuTreeItem>(mapper.ConfigurationProvider)
        //    .ToArrayAsync(RequestAborted);

        // 查询顶级菜单（带上它的下一级子菜单列表）
        var menus = await Entity.AsNoTrackingWithIdentityResolution()
            .Where(m => !m.ParentId.HasValue)
            .Select(m => new SysMenu
            {
                Id = m.Id,
                ParentId = m.ParentId,
                Name = m.Name,
                Order = m.Order,
                Children = m.Children!.Select(cm => new SysMenu
                {
                    Id = cm.Id,
                    ParentId = cm.ParentId,
                    Name = cm.Name,
                    Order = cm.Order,
                }).ToList(),
            })
            .ToListAsync(RequestAborted);

        // 按 Order 字段排序
        menus.Sort(menuComparisonByOrder);
        menus.ForEach(cm => cm.Children!.Sort(menuComparisonByOrder));

        return mapper.Map<SysMenuTreeItem[]>(menus);
    }

    public async Task<SysMenuModel2?> InfoAsync(Guid id)
    {
        var r = await Entity.AsNoTrackingWithIdentityResolution()
            .Select(SysMenuModel2.Expression)
            .FirstOrDefaultAsync(x => x.Id == id, RequestAborted);
        return r;
    }

    public async Task<(int rowCount, DbRowExecResult result)> InsertOrUpdateAsync(EditModel model, Guid userId, Guid tenantId)
    {
        var r = await InsertOrUpdateAsync(model, onAdd: (entity) =>
        {
            entity.CreateUserId = userId;
            entity.TenantId = tenantId;
        }, onUpdate: (entity) =>
        {
            entity.OperatorUserId = userId;
        });
        return r;
    }

    #region 菜单权限

    public async Task<bool> EditMenuButtonsAsync(Guid menuId, Guid[] buttons, Guid tenantId)
    {
        var menu = await Entity.AsNoTrackingWithIdentityResolution()
            .FirstOrDefaultAsync(x => x.Id == menuId && x.TenantId == tenantId, RequestAborted);
        if (menu == null) return false;
        // 删除此角色全部菜单按钮重新添加
        await (from mbr in db.MenuButtons
               where
               mbr.MenuId == menuId
#if !ALL_TENANT
               && mbr.TenantId == tenantId
#endif
               select mbr)
        .AsQueryable()
        .DeleteFromQueryAsync();
        await db.MenuButtons.AddRangeAsync(buttons.Select(x => new SysMenuButton
        {
            MenuId = menuId,
            ButtonId = x,
            TenantId = tenantId,
        }));
        await db.SaveChangesAsync();
        return true;
    }

    public async Task<SysMenuModel[]?> GetUserMenuAsync(Guid userId, Guid tenantId)
    {
        //按钮 字典
        var sysButtonDis = await db.Buttons
            .Where(x => x.TenantId == tenantId)
            .Select(x => new SysButtonModel
            {
                Id = x.Id,
                Name = x.Name,
                Type = x.Type,
            }).ToDictionaryAsync(x => x.Id);
        var menuButtonRoles = from u in db.Users
                              join bur in db.UserRoles on u.Id equals bur.UserId
                              join br in db.Roles on bur.RoleId equals br.Id
                              join bmbr in db.MenuButtonRoles on br.Id equals bmbr.RoleId
                              where u.Id == userId && bur.TenantId == tenantId
                              select bmbr;
        var menuButtons = await menuButtonRoles.Distinct()
            .GroupBy(x => x.MenuId)
            .ToArrayAsync();
        var dicMenuButton = menuButtons.Select(x => new
        {
            x.Key,
            Buttons = x.Select(item => GetMenuSysButtonModel(sysButtonDis, item.ButtonId))
            .Where(x => x != null && !x.Disable)
            .Select(x => x!)
        }).ToDictionary(x => x.Key, x => x.Buttons);
        var menus = await db.Menus
            .AsNoTrackingWithIdentityResolution()
            .Include(x => x.Children!)
            .ThenInclude(x => x.Children)
            .Where(x => !x.ParentId.HasValue)
            .Where(x => x.TenantId == tenantId)
            .OrderBy(x => x.Order)
            .ToArrayAsync(RequestAborted);

        return menus.Select(x => MapToTreeModel(dicMenuButton, x))
            .Where(x => x != null && x.Buttons != null)
            .OrderBy(x => x.Order)
            .Select(x => x!).ToArray();
    }

    public static SysMenuModel? MapToTreeModel(Dictionary<Guid, IEnumerable<SysButtonModel>>? dic, SysMenu item)
    {
        if (dic == null || !dic.TryGetValue(item.Id, out var sysButton))
            return null;
        if (sysButton == null)
            return null;
        var destination = new SysMenuModel()
        {
            Id = item.Id,
            Key = item.Key,
            Name = item.Name,
            Url = item.Url,
            IconUrl = item.IconUrl,
            Order = item.Order,
            Buttons = sysButton?.ToArray(),
            Children = item.Children?
            .Select(citem => MapToTreeModel(dic, citem))
            .Where(citem => citem != null && citem?.Buttons != null)
            .OrderBy(x => x.Order)
            .Select(x => x!)
            .ToArray()
        };
        return destination;
    }

    public SysButtonModel? GetMenuSysButtonModel(Dictionary<Guid, SysButtonModel> dic, Guid buttonId)
    {
        if (dic.TryGetValue(buttonId, out var sysButton))
            return sysButton;
        return null;
    }

    //public SysButtonModel? GetMenuButtonModel(Dictionary<Guid, SysButtonModel> dic, Guid buttonId)
    //{
    //    if (dic.TryGetValue(buttonId, out var sysButton))
    //        return sysButton;
    //    return null;
    //}

    //public async Task<SysMenuModel[]?> GetUserMenuAsync(Guid userId, Guid tenantId)
    //{
    //    var menuButtonRoles = from u in db.Users
    //                          join bur in db.UserRoles on u.Id equals bur.UserId
    //                          join br in db.Roles on bur.RoleId equals br.Id
    //                          join bmbr in db.MenuButtonRoles on br.Id equals bmbr.RoleId
    //                          where u.Id == userId &&
    //                          bur.TenantId == tenantId &&
    //                          !u.SoftDeleted &&
    //                          !br.SoftDeleted
    //                          select bmbr;
    //    var sysButton = await db.Buttons
    //        .Where(x => x.TenantId == tenantId)
    //        .Select(x => new SysButtonModel
    //        {
    //            Id = x.Id,
    //            Name = x.Name,
    //            Type = x.Type,
    //        }).ToDictionaryAsync(x => x.Id);

    //    var dicMenuButton = menuButtonRoles.GroupBy(x => x.MenuId)
    //        .Distinct()
    //        .Select(x => new
    //        {
    //            x.Key,
    //            Buttons = x.Select(item => GetMenuSysButtonModel(sysButton, x.Key))
    //        .Where(x => x != null)
    //        }).ToDictionary(x => x.Key, x => x.Buttons);

    //public async Task<SysMenuModel[]?> GetUserMenuAsync(Guid userId, Guid tenantId)
    //{
    //    var menuButtonRoles = from u in db.Users
    //                          join bur in db.UserRoles on u.Id equals bur.UserId
    //                          join br in db.Roles on bur.RoleId equals br.Id
    //                          join bmbr in db.MenuButtonRoles on br.Id equals bmbr.RoleId
    //                          where u.Id == userId &&
    //                          !u.SoftDeleted &&
    //                          !br.SoftDeleted
    //                          select bmbr;

    //    var menus = from x in db.Menus
    //                join dd in menuButtonRoles on x.Id equals dd.MenuId
    //                where x.ParentId == null &&
    //                x.Buttons!.Any() &&
    //                x.TenantId == tenantId &&
    //                x.SoftDeleted == false
    //                select new SysMenuModel
    //                {
    //                    Id = x.Id,
    //                    Key = x.Key,
    //                    Name = x.Name,
    //                    Url = x.Url,
    //                    IconUrl = x.IconUrl,
    //                    Order = x.Order,
    //                    Buttons = x.Buttons == null ? null : x.Buttons!.Where(s => !s.SoftDeleted && menuButtonRoles.Any(y => y.MenuId == x.Id && y.ButtonId == s.Id)).Select(static y => new SysButtonModel
    //                    {
    //                        Id = y.Id,
    //                        Name = y.Name,
    //                        Type = y.Type,
    //                    }).ToArray(),
    //                    Children = x.Children == null ? null : x.Children.OrderBy(x => x.Order).Where(s => menuButtonRoles.Any(y => y.MenuId == s.Id)).Select(x => new SysMenuModel()
    //                    {
    //                        Id = x.Id,
    //                        Key = x.Key,
    //                        Name = x.Name,
    //                        Url = x.Url,
    //                        IconUrl = x.IconUrl,
    //                        Order = x.Order,
    //                        Buttons = x.Buttons == null ? null : x.Buttons!.Where(s => !s.SoftDeleted && menuButtonRoles.Any(y => y.MenuId == x.Id && y.ButtonId == s.Id)).Select(y => new SysButtonModel
    //                        {
    //                            Id = y.Id,
    //                            Name = y.Name,
    //                            Type = y.Type,
    //                        }).ToArray(),
    //                    }).ToArray(),
    //                };
    //    var query = menus.OrderBy(x => x.Order).AsNoTrackingWithIdentityResolution();
    //    var menus_d = await query.ToArrayAsync(RequestAborted);
    //    return menus_d;
    //}

    public async Task<SysMenuModel2[]> GetRoleTreeAsync()
    {
        var query = Entity.AsNoTrackingWithIdentityResolution()
            .Include(x => x.Children)
            .Where(x => !x.SoftDeleted);
        var r = await query.Distinct()
            .Select(SysMenuModel2.Expression)
            .OrderBy(x => x.Order)
            .ToArrayAsync();
        return r;
    }

    public async Task<SysButtonModel[]> GetButtonsAsync()
    {
        var query = db.Set<SysButton>()
            .AsNoTrackingWithIdentityResolution()
            .Where(x => !x.SoftDeleted);
        var r = await query
            .Select(SysButtonModel.Expression)
            .ToArrayAsync();
        return r;
    }

    public async Task<Guid[]> GetMenuButtonsAsync(Guid menuId, Guid tenantId)
    {
        var query = db.MenuButtons.AsNoTrackingWithIdentityResolution()
            .Where(x => x.MenuId == menuId);
#if !ALL_TENANT
        query = query.Where(x => x.TenantId == tenantId);
#endif
        return await query.Select(x => x.ButtonId)
            .ToArrayAsync(RequestAborted);
    }

    public async Task<SysButtonModel[]> GetRoleMenuButtonsAsync(
        Guid roleId,
        Guid menuId,
        Guid tenantId)
    {
        var query = from button in db.Buttons.AsNoTrackingWithIdentityResolution()
                    join menuRol in db.MenuButtonRoles.AsNoTrackingWithIdentityResolution() on button.Id equals menuRol.ButtonId
                    where !button.SoftDeleted &&
                    menuRol.MenuId == menuId &&
                    menuRol.RoleId == roleId
                    select button;
#if !ALL_TENANT
        query = query.Where(x => x.TenantId == tenantId);
#endif
        var r = await query
            .Select(SysButtonModel.Expression).ToArrayAsync();
        return r;
    }

    internal static string GetControllerName(string key, SysButtonType type)
    {
        return key + type;
    }

    public async Task<bool> AddMenuButtonsAsync(
         Guid userId,
         Guid roleId,
         Guid menuId,
         Guid tenantId,
         SysButtonModel[] buttons)
    {
        var menu = await Entity.AsNoTrackingWithIdentityResolution()
            .FirstOrDefaultAsync(x => x.Id == menuId && x.TenantId == tenantId, RequestAborted);
        if (menu == null) return false;
        using var transaction = db.Database.BeginTransaction();
        var isMenuAny = await db.MenuButtonRoles.AsNoTrackingWithIdentityResolution().Where(x =>
                x.MenuId == menuId &&
                x.RoleId == roleId
#if !ALL_TENANT
                && x.TenantId == tenantId
#endif
                ).AnyAsync();
        if (!isMenuAny)
        {
            await db.MenuButtonRoles.AddRangeAsync(buttons.Select(x => new SysMenuButtonRole
            {
                ButtonId = x.Id,
                MenuId = menu.Id,
                RoleId = roleId,
                TenantId = tenantId,
                ControllerName = GetControllerName(menu.Key, x.Type),
            }));
            await db.SaveChangesAsync();
        }
        // 当前菜单有父级 需要判断是否已添加
        if (menu.ParentId.HasValue)
        {
            var isParent = await db.MenuButtonRoles
                .AsNoTrackingWithIdentityResolution()
                .Where(x => x.RoleId == roleId &&
                            x.MenuId == menu.ParentId
#if !ALL_TENANT
                            && x.TenantId == tenantId
#endif
                ).AnyAsync();
            if (!isParent)
            {
                var parentMenu = await db.Menus
                    .AsNoTrackingWithIdentityResolution()
                    .FirstOrDefaultAsync(x => x.Id == menu.ParentId.Value && !x.SoftDeleted);
                if (parentMenu == null) return false;
                await db.MenuButtonRoles.AddRangeAsync(buttons.Select(x => new SysMenuButtonRole
                {
                    ButtonId = x.Id,
                    MenuId = menu.ParentId.Value,
                    RoleId = roleId,
                    TenantId = tenantId,
                    ControllerName = GetControllerName(parentMenu.Key, x.Type),
                }));
                await db.SaveChangesAsync();
            }
        }
        else
        {
            // 判断是否父菜单下有其他子菜单
            var isParentMenu = await (from menuItem in Entity.AsNoTrackingWithIdentityResolution()
                                      where !menuItem.SoftDeleted &&
                                      menuItem.ParentId == menuId
#if !ALL_TENANT
                                      && menuItem.TenantId == tenantId
#endif
                                      select menuItem).AnyAsync();
            // 循环添加子菜单
            if (isParentMenu)
            {
                var query = await (from menuItem in Entity.AsNoTrackingWithIdentityResolution()
                                   where
                                   !menuItem.SoftDeleted &&
                                   menuItem.ParentId == menuId
#if !ALL_TENANT
                                   && menuItem.TenantId == tenantId
#endif
                                   select menuItem).Distinct().ToArrayAsync();
                List<SysMenuButtonRole> roles = new();
                foreach (var item in query)
                {
                    var isAny = await db.MenuButtonRoles.Where(x =>
                    x.MenuId == item.Id &&
                    x.RoleId == roleId
#if !ALL_TENANT
                    && x.TenantId == tenantId
#endif
                    ).AnyAsync();
                    // 查询不存在则添加
                    if (!isAny)
                        roles.AddRange(buttons.Select(x => new SysMenuButtonRole
                        {
                            ButtonId = x.Id,
                            MenuId = item.Id,
                            ControllerName = GetControllerName(item.Key, x.Type),
                            TenantId = tenantId,
                            RoleId = roleId,
                        }));
                }
                await db.MenuButtonRoles.AddRangeAsync(roles);
                await db.SaveChangesAsync();
            }
        }
        await transaction.CommitAsync();
        return true;
    }

    public async Task<bool> EditMenuButtonsAsync(
         string name,
         Guid userId,
         Guid roleId,
         Guid menuId,
         Guid tenantId,
         SysButtonModel[] buttons)
    {
        var menu = await Entity.AsNoTrackingWithIdentityResolution()
               .FirstOrDefaultAsync(x => x.Id == menuId && x.TenantId == tenantId, RequestAborted);
        if (menu == null) return false;

        // 更新菜单名字
        await db.Menus
            .AsNoTrackingWithIdentityResolution()
            .Where(x => x.Id == menuId &&
            !x.SoftDeleted
#if !ALL_TENANT
                   && x.TenantId == tenantId
#endif
            )
            .UpdateAsync(x => new SysMenu
            {
                Name = name,
            });

        // 删除此角色全部菜单按钮重新添加
        await (from mbr in db.MenuButtonRoles
               where mbr.RoleId == roleId &&
               mbr.MenuId == menuId
#if !ALL_TENANT
               && mbr.TenantId == tenantId
#endif
               select mbr)
        .AsQueryable()
        .DeleteFromQueryAsync();
        await db.MenuButtonRoles.AddRangeAsync(buttons.Select(x => new SysMenuButtonRole
        {
            ButtonId = x.Id,
            MenuId = menuId,
            RoleId = roleId,
            TenantId = tenantId,
            ControllerName = GetControllerName(menu.Key, x.Type),
        }));
        await db.SaveChangesAsync();
        return true;
    }

    public async Task<bool> DeleteMenuButtonsAsync(
        Guid userId,
        Guid roleId,
        Guid menuId,
        Guid tenantId)
    {
        var menu = await Entity.AsNoTrackingWithIdentityResolution()
              .FirstOrDefaultAsync(x => x.Id == menuId && x.TenantId == tenantId, RequestAborted);
        if (menu == null) return false;
        // 删除此角色全部菜单按钮
        await db.MenuButtonRoles.AsNoTrackingWithIdentityResolution()
            .Where(x => x.MenuId == menuId &&
            x.RoleId == roleId
#if !ALL_TENANT
            && x.TenantId == tenantId
#endif
            )
           .DeleteFromQueryAsync();
        // 如果是子菜单
        if (menu.ParentId.HasValue)
        {
            // 判断是否父菜单下有其他子菜单
            var isParentMenu = await (from r in db.Roles
                                      join bmbr in db.MenuButtonRoles on r.Id equals bmbr.RoleId
                                      join menuDB in db.Menus on bmbr.MenuId equals menuDB.Id
                                      where menuDB.ParentId == menu.ParentId &&
                                      r.Id == roleId
#if !ALL_TENANT
                                     && menuDB.TenantId == tenantId
#endif
                                      select bmbr).AnyAsync();
            // 不存在子菜单删除父级菜单权限
            if (!isParentMenu)
            {
                await db.MenuButtonRoles
                      .AsNoTrackingWithIdentityResolution()
                      .Where(x =>
                      x.RoleId == roleId &&
                      x.MenuId == menu.ParentId
#if !ALL_TENANT
                      && x.TenantId == tenantId
#endif
                      )
                      .DeleteFromQueryAsync();
            }
        }
        else
        {
            // 是否有子菜单
            var isParentMenu = await (from r in db.Roles
                                      join bmbr in db.MenuButtonRoles on r.Id equals bmbr.RoleId
                                      join menuDB in db.Menus on bmbr.MenuId equals menuDB.Id
                                      where menuDB.ParentId == menu.Id
#if !ALL_TENANT
                                      && menuDB.TenantId == tenantId &&
#endif
                                      r.Id == roleId
                                      select bmbr).AnyAsync();
            // 删除全部子菜单权限
            if (isParentMenu)
            {
                await (from r in db.Roles
                       join bmbr in db.MenuButtonRoles on r.Id equals bmbr.RoleId
                       join menuDB in db.Menus on bmbr.MenuId equals menuDB.Id
                       where menuDB.ParentId == menu.Id
#if !ALL_TENANT
                       && menuDB.TenantId == tenantId &&
#endif
                       r.Id == roleId
                       select bmbr)
                      .DeleteFromQueryAsync();
            }
        }
        return true;
    }

    public async Task<bool> DeleteMenuAsync(
        Guid menuId,
        Guid tenantId)
    {
        try
        {
            var menu = await Entity.AsNoTrackingWithIdentityResolution()
                     .Include(r => r.Children)
                     .FirstOrDefaultAsync(x => x.Id == menuId && x.TenantId == tenantId, RequestAborted);
            if (menu == null) return false;
            if (menu.Children?.Count > 0)
            {
                foreach (var item in menu.Children)
                {
                    await DeleteMenuAsync(item.Id, tenantId);
                }
            }
            else
            {
                await db.MenuButtons
                        .AsNoTrackingWithIdentityResolution()
                        .Where(x =>
                        x.MenuId == menuId
#if !ALL_TENANT
                        && x.TenantId == tenantId
#endif
                        )
                        .DeleteFromQueryAsync();
                await db.MenuButtonRoles
                        .AsNoTrackingWithIdentityResolution()
                        .Where(x =>
                        x.MenuId == menuId
#if !ALL_TENANT
                        && x.TenantId == tenantId
#endif
                        )
                        .DeleteFromQueryAsync();
                await db.Menus
                        .AsNoTrackingWithIdentityResolution()
                        .Where(x =>
                        x.Id == menuId
#if !ALL_TENANT
                        && x.TenantId == tenantId
#endif
                        )
                        .DeleteFromQueryAsync();
            }
            return true;
        }
        catch
        {
            return false;
        }
    }

    public async Task<Guid[]> GetRoleMenus(Guid userId, Guid? tenantId)
    {
        var role = await db.UserRoles.AsNoTrackingWithIdentityResolution()
            .Where(x => x.UserId == userId).Select(x => x.RoleId).ToArrayAsync();
        if (role.Length > 0)
        {
            var query = db.Set<SysMenuButtonRole>()
                    .AsNoTrackingWithIdentityResolution()
                    .Where(x => role.Contains(x.RoleId));
            if (tenantId.HasValue)
                query = query.Where(x => x.TenantId == tenantId);
            var r = await query.Select(x => x.MenuId).Distinct().ToArrayAsync();
            return r;
        }
        else
        {
            return [];
        }
    }

    public async Task<Dictionary<Guid, SysButton>> GetButtons(Guid? tenantId)
    {
        var buttons = await db.Buttons
            .AsNoTrackingWithIdentityResolution()
            .Where(x => x.TenantId == tenantId).ToDictionaryAsync(x => x.Id);
        return buttons;
    }

    //public async Task<Dictionary<Guid, SysButtonModel[]>> GetRoleButtons(Guid userId, Guid? tenantId)
    //{
    //    var buttons = await GetButtons(tenantId);
    //    var role = await db.UserRoles.AsNoTrackingWithIdentityResolution()
    //       .Where(x => x.UserId == userId).Select(x => x.RoleId).ToArrayAsync();
    //    if (role.Length > 0)
    //    {
    //        var query = db.Set<SysMenuButtonRole>()
    //                .AsNoTrackingWithIdentityResolution()
    //                .Where(x => role.Contains(x.RoleId));
    //        if (tenantId.HasValue)
    //            query = query.Where(x => x.TenantId == tenantId);
    //        var r = await query
    //            .GroupBy(x => x.MenuId)
    //            .Distinct()
    //            .ToDictionaryAsync(x => x.Key);
    //        return r;
    //    }
    //    else
    //    {

    //        return new Dictionary<Guid, SysButtonModel[]> { };
    //    }
    //}

    public async Task<SysMenuModel[]?> GetUserMenu2Async(Guid userId, Guid tenantId)
    {
        //var menuIds = await GetRoleMenus(userId, tenantId);
        //if (menuIds.Length > 0)
        //{
        //    var menus = from x in db.Menus
        //                where x.ParentId == null &&
        //                menuIds.Contains(x.Id) &&
        //                x.TenantId == tenantId &&
        //                x.SoftDeleted == false
        //                select new SysMenuModel
        //                {
        //                    Id = x.Id,
        //                    Key = x.Key,
        //                    Name = x.Name,
        //                    Url = x.Url,
        //                    IconUrl = x.IconUrl,
        //                    Order = x.Order,
        //                    //Buttons =
        //                    Children = x.Children == null ? null :
        //                    x.Children.OrderBy(x => x.Order)
        //                    .Where(s => menuIds.Contains(x.Id))
        //                    .Select(x => new SysMenuModel()
        //                    {
        //                        Id = x.Id,
        //                        Key = x.Key,
        //                        Name = x.Name,
        //                        Url = x.Url,
        //                        IconUrl = x.IconUrl,
        //                        Order = x.Order,
        //                        Buttons = x.Buttons == null ? null :
        //                        x.Buttons!.Where(s => !s.SoftDeleted
        //                        && menuButtonRoles.Any(y => y.MenuId == x.Id && y.ButtonId == s.Id))
        //                        .Select(y => new SysButtonModel
        //                        {
        //                            Type = y.Type,
        //                        }).ToArray(),
        //                    }).ToArray(),
        //                };
        //    var query = menus.OrderBy(x => x.Order).AsNoTrackingWithIdentityResolution();
        //    var menus_d = await query.ToArrayAsync(RequestAborted);
        //    return menus_d;
        //}
        //else
        //{
        return null;
        //}
    }
    #endregion
}

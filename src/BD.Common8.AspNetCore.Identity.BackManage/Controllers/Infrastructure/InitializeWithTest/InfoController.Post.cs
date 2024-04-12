//namespace BD.Common8.AspNetCore.Controllers;

//partial class InfoController
//{
//    /// <summary>
//    /// 创建一个默认系统管理员账号，且在 DEBUG 下将返回 JWT，用于测试
//    /// </summary>
//    /// <returns></returns>
//    [ProducesResponseType(StatusCodes.Status404NotFound)]
//    [HttpPost]
//    public async Task<ActionResult<ApiResponse>> Post([FromBody] InitSystemRequest model)
//    {
//        var hashPassword = Hashs.String.SHA384(DateTimeOffset.Now.ToString("yyyyMMdd") + settings.GetInitSystemSecuritySalt(), false);
//        if (!string.Equals(hashPassword, model.InitPassword))
//            return Unauthorized();

//        if (!ShortGuid.TryParse(model.TenantId, out Guid tenantId))
//            return Fail("租户 Id 格式不正确");
//        if (string.IsNullOrWhiteSpace(model.TenantName))
//            return Fail("租户名称不能为空或空白字符串");
//        var userName = model.UserName;
//        if (string.IsNullOrWhiteSpace(userName) || userName == "string")
//            userName = settings.AdminUserName;

//        using var transaction = db.Database.BeginTransaction();

//        try
//        {
//            #region 添加管理员用户与预设角色

//            const string adminRoleName = nameof(RoleEnum.Administrator);
//            List<string> addRoles = Enum.GetValues<RoleEnum>().Where(x => x != RoleEnum.Administrator).Select(x => x.ToString()).ToList();
//            var user = await userManager.FindByNameAsync(userName); // 查找默认初始管理员用户
//            if (user == null)
//            {
//                user = new() // 创建默认初始管理员用户
//                {
//                    UserName = userName,
//                    TenantId = tenantId,
//                };
//                var pwd = model.Password;
//                if (string.IsNullOrWhiteSpace(pwd) || pwd == "string")
//                    pwd = settings.AdminPassword;
//                var createResult = await userManager.CreateAsync(user, pwd);
//                if (!createResult.Succeeded)
//                    return Fail(createResult);
//                addRoles.Add(adminRoleName); // 添加管理员权限
//            }
//            else
//            {
//                var isUpdate = false; // 检查其他字段值是否需要更新
//                if (user.TenantId != tenantId)
//                {
//                    user.TenantId = tenantId;
//                    isUpdate = true;
//                }
//                if (isUpdate)
//                    await userManager.UpdateAsync(user);
//                var roles = await userManager.GetRolesAsync(user);
//                if (!(roles != null && roles.Any(x => x == adminRoleName)))
//                {
//                    // 更新角色
//                    addRoles.Add(adminRoleName);
//                }
//            }

//            if (addRoles.Count != 0)
//            {
//                IdentityResult identityResult;
//                foreach (var roleName_ in addRoles)
//                {
//                    var role = await db.Roles.FirstOrDefaultAsync(x => x.Name == roleName_ && x.TenantId == tenantId);
//                    if (role == null)
//                    {
//                        role = new()
//                        {
//                            Name = roleName_,
//                            NormalizedName = roleName_.ToUpper(),
//                            TenantId = tenantId,
//                            CreateUserId = user.Id,
//                        };
//                        db.Roles.Add(role);
//                        await db.SaveChangesAsync();
//                    }

//                    if (roleName_ == adminRoleName)
//                    {
//                        identityResult = await userManager.AddToRoleAsync(user, roleName_);
//                        if (!identityResult.Succeeded)
//                            return Fail(identityResult);
//                    }
//                }
//            }

//            #endregion 添加管理员用户与预设角色

//            #region 添加租户

//            var tenant = await db.Tenants.FindAsync(tenantId);
//            if (tenant == null)
//            {
//                await db.Tenants.AddAsync(new()
//                {
//                    Id = tenantId,
//                    CreateUserId = user.Id,
//                    Name = model.TenantName,
//                });
//                await db.SaveChangesAsync();
//            }

//            #endregion 添加租户

//            var isRootTenant = tenantId == TenantConstants.RootTenantIdG;

//            //#region 添加预设菜单

//            //var menus = await db.Menus.Where(x => x.TenantId == tenantId).ToListAsync();
//            //if (!menus.Any()) menus = await AddMenusAsync();

//            //async Task<List<MenuDataItem>> AddMenusAsync()
//            //{
//            //    var menus = new List<MenuDataItem>(GetMenuDataItems(isRootTenant));
//            //    SetUserIdAndTenantId(menus, user.Id, tenantId);

//            //    await db.Menus.AddRangeAsync(menus);
//            //    await db.SaveChangesAsync();
//            //    return menus;
//            //}

//            //#endregion

//            //#region 添加预设按钮

//            //var btns = await db.Buttons.Where(x => x.TenantId == tenantId).ToListAsync();
//            //if (!btns.Any()) btns = await AddButtonsAsync();

//            //async Task<List<SysButton>> AddButtonsAsync()
//            //{
//            //    var btnDict = new Dictionary<SysButtonType, string>
//            //    {
//            //        { SysButtonType.Edit, "编辑" },
//            //        { SysButtonType.Delete, "删除" },
//            //        { SysButtonType.Detail, "查看详情" },
//            //        { SysButtonType.Add, "新增" },
//            //        { SysButtonType.Query, "查询" },
//            //    };
//            //    var btns = btnDict.Select(x => new SysButton
//            //    {
//            //        CreateUserId = user.Id,
//            //        TenantId = user.TenantId,
//            //        Name = x.Value,
//            //        Type = x.Key,
//            //    }).ToList();

//            //    await db.Buttons.AddRangeAsync(btns);
//            //    await db.SaveChangesAsync();
//            //    return btns;
//            //}

//            //#endregion

//            //#region 添加预设菜单按钮关系

//            //var menuButtons = await db.MenuButtons.Where(x => x.TenantId == tenantId).ToListAsync();
//            //if (!menuButtons.Any()) menuButtons = await AddMenuButtonsAsync();

//            //async Task<List<SysMenuButton>> AddMenuButtonsAsync()
//            //{
//            //    var menuButtons = new List<SysMenuButton>();

//            //    foreach (var menu in menus.Concat(menus.Where(x => x.Children != null).SelectMany(x => x.Children)))
//            //    {
//            //        if (menu.Key == nameof(Titles.dashboard) ||
//            //            menu.Children?.Any() == true)
//            //        {
//            //            menuButtons.Add(new SysMenuButton
//            //            {
//            //                TenantId = tenantId,
//            //                MenuId = menu.Id,
//            //                ButtonId = btns.First(x => x.Type == SysButtonType.Query).Id,
//            //            });
//            //            continue;
//            //        }

//            //        foreach (var btn in btns)
//            //        {
//            //            menuButtons.Add(new SysMenuButton
//            //            {
//            //                TenantId = tenantId,
//            //                MenuId = menu.Id,
//            //                ButtonId = btn.Id,
//            //            });
//            //        }
//            //    }

//            //    await db.MenuButtons.AddRangeAsync(menuButtons);
//            //    await db.SaveChangesAsync();
//            //    return menuButtons;
//            //}

//            //#endregion

//            //#region 添加预设菜单按钮角色关系

//            //var menuButtonRoles = await db.MenuButtonRoles.Where(x => x.TenantId == tenantId).ToListAsync();
//            //if (!menuButtonRoles.Any()) await AddMenuButtonRolesAsync();

//            //async Task AddMenuButtonRolesAsync()
//            //{
//            //    if (addRoles.Any())
//            //    {
//            //        // 添加 客服 权限
//            //        var customerServiceRole = await db.Roles.FirstOrDefaultAsync(x => x.Name == nameof(RoleEnum.CustomerService) && x.TenantId == tenantId);
//            //        if (customerServiceRole != null)
//            //        {
//            //            var customerServiceRoleMenus = await GetCustomerServiceRoleItems();

//            //            foreach (var menu in customerServiceRoleMenus)
//            //                foreach (var btn in menu.Buttons!)
//            //                    await db.MenuButtonRoles.AddAsync(new SysMenuButtonRole
//            //                    {
//            //                        TenantId = tenantId,
//            //                        RoleId = customerServiceRole.Id,
//            //                        MenuId = menu.Id,
//            //                        ButtonId = btn.Id,
//            //                        ControllerName = menu.Key + btn.Type
//            //                    });
//            //        }

//            //        // 添加 商品管理员 权限
//            //        var commodityRole = await db.Roles.FirstOrDefaultAsync(x => x.Name == nameof(RoleEnum.Commodity) && x.TenantId == tenantId);
//            //        if (commodityRole != null)
//            //        {
//            //            var commodityRoleMenus = await GetCommodityRoleItems();

//            //            foreach (var menu in commodityRoleMenus)
//            //                foreach (var btn in menu.Buttons!)
//            //                    await db.MenuButtonRoles.AddAsync(new SysMenuButtonRole
//            //                    {
//            //                        TenantId = tenantId,
//            //                        RoleId = commodityRole.Id,
//            //                        MenuId = menu.Id,
//            //                        ButtonId = btn.Id,
//            //                        ControllerName = menu.Key + btn.Type,
//            //                    });
//            //        }

//            //        // 添加 系统管理员 权限
//            //        var adminRole = await db.Roles.FirstOrDefaultAsync(x => x.Name == nameof(RoleEnum.Administrator) && x.TenantId == tenantId);
//            //        if (adminRole != null)
//            //        {
//            //            var adminRoleMenus = await db.Menus.Include(x => x.Buttons).Where(x => x.TenantId == tenantId && x.Key != nameof(Titles.requiredproductkeys))
//            //                .AsNoTrackingWithIdentityResolution().ToListAsync();

//            //            foreach (var menu in adminRoleMenus)
//            //                foreach (var btn in menu.Buttons!)
//            //                    await db.MenuButtonRoles.AddAsync(new SysMenuButtonRole
//            //                    {
//            //                        TenantId = tenantId,
//            //                        RoleId = adminRole.Id,
//            //                        MenuId = menu.Id,
//            //                        ButtonId = btn.Id,
//            //                        ControllerName = menu.Key + btn.Type,
//            //                    });

//            //            if (isRootTenant)
//            //            {
//            //                await db.MenuButtonRoles.AddAsync(new SysMenuButtonRole
//            //                {
//            //                    TenantId = tenantId,
//            //                    RoleId = adminRole.Id,
//            //                    ControllerName = Controllers.ProxyManages,
//            //                });
//            //            }
//            //        }

//            //        await db.SaveChangesAsync();
//            //    }
//            //}

//            //async Task<IEnumerable<MenuDataItem>> GetCustomerServiceRoleItems()
//            //{
//            //    var menuKeys = new string[] {
//            //        nameof(MenuTitles.ProductManage),
//            //        nameof(Titles.requiredproductkeys),
//            //        nameof(MenuTitles.SteamPrepaidCard),
//            //        nameof(Titles.steamprepaidcardrecords),
//            //        nameof(MenuTitles.OrderManage),
//            //        nameof(Titles.steamrechargeorders),
//            //    };
//            //    var menus = await db.Menus.Include(x => x.Buttons).Where(x => x.TenantId == tenantId && menuKeys.Contains(x.Key))
//            //        .AsNoTrackingWithIdentityResolution().ToListAsync();

//            //    var productkeyMenu = menus.First(x => x.Key == nameof(Titles.requiredproductkeys));
//            //    if (productkeyMenu.Buttons.Any_Nullable())
//            //        productkeyMenu.Buttons = productkeyMenu.Buttons.Where(x => x.Type != SysButtonType.Add).ToList();

//            //    return menus;
//            //}

//            //async Task<IEnumerable<MenuDataItem>> GetCommodityRoleItems()
//            //{
//            //    var menuKeys = new string[] {
//            //        nameof(MenuTitles.ProductManage),
//            //        nameof(Titles.productkeys),
//            //        nameof(MenuTitles.SteamPrepaidCard),
//            //        nameof(Titles.steamprepaidcards),
//            //        nameof(Titles.steamprepaidcardrecords),
//            //        nameof(MenuTitles.OrderManage),
//            //        nameof(Titles.steamrechargeorders),
//            //        nameof(MenuTitles.ExtraFunction),
//            //        nameof(Titles.steamexchangerates),
//            //        //nameof(MenuTitles.ProxyManage),
//            //        //nameof(Titles.proxymanages),
//            //    };

//            //    return await db.Menus.Include(x => x.Buttons).Where(x => x.TenantId == tenantId && menuKeys.Contains(x.Key))
//            //        .AsNoTrackingWithIdentityResolution().ToListAsync();
//            //}

//            //#endregion

//            await transaction.CommitAsync();

//#if DEBUG
//            string[]? roles_ = [adminRoleName,];
//            JWTEntity? token = null;
//#if DEBUG
//            if (env.IsDevelopment())
//                token = await jwtValueProvider.GenerateTokenAsync(user.Id, roles_, aciton: null);
//#endif
//            return Ok(token);
//#else
//            return Ok();
//#endif
//        }
//        catch (Exception ex)
//        {
//            return Fail(ex.ToString());
//        }
//    }
//}
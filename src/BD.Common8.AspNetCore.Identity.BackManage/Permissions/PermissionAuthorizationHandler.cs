namespace BD.Common8.AspNetCore.Permissions;

/// <summary>
/// 用户权限验证
/// </summary>
/// <typeparam name="TDbContext"></typeparam>
/// <param name="db"></param>
/// <param name="userManager"></param>
public sealed class PermissionAuthorizationHandler<TDbContext>(TDbContext db, IUserManager userManager) : AuthorizationHandler<PermissionAuthorizationRequirement> where TDbContext : IBMDbContext
{
    /// <summary>
    /// 数据上下文对象
    /// </summary>
    readonly TDbContext db = db;

    /// <summary>
    /// 用户管理对象
    /// </summary>
    readonly IUserManager userManager = userManager;

    /// <summary>
    /// 用户权限验证
    /// </summary>
    protected override async Task HandleRequirementAsync(AuthorizationHandlerContext context, PermissionAuthorizationRequirement requirement)
    {
        if (userManager.TryGetUserId(context.User, out var userId))
        {
            var result = await (from role in db.SysUserRoles
                                join user in db.SysUsers on role.UserId equals user.Id
                                join buttonRole in db.MenuButtonRoles on role.RoleId equals buttonRole.RoleId
                                join tenant in db.Tenants on user.TenantId equals tenant.Id
                                where role.UserId == userId &&
                                    buttonRole.TenantId == user.TenantId &&
                                    buttonRole.ControllerName == requirement.ControllerName &&
                                    !tenant.SoftDeleted
                                select role).AnyAsync();
            if (result)
            {
                context.Succeed(requirement);
                return;
            }
        }
        context.Fail();
    }
}
namespace BD.Common8.AspNetCore.Permissions;

#pragma warning disable SA1600 // Elements should be documented

public sealed class PermissionAuthorizationHandler<TDbContext>(TDbContext db, IUserManager userManager) : AuthorizationHandler<PermissionAuthorizationRequirement> where TDbContext : IApplicationDbContext
{
    readonly TDbContext db = db;
    readonly IUserManager userManager = userManager;

    protected override async Task HandleRequirementAsync(AuthorizationHandlerContext context, PermissionAuthorizationRequirement requirement)
    {
        if (userManager.TryGetUserId(context.User, out var userId))
        {
            var result = await (from role in db.UserRoles
                                join user in db.Users on role.UserId equals user.Id
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
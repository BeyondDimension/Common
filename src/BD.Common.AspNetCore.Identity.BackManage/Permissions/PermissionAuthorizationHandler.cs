using BD.Common.Services;
using System.Data;

// ReSharper disable once CheckNamespace
namespace BD.Common;

public sealed class PermissionAuthorizationHandler<TDbContext> : AuthorizationHandler<PermissionAuthorizationRequirement> where TDbContext : IApplicationDbContext
{
    readonly TDbContext db;
    readonly IUserManager userManager;

    public PermissionAuthorizationHandler(TDbContext db, IUserManager userManager)
    {
        this.db = db;
        this.userManager = userManager;
    }

    protected override async Task HandleRequirementAsync(AuthorizationHandlerContext context, PermissionAuthorizationRequirement requirement)
    {
        if (userManager.TryGetUserId(context.User, out Guid userId))
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
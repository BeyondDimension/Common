using System.Data;

// ReSharper disable once CheckNamespace
namespace BD.Common;

public sealed class PermissionAuthorizationHandler<TDbContext> : AuthorizationHandler<PermissionAuthorizationRequirement> where TDbContext : IApplicationDbContext
{
    readonly TDbContext db;

    public PermissionAuthorizationHandler(TDbContext db)
    {
        this.db = db;
    }

    protected override async Task HandleRequirementAsync(AuthorizationHandlerContext context, PermissionAuthorizationRequirement requirement)
    {
        if (context.User != null)
        {
            var userIdClaim = context.User.FindFirst(_ => _.Type == ClaimTypes.NameIdentifier);
            if (userIdClaim != null && Guid.TryParse(userIdClaim.Value, out Guid userId))
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
        }
        context.Fail();
    }
}
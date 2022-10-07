// ReSharper disable once CheckNamespace
namespace BD.Common;

public static class IdentityExtensions
{
    public static bool TryGetUserId<TUser>(this UserManager<TUser> userManager, ClaimsPrincipal principal, out Guid userId) where TUser : class
    {
        var userIdString = userManager.GetUserId(principal);
        return Guid.TryParse(userIdString, out userId);
    }
}

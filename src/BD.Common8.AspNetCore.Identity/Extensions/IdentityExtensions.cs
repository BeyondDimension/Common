#pragma warning disable IDE0130 // 命名空间与文件夹结构不匹配
namespace Microsoft.AspNetCore.Identity;

public static partial class IdentityExtensions
{
    /// <summary>
    /// 尝试获取 UserId
    /// </summary>
    /// <typeparam name="TUser"></typeparam>
    /// <param name="userManager"></param>
    /// <param name="principal"></param>
    /// <param name="userId"></param>
    /// <returns></returns>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static bool TryGetUserId<TUser>(this UserManager<TUser> userManager, ClaimsPrincipal principal, out Guid userId) where TUser : class
    {
        var userIdString = userManager.GetUserId(principal);
        return Guid.TryParse(userIdString, out userId);
    }
}

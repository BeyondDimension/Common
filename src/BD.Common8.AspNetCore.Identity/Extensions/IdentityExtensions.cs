namespace BD.Common8.AspNetCore.Extensions;

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

    #region IdentityTableNames

    const string Users = "Users";
    const string Roles = "Roles";
    const string RoleClaims = "RoleClaims";
    const string UserClaims = "UserClaims";
    const string UserLogins = "UserLogins";
    const string UserRoles = "UserRoles";
    const string UserTokens = "UserTokens";

    public static void ReNameAspNetIdentity<TUser, TRole, TKey>(this ModelBuilder builder,
        string? tablePrefix = null)
        where TUser : IdentityUser<TKey>
        where TRole : IdentityRole<TKey>
        where TKey : IEquatable<TKey>
    {
        var hasTablePrefix = !string.IsNullOrEmpty(tablePrefix);
        string GetString(string str) => hasTablePrefix ? tablePrefix + str : str;
#pragma warning disable IDE0079 // 请删除不必要的忽略
#pragma warning disable IL2091 // Target generic argument does not satisfy 'DynamicallyAccessedMembersAttribute' in target method or type. The generic parameter of the source method or type does not have matching annotations.
        builder.Entity<TUser>().ToTable(GetString(Users));
        builder.Entity<TRole>().ToTable(GetString(Roles));
#pragma warning restore IL2091 // Target generic argument does not satisfy 'DynamicallyAccessedMembersAttribute' in target method or type. The generic parameter of the source method or type does not have matching annotations.
#pragma warning restore IDE0079 // 请删除不必要的忽略
        builder.Entity<IdentityRoleClaim<TKey>>().ToTable(GetString(RoleClaims));
        builder.Entity<IdentityUserClaim<TKey>>().ToTable(GetString(UserClaims));
        builder.Entity<IdentityUserLogin<TKey>>().ToTable(GetString(UserLogins));
        builder.Entity<IdentityUserRole<TKey>>().ToTable(GetString(UserRoles));
        builder.Entity<IdentityUserToken<TKey>>().ToTable(GetString(UserTokens));
    }

    #endregion
}

namespace BD.Common8.AspNetCore.Services;

#pragma warning disable SA1600 // Elements should be documented

public interface IUserManager
{
    internal IdentityOptions Options { get; }

    [Obsolete("use Guid userId", true)]
    Task<SysUser?> FindByIdAsync(string? userId)
    {
        if (userId == null) return Task.FromResult<SysUser?>(null);
        if (ShortGuid.TryParse(userId, out Guid useIdG))
            return FindByIdAsync(useIdG);
        return Task.FromResult<SysUser?>(null);
    }

    Task<SysUser?> FindByIdAsync(Guid userId);

    Task<IList<string>> GetRolesAsync(SysUser user);

    Task<SysUser?> FindByNameAsync(string? userName);

    Task<SysUser?> FindByNameAsync(string? userName, Guid tenantId);

    Task<IdentityResult> CreateAsync(SysUser user, string password);

    bool TryGetUserId(ClaimsPrincipal principal, out Guid userId);

    ValueTask<string> GetUserNameAsync(SysUser user);

    ValueTask<IdentityResult> SetUserNameAsync(SysUser user, string userName);

    Task<IdentityResult> RemoveFromRolesAsync(SysUser user, IEnumerable<string> roles);

    Task<IdentityResult> SetLockoutEndDateAsync(SysUser user, DateTimeOffset? lockoutEnd);

    Task<IdentityResult> UpdateAsync(SysUser user);

    ValueTask<bool> CheckPasswordAsync(SysUser user, string password);

    ValueTask<bool> IsLockedOutAsync(SysUser user);

    Task<IdentityResult> ChangePasswordAsync(SysUser user, string currentPassword, string newPassword);

    Task<SysUser?> GetUserAsync(ClaimsPrincipal principal);

    Task<IdentityResult> AddToRoleAsync(SysUser user, string role) => AddToRolesAsync(user, role);

    Task<IdentityResult> AddToRolesAsync(SysUser user, params string[] roles)
    {
        IEnumerable<string> roles_ = roles;
        return AddToRolesAsync(user, roles_);
    }

    Task<IdentityResult> AddToRolesAsync(SysUser user, IEnumerable<string> roles);
}

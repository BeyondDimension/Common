namespace BD.Common.Services;

public interface IUserManager
{
    Task<SysUser> FindByIdAsync(string userId);

    Task<IList<string>> GetRolesAsync(SysUser user);

    Task<SysUser> FindByNameAsync(string userName);

    Task<IdentityResult> CreateAsync(SysUser user, string password);

    Task<IdentityResult> AddToRoleAsync(SysUser user, string role);

    Task<IdentityResult> AddToRolesAsync(SysUser user, IEnumerable<string> roles);

    bool TryGetUserId(out Guid userId);

    bool TryGetUserId(ClaimsPrincipal principal, out Guid userId);

    ValueTask<string> GetUserNameAsync(SysUser user);

    Task<IdentityResult> SetUserNameAsync(SysUser user, string userName);

    Task<IdentityResult> RemoveFromRolesAsync(SysUser user, IEnumerable<string> roles);

    Task<IdentityResult> SetLockoutEndDateAsync(SysUser user, DateTimeOffset? lockoutEnd);

    Task<IdentityResult> UpdateAsync(SysUser user);

    Task<bool> CheckPasswordAsync(SysUser user, string password);

    Task<bool> IsLockedOutAsync(SysUser user);

    Task<IdentityResult> ChangePasswordAsync(SysUser user, string currentPassword, string newPassword);

    Task<SysUser> GetUserAsync(ClaimsPrincipal principal);
}

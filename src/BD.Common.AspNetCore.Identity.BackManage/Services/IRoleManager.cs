namespace BD.Common.Services;

public interface IRoleManager
{
    Task<SysRole> FindByNameAsync(string roleName);

    Task<IdentityResult> CreateAsync(SysRole role);

    Task<IdentityResult> AddToRoleAsync(SysUser user, string role);

    Task<IdentityResult> AddToRolesAsync(SysUser user, IEnumerable<string> roles);
}

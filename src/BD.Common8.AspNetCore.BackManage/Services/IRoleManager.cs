namespace BD.Common.Services;

public interface IRoleManager
{
    Task<SysRole?> FindByIdAsync(Guid roleId);

    Task<SysRole?> FindByNameAsync(string? roleName, Guid tenantId);

    Task<IdentityResult> CreateAsync(SysRole role);

    [return: NotNullIfNotNull("key")]
    internal string? NormalizeKey(string? key);

    Task<IdentityResult> UpdateAsync(SysRole role);

    Task<IdentityResult> DeleteAsync(SysRole role);
}

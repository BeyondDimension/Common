namespace BD.Common8.AspNetCore.Services;

#pragma warning disable SA1600 // Elements should be documented

public interface IRoleManager
{
    Task<SysRole?> FindByIdAsync(Guid roleId);

    Task<SysRole?> FindByNameAsync(string? roleName, Guid tenantId);

    Task<IdentityResult> CreateAsync(SysRole role);

    [return: NotNullIfNotNull(nameof(key))]
    internal string? NormalizeKey(string? key);

    Task<IdentityResult> UpdateAsync(SysRole role);

    Task<IdentityResult> DeleteAsync(SysRole role);
}

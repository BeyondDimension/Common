namespace BD.Common8.AspNetCore.Services;

/// <summary>
/// 提供角色管理接口定义
/// </summary>
public interface IRoleManager
{
    /// <summary>
    /// 根据角色 Id 查找角色
    /// </summary>
    /// <param name="roleId"></param>
    /// <returns></returns>
    Task<SysRole?> FindByIdAsync(Guid roleId);

    /// <summary>
    /// 根据角色名称和租户 Id 查找角色
    /// </summary>
    /// <param name="roleName"></param>
    /// <param name="tenantId"></param>
    /// <returns></returns>
    Task<SysRole?> FindByNameAsync(string? roleName, Guid tenantId);

    /// <summary>
    /// 创建角色
    /// </summary>
    Task<IdentityResult> CreateAsync(SysRole role);

    /// <summary>
    /// 获取规范化表示形式
    /// </summary>
    /// <param name="key">要规范化的值</param>
    [return: NotNullIfNotNull(nameof(key))]
    internal string? NormalizeKey(string? key);

    /// <summary>
    /// 更新角色
    /// </summary>
    Task<IdentityResult> UpdateAsync(SysRole role);

    /// <summary>
    /// 删除角色
    /// </summary>
    Task<IdentityResult> DeleteAsync(SysRole role);
}

namespace BD.Common8.AspNetCore.Services;

/// <summary>
/// 提供用户管理
/// </summary>
public interface IUserManager
{
    /// <summary>
    /// 用于配置标识的 <see cref="IdentityOptions"/>
    /// </summary>
    internal IdentityOptions Options { get; }

    /// <summary>
    /// 根据 <see cref="string"/> 类型用户 Id 异步查找用户
    /// </summary>
    [Obsolete("use Guid userId", true)]
    Task<SysUser?> FindByIdAsync(string? userId)
    {
        if (userId == null) return Task.FromResult<SysUser?>(null);
        if (ShortGuid.TryParse(userId, out Guid useIdG))
            return FindByIdAsync(useIdG);
        return Task.FromResult<SysUser?>(null);
    }

    /// <summary>
    /// 根据 <see cref="Guid"/> 类型用户 Id 异步查找用户
    /// </summary>
    Task<SysUser?> FindByIdAsync(Guid userId);

    /// <summary>
    /// 异步获取用户的角色列表
    /// </summary>
    Task<IList<string>> GetRolesAsync(SysUser user);

    /// <summary>
    /// 根据用户名异步查找用户
    /// </summary>
    Task<SysUser?> FindByNameAsync(string? userName);

    /// <summary>
    /// 根据用户名和租户 Id 异步查找用户
    /// </summary>
    Task<SysUser?> FindByNameAsync(string? userName, Guid tenantId);

    /// <summary>
    /// 创建用户
    /// </summary>
    Task<IdentityResult> CreateAsync(SysUser user, string password);

    /// <summary>
    /// 尝试获取用户 Id
    /// </summary>
    bool TryGetUserId(ClaimsPrincipal principal, out Guid userId);

    /// <summary>
    /// 获取用户名称
    /// </summary>
    ValueTask<string> GetUserNameAsync(SysUser user);

    /// <summary>
    /// 设置用户名称
    /// </summary>
    ValueTask<IdentityResult> SetUserNameAsync(SysUser user, string userName);

    /// <summary>
    /// 从多个角色中删除用户
    /// </summary>
    Task<IdentityResult> RemoveFromRolesAsync(SysUser user, IEnumerable<string> roles);

    /// <summary>
    /// 设置用户锁定的截止日期
    /// </summary>
    Task<IdentityResult> SetLockoutEndDateAsync(SysUser user, DateTimeOffset? lockoutEnd);

    /// <summary>
    /// 更新用户
    /// </summary>
    Task<IdentityResult> UpdateAsync(SysUser user);

    /// <summary>
    /// 校验密码
    /// </summary>
    ValueTask<bool> CheckPasswordAsync(SysUser user, string password);

    /// <summary>
    /// 检查用户是否被锁定
    /// </summary>
    ValueTask<bool> IsLockedOutAsync(SysUser user);

    /// <summary>
    /// 更改用户密码
    /// </summary>
    Task<IdentityResult> ChangePasswordAsync(SysUser user, string currentPassword, string newPassword);

    /// <summary>
    /// 根据 <see cref="ClaimsPrincipal"/> 获取用户
    /// </summary>
    Task<SysUser?> GetUserAsync(ClaimsPrincipal principal);

    /// <summary>
    /// 将用户添加到角色
    /// </summary>
    Task<IdentityResult> AddToRoleAsync(SysUser user, string role) => AddToRolesAsync(user, role);

    /// <summary>
    /// 将用户添加到多个角色中
    /// </summary>
    Task<IdentityResult> AddToRolesAsync(SysUser user, params string[] roles)
    {
        IEnumerable<string> roles_ = roles;
        return AddToRolesAsync(user, roles_);
    }

    /// <summary>
    /// 将用户添加到多个角色中
    /// </summary>
    Task<IdentityResult> AddToRolesAsync(SysUser user, IEnumerable<string> roles);
}

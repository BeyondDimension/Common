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
    Task<BMUser?> FindByIdAsync(string? userId)
    {
        if (userId == null) return Task.FromResult<BMUser?>(null);
        if (ShortGuid.TryParse(userId, out Guid useIdG))
            return FindByIdAsync(useIdG);
        return Task.FromResult<BMUser?>(null);
    }

    /// <summary>
    /// 根据 <see cref="Guid"/> 类型用户 Id 异步查找用户
    /// </summary>
    Task<BMUser?> FindByIdAsync(Guid userId);

    /// <summary>
    /// 异步获取用户的角色列表
    /// </summary>
    Task<IList<string>> GetRolesAsync(BMUser user);

    /// <summary>
    /// 根据用户名异步查找用户
    /// </summary>
    Task<BMUser?> FindByNameAsync(string? userName);

    /// <summary>
    /// 根据用户名和租户 Id 异步查找用户
    /// </summary>
    Task<BMUser?> FindByNameAsync(string? userName, Guid tenantId);

    /// <summary>
    /// 创建用户
    /// </summary>
    Task<IdentityResult> CreateAsync(BMUser user, string password);

    /// <summary>
    /// 尝试获取用户 Id
    /// </summary>
    bool TryGetUserId(ClaimsPrincipal principal, out Guid userId);

    /// <summary>
    /// 获取用户名称
    /// </summary>
    ValueTask<string> GetUserNameAsync(BMUser user);

    /// <summary>
    /// 设置用户名称
    /// </summary>
    ValueTask<IdentityResult> SetUserNameAsync(BMUser user, string userName);

    /// <summary>
    /// 从多个角色中删除用户
    /// </summary>
    Task<IdentityResult> RemoveFromRolesAsync(BMUser user, IEnumerable<string> roles);

    /// <summary>
    /// 设置用户锁定的截止日期
    /// </summary>
    Task<IdentityResult> SetLockoutEndDateAsync(BMUser user, DateTimeOffset? lockoutEnd);

    /// <summary>
    /// 更新用户
    /// </summary>
    Task<IdentityResult> UpdateAsync(BMUser user);

    /// <summary>
    /// 校验密码
    /// </summary>
    ValueTask<bool> CheckPasswordAsync(BMUser user, string password);

    /// <summary>
    /// 检查用户是否被锁定
    /// </summary>
    ValueTask<bool> IsLockedOutAsync(BMUser user);

    /// <summary>
    /// 更改用户密码
    /// </summary>
    Task<IdentityResult> ChangePasswordAsync(BMUser user, string currentPassword, string newPassword);

    /// <summary>
    /// 根据 <see cref="ClaimsPrincipal"/> 获取用户
    /// </summary>
    Task<BMUser?> GetUserAsync(ClaimsPrincipal principal);

    /// <summary>
    /// 将用户添加到角色
    /// </summary>
    Task<IdentityResult> AddToRoleAsync(BMUser user, string role) => AddToRolesAsync(user, role);

    /// <summary>
    /// 将用户添加到多个角色中
    /// </summary>
    Task<IdentityResult> AddToRolesAsync(BMUser user, params string[] roles)
    {
        IEnumerable<string> roles_ = roles;
        return AddToRolesAsync(user, roles_);
    }

    /// <summary>
    /// 将用户添加到多个角色中
    /// </summary>
    Task<IdentityResult> AddToRolesAsync(BMUser user, IEnumerable<string> roles);
}

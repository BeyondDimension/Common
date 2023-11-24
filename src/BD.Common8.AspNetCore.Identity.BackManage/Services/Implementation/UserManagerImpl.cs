namespace BD.Common8.AspNetCore.Services.Implementation;

/// <summary>
/// 用户管理
/// </summary>
/// <typeparam name="TDbContext"></typeparam>
public class UserManagerImpl<TDbContext> : IUserManager, IDisposable where TDbContext : DbContext, IApplicationDbContext
{
    bool _disposed;

    /// <summary>
    /// 数据库上下文
    /// </summary>
    protected readonly TDbContext db;

    /// <summary>
    /// 用于规范化用户名和角色名等内容 <see cref="ILookupNormalizer"/>
    /// </summary>
    public ILookupNormalizer KeyNormalizer { get; set; }

    /// <summary>
    /// 用于散列密码的 <see cref="IPasswordHasher{TUser}"/>
    /// </summary>
    public IPasswordHasher<SysUser> PasswordHasher { get; set; }

    /// <inheritdoc/>
    public IdentityOptions Options { get; set; }

    /// <summary>
    /// 用于验证密码的 <see cref="IPasswordValidator{TUser}"/>
    /// </summary>
    public IList<IPasswordValidator> PasswordValidators { get; } = new List<IPasswordValidator>();

    /// <summary>
    /// 用于生成错误消息的 <see cref="IdentityErrorDescriber"/>
    /// </summary>
    public IdentityErrorDescriber ErrorDescriber { get; set; }

    /// <summary>
    /// 取消标记令牌
    /// </summary>
    protected virtual CancellationToken CancellationToken => CancellationToken.None;

    /// <summary>
    /// 初始化 <see cref="UserManagerImpl{TDbContext}"/> 实例
    /// </summary>
    /// <param name="db"></param>
    /// <param name="keyNormalizer"></param>
    /// <param name="optionsAccessor"></param>
    /// <param name="passwordHasher"></param>
    /// <param name="passwordValidators"></param>
    /// <param name="errors"></param>
    public UserManagerImpl(TDbContext db,
        ILookupNormalizer keyNormalizer,
        IOptions<IdentityOptions> optionsAccessor,
        IPasswordHasher<SysUser> passwordHasher,
        IEnumerable<IPasswordValidator> passwordValidators,
        IdentityErrorDescriber errors)
    {
        this.db = db;
        Options = optionsAccessor?.Value ?? new IdentityOptions();
        PasswordHasher = passwordHasher;
        KeyNormalizer = keyNormalizer;
        ErrorDescriber = errors;

        if (passwordValidators != null)
            foreach (var v in passwordValidators)
                PasswordValidators.Add(v);
    }

    /// <inheritdoc/>
    public async Task<SysUser?> FindByIdAsync(Guid userId)
    {
        ThrowIfDisposed();
        var user = await db.Users.FindAsync(new object[] { userId, }, CancellationToken);
        return user;
    }

    /// <inheritdoc/>
    public async Task<IList<string>> GetRolesAsync(SysUser user)
    {
        ThrowIfDisposed();
        var query = from userRole in db.UserRoles
                    join role in db.Roles on userRole.RoleId equals role.Id
                    where userRole.UserId == user.Id &&
                        userRole.TenantId == user.TenantId &&
                        role.TenantId == user.TenantId
                    select role.Name;
        return await query.ToListAsync(CancellationToken);
    }

    /// <inheritdoc/>
    public async Task<SysUser?> FindByNameAsync(string? userName)
    {
        if (userName == null) return null;
        ThrowIfDisposed();
        var normalizedUserName = NormalizeName(userName);
        var user = await db.Users.FirstOrDefaultAsync(x =>
            x.NormalizedUserName == normalizedUserName, CancellationToken);
        return user;
    }

    /// <inheritdoc/>
    public async Task<SysUser?> FindByNameAsync(string? userName, Guid tenantId)
    {
        if (userName == null) return null;
        ThrowIfDisposed();
        var normalizedUserName = NormalizeName(userName);
        var user = await db.Users.FirstOrDefaultAsync(x =>
            x.NormalizedUserName == normalizedUserName && x.TenantId == tenantId, CancellationToken);
        return user;
    }

    /// <inheritdoc/>
    public async Task<IdentityResult> CreateAsync(SysUser user, string password)
    {
        ThrowIfDisposed();
        var result = await ValidateUserAsync(user);
        if (!result.Succeeded)
            return result;
        result = await UpdatePasswordHash(user, password);
        if (!result.Succeeded)
            return result;
        if (Options.Lockout.AllowedForNewUsers)
            user.LockoutEnabled = true;
        await UpdateNormalizedUserNameAsync(user);
        await db.Users.AddAsync(user, CancellationToken);
        await db.SaveChangesAsync(CancellationToken);
        return IdentityResult.Success;
    }

    /// <summary>
    /// 更新用户的密码哈希
    /// </summary>
    /// <param name="user">用户</param>
    /// <param name="newPassword">新密码</param>
    /// <param name="validatePassword">是否验证密码</param>
    /// <returns>密码是否已成功更新</returns>
    protected virtual async ValueTask<IdentityResult> UpdatePasswordHash(SysUser user, string? newPassword, bool validatePassword = true)
    {
        if (validatePassword)
        {
            var validate = await ValidatePasswordAsync(user, newPassword);
            if (!validate.Succeeded)
                return validate;
        }
        var hash = newPassword != null ? PasswordHasher.HashPassword(user, newPassword) : null;
        user.PasswordHash = hash;
        return IdentityResult.Success;
    }

    /// <summary>
    /// 如果验证成功，则应返回 <see cref="IdentityResult.Success"/>。这是在更新密码哈希之前调用
    /// </summary>
    /// <param name="user">用户</param>
    /// <param name="password">密码</param>
    /// <returns>返回 <see cref="IdentityResult"/>，表示验证是否成功</returns>
    protected async ValueTask<IdentityResult> ValidatePasswordAsync(SysUser user, string? password)
    {
        List<IdentityError>? errors = null;
        var isValid = true;
        foreach (var v in PasswordValidators)
        {
            var result = await v.ValidateAsync(this, user, password).ConfigureAwait(false);
            if (!result.Succeeded)
            {
                if (result.Errors.Any())
                {
                    errors ??= [];
                    errors.AddRange(result.Errors);
                }

                isValid = false;
            }
        }
        if (!isValid)
            //Logger.LogDebug(LoggerEventIds.PasswordValidationFailed, "User password validation failed: {errors}.", string.Join(";", errors?.Select(e => e.Code) ?? Array.Empty<string>()));
            return IdentityResult.Failed(errors!.ToArray());
        return IdentityResult.Success;
    }

    /// <summary>
    /// 如果验证成功，则应返回 <see cref="IdentityResult.Success"/>。这是在通过创建或更新保存用户之前调用
    /// </summary>
    /// <param name="user">用户</param>
    /// <returns>返回 <see cref="IdentityResult"/>，表示验证是否成功</returns>
    protected ValueTask<IdentityResult> ValidateUserAsync(SysUser user)
    {
        if (user.TenantId == default)
            return new(IdentityResult.Failed(new IdentityError[]
            {
                new IdentityError
                {
                    Description = "租户 Id 不能为空",
                },
            }));
        if (string.IsNullOrWhiteSpace(user.UserName))
            return new(UserNameIsNullOrWhiteSpace());
        return new(IdentityResult.Success);
    }

    static IdentityResult UserNameIsNullOrWhiteSpace() => IdentityResult.Failed(new IdentityError[]
    {
        new IdentityError
        {
            Description = "用户名不能为空或空白字符",
        },
    });

    /// <inheritdoc/>
    public bool TryGetUserId(ClaimsPrincipal principal, out Guid userId)
    {
        var userIdS = principal.FindFirstValue(Options.ClaimsIdentity.UserIdClaimType);
        return ShortGuid.TryParse(userIdS, out userId);
    }

    /// <inheritdoc/>
    public ValueTask<string> GetUserNameAsync(SysUser user) => new(user.UserName);

    /// <inheritdoc/>
    public async ValueTask<IdentityResult> SetUserNameAsync(SysUser user, string userName)
    {
        if (string.IsNullOrWhiteSpace(userName))
            return UserNameIsNullOrWhiteSpace();
        user.UserName = userName;
        await UpdateUserAsync(user);
        return IdentityResult.Success;
    }

    /// <inheritdoc/>
    public Task<IdentityResult> UpdateAsync(SysUser user)
    {
        ThrowIfDisposed();
        return UpdateUserAsync(user);
    }

    /// <summary>
    /// 更新用户
    /// </summary>
    protected virtual async Task<IdentityResult> UpdateUserAsync(SysUser user)
    {
        var result = await ValidateUserAsync(user);
        if (!result.Succeeded)
            return result;
        await UpdateNormalizedUserNameAsync(user);
        db.Users.Attach(user);
        db.Users.Update(user);
        try
        {
            await db.SaveChangesAsync(CancellationToken);
        }
        catch (DbUpdateConcurrencyException)
        {
            return IdentityResult.Failed(ErrorDescriber.ConcurrencyFailure());
        }
        return IdentityResult.Success;
    }

    /// <inheritdoc/>
    public async Task<SysUser?> GetUserAsync(ClaimsPrincipal principal)
    {
        if (!TryGetUserId(principal, out var userId)) return null;
        var user = await FindByIdAsync(userId);
        return user;
    }

    /// <inheritdoc/>
    public async Task<IdentityResult> SetLockoutEndDateAsync(SysUser user, DateTimeOffset? lockoutEnd)
    {
        ThrowIfDisposed();
        if (!user.LockoutEnabled)
            return IdentityResult.Failed(ErrorDescriber.UserLockoutNotEnabled());
        user.LockoutEnd = lockoutEnd;
        return await UpdateUserAsync(user);
    }

    /// <inheritdoc/>
    public ValueTask<bool> IsLockedOutAsync(SysUser user)
    {
        ThrowIfDisposed();
        if (!user.LockoutEnabled)
            return new(false);
        var lockoutTime = user.LockoutEnd;
        return new(lockoutTime >= DateTimeOffset.UtcNow);
    }

    /// <summary>
    /// 校验密码
    /// </summary>
    public async ValueTask<bool> CheckPasswordAsync(SysUser user, string password)
    {
        ThrowIfDisposed();
        var result = await VerifyPasswordAsync(user, password);
        if (result == PasswordVerificationResult.SuccessRehashNeeded)
        {
            await UpdatePasswordHash(user, password, validatePassword: false);
            await UpdateUserAsync(user);
        }

        var success = result != PasswordVerificationResult.Failed;
        if (!success)
        {
            //Logger.LogDebug(LoggerEventIds.InvalidPassword, "Invalid password for user.");
        }
        return success;
    }

    /// <summary>
    /// 返回指示密码哈希比较结果的 <see cref="PasswordVerificationResult"/>
    /// </summary>
    /// <param name="user">应验证其密码的用户</param>
    /// <param name="password">要验证的密码</param>
    /// <returns>
    /// 返回异步操作，包含 <see cref="PasswordVerificationResult"/>
    /// </returns>
    protected virtual ValueTask<PasswordVerificationResult> VerifyPasswordAsync(SysUser user, string password)
    {
        var hash = user.PasswordHash;
        if (hash == null)
            return new(PasswordVerificationResult.Failed);
        return new(PasswordHasher.VerifyHashedPassword(user, hash, password));
    }

    /// <inheritdoc/>
    public async Task<IdentityResult> ChangePasswordAsync(SysUser user, string currentPassword, string newPassword)
    {
        ThrowIfDisposed();
        if (await VerifyPasswordAsync(user, currentPassword) != PasswordVerificationResult.Failed)
        {
            var result = await UpdatePasswordHash(user, newPassword);
            if (!result.Succeeded)
                return result;
            return await UpdateUserAsync(user);
        }
        //Logger.LogDebug(LoggerEventIds.ChangePasswordFailed, "Change password failed for user.");
        return IdentityResult.Failed(ErrorDescriber.PasswordMismatch());
    }

    /// <summary>
    /// 判断用户是否属于指定角色
    /// </summary>
    public async Task<bool> IsInRoleAsync(SysUser user, string role)
    {
        ThrowIfDisposed();
        var normalizedRole = NormalizeName(role);
        var (isInRole, _, _) = await IsInRoleCoreAsync(user, normalizedRole);
        return isInRole;
    }

    /// <summary>
    /// 核心方法，判断用户是否属于指定角色
    /// </summary>
    protected virtual async Task<(bool isInRole, Guid roleId, SysUserRole? userRole)> IsInRoleCoreAsync(SysUser user, string normalizedRole)
    {
        var role = await db.Roles.FirstOrDefaultAsync(x =>
            x.NormalizedName == normalizedRole && x.TenantId == user.TenantId, CancellationToken);
        if (role != null)
        {
            var userRole = await db.UserRoles.FindAsync(
                new object[] { user.Id, role.Id, user.TenantId }, CancellationToken);
            return (userRole != null, role.Id, userRole);
        }
        return (false, default, default);
    }

    /// <summary>
    /// 将用户添加到角色
    /// </summary>
    protected virtual async ValueTask AddToRoleAsync(SysUser user, Guid roleId)
    {
        var userRole = new SysUserRole { UserId = user.Id, RoleId = roleId, TenantId = user.TenantId };
        await db.UserRoles.AddAsync(userRole, CancellationToken);
    }

    /// <summary>
    /// 将用户添加到多个角色中
    /// </summary>
    public async Task<IdentityResult> AddToRolesAsync(SysUser user, IEnumerable<string> roles)
    {
        ThrowIfDisposed();
        foreach (var role in roles.Distinct())
        {
            var normalizedRole = NormalizeName(role);
            var (isInRole, roleId, _) = await IsInRoleCoreAsync(user, normalizedRole);
            if (isInRole)
                continue;
            await AddToRoleAsync(user, roleId);
        }
        return await UpdateUserAsync(user);
    }

    /// <summary>
    /// 从角色中删除用户
    /// </summary>
    protected virtual ValueTask RemoveFromRoleAsync(SysUserRole userRole)
    {
        db.UserRoles.Remove(userRole);
        return ValueTask.CompletedTask;
    }

    /// <summary>
    /// 从多个角色中删除用户
    /// </summary>
    public async Task<IdentityResult> RemoveFromRolesAsync(SysUser user, IEnumerable<string> roles)
    {
        ThrowIfDisposed();
        foreach (var role in roles.Distinct())
        {
            var normalizedRole = NormalizeName(role);
            var (isInRole, _, userRole) = await IsInRoleCoreAsync(user, normalizedRole);
            if (!isInRole)
                continue;
            await RemoveFromRoleAsync(userRole!);
        }
        return await UpdateUserAsync(user);
    }

    /// <summary>
    /// 规范化用户名或角色名以进行一致的比较
    /// </summary>
    /// <param name="name">要规范化的名称</param>
    /// <returns>返回指定的规范值</returns>
    [return: NotNullIfNotNull(nameof(name))]
    public virtual string? NormalizeName(string? name)
        => KeyNormalizer == null ? name : KeyNormalizer.NormalizeName(name);

    /// <summary>
    /// 更新指定的 <paramref name="user"/> 的规范化用户名
    /// </summary>
    /// <param name="user">应规范化和更新其用户名的用户</param>
    /// <returns>返回异步操作 <see cref="Task"/></returns>
    public virtual ValueTask UpdateNormalizedUserNameAsync(SysUser user)
    {
        var normalizedName = NormalizeName(user.UserName);
        user.NormalizedUserName = normalizedName;
        return ValueTask.CompletedTask;
    }

    /// <summary>
    /// 释放角色管理器使用的所有资源
    /// </summary>
    public void Dispose()
    {
        Dispose(true);
        GC.SuppressFinalize(this);
    }

    /// <summary>
    /// 释放角色管理器使用的非托管资源，也可以选择释放托管资源
    /// </summary>
    /// <param name="disposing"><see langword="true"/> 同时释放托管和非托管资源；<see langword="false"/> 仅释放非托管资源</param>
    protected virtual void Dispose(bool disposing)
    {
        if (disposing && !_disposed)
            db.Dispose();
        _disposed = true;
    }

    /// <summary>
    /// 如果该类已被释放则抛出异常
    /// </summary>
    /// <exception cref="ObjectDisposedException"></exception>
    protected void ThrowIfDisposed()
    {
        if (_disposed)
            throw new ObjectDisposedException(GetType().Name);
    }
}

/// <summary>
///  ASP.NET 应用程序中进行用户管理
/// </summary>
/// <typeparam name="TDbContext"></typeparam>
public class AspNetUserManager<TDbContext> : UserManagerImpl<TDbContext> where TDbContext : ApplicationDbContextBase
{
    /// <summary>
    /// 请求上下文的访问器
    /// </summary>
    protected readonly IHttpContextAccessor contextAccessor;

    /// <summary>
    /// 初始化 <see cref="AspNetUserManager{TDbContext}"/> 实例
    /// </summary>
    public AspNetUserManager(IHttpContextAccessor contextAccessor, TDbContext db, ILookupNormalizer keyNormalizer, IOptions<IdentityOptions> optionsAccessor, IPasswordHasher<SysUser> passwordHasher, IEnumerable<IPasswordValidator> passwordValidators, IdentityErrorDescriber errors) : base(db, keyNormalizer, optionsAccessor, passwordHasher, passwordValidators, errors)
    {
        this.contextAccessor = contextAccessor;
    }

    /// <summary>
    /// 获取请求的取消令牌
    /// </summary>
    protected override CancellationToken CancellationToken => contextAccessor?.HttpContext?.RequestAborted ?? CancellationToken.None;
}

namespace BD.Common.Services.Implementation;

public class UserManager<TDbContext> : IUserManager, IDisposable where TDbContext : DbContext, IApplicationDbContext
{
    private bool _disposed;

    protected readonly TDbContext db;

    /// <summary>
    /// The <see cref="ILookupNormalizer"/> used to normalize things like user and role names.
    /// </summary>
    public ILookupNormalizer KeyNormalizer { get; set; }

    /// <summary>
    /// The <see cref="IPasswordHasher{TUser}"/> used to hash passwords.
    /// </summary>
    public IPasswordHasher<SysUser> PasswordHasher { get; set; }

    /// <summary>
    /// The <see cref="IdentityOptions"/> used to configure Identity.
    /// </summary>
    public IdentityOptions Options { get; set; }

    /// <summary>
    /// The <see cref="IPasswordValidator{TUser}"/> used to validate passwords.
    /// </summary>
    public IList<IPasswordValidator> PasswordValidators { get; } = new List<IPasswordValidator>();

    /// <summary>
    /// The <see cref="IdentityErrorDescriber"/> used to generate error messages.
    /// </summary>
    public IdentityErrorDescriber ErrorDescriber { get; set; }

    protected virtual CancellationToken CancellationToken => CancellationToken.None;

    public UserManager(TDbContext db,
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
        {
            foreach (var v in passwordValidators)
            {
                PasswordValidators.Add(v);
            }
        }

    }

    public async Task<SysUser?> FindByIdAsync(Guid userId)
    {
        ThrowIfDisposed();
        var user = await db.Users.FindAsync(new object[] { userId, }, CancellationToken);
        return user;
    }

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

    public async Task<SysUser?> FindByNameAsync(string? userName)
    {
        if (userName == null) return null;
        ThrowIfDisposed();
        var normalizedUserName = NormalizeName(userName);
        var user = await db.Users.FirstOrDefaultAsync(x =>
            x.NormalizedUserName == normalizedUserName, CancellationToken);
        return user;
    }

    public async Task<SysUser?> FindByNameAsync(string? userName, Guid tenantId)
    {
        if (userName == null) return null;
        ThrowIfDisposed();
        var normalizedUserName = NormalizeName(userName);
        var user = await db.Users.FirstOrDefaultAsync(x =>
            x.NormalizedUserName == normalizedUserName && x.TenantId == tenantId, CancellationToken);
        return user;
    }

    public async Task<IdentityResult> CreateAsync(SysUser user, string password)
    {
        ThrowIfDisposed();
        var result = await ValidateUserAsync(user);
        if (!result.Succeeded)
        {
            return result;
        }
        result = await UpdatePasswordHash(user, password);
        if (!result.Succeeded)
        {
            return result;
        }
        if (Options.Lockout.AllowedForNewUsers)
        {
            user.LockoutEnabled = true;
        }
        await UpdateNormalizedUserNameAsync(user);
        await db.Users.AddAsync(user, CancellationToken);
        await db.SaveChangesAsync(CancellationToken);
        return IdentityResult.Success;
    }

    /// <summary>
    /// Updates a user's password hash.
    /// </summary>
    /// <param name="user">The user.</param>
    /// <param name="newPassword">The new password.</param>
    /// <param name="validatePassword">Whether to validate the password.</param>
    /// <returns>Whether the password has was successfully updated.</returns>
    protected virtual async ValueTask<IdentityResult> UpdatePasswordHash(SysUser user, string? newPassword, bool validatePassword = true)
    {
        if (validatePassword)
        {
            var validate = await ValidatePasswordAsync(user, newPassword);
            if (!validate.Succeeded)
            {
                return validate;
            }
        }
        var hash = newPassword != null ? PasswordHasher.HashPassword(user, newPassword) : null;
        user.PasswordHash = hash;
        return IdentityResult.Success;
    }

    /// <summary>
    /// Should return <see cref="IdentityResult.Success"/> if validation is successful. This is
    /// called before updating the password hash.
    /// </summary>
    /// <param name="user">The user.</param>
    /// <param name="password">The password.</param>
    /// <returns>A <see cref="IdentityResult"/> representing whether validation was successful.</returns>
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
                    errors ??= new List<IdentityError>();
                    errors.AddRange(result.Errors);
                }

                isValid = false;
            }
        }
        if (!isValid)
        {
            //Logger.LogDebug(LoggerEventIds.PasswordValidationFailed, "User password validation failed: {errors}.", string.Join(";", errors?.Select(e => e.Code) ?? Array.Empty<string>()));
            return IdentityResult.Failed(errors!.ToArray());
        }
        return IdentityResult.Success;
    }

    /// <summary>
    /// Should return <see cref="IdentityResult.Success"/> if validation is successful. This is
    /// called before saving the user via Create or Update.
    /// </summary>
    /// <param name="user">The user</param>
    /// <returns>A <see cref="IdentityResult"/> representing whether validation was successful.</returns>
    protected ValueTask<IdentityResult> ValidateUserAsync(SysUser user)
    {
        if (user.TenantId == default)
        {
            return new(IdentityResult.Failed(new IdentityError[]
            {
                new IdentityError
                {
                    Description = "租户 Id 不能为空",
                },
            }));
        }
        if (string.IsNullOrWhiteSpace(user.UserName))
        {
            return new(UserNameIsNullOrWhiteSpace());
        }
        return new(IdentityResult.Success);
    }

    static IdentityResult UserNameIsNullOrWhiteSpace() => IdentityResult.Failed(new IdentityError[]
    {
        new IdentityError
        {
            Description = "用户名不能为空或空白字符",
        },
    });

    public bool TryGetUserId(ClaimsPrincipal principal, out Guid userId)
    {
        var userIdS = principal.FindFirstValue(Options.ClaimsIdentity.UserIdClaimType);
        return ShortGuid.TryParse(userIdS, out userId);
    }

    public ValueTask<string> GetUserNameAsync(SysUser user) => new(user.UserName);

    public async ValueTask<IdentityResult> SetUserNameAsync(SysUser user, string userName)
    {
        if (string.IsNullOrWhiteSpace(userName))
        {
            return UserNameIsNullOrWhiteSpace();
        }
        user.UserName = userName;
        await UpdateUserAsync(user);
        return IdentityResult.Success;
    }

    public Task<IdentityResult> UpdateAsync(SysUser user)
    {
        ThrowIfDisposed();
        return UpdateUserAsync(user);
    }

    /// <summary>
    /// Called to update the user after validating and updating the normalized email/user name.
    /// </summary>
    /// <param name="user">The user.</param>
    /// <returns>Whether the operation was successful.</returns>
    protected virtual async Task<IdentityResult> UpdateUserAsync(SysUser user)
    {
        var result = await ValidateUserAsync(user);
        if (!result.Succeeded)
        {
            return result;
        }
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

    public async Task<SysUser?> GetUserAsync(ClaimsPrincipal principal)
    {
        if (!TryGetUserId(principal, out var userId)) return null;
        var user = await FindByIdAsync(userId);
        return user;
    }

    public async Task<IdentityResult> SetLockoutEndDateAsync(SysUser user, DateTimeOffset? lockoutEnd)
    {
        ThrowIfDisposed();
        if (!user.LockoutEnabled)
        {
            return IdentityResult.Failed(ErrorDescriber.UserLockoutNotEnabled());
        }
        user.LockoutEnd = lockoutEnd;
        return await UpdateUserAsync(user);
    }

    public ValueTask<bool> IsLockedOutAsync(SysUser user)
    {
        ThrowIfDisposed();
        if (!user.LockoutEnabled)
        {
            return new(false);
        }
        var lockoutTime = user.LockoutEnd;
        return new(lockoutTime >= DateTimeOffset.UtcNow);
    }

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
    /// Returns a <see cref="PasswordVerificationResult"/> indicating the result of a password hash comparison.
    /// </summary>
    /// <param name="store">The store containing a user's password.</param>
    /// <param name="user">The user whose password should be verified.</param>
    /// <param name="password">The password to verify.</param>
    /// <returns>
    /// The <see cref="Task"/> that represents the asynchronous operation, containing the <see cref="PasswordVerificationResult"/>
    /// of the operation.
    /// </returns>
    protected virtual ValueTask<PasswordVerificationResult> VerifyPasswordAsync(SysUser user, string password)
    {
        var hash = user.PasswordHash;
        if (hash == null)
        {
            return new(PasswordVerificationResult.Failed);
        }
        return new(PasswordHasher.VerifyHashedPassword(user, hash, password));
    }

    public async Task<IdentityResult> ChangePasswordAsync(SysUser user, string currentPassword, string newPassword)
    {
        ThrowIfDisposed();
        if (await VerifyPasswordAsync(user, currentPassword) != PasswordVerificationResult.Failed)
        {
            var result = await UpdatePasswordHash(user, newPassword);
            if (!result.Succeeded)
            {
                return result;
            }
            return await UpdateUserAsync(user);
        }
        //Logger.LogDebug(LoggerEventIds.ChangePasswordFailed, "Change password failed for user.");
        return IdentityResult.Failed(ErrorDescriber.PasswordMismatch());
    }

    public async Task<bool> IsInRoleAsync(SysUser user, string role)
    {
        ThrowIfDisposed();
        var normalizedRole = NormalizeName(role);
        var (isInRole, _, _) = await IsInRoleCoreAsync(user, normalizedRole);
        return isInRole;
    }

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

    protected virtual async ValueTask AddToRoleAsync(SysUser user, Guid roleId)
    {
        var userRole = new SysUserRole { UserId = user.Id, RoleId = roleId, TenantId = user.TenantId };
        await db.UserRoles.AddAsync(userRole, CancellationToken);
    }

    public async Task<IdentityResult> AddToRolesAsync(SysUser user, IEnumerable<string> roles)
    {
        ThrowIfDisposed();
        foreach (var role in roles.Distinct())
        {
            var normalizedRole = NormalizeName(role);
            var (isInRole, roleId, _) = await IsInRoleCoreAsync(user, normalizedRole);
            if (isInRole)
            {
                continue;
                //return UserAlreadyInRoleError(role);
            }
            await AddToRoleAsync(user, roleId);
        }
        return await UpdateUserAsync(user);
    }

    protected virtual ValueTask RemoveFromRoleAsync(SysUserRole userRole)
    {
        db.UserRoles.Remove(userRole);
        return ValueTask.CompletedTask;
    }

    public async Task<IdentityResult> RemoveFromRolesAsync(SysUser user, IEnumerable<string> roles)
    {
        ThrowIfDisposed();
        foreach (var role in roles.Distinct())
        {
            var normalizedRole = NormalizeName(role);
            var (isInRole, _, userRole) = await IsInRoleCoreAsync(user, normalizedRole);
            if (!isInRole)
            {
                continue;
                //return UserNotInRoleError(role);
            }
            await RemoveFromRoleAsync(userRole!);
        }
        return await UpdateUserAsync(user);
    }

    /// <summary>
    /// Normalize user or role name for consistent comparisons.
    /// </summary>
    /// <param name="name">The name to normalize.</param>
    /// <returns>A normalized value representing the specified <paramref name="name"/>.</returns>
    [return: NotNullIfNotNull("name")]
    public virtual string? NormalizeName(string? name)
        => KeyNormalizer == null ? name : KeyNormalizer.NormalizeName(name);

    /// <summary>
    /// Updates the normalized user name for the specified <paramref name="user"/>.
    /// </summary>
    /// <param name="user">The user whose user name should be normalized and updated.</param>
    /// <returns>The <see cref="Task"/> that represents the asynchronous operation.</returns>
    public virtual ValueTask UpdateNormalizedUserNameAsync(SysUser user)
    {
        var normalizedName = NormalizeName(user.UserName);
        user.NormalizedUserName = normalizedName;
        return ValueTask.CompletedTask;
    }

    /// <summary>
    /// Releases all resources used by the role manager.
    /// </summary>
    public void Dispose()
    {
        Dispose(true);
        GC.SuppressFinalize(this);
    }

    /// <summary>
    /// Releases the unmanaged resources used by the role manager and optionally releases the managed resources.
    /// </summary>
    /// <param name="disposing">true to release both managed and unmanaged resources; false to release only unmanaged resources.</param>
    protected virtual void Dispose(bool disposing)
    {
        if (disposing && !_disposed)
        {
            db.Dispose();
        }
        _disposed = true;
    }

    /// <summary>
    /// Throws if this class has been disposed.
    /// </summary>
    protected void ThrowIfDisposed()
    {
        if (_disposed)
        {
            throw new ObjectDisposedException(GetType().Name);
        }
    }
}

public class AspNetUserManager<TDbContext> : UserManager<TDbContext> where TDbContext : ApplicationDbContextBase
{
    protected readonly IHttpContextAccessor contextAccessor;

    public AspNetUserManager(IHttpContextAccessor contextAccessor, TDbContext db, ILookupNormalizer keyNormalizer, IOptions<IdentityOptions> optionsAccessor, IPasswordHasher<SysUser> passwordHasher, IEnumerable<IPasswordValidator> passwordValidators, IdentityErrorDescriber errors) : base(db, keyNormalizer, optionsAccessor, passwordHasher, passwordValidators, errors)
    {
        this.contextAccessor = contextAccessor;
    }

    protected override CancellationToken CancellationToken => contextAccessor?.HttpContext?.RequestAborted ?? CancellationToken.None;
}

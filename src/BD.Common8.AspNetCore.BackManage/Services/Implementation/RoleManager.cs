namespace BD.Common.Services.Implementation;

public class RoleManager<TDbContext> : IRoleManager, IDisposable where TDbContext : DbContext, IApplicationDbContext
{
    private bool _disposed;

    protected readonly TDbContext db;

    /// <summary>
    /// Gets the normalizer to use when normalizing role names to keys.
    /// </summary>
    /// <value>
    /// The normalizer to use when normalizing role names to keys.
    /// </value>
    public ILookupNormalizer KeyNormalizer { get; set; }

    /// <summary>
    /// The <see cref="IdentityErrorDescriber"/> used to generate error messages.
    /// </summary>
    public IdentityErrorDescriber ErrorDescriber { get; set; }

    protected virtual CancellationToken CancellationToken => CancellationToken.None;

    public RoleManager(
        TDbContext db,
        ILookupNormalizer lookupNormalizer,
        IdentityErrorDescriber errors)
    {
        this.db = db;
        KeyNormalizer = lookupNormalizer;
        ErrorDescriber = errors;
    }

    public async Task<SysRole?> FindByIdAsync(Guid roleId)
    {
        ThrowIfDisposed();
        var role = await db.Roles.FindAsync(new object[] { roleId, }, CancellationToken);
        return role;
    }

    public async Task<IdentityResult> CreateAsync(SysRole role)
    {
        ThrowIfDisposed();
        var result = await ValidateRoleAsync(role);
        if (!result.Succeeded)
        {
            return result;
        }
        await UpdateNormalizedRoleNameAsync(role);
        await db.Roles.AddAsync(role, CancellationToken);
        await db.SaveChangesAsync(CancellationToken);
        return IdentityResult.Success;
    }

    public async Task<SysRole?> FindByNameAsync(string? roleName, Guid tenantId)
    {
        if (roleName == null) return null;
        ThrowIfDisposed();
        var normalizedName = NormalizeKey(roleName);
        var role = await db.Roles.FirstOrDefaultAsync(x =>
            x.NormalizedName == normalizedName && x.TenantId == tenantId, CancellationToken);
        return role;
    }

    /// <summary>
    /// Gets a normalized representation of the specified <paramref name="key"/>.
    /// </summary>
    /// <param name="key">The value to normalize.</param>
    /// <returns>A normalized representation of the specified <paramref name="key"/>.</returns>
    [return: NotNullIfNotNull("key")]
    public virtual string? NormalizeKey(string? key) => KeyNormalizer == null ? key : KeyNormalizer.NormalizeName(key);

    protected virtual ValueTask<IdentityResult> ValidateRoleAsync(SysRole role)
    {
        if (role.TenantId == default)
        {
            return new(IdentityResult.Failed(new IdentityError[]
            {
                new IdentityError
                {
                    Description = "租户 Id 不能为空",
                },
            }));
        }
        if (string.IsNullOrWhiteSpace(role.Name))
        {
            return new(IdentityResult.Failed(new IdentityError[]
            {
                new IdentityError
                {
                    Description = "角色名称不能为空或空白字符",
                },
            }));
        }

        return new(IdentityResult.Success);
    }

    public virtual ValueTask UpdateNormalizedRoleNameAsync(SysRole role)
    {
        role.NormalizedName = NormalizeKey(role.Name);
        return ValueTask.CompletedTask;
    }

    public Task<IdentityResult> UpdateAsync(SysRole role)
    {
        ThrowIfDisposed();
        return UpdateRoleAsync(role);
    }

    public Task<IdentityResult> DeleteAsync(SysRole role)
    {
        ThrowIfDisposed();
        role.SoftDeleted = true;
        return UpdateRoleAsync(role);
    }

    protected virtual async Task<IdentityResult> UpdateRoleAsync(SysRole role)
    {
        var result = await ValidateRoleAsync(role);
        if (!result.Succeeded)
        {
            return result;
        }
        await UpdateNormalizedRoleNameAsync(role);
        db.Roles.Attach(role);
        db.Roles.Update(role);
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

public class AspNetRoleManager<TDbContext> : RoleManager<TDbContext> where TDbContext : ApplicationDbContextBase
{
    protected readonly IHttpContextAccessor contextAccessor;

    public AspNetRoleManager(IHttpContextAccessor contextAccessor, TDbContext db, ILookupNormalizer lookupNormalizer, IdentityErrorDescriber errors) : base(db, lookupNormalizer, errors)
    {
        this.contextAccessor = contextAccessor;
    }

    protected override CancellationToken CancellationToken => contextAccessor?.HttpContext?.RequestAborted ?? CancellationToken.None;
}
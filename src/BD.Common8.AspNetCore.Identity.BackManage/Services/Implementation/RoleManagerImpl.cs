namespace BD.Common8.AspNetCore.Services.Implementation;

/// <summary>
/// 角色管理器
/// </summary>
/// <typeparam name="TDbContext"></typeparam>
public class RoleManagerImpl<TDbContext>(
    TDbContext db,
    ILookupNormalizer lookupNormalizer,
    IdentityErrorDescriber errors) : IRoleManager, IDisposable where TDbContext : DbContext, IApplicationDbContext
{
    bool _disposed;

    /// <summary>
    /// 数据库上下文
    /// </summary>
    protected readonly TDbContext db = db;

    /// <summary>
    /// 获取将角色名称规范化为键时要使用的规范化器
    /// </summary>
    /// <value>
    /// 将角色名称规范化为键时要使用的规范化器
    /// </value>
    public ILookupNormalizer KeyNormalizer { get; set; } = lookupNormalizer;

    /// <summary>
    /// 用于生成错误消息的 <see cref="IdentityErrorDescriber"/>
    /// </summary>
    public IdentityErrorDescriber ErrorDescriber { get; set; } = errors;

    /// <summary>
    /// 获取取消令牌
    /// </summary>
    protected virtual CancellationToken CancellationToken => CancellationToken.None;

    /// <summary>
    /// 根据角色 Id 查找角色
    /// </summary>
    /// <param name="roleId"></param>
    /// <returns></returns>
    public async Task<SysRole?> FindByIdAsync(Guid roleId)
    {
        ThrowIfDisposed();
        var role = await db.Roles.FindAsync(new object[] { roleId, }, CancellationToken);
        return role;
    }

    /// <summary>
    /// 创建角色
    /// </summary>
    public async Task<IdentityResult> CreateAsync(SysRole role)
    {
        ThrowIfDisposed();
        var result = await ValidateRoleAsync(role);
        if (!result.Succeeded)
            return result;
        await UpdateNormalizedRoleNameAsync(role);
        await db.Roles.AddAsync(role, CancellationToken);
        await db.SaveChangesAsync(CancellationToken);
        return IdentityResult.Success;
    }

    /// <summary>
    /// 根据角色名称和租户 Id 查找角色
    /// </summary>
    /// <param name="roleName"></param>
    /// <param name="tenantId"></param>
    /// <returns></returns>
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
    /// 获取指定的的规范化表示形式 <paramref name="key"/>
    /// </summary>
    /// <param name="key">要规范化的值</param>
    /// <returns>指定的规范化表示 <paramref name="key"/> </returns>
    [return: NotNullIfNotNull(nameof(key))]
    public virtual string? NormalizeKey(string? key) => KeyNormalizer == null ? key : KeyNormalizer.NormalizeName(key);

    /// <summary>
    /// 验证角色对象的有效性
    /// </summary>
    protected virtual ValueTask<IdentityResult> ValidateRoleAsync(SysRole role)
    {
        if (role.TenantId == default)
            return new(IdentityResult.Failed(new IdentityError[]
            {
                new IdentityError
                {
                    Description = "租户 Id 不能为空",
                },
            }));
        if (string.IsNullOrWhiteSpace(role.Name))
            return new(IdentityResult.Failed(new IdentityError[]
            {
                new IdentityError
                {
                    Description = "角色名称不能为空或空白字符",
                },
            }));

        return new(IdentityResult.Success);
    }

    /// <summary>
    /// 规范化更新角色名称
    /// </summary>
    public virtual ValueTask UpdateNormalizedRoleNameAsync(SysRole role)
    {
        role.NormalizedName = NormalizeKey(role.Name);
        return ValueTask.CompletedTask;
    }

    /// <summary>
    /// 更新角色
    /// </summary>
    public Task<IdentityResult> UpdateAsync(SysRole role)
    {
        ThrowIfDisposed();
        return UpdateRoleAsync(role);
    }

    /// <summary>
    /// 删除角色
    /// </summary>
    public Task<IdentityResult> DeleteAsync(SysRole role)
    {
        ThrowIfDisposed();
        role.SoftDeleted = true;
        return UpdateRoleAsync(role);
    }

    /// <summary>
    /// 更新角色信息
    /// </summary>
    protected virtual async Task<IdentityResult> UpdateRoleAsync(SysRole role)
    {
        var result = await ValidateRoleAsync(role);
        if (!result.Succeeded)
            return result;
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
/// ASP.NET 应用程序中进行角色管理
/// </summary>
/// <typeparam name="TDbContext"></typeparam>
public class AspNetRoleManager<TDbContext>(IHttpContextAccessor contextAccessor, TDbContext db, ILookupNormalizer lookupNormalizer, IdentityErrorDescriber errors) : RoleManagerImpl<TDbContext>(db, lookupNormalizer, errors) where TDbContext : ApplicationDbContextBase
{
    /// <summary>
    /// 请求上下文的访问器
    /// </summary>
    protected readonly IHttpContextAccessor contextAccessor = contextAccessor;

    /// <summary>
    /// 获取请求的取消令牌
    /// </summary>
    protected override CancellationToken CancellationToken => contextAccessor?.HttpContext?.RequestAborted ?? CancellationToken.None;
}
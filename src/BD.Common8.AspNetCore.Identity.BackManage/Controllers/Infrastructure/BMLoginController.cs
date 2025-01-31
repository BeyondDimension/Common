namespace BD.Common8.AspNetCore.Controllers.Infrastructure;

/// <summary>
/// 后台管理系统用户登录
/// </summary>
/// <remarks>
/// Initializes a new instance of the <see cref="BMLoginController"/> class.
/// </remarks>
/// <param name="userManager"></param>
/// <param name="jwtValueProvider"></param>
/// <param name="db"></param>
/// <param name="optionsAccessor"></param>
/// <param name="cache"></param>
/// <param name="logger"></param>
[Route("bm/login")]
public sealed class BMLoginController(
    IUserManager userManager,
    IJWTValueProvider jwtValueProvider,
    IBMDbContextBase db,
    IOptions<IdentityOptions> optionsAccessor,
    IDistributedCache cache,
    ILogger<BMLoginController> logger) : AllowAnonymousApiController<BMLoginController>(logger)
{
    readonly IUserManager userManager = userManager;
    readonly IJWTValueProvider jwtValueProvider = jwtValueProvider;
    readonly IBMDbContextBase db = db;
    readonly IdentityOptions options = optionsAccessor?.Value ?? new();
    readonly IDistributedCache cache = cache;
    const string ResponseDataUserNameNotFoundOrPasswordInvalid = "错误：用户名不存在或密码错误";
    const int MaxIpAccessFailedCount = 10;

    /// <summary>
    /// 后台用户登录
    /// </summary>
    /// <param name="args">加密了的用户名与密码</param>
    /// <returns></returns>
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [HttpPost]
    public async Task<ActionResult<ApiResponse<JWTEntity>>> Post([FromBody] string[] args)
    {
        if (args.Length != 2) return BadRequest();

        var ip = HttpContext.Connection.RemoteIpAddress?.ToString();
        if (string.IsNullOrWhiteSpace(ip)) return Fail("未知的 IP 地址");

        var ipCacheKey = $"WTTS_BM_Login_Ip_[{ip}]";
        var ipAccessFailedCount = await cache.GetV2Async<int>(ipCacheKey, HttpContext.RequestAborted);
        if (ipAccessFailedCount >= MaxIpAccessFailedCount)
            return StatusCode((int)HttpStatusCode.TooManyRequests);

        var userName = ServerSecurity.Decrypt(args[0]);
        var user = await userManager.FindByNameAsync(userName);
        if (user == null) return Fail(ResponseDataUserNameNotFoundOrPasswordInvalid);

        var isLocked = await userManager.IsLockedOutAsync(user);
        if (isLocked) return Fail("该账号已被锁定");

        var password = ServerSecurity.Decrypt(args[1]);
        if (!await userManager.CheckPasswordAsync(user, password))
        {
            user.AccessFailedCount++;
            await userManager.UpdateAsync(user);
            var lockoutEnd = DateTimeOffset.Now + options.Lockout.DefaultLockoutTimeSpan.Duration();
            await cache.SetV2Async(ipCacheKey, ipAccessFailedCount + 1,
                new DistributedCacheEntryOptions { AbsoluteExpiration = lockoutEnd });
            if (user.AccessFailedCount + 1 >= options.Lockout.MaxFailedAccessAttempts)
            {
                var lockout = await userManager.SetLockoutEndDateAsync(user, lockoutEnd);
                if (!lockout.Succeeded) return Fail(lockout);
                await ClearAccessFailedCount();
            }
            return Fail(ResponseDataUserNameNotFoundOrPasswordInvalid);
        }

        await ClearAccessFailedCount();
        var token = await GenerateTokenAsync(user);
        return Ok(token);

        async Task ClearAccessFailedCount()
        {
            if (user.AccessFailedCount != 0)
            {
                user.AccessFailedCount = 0;
                await userManager.UpdateAsync(user);
            }
        }
    }

    /// <summary>
    /// 刷新 JWT
    /// </summary>
    /// <param name="refresh_token"></param>
    /// <returns></returns>
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    [HttpPut("{refresh_token}")]
    public async Task<ActionResult<ApiResponse<JWTEntity?>>> Put([FromRoute] string refresh_token)
    {
        var user = await ((IBMDbContext)db).SysUsers.IgnoreQueryFilters().SingleOrDefaultAsync(x => x.RefreshToken == refresh_token, HttpContext.RequestAborted);
        if (user == null) return NotFound();

        var now = DateTimeOffset.Now;
        if (now >= user.NotBefore && user.RefreshExpiration >= now)
        {
            var token = await GenerateTokenAsync(user);
            return Ok(token);
        }

        return Unauthorized();
    }

    [NonAction]
    async Task<JWTEntity?> GenerateTokenAsync(BMUser user)
    {
        IEnumerable<string>? roles = await userManager.GetRolesAsync(user);

        var token = await jwtValueProvider.GenerateTokenAsync(user.Id, roles, aciton: (list) =>
        {
            list.Add(new Claim(nameof(ITenant.TenantId), user.TenantId.ToString()));
        }, HttpContext.RequestAborted);

        return token;
    }
}
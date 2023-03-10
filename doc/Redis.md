### Redis/分布式缓存

[ASP.NET Core 中的分布式缓存](https://learn.microsoft.com/zh-cn/aspnet/core/performance/caching/distributed)
使用接口 [IDistributedCache](https://learn.microsoft.com/zh-cn/dotnet/api/microsoft.extensions.caching.distributed.idistributedcache) 该接口的实现有三种可选方案  
之前用过 SqlServer 的版本所以 EF 的文档中有相关的命令行记录，但现在通常使用 Redis

- 若要使用 SQL Server 分布式缓存，请添加对 Microsoft.Extensions.Caching.SqlServer 包的包引用。
- 若要使用 Redis 分布式缓存，请添加对 Microsoft.Extensions.Caching.StackExchangeRedis 包的包引用。
- 若要使用 NCache 分布式缓存，请添加对 NCache.Microsoft.Extensions.Caching.OpenSource 包的包引用。

在我们项目中封装了一个函数 ```AddStackExchangeRedisCache```  
从 ```appsettings.json``` 中 ```ConnectionStrings``` 中读取 ```RedisConnection``` 写上连接字符串 
通过构造函数注入类型 
- ```IDistributedCache```(此类型为通用的抽象类型仅提供简单的键值对存储) 
- ```StackExchange.Redis.IConnectionMultiplexer```(原始 Redis SDK 连接，通常用于需要使用更高级的功能)

自定义数据类型建立模型类，推荐使用 C# 9.0 中新增的 [record](https://learn.microsoft.com/zh-cn/dotnet/csharp/language-reference/builtin-types/record) 类型  
- 对于**可读性要求**高的，就使用[序列化与反序列化文档](./Serializable-And-Deserialize.md)中的 System.Text.Json 将模型类序列化成 Json 字符串存入  
- 对于**性能**要求高的，就使用[序列化与反序列化文档](./Serializable-And-Deserialize.md)中的 **Memory**Pack 将模型类序列化成 Byte[] 存入缓存中  

在[通用库 BD.Common.AspNetCore 中的扩展](https://github.com/BeyondDimension/Common/blob/1.23.10309.12315/src/BD.Common.AspNetCore/Extensions/CacheExtensions.cs)  
- GetAsync/SetAsync 使用的是 **Message**Pack
- GetV2Async/SetV2Async 使用的是 **Memory**Pack

#### 用例
```
/// <summary>
/// 网络加速
/// </summary>
public sealed class AccelerateController : ApiController.AllowAnonymous
{
    readonly IDistributedCache cache;
    ...
    public AccelerateController(
        ...
        IDistributedCache cache,
        ILogger<AccelerateController> logger) : base(logger)
    {
        ...
        this.cache = cache;
    }

    /// <summary>
    /// 获取所有加速项目组数据
    /// </summary>
    /// <returns></returns>
    [HttpGet("[action]")]
    public async Task<ApiRsp<AccelerateProjectGroupDTO[]?>> All()
    {
        ...
        // 从缓存中取出所有加速项目组数据，赞助用户与非赞助用户的 key 不同
        result = await cache.GetAsync<AccelerateProjectGroupDTO[]>(key, HttpContext.RequestAborted);
        return ApiRspHelper.Ok(result ?? Array.Empty<AccelerateProjectGroupDTO>());
    }
```

```
/// <summary>
/// 平台登录
/// </summary>
public sealed class LoginController : AllowAnonymousApiController<LoginController>
{
    readonly IDistributedCache cache;
    ...
    public LoginController(
        ...
        IDistributedCache cache,
        ILogger<LoginController> logger) : base(logger)
    {
        ...
        this.cache = cache;
    }

    /// <summary>
    /// 管理后台登录
    /// </summary>
    /// <param name="args">加密了的用户名与密码</param>
    /// <returns></returns>
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [HttpPost]
    public async Task<ActionResult<ApiResponse<JWTEntity>>> Post([FromBody] string[] args)
    {
        // 从缓存中取出当前客户端 IP 登录失败次数，当超过最大限制时返回状态码 TooManyRequests
        var ipAccessFailedCount = await cache.GetAsync<int>(ipCacheKey, HttpContext.RequestAborted);
        if (ipAccessFailedCount >= MaxIpAccessFailedCount)
            return StatusCode((int)HttpStatusCode.TooManyRequests);
        ...
        // 登录失败时，增加一个计数，并且设置缓存到期时间
        var lockoutEnd = DateTimeOffset.Now + options.Lockout.DefaultLockoutTimeSpan.Duration();
        await cache.SetAsync(ipCacheKey, ipAccessFailedCount + 1,
            new DistributedCacheEntryOptions { AbsoluteExpiration = lockoutEnd });
    }
}
```

```
/// <summary>
/// 广告
/// </summary>
[RequiredSecurityKey]
[Route("api/[controller]/[action]")]
public sealed class AdvertisementController : ApiController.AllowAnonymous
{
    readonly IDistributedCache cache;
    readonly IConnectionMultiplexer connection;

    public AdvertisementController(
        IDistributedCache cache,
        IConnectionMultiplexer connection,
        ILogger<AdvertisementController> logger) : base(logger)
    {
        this.cache = cache;
        this.connection = connection;
    }

    /// <summary>
    /// 获取广告列表
    /// </summary>
    /// <param name="type">广告类型</param>
    /// <param name="platform"></param>
    /// <param name="deviceIdiom"></param>
    /// <returns></returns>
    [HttpGet("{platform}/{deviceIdiom}")]
    public async Task<ApiRsp<AdvertisementDTO[]?>> All(
        [FromQuery] AdvertisementType? type,
        [FromRoute] Platform platform = Platform.Windows,
        [FromRoute] DeviceIdiom deviceIdiom = DeviceIdiom.Desktop)
    {
        var info = await cache.GetAsync<AdvertisementCacheDTO[]>(CacheKeys.AdvertisementSponsorCacheKey);
        if (info != null)
        {
            var r = info.Where(x => type.HasValue ? x.Type == type : true &&
                x.Platform.HasFlag(platform) &&
                x.DeviceIdiom.HasFlag(deviceIdiom))
                .Select(x => new AdvertisementDTO
                {
                    Id = x.Id,
                    Order = x.Order,
                    Type = x.Type,
                    Standard = x.Standard,
                    Remark = x.Remark,
                }).ToArray();
            return r;
        }
        return Array.Empty<AdvertisementDTO>();
    }

    /// <summary>
    /// 访问广告图片
    /// </summary>
    /// <param name="id">广告 Id</param>
    /// <returns>302 跳转到广告图</returns>
    [HttpGet("{id?}")]
    public async Task<IActionResult> Images([FromRoute] Guid? id)
    {
        if (id.HasValue)
        {
            var info = await cache.GetAsync<AdvertisementCacheDTO>(id.Value.ToString());
            if (info != null)
            {
                var dbConnection = connection.GetDatabase(1, HttpContext.RequestAborted);
                await dbConnection.HashIncrementAsync(CacheKeys.AdvertisementImagesHashKey, id.Value.ToString());
                return Redirect(info!.Url);
            }
        }
        return Redirect("https://steampp.net/logo.svg");
    }

    /// <summary>
    /// 访问广告连接
    /// </summary>
    /// <param name="id">广告 Id</param>
    /// <returns>302 跳转到地址</returns>
    [HttpGet("{id?}")]
    public async Task<IActionResult> Jump([FromRoute] Guid? id)
    {
        if (id.HasValue)
        {
            var info = await cache.GetAsync<AdvertisementCacheDTO>(id.Value.ToString());
            if (info != null)
            {
                var dbConnection = connection.GetDatabase(1, HttpContext.RequestAborted);
                await dbConnection.HashIncrementAsync(CacheKeys.AdvertisementJumpHashKey, id.Value.ToString());
                return Redirect(info!.JumpUrl);
            }
        }
        return Redirect("https://steampp.net");
    }
}
```
namespace Microsoft.Extensions.DependencyInjection;

public static partial class ServiceCollectionExtensions
{
    /// <summary>
    /// 添加由 Redis 实现的分布式缓存
    /// </summary>
    /// <param name="builder"></param>
    /// <param name="instanceName"></param>
    /// <param name="connectionStringKeyName"></param>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static void AddStackExchangeRedisCache(
        this WebApplicationBuilder builder,
        string instanceName,
        string connectionStringKeyName = "RedisConnection")
    {
        // 添加分布式缓存
        // https://learn.microsoft.com/zh-cn/aspnet/core/performance/caching/distributed
        var redisConnection = builder.Configuration.GetConnectionString(connectionStringKeyName);
        if (!string.IsNullOrWhiteSpace(redisConnection))
        {
            builder.Services.AddStackExchangeRedisCache(options =>
            {
                options.Configuration = redisConnection;
                options.InstanceName = instanceName;
            }).AddConnectionMultiplexer(redisConnection);
        }
        else
        {
            if (Environment.UserInteractive)
                builder.Services.AddDistributedMemoryCache();
        }
    }

    /// <summary>
    /// 添加 ConnectionMultiplexer 实例到 <see cref="IServiceCollection"/>中
    /// </summary>
    /// <param name="services">IServiceCollection 实例</param>
    /// <param name="redisConnection">Redis 连接字符串</param>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    static void AddConnectionMultiplexer(this IServiceCollection services, string redisConnection)
    {
        try
        {
            // https://docs.redis.com/latest/rs/references/client_references/client_csharp/#aspnet-core
            var multiplexer = ConnectionMultiplexer.Connect(redisConnection);
            services.AddSingleton<IConnectionMultiplexer>(multiplexer);
        }
        catch
        {
            if (Assembly.GetEntryAssembly()?.GetName()?.Name == "GetDocument.Insider")
            {
                return;
            }
            throw;
        }
    }
}
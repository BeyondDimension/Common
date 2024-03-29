namespace Microsoft.Extensions.DependencyInjection;

public static partial class ServiceCollectionExtensions
{
    /// <summary>
    /// 尝试添加 <see cref="IHttpPlatformHelperService"/> 的实现
    /// </summary>
    /// <param name="services"></param>
    /// <param name="userAgent"></param>
    /// <returns></returns>
    public static IServiceCollection TryAddHttpPlatformHelper(
        this IServiceCollection services,
        string? userAgent = null)
    {
        if (userAgent != null)
            HttpPlatformHelperImpl.PresetUserAgent = userAgent;
        services.TryAddSingleton<IHttpPlatformHelperService, HttpPlatformHelperImpl>();
        return services;
    }

    sealed class HttpPlatformHelperImpl : HttpPlatformHelperService
    {
        public static string? PresetUserAgent { get; internal set; }

        public override string UserAgent => PresetUserAgent ?? DefaultUserAgent;
    }
}
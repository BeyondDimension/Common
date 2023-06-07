// ReSharper disable once CheckNamespace
namespace Microsoft.Extensions.DependencyInjection;

public static partial class ServiceCollectionExtensions
{
    public static IServiceCollection TryAddHttpPlatformHelper(
        this IServiceCollection services,
        string? userAgent = null)
    {
        if (userAgent != null) HttpPlatformHelperImpl.DefaultUserAgent = userAgent;
        services.TryAddSingleton<IHttpPlatformHelperService, HttpPlatformHelperImpl>();
        return services;
    }

    sealed class HttpPlatformHelperImpl : HttpPlatformHelperService
    {
        public static new string DefaultUserAgent { get; internal set; } = "Mozilla/5.0 (Windows NT 10.0; Win64; x64) AppleWebKit/537.36 (KHTML, like Gecko) Chrome/90.0.4430.93 Safari/537.36 Edg/90.0.818.51";

        public override string UserAgent => DefaultUserAgent;
    }
}
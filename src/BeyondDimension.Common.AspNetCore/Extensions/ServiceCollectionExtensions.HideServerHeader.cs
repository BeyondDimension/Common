// ReSharper disable once CheckNamespace
namespace Microsoft.Extensions.DependencyInjection;

public static partial class ServiceCollectionExtensions
{
    /// <summary>
    /// 隐藏服务响应头
    /// </summary>
    /// <param name="services"></param>
    /// <returns></returns>
    public static IServiceCollection HideServerHeader(this IServiceCollection services)
    {
        services.Configure<KestrelServerOptions>(options =>
        {
            options.AddServerHeader = false;
        });
        return services;
    }
}
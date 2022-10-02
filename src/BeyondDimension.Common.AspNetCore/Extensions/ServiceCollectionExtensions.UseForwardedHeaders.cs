// ReSharper disable once CheckNamespace
namespace Microsoft.Extensions.DependencyInjection;

public static partial class ServiceCollectionExtensions
{
    public static IApplicationBuilder UseForwardedHeaders<TAppSettings>(this IApplicationBuilder builder, TAppSettings appSettings)
        where TAppSettings : class, INotUseForwardedHeaders
    {
        if (!appSettings.NotUseForwardedHeaders)
        {
            // https://docs.microsoft.com/zh-cn/aspnet/core/host-and-deploy/linux-nginx#use-a-reverse-proxy-server
            // Nginx 反向代理需要的配置，如果不适用反向代理部署则注释下面这行
            builder.UseForwardedHeaders(new ForwardedHeadersOptions
            {
                ForwardedHeaders = ForwardedHeaders.XForwardedFor | ForwardedHeaders.XForwardedProto,
            });
        }
        return builder;
    }
}

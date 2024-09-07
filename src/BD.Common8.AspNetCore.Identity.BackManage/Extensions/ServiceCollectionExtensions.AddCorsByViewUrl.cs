namespace BD.Common8.AspNetCore.Extensions;

public static partial class ServiceCollectionExtensions
{
    /// <summary>
    /// 配置允许跨域访问的 Web UI 地址
    /// </summary>
    /// <param name="services"></param>
    /// <param name="appSettings"></param>
    public static void AddCorsByViewUrl(this IServiceCollection services, IViewsUrl appSettings)
    {
        if (!string.IsNullOrWhiteSpace(appSettings.ViewsUrl))
        {
            var origins = appSettings.ViewsUrl.Split([',', ';', '|', '，', '；'], StringSplitOptions.RemoveEmptyEntries).Where(x => String2.IsHttpUrl(x)).ToArray();
            if (origins.Length != 0)
            {
                appSettings.UseCors = true;
                services.AddCors(options =>
                {
                    options.AddDefaultPolicy(
                        builder => builder.WithOrigins(origins).AllowCredentials().AllowAnyMethod().AllowAnyHeader());
                });
            }
        }
    }
}
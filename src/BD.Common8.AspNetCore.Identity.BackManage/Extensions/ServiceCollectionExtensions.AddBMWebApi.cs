using Microsoft.Extensions.WebEncoders;

namespace Microsoft.Extensions.DependencyInjection;

public static partial class ServiceCollectionExtensions
{
    /// <summary>
    /// 添加后台管理系统的 WebApi
    /// </summary>
    /// <typeparam name="TBMAppSettings"></typeparam>
    /// <param name="services"></param>
    /// <param name="appSettings"></param>
    /// <param name="configureApplicationPartManager"></param>
    public static unsafe void AddBMWebApi<TBMAppSettings>(this IServiceCollection services,
        TBMAppSettings appSettings,
        delegate* managed<ApplicationPartManager, void> configureApplicationPartManager = default)
        where TBMAppSettings : BMAppSettings
    {
        services.AddResponseCompression(options =>
        {
            options.EnableForHttps = true;
        });

        services.AddRouting();
        services.AddControllers(options =>
        {
            options.Filters.Add<UserIsLockedOutFilterAttribute>();
        }).ConfigureApplicationPartManager(apm =>
        {
            apm.ApplicationParts.Add(
                new AssemblyPart(typeof(InfoController).GetTypeInfo().Assembly));
            if (configureApplicationPartManager != default)
                configureApplicationPartManager(apm);
        });
        //services.AddControllers().AddNewtonsoftJson(options =>
        //{
        //    options.SerializerSettings.ContractResolver = new IgnoreJsonPropertyContractResolver(useCamelCase: true);
        //    //options.JsonSerializerOptions.Encoder = JavaScriptEncoder.UnsafeRelaxedJsonEscaping;
        //});
        services.Configure<WebEncoderOptions>(options =>
        {
            options.TextEncoderSettings = new TextEncoderSettings(UnicodeRanges.All);
        });
        services.ConfigureApiBehaviorOptionsInvalidModelStateResponseFactory();
        if (!string.IsNullOrWhiteSpace(appSettings.ViewsUrl))
        {
            var origins = appSettings.ViewsUrl.Split([',', ';'], StringSplitOptions.RemoveEmptyEntries).Where(x => String2.IsHttpUrl(x)).ToArray();
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
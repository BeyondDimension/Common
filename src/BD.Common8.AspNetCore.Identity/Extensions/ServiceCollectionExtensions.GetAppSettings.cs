namespace BD.Common8.AspNetCore.Extensions;

public static partial class ServiceCollectionExtensions
{
    /// <summary>
    /// 获取 AppSettings
    /// </summary>
    /// <typeparam name="TAppSettings"></typeparam>
    /// <param name="builder"></param>
    /// <returns></returns>
    public static TAppSettings GetAppSettings<TAppSettings>(this WebApplicationBuilder builder) where TAppSettings : class
    {
        var appSettings_ = builder.Configuration.GetSection("AppSettings");
#pragma warning disable IL2026 // Members annotated with 'RequiresUnreferencedCodeAttribute' require dynamic access otherwise can break functionality when trimming application code
        builder.Services.Configure<TAppSettings>(appSettings_);
        var appSettings = appSettings_.Get<TAppSettings>();
        appSettings.ThrowIsNull();
#pragma warning restore IL2026 // Members annotated with 'RequiresUnreferencedCodeAttribute' require dynamic access otherwise can break functionality when trimming application code
        return appSettings;
    }
}
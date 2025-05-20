using Microsoft.AspNetCore.Builder;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using System.Diagnostics.CodeAnalysis;
using System.Extensions;

namespace BD.Common8.AspNetCore.Extensions;

public static partial class ServiceCollectionExtensions
{
    /// <summary>
    /// 获取 AppSettings
    /// </summary>
    /// <typeparam name="TAppSettings"></typeparam>
    /// <param name="builder"></param>
    /// <returns></returns>
    public static TAppSettings GetAppSettings<[DynamicallyAccessedMembers(DynamicallyAccessedMemberTypes.All)] TAppSettings>(this WebApplicationBuilder builder) where TAppSettings : class
    {
        var appSettings_ = builder.Configuration.GetSection("AppSettings");
        builder.Services.Configure<TAppSettings>(appSettings_);
        var appSettings = appSettings_.Get<TAppSettings>();
        appSettings.ThrowIsNull();
        return appSettings;
    }
}
using BD.Common.Services.Implementation.Essentials;
using CommonEssentials = BD.Common.CommonEssentials;
#if ANDROID
using DeviceInfoPlatformServiceImpl_ = BD.Common.Services.Implementation.Essentials.AndroidDeviceInfoPlatformServiceImpl;
#else
using DeviceInfoPlatformServiceImpl_ = BD.Common.Services.Implementation.Essentials.DefaultDeviceInfoPlatformServiceImpl;
#endif

// ReSharper disable once CheckNamespace
namespace Microsoft.Extensions.DependencyInjection;

public static class ServiceCollectionExtensions
{
    /// <summary>
    /// 添加 Essentials
    /// </summary>
    /// <typeparam name="TApplicationVersionServiceImpl"></typeparam>
    /// <param name="services"></param>
    /// <returns></returns>
    public static IServiceCollection TryAddEssentials<TApplicationVersionServiceImpl>(this IServiceCollection services) where TApplicationVersionServiceImpl : class, IApplicationVersionService
    {
        CommonEssentials.IsSupported = true;
        services.TryAddSingleton<IApplicationVersionService, TApplicationVersionServiceImpl>();
        services.TryAddSingleton<IBrowserPlatformService, BrowserPlatformServiceImpl>();
        services.TryAddSingleton<IClipboardPlatformService, ClipboardPlatformServiceImpl>();
        services.TryAddSingleton<IConnectivityPlatformService, ConnectivityPlatformServiceImpl>();
        services.TryAddSingleton<IDeviceInfoPlatformService, DeviceInfoPlatformServiceImpl_>();
        services.TryAddSingleton<IEmailPlatformService, EmailPlatformServiceImpl>();
        services.TryAddSingleton<IFilePickerPlatformService, FilePickerPlatformServiceImpl>();
#if !MAUI_RUNTIME
        services.TryAddSingleton<IMainThreadPlatformService, MainThreadPlatformServiceImpl>();
#endif
        services.TryAddSingleton<IPermissionsPlatformService, PermissionsPlatformServiceImpl>();
        services.TryAddSingleton<IPreferencesPlatformService, PreferencesPlatformServiceImpl>();
        services.TryAddSingleton(s => s.GetRequiredService<IFilePickerPlatformService>().OpenFileDialogService);
        services.TryAddSingleton(s => s.GetRequiredService<IFilePickerPlatformService>().SaveFileDialogService);
        services.TryAddSingleton<IInternalFilePickerPlatformService>(s => s.GetRequiredService<IFilePickerPlatformService>());
        return services;
    }

    public static IServiceCollection AddDeviceInfoPlatformService<TService>(this IServiceCollection services) where TService : DeviceInfoPlatformServiceImpl
    {
        services.AddSingleton<IDeviceInfoPlatformService, TService>();
        return services;
    }

    public static IServiceCollection AddPermissionsPlatformServiceImpl<TService>(this IServiceCollection services) where TService : PermissionsPlatformServiceImpl
    {
        services.AddSingleton<IPermissionsPlatformService, TService>();
        return services;
    }

    /// <summary>
    /// 添加由 Xamarin.Essentials.SecureStorage 实现的 <see cref="ISecureStorage"/>
    /// </summary>
    /// <param name="services"></param>
    /// <returns></returns>
    public static IServiceCollection TryAddEssentialsSecureStorage(this IServiceCollection services)
    {
        services.TryAddSingleton<ISecureStorage, EssentialsSecureStorage>();
        return services;
    }
}
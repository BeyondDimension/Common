#pragma warning disable IDE0130 // 命名空间与文件夹结构不匹配
namespace Microsoft.Extensions.DependencyInjection;

public static partial class ServiceCollectionExtensions
{
    /// <summary>
    /// 添加 Essentials，使用 TryAddXYZ 在此函数之前添加可覆盖接口默认实现
    /// </summary>
    /// <typeparam name="TApplicationVersionServiceImpl"></typeparam>
    /// <typeparam name="TFilePickerPlatformServiceImpl"></typeparam>
    /// <typeparam name="TMainThreadPlatformServiceImpl"></typeparam>
    /// <param name="services"></param>
    /// <returns></returns>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static IServiceCollection TryAddEssentials<
        [DynamicallyAccessedMembers(DynamicallyAccessedMemberTypes.PublicConstructors)] TApplicationVersionServiceImpl,
        [DynamicallyAccessedMembers(DynamicallyAccessedMemberTypes.PublicConstructors)] TFilePickerPlatformServiceImpl,
        [DynamicallyAccessedMembers(DynamicallyAccessedMemberTypes.PublicConstructors)] TMainThreadPlatformServiceImpl>(
        this IServiceCollection services)
        where TApplicationVersionServiceImpl : class, IApplicationVersionService
        where TFilePickerPlatformServiceImpl : class, IFilePickerPlatformService
        where TMainThreadPlatformServiceImpl : class, IMainThreadPlatformService
    {
        services.TryAddSingleton<IApplicationVersionService, TApplicationVersionServiceImpl>();
        services.TryAddSingleton<IBrowserPlatformService, BrowserPlatformServiceImpl>();
        services.TryAddSingleton<IClipboardPlatformService, ClipboardPlatformServiceImpl>();
        services.TryAddSingleton<IConnectivityPlatformService, ConnectivityPlatformServiceImpl>();
        services.TryAddSingleton<IDeviceInfoPlatformService, DeviceInfoPlatformServiceImpl>();
        //services.TryAddSingleton<IEmailPlatformService, EmailPlatformServiceImpl>();
#pragma warning disable IDE0079 // 请删除不必要的忽略
#pragma warning disable CA1416 // 验证平台兼容性
        services.AddSingleton<IPermissionsPlatformService, PermissionsPlatformServiceImpl>();
#pragma warning restore CA1416 // 验证平台兼容性
#pragma warning restore IDE0079 // 请删除不必要的忽略
        //services.TryAddSingleton<IPreferencesPlatformService, PreferencesPlatformServiceImpl>();
        services.TryAddSingleton<IFilePickerPlatformService, TFilePickerPlatformServiceImpl>();
        services.TryAddSingleton(s => s.GetRequiredService<IFilePickerPlatformService>().OpenFileDialogService);
        services.TryAddSingleton(s => s.GetRequiredService<IFilePickerPlatformService>().SaveFileDialogService);
        services.TryAddSingleton<IPresetFilePickerPlatformService>(s => s.GetRequiredService<IFilePickerPlatformService>());
        services.TryAddSingleton<IMainThreadPlatformService, TMainThreadPlatformServiceImpl>();
        return services;
    }
}
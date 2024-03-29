namespace Microsoft.Extensions.DependencyInjection;

public static partial class ServiceCollectionExtensions
{
    /// <summary>
    /// 添加 Essentials，使用 TryAddXYZ 在此函数之前添加可覆盖接口默认实现
    /// </summary>
    /// <typeparam name="TApplicationVersionServiceImpl"></typeparam>
    /// <param name="services"></param>
    /// <returns></returns>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static IServiceCollection TryAddEssentials<
        [DynamicallyAccessedMembers(DynamicallyAccessedMemberTypes.PublicConstructors)] TApplicationVersionServiceImpl>(
        this IServiceCollection services)
        where TApplicationVersionServiceImpl : class, IApplicationVersionService
    {
        if (OperatingSystem.IsLinux())
        {
            services.AddSingleton<IClipboardPlatformService, AvaloniaClipboardPlatformServiceImpl>();
        }
        services.TryAddEssentials<TApplicationVersionServiceImpl,
            AvaloniaFilePickerPlatformServiceImpl,
            AvaloniaMainThreadPlatformServiceImpl>();
        return services;
    }
}
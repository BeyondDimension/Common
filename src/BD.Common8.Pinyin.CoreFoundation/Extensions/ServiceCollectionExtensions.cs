namespace Microsoft.Extensions.DependencyInjection;

public static partial class ServiceCollectionExtensions
{
    /// <summary>
    /// 添加使用 <see cref="CoreFoundation.CFStringTransform"/> 实现的拼音功能
    /// </summary>
    /// <param name="services"></param>
    /// <returns></returns>
    [SupportedOSPlatform("ios")]
    [SupportedOSPlatform("maccatalyst")]
    [SupportedOSPlatform("macos")]
    [SupportedOSPlatform("tvos")]
    public static IServiceCollection AddPinyinCFStringTransform(this IServiceCollection services)
    {
        services.AddSingleton<IPinyin, CoreFoundationPinyinImpl>();
        return services;
    }

    /// <inheritdoc cref="AddPinyinCFStringTransform(IServiceCollection)"/>
    [SupportedOSPlatform("ios")]
    [SupportedOSPlatform("maccatalyst")]
    [SupportedOSPlatform("macos")]
    [SupportedOSPlatform("tvos")]
    public static IServiceCollection AddPinyin(this IServiceCollection services) => services.AddPinyinCFStringTransform();
}
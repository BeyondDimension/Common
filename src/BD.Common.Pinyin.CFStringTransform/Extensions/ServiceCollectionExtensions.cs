using CoreFoundation;
using System.Runtime.Versioning;

// ReSharper disable once CheckNamespace
namespace Microsoft.Extensions.DependencyInjection;

public static class ServiceCollectionExtensions
{
    /// <summary>
    /// 添加使用 <see cref="CFStringTransform"/> 实现的拼音功能
    /// </summary>
    /// <param name="services"></param>
    /// <returns></returns>
    [SupportedOSPlatform("ios")]
    [SupportedOSPlatform("maccatalyst")]
    [SupportedOSPlatform("macos")]
    [SupportedOSPlatform("tvos")]
    public static IServiceCollection AddPinyinCFStringTransform(this IServiceCollection services)
    {
        services.AddSingleton<IPinyin, PinyinImpl>();
        return services;
    }

    /// <inheritdoc cref="AddPinyinCFStringTransform(IServiceCollection)"/>
    [SupportedOSPlatform("ios")]
    [SupportedOSPlatform("maccatalyst")]
    [SupportedOSPlatform("macos")]
    [SupportedOSPlatform("tvos")]
    public static IServiceCollection AddPinyin(this IServiceCollection services) => services.AddPinyinCFStringTransform();
}
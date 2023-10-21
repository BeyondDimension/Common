using BD.Common8.Pinyin.Services.Implementation;

#pragma warning disable IDE0130 // 命名空间与文件夹结构不匹配
namespace Microsoft.Extensions.DependencyInjection;

public static partial class ServiceCollectionExtensions
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
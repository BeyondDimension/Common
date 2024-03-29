namespace Microsoft.Extensions.DependencyInjection;

public static partial class ServiceCollectionExtensions
{
    /// <summary>
    /// 添加使用 Microsoft Visual Studio International Pack 1.0 中的 Simplified Chinese Pin-Yin Conversion Library（简体中文拼音转换类库）实现的拼音功能
    /// </summary>
    /// <param name="services"></param>
    /// <returns></returns>
    public static IServiceCollection AddPinyinChnCharInfo(this IServiceCollection services)
    {
        services.AddSingleton<IPinyin, ChnCharInfoPinyinImpl>();
        return services;
    }

    /// <inheritdoc cref="AddPinyinChnCharInfo(IServiceCollection)"/>
    public static IServiceCollection AddPinyin(this IServiceCollection services) => services.AddPinyinChnCharInfo();
}
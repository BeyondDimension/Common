using TinyPinyin;
#if MONOANDROID || ANDROID
using AndroidAppApplication = Android.App.Application;
#endif

// ReSharper disable once CheckNamespace
namespace Microsoft.Extensions.DependencyInjection;

public static class ServiceCollectionExtensions
{
    /// <summary>
    /// 添加使用 <see cref="PinyinHelper"/>(https://github.com/promeG/TinyPinyin) or (https://github.com/hueifeng/TinyPinyin.Net) 实现的拼音功能
    /// </summary>
    /// <param name="services"></param>
    /// <returns></returns>
    public static IServiceCollection AddTinyPinyin(this IServiceCollection services)
    {
#if MONOANDROID || ANDROID
        PinyinImpl.InitWithCnCityDict(AndroidAppApplication.Context);
#endif
        services.AddSingleton<IPinyin, PinyinImpl>();
        return services;
    }

    /// <inheritdoc cref="AddTinyPinyin(IServiceCollection)"/>
    public static IServiceCollection AddPinyin(this IServiceCollection services) => services.AddTinyPinyin();
}
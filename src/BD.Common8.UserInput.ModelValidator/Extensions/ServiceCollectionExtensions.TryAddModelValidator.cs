#pragma warning disable IDE0130 // 命名空间与文件夹结构不匹配
namespace Microsoft.Extensions.DependencyInjection;

public static partial class ServiceCollectionExtensions
{
    /// <summary>
    /// 尝试添加模型验证
    /// </summary>
    /// <param name="services"></param>
    /// <returns></returns>
    public static IServiceCollection TryAddModelValidator(this IServiceCollection services)
    {
        services.TryAddSingleton<IModelValidator, ModelValidator>();
        return services;
    }
}


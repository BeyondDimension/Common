using BD.Common8.UserInput.ModelValidator.Services;
using BD.Common8.UserInput.ModelValidator.Services.Implementation;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;

namespace BD.Common8.UserInput.ModelValidator.Extensions;

public static partial class ServiceCollectionExtensions
{
    /// <summary>
    /// 尝试添加模型验证
    /// </summary>
    /// <param name="services"></param>
    /// <returns></returns>
    public static IServiceCollection TryAddModelValidator(this IServiceCollection services)
    {
        services.TryAddSingleton<IModelValidator, ModelValidatorImpl>();
        return services;
    }
}


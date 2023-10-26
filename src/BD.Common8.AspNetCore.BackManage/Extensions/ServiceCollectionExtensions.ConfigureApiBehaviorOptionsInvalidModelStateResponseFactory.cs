#pragma warning disable IDE0130 // 命名空间与文件夹结构不匹配
namespace Microsoft.Extensions.DependencyInjection;

public static partial class ServiceCollectionExtensions
{
    static OkObjectResult InvalidModelStateResponseFactory(ActionContext ctx) => new(new ApiResponse
    {
        IsSuccess = false,
        Messages = (from x in ctx.ModelState
                    let errorMessage = x.Value?.Errors.Select(x => x.ErrorMessage).FirstOrDefault()
                    where !string.IsNullOrEmpty(errorMessage)
                    select errorMessage).ToArray(),
    });

    /// <summary>
    /// 配置 <see cref="ApiBehaviorOptions.InvalidModelStateResponseFactory"/> 返回类型为 <see cref="ApiResponse"/>
    /// </summary>
    /// <param name="services"></param>
    /// <param name="action"></param>
    /// <returns></returns>
    public static IServiceCollection ConfigureApiBehaviorOptionsInvalidModelStateResponseFactory(this IServiceCollection services, Action<ApiBehaviorOptions>? action = null) => services.Configure<ApiBehaviorOptions>(options =>
    {
        options.InvalidModelStateResponseFactory = InvalidModelStateResponseFactory;
        action?.Invoke(options);
    });
}
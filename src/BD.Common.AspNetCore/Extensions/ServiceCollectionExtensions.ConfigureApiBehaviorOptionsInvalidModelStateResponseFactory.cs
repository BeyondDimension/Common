// ReSharper disable once CheckNamespace
namespace Microsoft.Extensions.DependencyInjection;

#if !BLAZOR
public static partial class ServiceCollectionExtensions
{
    static IActionResult InvalidModelStateResponseFactory(ActionContext ctx) => new OkObjectResult(new ApiResponse
    {
        IsSuccess = false,
        Messages = (from x in ctx.ModelState
                    let errorMessage = x.Value?.Errors.Select(x => x.ErrorMessage).FirstOrDefault()
                    where !string.IsNullOrEmpty(errorMessage)
                    select errorMessage).ToArray(),
    });

    public static IServiceCollection ConfigureApiBehaviorOptionsInvalidModelStateResponseFactory(this IServiceCollection services, Action<ApiBehaviorOptions>? action = null) => services.Configure<ApiBehaviorOptions>(options =>
    {
        options.InvalidModelStateResponseFactory = InvalidModelStateResponseFactory;
        action?.Invoke(options);
    });
}
#endif
// ReSharper disable once CheckNamespace
namespace BD.Common.Models;

#if !BLAZOR
public static partial class ApiResponseMvcHelper
{
    public static IActionResult InvalidModelStateResponseFactory(ActionContext ctx) => new OkObjectResult(new ApiResponse
    {
        IsSuccess = false,
        Messages = (from x in ctx.ModelState
                    let errorMessage = x.Value?.Errors.Select(x => x.ErrorMessage).FirstOrDefault()
                    where !string.IsNullOrEmpty(errorMessage)
                    select errorMessage).ToArray(),
    });
}
#endif
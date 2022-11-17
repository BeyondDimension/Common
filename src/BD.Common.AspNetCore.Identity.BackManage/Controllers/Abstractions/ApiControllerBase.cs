namespace BD.Common.Controllers.Abstractions;

/// <summary>
/// API 控制器基类
/// </summary>
/// <typeparam name="T"></typeparam>
[Route("api/[controller]")]
[ApiController]
[Produces(MimeTypes.Json)]
[ProducesResponseType(StatusCodes.Status200OK)]
[ProducesResponseType(StatusCodes.Status500InternalServerError, Type = typeof(string))]
public abstract class ApiControllerBase<T> : ControllerBase where T : ApiControllerBase<T>
{
    /// <summary>
    /// 用于当前控制器的日志
    /// </summary>
    protected readonly ILogger<T> logger;

    public ApiControllerBase(ILogger<T> logger)
    {
        this.logger = logger;
    }

    [NonAction]
    public virtual ActionResult Fail(params string[] errorMessages)
    {
        var rsp = new ApiResponse
        {
            IsSuccess = false,
            Messages = errorMessages,
        };
        return base.Ok(rsp);
    }

    [NonAction]
    public virtual ActionResult Fail(IdentityResult identityResult)
    {
        if (identityResult.Succeeded) throw new ArgumentOutOfRangeException(nameof(identityResult));
        var errorMessages = GetErrors(identityResult).ToArray();
        if (!errorMessages.Any()) errorMessages = new[] { "Identity Error", };
        return Fail(errorMessages);
    }

    [NonAction]
    public virtual ActionResult Ok<TData>(TData data)
    {
        if (data is IApiResponse rsp) // 确保兼容，不可更改此次
            return base.Ok(rsp);
        rsp = new ApiResponse<TData>
        {
            IsSuccess = true,
            Data = data,
        };
        return base.Ok(rsp);
    }

    [NonAction]
    public virtual new ActionResult Ok() => base.Ok(ApiResponse.Ok);

    static IEnumerable<string> GetErrors(IdentityResult identityResult)
    {
        foreach (var error in identityResult.Errors)
        {
            if (!string.IsNullOrWhiteSpace(error.Description))
            {
                yield return error.Description;
            }
            else if (!string.IsNullOrWhiteSpace(error.Code))
            {
                yield return $"Identity Error, Code: {error.Code}";
            }
        }
    }
}
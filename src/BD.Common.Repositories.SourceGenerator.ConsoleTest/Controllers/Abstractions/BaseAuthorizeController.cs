namespace BD.Common.Repositories.SourceGenerator.ConsoleTest.Controllers.Abstractions;

public abstract class BaseAuthorizeController<T> : ApiAuthorizeControllerBase<T> where T : ApiAuthorizeControllerBase<T>
{
    public BaseAuthorizeController(ILogger<T> logger) : base(logger)
    {

    }
}
// ReSharper disable once CheckNamespace
namespace Microsoft.Extensions.DependencyInjection;

public static partial class ServiceCollectionExtensions
{
    sealed class HttpContextRequestAbortedProvider : IRequestAbortedProvider
    {
        readonly IHttpContextAccessor httpContextAccessor;

        public HttpContextRequestAbortedProvider(IHttpContextAccessor httpContextAccessor)
        {
            this.httpContextAccessor = httpContextAccessor;
        }

        CancellationToken IRequestAbortedProvider.RequestAborted => httpContextAccessor.HttpContext?.RequestAborted ?? default;
    }

    public static IServiceCollection AddHttpContextRequestAbortedProvider(this IServiceCollection services)
    {
        // https://github.com/dotnet/aspnetcore/blob/v7.0.0-rc.2.22476.2/src/Http/Http/src/HttpServiceCollectionExtensions.cs#L26
        return services.AddSingleton<IRequestAbortedProvider, HttpContextRequestAbortedProvider>();
    }
}

namespace Microsoft.Extensions.DependencyInjection;

public static partial class ServiceCollectionExtensions
{
    sealed class HttpContextRequestAbortedProvider(IHttpContextAccessor httpContextAccessor) : IRequestAbortedProvider
    {
        readonly IHttpContextAccessor httpContextAccessor = httpContextAccessor;

        CancellationToken IRequestAbortedProvider.RequestAborted => httpContextAccessor.HttpContext?.RequestAborted ?? default;
    }

    /// <summary>
    /// 添加 <see cref="IRequestAbortedProvider"/> 的 AspNetCore 实现
    /// </summary>
    /// <param name="services"></param>
    /// <returns></returns>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static IServiceCollection AddHttpContextRequestAbortedProvider(this IServiceCollection services)
    {
        // https://github.com/dotnet/aspnetcore/blob/v7.0.0-rc.2.22476.2/src/Http/Http/src/HttpServiceCollectionExtensions.cs#L26
        return services.AddSingleton<IRequestAbortedProvider, HttpContextRequestAbortedProvider>();
    }
}

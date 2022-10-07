// ReSharper disable once CheckNamespace
namespace Microsoft.Extensions.DependencyInjection;

public static partial class ServiceCollectionExtensions
{
#if !BLAZOR_TEMPLATE
    public static IHttpClientBuilder AddHttpClientW<TClient, [DynamicallyAccessedMembers(DynamicallyAccessedMemberTypes.PublicConstructors)] TImplementation>(this IServiceCollection services)
        where TClient : class
        where TImplementation : HttpClientWrapper, TClient => services.AddHttpClient<TClient, TImplementation>((s, client) =>
        {
            var settings = s.GetRequiredService<IOptions<AppSettings>>().Value;
            client.BaseAddress = new Uri(settings.ApiBaseAddress);
            client.Timeout = TimeSpan.FromSeconds(45);
        });

    public static IHttpClientBuilder AddAuthService(this IServiceCollection services)
        => services.AddHttpClientW<IAuthService, AuthService>();

    public static IHttpClientBuilder AddApiConnection(this IServiceCollection services)
        => services.AddHttpClientW<IApiConnection, ApiConnection>();
#endif
}

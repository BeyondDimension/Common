namespace Microsoft.Extensions.DependencyInjection;

public static partial class ServiceCollectionExtensions
{
    /// <inheritdoc cref="ForwardedHeadersExtensions.UseForwardedHeaders(IApplicationBuilder, ForwardedHeadersOptions)"/>
    public static IApplicationBuilder UseForwardedHeaders<TAppSettings>(this IApplicationBuilder builder, TAppSettings appSettings)
        where TAppSettings : class, INotUseForwardedHeaders
    {
        if (!appSettings.NotUseForwardedHeaders)
        {
            ForwardedHeadersOptions o = new()
            {
                ForwardedHeaders = ForwardedHeaders.XForwardedFor | ForwardedHeaders.XForwardedProto,
                RequireHeaderSymmetry = false,
                ForwardLimit = null,
            };
            var knownProxies = appSettings.GetForwardedHeadersKnownProxies();
            foreach (var item in knownProxies)
            {
                o.KnownProxies.Add(item);
            }
            builder.UseForwardedHeaders(o);
        }
        return builder;
    }
}

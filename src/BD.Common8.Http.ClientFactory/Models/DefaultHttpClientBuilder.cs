namespace BD.Common8.Http.ClientFactory.Models;

#pragma warning disable SA1600 // Elements should be documented

record class DefaultHttpClientBuilder(
    string Name,
    IServiceCollection Services,
    Action<HttpClient>? ConfigureClient) : IFusilladeHttpClientBuilder
{
    public Func<Func<HttpMessageHandler>, HttpMessageHandler>? ConfigureHandler { get; set; }
}
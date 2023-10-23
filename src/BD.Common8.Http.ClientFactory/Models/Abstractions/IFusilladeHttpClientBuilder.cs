namespace BD.Common8.Http.ClientFactory.Models.Abstractions;

/// <summary>
/// A builder for configuring named <see cref="HttpClient"/> instances returned by <see cref="IClientHttpClientFactory"/>.
/// </summary>
public interface IFusilladeHttpClientBuilder
{
    /// <summary>
    /// Gets the name of the client configured by this builder.
    /// </summary>
    string Name { get; }

    /// <summary>
    /// Gets the application service collection.
    /// </summary>
    IServiceCollection Services { get; }
}
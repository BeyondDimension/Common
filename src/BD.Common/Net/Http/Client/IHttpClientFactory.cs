// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// https://github.com/dotnet/runtime/issues/66863
// https://devblogs.microsoft.com/dotnet/performance-improvements-in-dotnet-maui/#remove-microsoft-extensions-http-usage

// ReSharper disable once CheckNamespace
namespace System.Net.Http.Client;

/// <summary>
/// A factory abstraction for a component that can create <see cref="HttpClient"/> instances with custom
/// configuration for a given logical name.
/// </summary>
/// <remarks>
/// A default <see cref="IHttpClientFactory"/> can be registered in an <see cref="IServiceCollection"/>
/// The default <see cref="IHttpClientFactory"/> will be registered in the service collection as a singleton.
/// </remarks>
public interface IHttpClientFactory
{
    /// <summary>
    /// Creates and configures an <see cref="HttpClient"/> instance using the configuration that corresponds
    /// to the logical name specified by <paramref name="name"/>.
    /// </summary>
    /// <param name="name">The logical name of the client to create.</param>
    /// <param name="category"></param>
    /// <returns>A new <see cref="HttpClient"/> instance.</returns>
    /// <remarks>
    /// <para>
    /// Each call to <see cref="CreateClient(string, HttpHandlerCategory)"/> is guaranteed to return a new <see cref="HttpClient"/>
    /// instance. It is generally not necessary to dispose of the <see cref="HttpClient"/> as the
    /// <see cref="IHttpClientFactory"/> tracks and disposes resources used by the <see cref="HttpClient"/>.
    /// </para>
    /// <para>
    /// Callers are also free to mutate the returned <see cref="HttpClient"/> instance's public properties
    /// as desired.
    /// </para>
    /// </remarks>
    HttpClient CreateClient(string name, HttpHandlerCategory category = default);
}
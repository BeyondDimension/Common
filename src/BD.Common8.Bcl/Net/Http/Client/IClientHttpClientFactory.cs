// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// https://github.com/dotnet/runtime/issues/66863
// https://devblogs.microsoft.com/dotnet/performance-improvements-in-dotnet-maui/#remove-microsoft-extensions-http-usage

namespace System.Net.Http.Client;

#if PROJ_SETUP
/// <summary>
/// A factory abstraction for a component that can create <see cref="HttpClient"/> instances with custom
/// configuration for a given logical name.
/// <para>适用于客户端的 HttpClient 工厂接口</para>
/// </summary>
public partial interface IClientHttpClientFactory
#else
/// <summary>
/// A factory abstraction for a component that can create <see cref="HttpClient"/> instances with custom
/// configuration for a given logical name.
/// <para>适用于客户端的 HttpClient 工厂接口</para>
/// </summary>
/// <remarks>
/// A default <see cref="IClientHttpClientFactory"/> can be registered in an <see cref="IServiceCollection"/>
/// The default <see cref="IClientHttpClientFactory"/> will be registered in the service collection as a singleton.
/// </remarks>
public partial interface IClientHttpClientFactory
#endif
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
    /// <see cref="IClientHttpClientFactory"/> tracks and disposes resources used by the <see cref="HttpClient"/>.
    /// </para>
    /// <para>
    /// Callers are also free to mutate the returned <see cref="HttpClient"/> instance's public properties
    /// as desired.
    /// </para>
    /// </remarks>
    HttpClient CreateClient(string name, HttpHandlerCategory category = default);

#if !PROJ_SETUP
    /// <inheritdoc cref="AddHttpClientDelegate"/>
    protected static AddHttpClientDelegate? AddHttpClientDelegateValue { private get; set; }

    /// <summary>
    /// Adds the IHttpClientFactory and related services to the <see cref="IServiceCollection"/> and configures
    /// a binding between the TClient type and a named <see cref="HttpClient"/>. The client name
    /// will be set to the type name of TClient.
    /// </summary>
    /// <param name="services">The <see cref="IServiceCollection"/>.</param>
    /// <remarks>
    /// <para>
    /// <param name="type">
    /// The type of the typed client. The type specified will be registered in the service collection as
    /// a transient service. See ITypedHttpClientFactory{TClient} for more details about authoring typed clients.
    /// </param>
    /// <see cref="HttpClient"/> instances that apply the provided configuration can be retrieved using
    /// IHttpClientFactory.CreateClient(string) and providing the matching name.
    /// </para>
    /// <para>
    /// TClient instances constructed with the appropriate <see cref="HttpClient" />
    /// can be retrieved from <see cref="IServiceProvider.GetService(Type)" /> (and related methods) by providing
    /// TClient as the service type.
    /// </para>
    /// </remarks>
    protected delegate void AddHttpClientDelegate(IServiceCollection services, [DynamicallyAccessedMembers(DynamicallyAccessedMemberTypes.PublicConstructors)] Type type);

    /// <summary>
    /// Adds the IHttpClientFactory and related services to the <see cref="IServiceCollection"/> and configures
    /// a binding between the <typeparamref name="TClient"/> type and a named <see cref="HttpClient"/>. The client name
    /// will be set to the type name of <typeparamref name="TClient"/>.
    /// </summary>
    /// <typeparam name="TClient">
    /// The type of the typed client. The type specified will be registered in the service collection as
    /// a transient service. See ITypedHttpClientFactory{TClient} for more details about authoring typed clients.
    /// </typeparam>
    /// <param name="services">The <see cref="IServiceCollection"/>.</param>
    /// <remarks>
    /// <para>
    /// <see cref="HttpClient"/> instances that apply the provided configuration can be retrieved using
    /// <see cref="IClientHttpClientFactory.CreateClient(string, HttpHandlerCategory)"/> and providing the matching name.
    /// </para>
    /// <para>
    /// <typeparamref name="TClient"/> instances constructed with the appropriate <see cref="HttpClient" />
    /// can be retrieved from <see cref="IServiceProvider.GetService(Type)" /> (and related methods) by providing
    /// <typeparamref name="TClient"/> as the service type.
    /// </para>
    /// </remarks>
    static void AddHttpClient<[DynamicallyAccessedMembers(DynamicallyAccessedMemberTypes.PublicConstructors)] TClient>(IServiceCollection services)
    {
        var addHttpClientDelegateValue = AddHttpClientDelegateValue;
        addHttpClientDelegateValue.ThrowIsNull();
        addHttpClientDelegateValue(services, typeof(TClient));
    }
#endif
}

#if NETFRAMEWORK
/// <summary>
/// <see cref="IClientHttpClientFactory"/> 的 NETFRAMEWORK 接口默认实现部分
/// </summary>
public static class IClientHttpClientFactory_
#else
partial interface IClientHttpClientFactory
#endif
{
    /// <summary>
    /// 默认超时时间，25 秒
    /// </summary>
    static readonly TimeSpan DefaultTimeout = TimeSpan.FromMilliseconds(DefaultTimeoutMilliseconds);

    /// <inheritdoc cref="DefaultTimeout"/>
    const int DefaultTimeoutMilliseconds = 25000;

    /// <summary>
    /// 默认本地超时时间，单位秒
    /// </summary>
    public const double DefaultLocalTimeoutFromSeconds = 4.99;

    /// <summary>
    /// <see cref="HttpClientHandler.AutomaticDecompression"/> 的默认值
    /// </summary>
    const DecompressionMethods DefaultAutomaticDecompression =
#if NETFRAMEWORK
        DecompressionMethods.GZip | DecompressionMethods.Deflate;
#else
        DecompressionMethods.Brotli | DecompressionMethods.GZip | DecompressionMethods.Deflate;
#endif

    /// <summary>
    /// 创建默认的处理程序
    /// </summary>
    /// <param name="useCookies"></param>
    /// <returns></returns>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    static HttpMessageHandler CreateHandler(bool useCookies = false)
    {
        HttpClientHandler handler = new()
        {
            UseCookies = useCookies,
            AutomaticDecompression = DefaultAutomaticDecompression,
        };
        return handler;
    }

    /// <summary>
    /// 创建一个 <see cref="HttpClient"/> 实例并设置默认超时时间
    /// </summary>
    /// <param name="handler"></param>
    /// <returns></returns>
    static HttpClient CreateClient(HttpMessageHandler handler)
    {
        var client = new HttpClient(handler);
        try
        {
            client.Timeout = DefaultTimeout;
        }
        catch
        {
        }
        return client;
    }
}
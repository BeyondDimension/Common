namespace BD.Common8.Ipc.Server.Models;

/// <inheritdoc cref="WebApplication"/>
public sealed class IpcApp(WebApplication webApp, IpcAppBuilderOptions o) : IDisposable, IAsyncDisposable
{
    /// <inheritdoc cref="WebApplication"/>
    public WebApplication WebApp => webApp.ThrowIsNull();

    /// <inheritdoc cref="IpcAppConnectionStringType"/>
    public IpcAppConnectionString ConnectionString { get; } = o.ConnectionString;

    /// <inheritdoc cref="WebApplication.RunAsync(string?)"/>
    public Task RunAsync() => WebApp.RunAsync();

    /// <inheritdoc/>
    public void Dispose()
    {
        if (webApp != null)
        {
            ((IDisposable)webApp).Dispose();
            webApp = null!;
        }
        GC.SuppressFinalize(this);
    }

    /// <inheritdoc/>
    public async ValueTask DisposeAsync()
    {
        if (webApp != null)
        {
            await webApp.DisposeAsync();
            webApp = null!;
        }
        GC.SuppressFinalize(this);
    }
}

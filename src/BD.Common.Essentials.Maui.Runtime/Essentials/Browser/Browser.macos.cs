namespace Microsoft.Maui.ApplicationModel;

partial class BrowserImplementation : IBrowser
{
    public Task<bool> OpenAsync(Uri uri, BrowserLaunchOptions options) =>
        Task.FromResult(NSWorkspace.SharedWorkspace.OpenUrl(new NSUrl(uri.AbsoluteUri)));
}

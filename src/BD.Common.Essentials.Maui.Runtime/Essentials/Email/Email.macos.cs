namespace Microsoft.Maui.ApplicationModel.Communication;

partial class EmailImplementation : IEmail
{
    public bool IsComposeSupported =>
        PlatformUtils.InvokeOnMainThread(() => NSWorkspace.SharedWorkspace.UrlForApplication(NSUrl.FromString("mailto:")!) != null);

    Task PlatformComposeAsync(EmailMessage? message)
    {
        if (message == null) return Task.CompletedTask;
        var url = GetMailToUri(message);

        using var nsurl = NSUrl.FromString(url)!;
        NSWorkspace.SharedWorkspace.OpenUrl(nsurl);
        return Task.CompletedTask;
    }
}

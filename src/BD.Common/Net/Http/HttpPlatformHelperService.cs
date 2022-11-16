using System.IO.FileFormats;

// ReSharper disable once CheckNamespace
namespace System.Net.Http;

/// <inheritdoc cref="IHttpPlatformHelperService"/>
public abstract class HttpPlatformHelperService : IHttpPlatformHelperService
{
    protected const string DefaultUserAgent = Headers.UserAgent.Lumia950;

    public abstract string UserAgent { get; }

    public virtual string AcceptImages => "image/png, image/*; q=0.8";

    public virtual string AcceptLanguage => CultureInfo.CurrentUICulture.GetAcceptLanguage();

    public virtual ImageFormat[] SupportedImageFormats => SupportedImageFormats_.V.Value;

    static class SupportedImageFormats_
    {
        internal static readonly Lazy<ImageFormat[]> V = new(() => new ImageFormat[]
        {
            ImageFormat.JPEG,
            ImageFormat.PNG,
            ImageFormat.GIF,
            ImageFormat.WebP,
        });
    }

    public virtual (string filePath, string mime)? TryHandleUploadFile(Stream fileStream) => null;

    protected virtual bool IsConnected => true;

    public virtual Task<bool> IsConnectedAsync() => Task.FromResult(IsConnected);
}
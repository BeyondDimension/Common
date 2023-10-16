namespace System.Net.Http;

/// <inheritdoc cref="IHttpPlatformHelperService"/>
public abstract partial class HttpPlatformHelperService : IHttpPlatformHelperService
{
    /// <inheritdoc cref="UserAgentConstants.Default"/>
    protected const string DefaultUserAgent = UserAgentConstants.Default;

    /// <inheritdoc/>
    public abstract string UserAgent { get; }

    /// <inheritdoc/>
    public virtual string AcceptImages => "image/png, image/*; q=0.8";

    /// <inheritdoc/>
    public virtual string AcceptLanguage => CultureInfo.CurrentUICulture.GetAcceptLanguage();

    /// <inheritdoc/>
    public virtual ImageFormat[] SupportedImageFormats => SupportedImageFormats_.V.Value;

    static partial class SupportedImageFormats_
    {
        internal static readonly Lazy<ImageFormat[]> V = new(() => new ImageFormat[]
        {
            ImageFormat.JPEG,
            ImageFormat.PNG,
            ImageFormat.GIF,
            ImageFormat.WebP,
        });
    }

    /// <inheritdoc/>
    public virtual (string filePath, string mime)? TryHandleUploadFile(Stream fileStream) => null;

    /// <summary>
    /// 是否有网络链接
    /// </summary>
    protected virtual bool IsConnected => true;

    /// <inheritdoc/>
    public virtual ValueTask<bool> IsConnectedAsync() => new(IsConnected);
}
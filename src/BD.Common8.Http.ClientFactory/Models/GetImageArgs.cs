#if !NETFRAMEWORK && !PROJ_SETUP
namespace BD.Common8.Http.ClientFactory.Models;

/// <summary>
/// 通过 Get 请求 Image 的参数
/// <para>缓存机制：</para>
/// <list type="bullet">
/// <item><para>设置 <see cref="HashValue"/> + <see cref="UseCache"/> 与 <see cref="CacheFirst"/> 为 <see langword="true"/> 时，优先从本地根据哈希值加载缓存</para></item>
/// </list>
/// </summary>
public readonly record struct GetImageArgs
{
    /// <summary>
    /// Initializes a new instance of the <see cref="GetImageArgs"/> struct.
    /// </summary>
    public GetImageArgs()
    {
    }

    /// <summary>
    /// 请求地址
    /// </summary>
    public required string RequestUri { get; init; }

    /// <summary>
    /// 是否使用 <see cref="Polly"/> 重试次数，为 0 时不重试
    /// </summary>
    public int IsPollyNum { get; init; } = 3;

    /// <summary>
    /// 是否进行缓存
    /// </summary>
    public bool UseCache { get; init; } = true;

    /// <summary>
    /// 是否优先使用缓存加载
    /// </summary>
    public bool CacheFirst { get; init; } = true;

    /// <summary>
    /// <see cref="HttpHandlerCategory"/> 的默认值
    /// </summary>
    public const HttpHandlerCategory DefaultHttpHandlerCategory = IImageHttpClientService.DefaultHttpHandlerCategory;

    /// <inheritdoc cref="HttpHandlerCategory"/>
    public HttpHandlerCategory Category { get; init; } = DefaultHttpHandlerCategory;

    /// <summary>
    /// 哈希值
    /// </summary>
    public string? HashValue { get; init; }

    public void Deconstruct(out string requestUri, out int isPollyNum, out bool cache, out bool cacheFirst, out HttpHandlerCategory category)
    {
        requestUri = RequestUri;
        isPollyNum = IsPollyNum;
        cache = UseCache;
        cacheFirst = CacheFirst;
        category = Category;

        if (!cache)
        {
            category = HttpHandlerCategory.Default;
        }
        else
        {
            isPollyNum = 0;
        }
    }
}
#endif
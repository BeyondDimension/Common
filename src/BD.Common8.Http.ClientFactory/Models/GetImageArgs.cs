#if !NETFRAMEWORK && !PROJ_SETUP
namespace BD.Common8.Http.ClientFactory.Models;

/// <summary>
/// 通过 Get 请求 Image 的参数
/// <para>缓存机制：</para>
/// <list type="bullet">
/// <item><para>设置 <see cref="HashValue"/> + <see cref="UseCache"/> 与 <see cref="CacheFirst"/> 为 <see langword="true"/> 时，优先从本地根据哈希值加载缓存</para></item>
/// </list>
/// </summary>
public record struct GetImageArgs
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
    /// 是否使用 <see cref="Polly"/> 重试
    /// </summary>
    public bool IsPolly { get; set; }

    /// <summary>
    /// 是否进行缓存
    /// </summary>
    public bool UseCache { get; set; }

    /// <summary>
    /// 是否优先使用缓存加载
    /// </summary>
    public bool CacheFirst { get; set; }

    /// <summary>
    /// <see cref="HttpHandlerCategory"/> 的默认值
    /// </summary>
    public const HttpHandlerCategory DefaultHttpHandlerCategory = HttpHandlerCategory.UserInitiated;

    /// <inheritdoc cref="HttpHandlerCategory"/>
    public HttpHandlerCategory Category { get; set; } = DefaultHttpHandlerCategory;

    /// <summary>
    /// 哈希值
    /// </summary>
    public string? HashValue { get; set; }
}
#endif
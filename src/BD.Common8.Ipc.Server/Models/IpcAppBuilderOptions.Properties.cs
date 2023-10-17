namespace BD.Common8.Ipc.Server.Models;

partial class IpcAppBuilderOptions
{
    /// <inheritdoc cref="WebApplicationOptions"/>
    public WebApplicationOptions? Options { get; init; }

    /// <summary>
    /// SSL 证书，可选，如果需要监听 Https 则必填
    /// </summary>
    public required X509Certificate2? ServerCertificate { get; set; }

    /// <summary>
    /// Json 源生成解析
    /// </summary>
    public required IJsonTypeInfoResolver JsonTypeInfoResolver { get; init; }

    /// <summary>
    /// 配置 <see cref="WebApplication"/> 服务
    /// </summary>
    public Action<WebApplicationBuilder>? OnConfigureServices { get; init; }

    /// <summary>
    /// 监听本地的端口号，可使用 PortHelper.GetRandomUnusedPort(IPAddress.Loopback) 创建
    /// </summary>
    public int HttpsPort { get; set; }

    /// <summary>
    /// 是否监听本地
    /// </summary>
    public bool ListenLocalhost { get; set; }

    /// <summary>
    /// Unix 套接字文件路径，可使用 var socketPath = Path.Combine(Path.GetTempPath(), "socket_xyz.tmp");
    /// </summary>
    public string? UnixSocketPath { get; set; }

    /// <summary>
    /// 是否监听 Unix 套接字
    /// </summary>
    public bool ListenUnixSocket { get; set; }

    /// <summary>
    /// 命名管道名称
    /// </summary>
    public string? PipeName { get; set; }

    /// <summary>
    /// 是否监听命名管道
    /// </summary>
    public bool ListenNamedPipe { get; set; }

    /// <summary>
    /// Ipc 应用程序连接字符串
    /// </summary>
    internal string? ConnectionString { get; private set; }

    /// <summary>
    /// <see cref="IMapGroup.OnMapGroup(IEndpointRouteBuilder)"/> 的事件
    /// </summary>
    internal static event Action<IEndpointRouteBuilder>? OnMapGroupEvent;
}

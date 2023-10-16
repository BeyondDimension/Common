namespace System.Net.Http.Client;

/// <summary>
/// HttpHandler 类别
/// </summary>
public enum HttpHandlerCategory : byte
{
    Default,

    /// <summary>。
    /// 这是为读取用户以后可能使用也可能不使用的数据而设计的，
    /// 以便在用户以后请求该数据时提高响应速度。
    /// 允许在取消所有未来请求之前读取一定数量的字节
    /// </summary>
    Speculative,

    /// <summary>
    /// 调度器应该用于由用户动作发起的请求，
    /// 如点击一个项目，它们具有最高的优先级。
    /// </summary>
    UserInitiated,

    /// <summary>
    /// 该调度器应该用于在后台发起的请求，并以较低的优先级调度。
    /// </summary>
    Background,

    /// <summary>
    /// 调度器只从 Fusillade.NetCache.RequestCache 中指定的缓存中获取结果。
    /// </summary>
    Offline,
}

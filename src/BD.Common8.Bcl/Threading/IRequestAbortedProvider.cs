namespace System.Threading;

/// <summary>
/// 请求中止提供接口
/// </summary>
public interface IRequestAbortedProvider
{
    /// <summary>
    /// 当此请求的底层连接被中止时发出通知，因此应取消请求操作。
    /// </summary>
    CancellationToken RequestAborted
#if NETCOREAPP || NETSTANDARD2_1_OR_GREATER
        => default;
#else
    { get; }
#endif
}
namespace BD.Common8.Essentials.Helpers;

static partial class Utils
{
    /// <summary>
    /// 用于解析版本号字符串
    /// </summary>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static Version ParseVersion(string version)
    {
        if (Version.TryParse(version, out var number))
            return number;

        if (int.TryParse(version, out var major))
            return new Version(major, 0);

        return new Version(0, 0);
    }

    /// <summary>
    /// 创建新的取消令牌令牌，如果给定超时时间，则在令牌过期后取消它
    /// </summary>
    /// <param name="cancellationToken"></param>
    /// <param name="timeout"></param>
    /// <returns></returns>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static CancellationToken TimeoutToken(CancellationToken cancellationToken, TimeSpan timeout)
    {
        // create a new linked cancellation token source
        var cancelTokenSrc = CancellationTokenSource.CreateLinkedTokenSource(cancellationToken);

        // if a timeout was given, make the token source cancel after it expires
        if (timeout > TimeSpan.Zero)
            cancelTokenSrc.CancelAfter(timeout);

        // our Cancel method will handle the actual cancellation logic
        return cancelTokenSrc.Token;
    }

    /// <summary>
    /// 使用指定的超时时间等待任务的完成，并返回任务的结果，如果任务在超时时间内未完成，则返回默认值
    /// </summary>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static async Task<T?> WithTimeout<T>(Task<T> task, TimeSpan timeSpan)
    {
        var retTask = await Task.WhenAny(task, Task.Delay(timeSpan))
            .ConfigureAwait(false);

        return retTask is Task<T> ? task.Result : default(T);
    }
}
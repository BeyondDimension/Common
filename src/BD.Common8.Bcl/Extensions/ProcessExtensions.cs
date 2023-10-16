namespace System.Extensions;

/// <summary>
/// 提供对 <see cref="Process"/> 类型的扩展函数
/// </summary>
public static partial class ProcessExtensions
{
    /// <summary>
    /// 尝试获取关联进程的主模块
    /// </summary>
    /// <param name="process"></param>
    /// <returns></returns>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static ProcessModule? TryGetMainModule(this Process process)
    {
        try
        {
            return process.MainModule;
        }
        catch
        {
            return null;
        }
    }

    /// <summary>
    /// 立即停止关联的进程，并停止其子/后代进程。
    /// </summary>
    /// <param name="process"></param>
#if NET5_0_OR_GREATER
    [UnsupportedOSPlatform("ios")]
    [UnsupportedOSPlatform("tvos")]
#endif
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static void KillEntireProcessTree(this Process process)
    {
#if NETCOREAPP3_0_OR_GREATER
        process.Kill(entireProcessTree: true);
#else
        process.Kill();
#endif
    }

    /// <summary>
    /// 尝试 <see cref="Process"/> 组件在指定的毫秒数内等待关联进程退出。
    /// </summary>
    /// <param name="process"></param>
    /// <param name="milliseconds"></param>
    /// <returns></returns>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static bool TryWaitForExit(this Process process, int milliseconds = 9000)
    {
        try
        {
            return process.WaitForExit(milliseconds);
        }
        catch
        {
        }
        return false;
    }
}
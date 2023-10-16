#if NET6_0_OR_GREATER
using System.IO.Pipelines;

namespace System.Extensions;

/// <summary>
/// 提供对 <see cref="IDuplexPipe"/> 类型的扩展函数
/// </summary>
public static partial class DuplexPipeStreamExtensions
{
    /// <summary>
    /// 将 <see cref="IDuplexPipe"/> 转换为 <see cref="Stream"/>
    /// </summary>
    /// <param name="duplexPipe"></param>
    /// <param name="throwOnCancelled"></param>
    /// <returns></returns>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static Stream AsStream(this IDuplexPipe duplexPipe, bool throwOnCancelled = false)
        => new DuplexPipeStream(duplexPipe, throwOnCancelled);
}
#endif
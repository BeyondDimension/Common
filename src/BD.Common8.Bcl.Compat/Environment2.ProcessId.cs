#pragma warning disable RS1035 // 不要使用禁用于分析器的 API
#if !NET5_0_OR_GREATER
using Process = System.Diagnostics.Process;
#endif

namespace System;

partial class Environment2
{
    /// <summary>
    /// 获取当前进程的唯一标识符
    /// </summary>
    public static partial int ProcessId { get; }
}

#if NET5_0_OR_GREATER
partial class Environment2
{
    public static partial int ProcessId => Environment.ProcessId;
}
#else
partial class Environment2
{
    public static partial int ProcessId
    {
        get
        {
            if (!_ProcessId.HasValue)
            {
                _ProcessId = Process.GetCurrentProcess().Id;
            }
            return _ProcessId.Value;
        }
    }

    static int? _ProcessId;
}
#endif
namespace System;

partial class Environment2
{
    /// <summary>
    /// 获取当前进程的唯一标识符。
    /// </summary>
    public static int ProcessId

#if NET5_0_OR_GREATER
        => Environment.ProcessId;
#else
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
#endif
}

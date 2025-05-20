#pragma warning disable RS1035 // 不要使用禁用于分析器的 API
#if !(NET6_0_OR_GREATER && !USE_WINDOWSFORMS)
using Process = System.Diagnostics.Process;
#endif

namespace System;

partial class Environment2
{
    /// <summary>
    /// 返回启动当前正在执行的进程的可执行文件的路径
    /// </summary>
    public static partial string? ProcessPath { get; }
}

#if NET6_0_OR_GREATER && !USE_WINDOWSFORMS
partial class Environment2
{
    public static partial string? ProcessPath => Environment.ProcessPath;
}
#else
partial class Environment2
{
    public static partial string? ProcessPath => Process.GetCurrentProcess().MainModule?.FileName;
}
#endif
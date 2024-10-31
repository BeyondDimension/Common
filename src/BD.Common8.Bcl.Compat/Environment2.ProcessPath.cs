#pragma warning disable RS1035 // 不要使用禁用于分析器的 API

namespace System;

partial class Environment2
{
    /// <summary>
    /// 获取当前进程的唯一标识符。
    /// </summary>
    public static string? ProcessPath =>
#if NET6_0_OR_GREATER && !USE_WINDOWSFORMS

        Environment.ProcessPath;
#else
        Process.GetCurrentProcess().MainModule?.FileName;

#endif
}
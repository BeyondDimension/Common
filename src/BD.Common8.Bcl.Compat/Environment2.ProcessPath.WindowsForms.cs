namespace System;

partial class Environment2
{
#if USE_WINDOWSFORMS
    /// <summary>
    /// 返回启动当前正在执行的进程的可执行文件的路径。
    /// </summary>
    public static string ProcessPath => System.Windows.Forms.Application.ExecutablePath;
#endif
}

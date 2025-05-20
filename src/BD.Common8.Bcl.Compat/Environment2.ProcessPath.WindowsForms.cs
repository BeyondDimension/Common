#if USE_WINDOWSFORMS
using WinFormsApp = System.Windows.Forms.Application;

namespace System;

partial class Environment2
{
    /// <summary>
    /// 返回启动当前正在执行的进程的可执行文件的路径
    /// </summary>
    public static partial string ProcessPath => WinFormsApp.ExecutablePath;
}
#endif
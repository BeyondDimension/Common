namespace System.Diagnostics;

partial class Process2
{
    /// <summary>
    /// 提权启动进程，如果 UAC 弹窗取消则返回 <see langword="null"/>
    /// </summary>
    /// <param name="fileName">进程可执行文件路径</param>
    /// <param name="arguments">参数</param>
    /// <param name="forceRunas">是否强制用 runas 启动</param>
    /// <returns></returns>
    public static Process? StartAsPrivileged(
        string fileName,
        object? arguments = null,
        bool forceRunas = false)
    {
        Process? process;
        try
        {
            if (!forceRunas && Environment.IsPrivilegedProcess)
            {
                process = Start(fileName, arguments);
            }
            else
            {
                ProcessStartInfo psi = new()
                {
                    FileName = fileName,
                    UseShellExecute = true,
                    Verb = "runas",
                };
                psi.SetArguments(arguments);
                process = Process.Start(psi);
            }
        }
        catch
        {
            process = null;
            // System.ComponentModel.Win32Exception
            //  HResult=0x80004005
            //  Message=An error occurred trying to start process '{fileName}' with working directory '{workingDirectory?}'. 操作已被用户取消。
            //  Source=System.Diagnostics.Process
            //  StackTrace:
            //   在 System.Diagnostics.Process.StartWithShellExecuteEx(ProcessStartInfo startInfo)
            //   在 System.Diagnostics.Process.Start(ProcessStartInfo startInfo)
        }
        return process;
    }
}
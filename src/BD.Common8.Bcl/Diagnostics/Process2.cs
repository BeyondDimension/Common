using SR = BD.Common8.Resources.SR;

namespace System.Diagnostics;

/// <summary>
/// 提供进程相关的操作
/// </summary>
public static partial class Process2
{
    /// <summary>
    /// /bin/bash
    /// </summary>
    public const string BinBash = "/bin/bash";

    /// <summary>
    /// 通过指定应用程序的名称和路径参数来启动一个进程资源，并将该资源与新的 Process 组件相关联
    /// </summary>
    /// <param name="fileName">要在进程中运行的应用程序文件的名称</param>
    /// <param name="path">要传递的路径</param>
    /// <returns></returns>
    /// <exception cref="ObjectDisposedException">已释放此进程对象</exception>
    /// <exception cref="Win32Exception">
    /// <para>打开关联的文件时出错</para>
    /// <para>或 fileName 中指定的文件未找到</para>
    /// <para>或 参数的长度与该进程的完整路径的长度的总和超过了 2080。 与此异常关联的错误消息可能为以下消息之一：“传递到系统调用的数据区域太小。” 或“拒绝访问。”</para>
    /// </exception>
    /// <exception cref="PlatformNotSupportedException">不支持 shell 的操作系统（如，仅适用于.NET Core 的 Nano Server）不支持此方法</exception>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static Process? StartPath(string fileName, string path)
    {
#if NET5_0_OR_GREATER
        if (OperatingSystem.IsMacOS())
#else
        if (OperatingSystem2.IsMacOS())
#endif
        {
            var arguments = $"-a \"{fileName}\" \"{path}\"";
            using var process = Start("open", arguments, true);
            if (process == null)
            {
                Log.Error(nameof(Process2), "StartPath(macOS) return null, fileName:{0}, path: {1}",
                    fileName, path);
                return null;
            }
            process.Close();
            return process;
        }
        else
        {
            return Start(fileName, $"\"{path}\"", false);
        }
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static void SetArguments(this ProcessStartInfo p, object? arguments = null)
    {
        if (arguments != null)
        {
            if (arguments is IEnumerable<string> arguments_enumerable)
            {
                foreach (var argument in arguments_enumerable)
                {
                    p.ArgumentList.Add(argument);
                }
            }
            else if (arguments is string arguments_string)
            {
                if (!string.IsNullOrEmpty(arguments_string))
                {
                    p.Arguments = arguments_string;
                }
            }
        }
    }

    /// <summary>
    /// 获取用于启动进程的 ProcessStartInfo 对象
    /// </summary>
    /// <param name="fileName">要执行的可执行文件的名称或路径</param>
    /// <param name="arguments">传递的参数</param>
    /// <param name="useShellExecute">指定是否使用 Shell 来执行</param>
    /// <param name="workingDirectory">设置执行的工作目录</param>
    /// <param name="environment">要传递给进程环境变量的键值对</param>
    /// <returns>用于启动进程的 ProcessStartInfo 对象</returns>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static ProcessStartInfo GetProcessStartInfo(string fileName, object? arguments = null, bool useShellExecute = false, string? workingDirectory = null, IReadOnlyDictionary<string, string>? environment = null)
    {
        var p = new ProcessStartInfo(fileName);

        SetArguments(p, arguments);

        if (environment != null)
        {
            foreach (var item in environment)
            {
                if (!p.Environment.TryAdd(item.Key, item.Value))
                {
                    p.Environment[item.Key] = item.Value;
                }
            }
        }
        if (!string.IsNullOrEmpty(workingDirectory))
        {
            p.WorkingDirectory = workingDirectory;
        }
        else if (!useShellExecute && fileName.Contains(Path.DirectorySeparatorChar))
        {
            FileInfo fileInfo = new(fileName);
            if (fileInfo.Exists && !string.IsNullOrEmpty(fileInfo.DirectoryName))
            {
                p.WorkingDirectory = fileInfo.DirectoryName;
            }
        }
        p.UseShellExecute = useShellExecute;

        return p;
    }

    /// <summary>
    /// 通过指定应用程序的名称和一组命令行参数来启动一个进程资源，并将该资源与新的 Process 组件相关联
    /// </summary>
    /// <param name="fileName">要在进程中运行的应用程序文件的名称</param>
    /// <param name="arguments">启动该进程时传递的命令行参数</param>
    /// <param name="useShellExecute">获取或设置指示是否使用操作系统 shell 启动进程的值</param>
    /// <param name="workingDirectory">当 useShellExecute 属性为 <see langword="false"/> 时，将获取或设置要启动的进程的工作目录。 当 useShellExecute 为 <see langword="true"/> 时，获取或设置包含要启动的进程的目录<para>注意：当 UseShellExecute 为  <see langword="true"/> 时，是包含要启动的进程的目录的完全限定名</para></param>
    /// <param name="environment"></param>
    /// <returns></returns>
    /// <exception cref="ObjectDisposedException">已释放此进程对象</exception>
    /// <exception cref="Win32Exception">
    /// <para>打开关联的文件时出错</para>
    /// <para>或 fileName 中指定的文件未找到</para>
    /// <para>或 参数的长度与该进程的完整路径的长度的总和超过了 2080。 与此异常关联的错误消息可能为以下消息之一：“传递到系统调用的数据区域太小。” 或“拒绝访问。”</para>
    /// </exception>
    /// <exception cref="PlatformNotSupportedException">不支持 shell 的操作系统（如，仅适用于.NET Core 的 Nano Server）不支持此方法</exception>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static Process? Start(string fileName, object? arguments = null, bool useShellExecute = false, string? workingDirectory = null, IReadOnlyDictionary<string, string>? environment = null)
    {
        if (string.IsNullOrEmpty(fileName))
            return null;
        var p = GetProcessStartInfo(fileName, arguments, useShellExecute, workingDirectory, environment);
        try
        {
            return Process.Start(p);
        }
        catch (Win32Exception)
        {
            if (!p.UseShellExecute)
            {
                // System.ComponentModel.Win32Exception: An error occurred trying to start process 'a.exe' with working directory 'path'. 请求的操作需要提升。
                p = GetProcessStartInfo(fileName, arguments, true, workingDirectory: null, environment);
                return Process.Start(p);
            }
        }
        return null;
    }

    /// <summary>
    /// 使用进程打开指定的 URL
    /// </summary>
    /// <param name="url">要打开的 URL </param>
    /// <param name="onError">错误回调函数</param>
    /// <returns>如果成功打开 URL，则返回 <see langword="true"/>；否则为 <see langword="false"/> </returns>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static bool OpenCoreByProcess(string url, Action<string>? onError = null)
    {
        try
        {
            Start(url, useShellExecute: true);
            return true;
        }
        catch (Win32Exception e)
        {
            // [Win32Exception: 找不到应用程序] 39次报告
            // 疑似缺失没有默认浏览器设置会导致此异常，可能与杀毒软件有关
            onError?.Invoke(SR.OpenCoreByProcess_Win32Exception_.Format(Convert.ToString(e.NativeErrorCode, 16)));
            return false;
        }
    }

    /// <summary>
    /// 执行 Shell 命令并返回结果
    /// </summary>
    /// <param name="fileName">要执行的命令</param>
    /// <param name="value">传递给命令的参数</param>
    /// <param name="onError">发生异常时执行的回调函数</param>
    /// <returns>命令执行后的输出结果</returns>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    [Obsolete]
    public static string RunShell(string fileName, string value, Action<Exception>? onError = null)
    {
        try
        {
            using var p = new Process();
            p.StartInfo.FileName = fileName;
            p.StartInfo.Arguments = "";
            p.StartInfo.RedirectStandardOutput = true;
            p.StartInfo.RedirectStandardInput = true;
            p.StartInfo.UseShellExecute = false;
            p.Start();
            p.StandardInput.WriteLine(value);
            p.StandardInput.Close();
            string result = p.StandardOutput.ReadToEnd();
            p.StandardOutput.Close();
            p.WaitForExit();
            p.Close();
            p.Dispose();
            return result;
        }
        catch (Exception e)
        {
            onError?.Invoke(e);
        }
        return string.Empty;
    }

    /// <summary>
    /// 进程是否活着
    /// </summary>
    /// <param name="process"></param>
    /// <returns></returns>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static bool IsAlive(Process? process)
    {
        if (process == null)
            return false;

        if (process.Id == Environment.ProcessId)
            return true;

        try
        {
            if (process.HasExited)
                return false;
        }
        catch
        {
        }

        return true;
    }

    /// <summary>
    /// 进程是否活着
    /// </summary>
    /// <param name="pid"></param>
    /// <returns></returns>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static bool IsAlive(int pid)
    {
        if (pid <= 0)
            return false;

        if (pid == Environment.ProcessId)
            return true;

        Process? process = null;
        try
        {
            process = Process.GetProcessById(pid);
        }
        catch
        {
        }
        return IsAlive(process);
    }

    /// <summary>
    /// 进程是否活着
    /// </summary>
    /// <param name="pid"></param>
    /// <param name="process"></param>
    /// <returns></returns>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static bool IsAlive(int pid, [NotNullWhen(true)] out Process? process)
    {
        process = null;

        if (pid <= 0)
            return false;

        if (pid == Environment.ProcessId)
        {
            process = Process.GetCurrentProcess();
            return true;
        }

        try
        {
            process = Process.GetProcessById(pid);
        }
        catch
        {
        }
        return IsAlive(process);
    }

#if WINDOWS
    /// <summary>
    /// 根据进程获取进程文件路径
    /// </summary>
    /// <param name="process"></param>
    /// <returns></returns>
    public static string? GetPathByDwProcessId(Process process) => GetPathByDwProcessId(unchecked((uint)process.Id));

    /// <summary>
    /// 根据进程 Id 获取进程文件路径
    /// </summary>
    /// <param name="dwProcessId"></param>
    /// <returns></returns>
    public static unsafe string? GetPathByDwProcessId(uint dwProcessId)
    {
        if (dwProcessId == Environment.ProcessId)
        {
            return Environment.ProcessPath;
        }

        using var hProcess = CsWin32.PInvoke.OpenProcess_SafeHandle(
            CsWin32.System.Threading.PROCESS_ACCESS_RIGHTS.PROCESS_QUERY_LIMITED_INFORMATION,
            false,
            dwProcessId);
        fixed (char* lpExeName = stackalloc char[1024])
        {
            uint lpdwSize = default;
            CsWin32.Foundation.PWSTR lpExeNamePWSTR = new(lpExeName);
            bool isOK = CsWin32.PInvoke.QueryFullProcessImageName(
                hProcess,
                CsWin32.System.Threading.PROCESS_NAME_FORMAT.PROCESS_NAME_WIN32,
                lpExeNamePWSTR,
                ref lpdwSize);
            if (isOK && lpdwSize > 0)
            {
                return new string(lpExeName, 0, (int)lpdwSize);
            }
        }
        return default;
    }

    /// <summary>
    /// 检查指定的进程是否以管理员权限运行
    /// </summary>
    /// <param name="process"></param>
    /// <returns></returns>
    public static unsafe bool IsProcessElevated(Process process)
    {
        if (process.Id == Environment.ProcessId)
        {
            return Environment.IsPrivilegedProcess;
        }

        try
        {
            var handle = process.SafeHandle;

            // IsPrivilegedProcess
            // https://github.com/dotnet/runtime/pull/77355/files#diff-1c6f0e5208d48036e96fcc9c0243d93595f3ce16f2d1a50f51ba604d930ca69dR87
            SafeFileHandle? token = null;
            try
            {
                if (CsWin32.PInvoke.OpenProcessToken(handle,
                     CsWin32.Security.TOKEN_ACCESS_MASK.TOKEN_READ,
                     out token))
                {
                    TOKEN_ELEVATION elevation = default;
                    if (CsWin32.PInvoke.GetTokenInformation(
                        token,
                        CsWin32.Security.TOKEN_INFORMATION_CLASS.TokenElevation,
                        &elevation,
                        (uint)sizeof(TOKEN_ELEVATION),
                        out _))
                    {
                        return elevation.TokenIsElevated;
                    }
                }
            }
            finally
            {
                token?.Dispose();
            }

            var error = Marshal.GetLastPInvokeError();
            throw new Win32Exception(error);

            //using WindowsIdentity identity = new(handle);
            //WindowsPrincipal principal = new(identity);
            //return principal.IsInRole(WindowsBuiltInRole.Administrator);
        }
        catch (Win32Exception ex)
        {
            /* “process.Handle”引发了类型“System.ComponentModel.Win32Exception”的异常
             * Data: {System.Collections.ListDictionaryInternal}
             * ErrorCode: -2147467259
             * HResult: -2147467259
             * HelpLink: null
             * InnerException: null
             * Message: "拒绝访问。"
             * NativeErrorCode: 5
             * Source: "System.Diagnostics.Process"
             */
            if (ex.NativeErrorCode == 5)
                return true;
        }
        return false;
    }

    // https://msdn.microsoft.com/en-us/library/windows/desktop/bb530717.aspx
    struct TOKEN_ELEVATION
    {
        public CsWin32.Foundation.BOOL TokenIsElevated;
    }
#endif
}
namespace BD.Common8.AspNetCore.Helpers;

static partial class ProgramHelper
{
    const string VersionZero = "0.0.0.0";

    /// <summary>
    /// 当前 Program 的项目名称
    /// </summary>
    public static string ProjectName { get; private set; } = string.Empty;

    /// <summary>
    /// 当前 Program 版本号
    /// </summary>
    public static string Version { get; private set; } = VersionZero;

    static void CalcVersion(Assembly? callingAssembly = default)
    {
        if (callingAssembly == default)
        {
            try
            {
                callingAssembly = Assembly.GetCallingAssembly();
            }
            catch
            {
            }
        }
        Version = callingAssembly?.GetName()?.Version?.ToString() ?? VersionZero;
    }

    public static void ConsoleWriteInfo(string? projectName = default)
    {
        if (!string.IsNullOrWhiteSpace(projectName))
            ProjectName = projectName;

        #region 项目代号和版本信息

        if (!string.IsNullOrWhiteSpace(projectName))
        {
            Console.Write("Project ");
            Console.Write(projectName.TrimStart("Project"));
            const string version_f = $" [{nameof(Version)} ";
            Console.Write(version_f);

            if (string.IsNullOrWhiteSpace(Version) || Version == VersionZero)
            {
                Assembly? callingAssembly = default;
                try
                {
                    callingAssembly = Assembly.GetCallingAssembly();
                }
                catch
                {
                }
                CalcVersion(callingAssembly);
            }

            Console.Write(Version);
            Console.Write(" / Runtime ");
            Console.Write(Environment.Version);
            Console.Write(']');
            Console.Write('\n');
            Console.Write('\n');
        }

        #endregion

        #region 当前运行的计算机 CPU 显示名称

        if (!string.IsNullOrEmpty(CentralProcessorName))
        {
            Console.Write("CentralProcessorName: ");
            Console.Write(CentralProcessorName);
            Console.Write(" x");
            Console.Write(Environment.ProcessorCount);
            Console.Write('\n');
        }

        #endregion

        #region 本地时间与当前系统设置区域

        Console.Write("LocalTime: ");
        Console.Write(DateTimeOffset.Now.ToLocalTime());
        Console.Write('\n');

        Console.Write("CurrentCulture: ");
        Console.Write(CultureInfo.CurrentCulture.Name);
        Console.Write(' ');
        Console.Write(CultureInfo.CurrentCulture.EnglishName);
        Console.Write('\n');

        #endregion

        #region ShowInfo

        Console.Write("BaseDirectory: ");
        Console.Write(AppContext.BaseDirectory);
        Console.Write('\n');

        Console.Write("OSArchitecture: ");
#if PROJ_ASPIRE_APPHOST
        Console.Write(RuntimeInformation.OSArchitecture);
#else
        Console.Write(OSHelper.Architecture);
#endif
        Console.Write('\n');

        Console.Write("ProcessArchitecture: ");
        Console.Write(RuntimeInformation.ProcessArchitecture);
        Console.Write('\n');

        Console.Write("ProcessId: ");
        Console.Write(Environment.ProcessId);
        Console.Write('\n');

        Console.Write("ProcessorCount: ");
        Console.Write(Environment.ProcessorCount);
        Console.Write('\n');

        Console.Write("CurrentManagedThreadId: ");
        Console.Write(Environment.CurrentManagedThreadId);
        Console.Write('\n');

        Console.Write("RuntimeVersion: ");
        Console.Write(Environment.Version);
        Console.Write('\n');

        Console.Write("OSVersion: ");
        Console.Write(Environment.OSVersion.Version);
        Console.Write('\n');

        Console.Write("OSVersionString: ");
        Console.Write(Environment.OSVersion.VersionString);
        Console.Write('\n');

        Console.Write("UserInteractive: ");
        Console.Write(Environment.UserInteractive);
        Console.Write('\n');

        Console.Write("MachineName: ");
        Console.Write(Environment.MachineName);
        Console.Write('\n');

        Console.Write("UserName: ");
        Console.Write(Environment.UserName);
        Console.Write('\n');

        Console.Write("UserDomainName: ");
        Console.Write(Environment.UserDomainName);
        Console.Write('\n');

        Console.Write("IsPrivilegedProcess: ");
        Console.Write(Environment.IsPrivilegedProcess);
        Console.Write('\n');

        Console.Write("Is64BitOperatingSystem: ");
        Console.Write(Environment.Is64BitOperatingSystem);
        Console.Write('\n');

        Console.Write("Is64BitProcess: ");
        Console.Write(Environment.Is64BitProcess);
        Console.Write('\n');

#endregion

        Console.Write('\n');
    }

    /// <summary>
    /// 获取当前运行的计算机 CPU 显示名称
    /// </summary>
    public static string CentralProcessorName => mCentralProcessorName.Value;

    static readonly Lazy<string> mCentralProcessorName = new(() =>
    {
        try
        {
            if (OperatingSystem.IsWindows())
            {
                using var registryKey = Registry.LocalMachine.OpenSubKey(
                    @"HARDWARE\DESCRIPTION\System\CentralProcessor\0\");
                return registryKey?.GetValue("ProcessorNameString")?.ToString()?.Trim() ?? string.Empty;
            }
            else if (OperatingSystem.IsLinux())
            {
                const string filePath = "/proc/cpuinfo";
                if (File.Exists(filePath))
                {
                    using var fs = File.OpenRead(filePath);
                    using var sr = new StreamReader(fs);
                    while (sr.Peek() >= 0)
                    {
                        var line = sr.ReadLine();
                        if (line == null) break;
                        var array = line.Split(':', StringSplitOptions.RemoveEmptyEntries);
                        if (array.Length == 2)
                            if (array[0].Trim() == "model name")
                                return array[1].Trim();
                    }
                }
            }
            else if (OperatingSystem.IsMacOS())
            {
                using var p = new Process();
                p.StartInfo.FileName = "/bin/bash";
                p.StartInfo.RedirectStandardOutput = true;
                p.StartInfo.RedirectStandardInput = true;
                p.StartInfo.UseShellExecute = false;
                p.Start();
                p.StandardInput.WriteLine("sysctl -n machdep.cpu.brand_string");
                p.StandardInput.Close();
                var result = p.StandardOutput.ReadToEnd();
                p.StandardOutput.Close();
                p.WaitForExit();
                p.Close();
                return result;
            }
        }
        catch
        {
        }
        return string.Empty;
    });
}

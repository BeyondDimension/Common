using System.Runtime.InteropServices;
using System.Runtime.Versioning;

// ReSharper disable once CheckNamespace
namespace BD.Common;

public static partial class ProgramHelper
{
    public static string Version { get; private set; } = string.Empty;

    /// <summary>
    /// 适用于 ASP.NET Core 6.0+ 中新的最小托管模型的代码
    /// </summary>
    /// <param name="projectName"></param>
    /// <param name="args"></param>
    /// <param name="configureServices"></param>
    /// <param name="configure"></param>
    public static void Main(
       string projectName,
       string[] args,
       Action<WebApplicationBuilder>? configureServices = null,
       Action<WebApplication>? configure = null)
    {
        var logger = NLogBuilder.ConfigureNLog(InitNLogConfig()).GetCurrentClassLogger();
        try
        {
            // https://github.com/NLog/NLog/wiki/Getting-started-with-ASP.NET-Core-6
            logger.Debug("init main");
            var builder = WebApplication.CreateBuilder(new WebApplicationOptions
            {
                Args = args,
                ContentRootPath = AppContext.BaseDirectory,
                WebRootPath = Path.Combine(AppContext.BaseDirectory, "wwwroot"),
            });
            builder.Host.UseNLog();
            configureServices?.Invoke(builder);
            var app = builder.Build();
            configure?.Invoke(app);

            Console.OutputEncoding = Encoding.Unicode; // 使用 UTF16 编码输出控制台文字
            Version = ((configureServices?.Method ?? configure?.Method)?.Module.Assembly ?? Assembly.GetCallingAssembly()).GetName().Version?.ToString() ?? string.Empty;

            // 项目代号和版本信息
            Console.WriteLine($"Project {projectName.TrimStart("Project")} [{nameof(Version)} {Version} / Runtime {Environment.Version}]{Environment.NewLine}");
            if (!string.IsNullOrEmpty(CentralProcessorName))
                Console.WriteLine($"CentralProcessorName: {CentralProcessorName} x{Environment.ProcessorCount}");
            Console.WriteLine($"LocalTime: {DateTimeOffset.Now.ToLocalTime()}");
            // 输出当前系统设置区域
            Console.WriteLine($"CurrentCulture: {CultureInfo.CurrentCulture.Name} {CultureInfo.CurrentCulture.EnglishName}");
            Console.WriteLine(string.Empty);

            app.Run();
        }
        catch (Exception exception)
        {
            //NLog: catch setup errors
            logger.Error(exception, "Stopped program because of exception");
            throw;
        }
        finally
        {
            // Ensure to flush and stop internal timers/threads before application-exit (Avoid segmentation fault on Linux)
            LogManager.Shutdown();
        }
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
                using var registryKey = Registry.LocalMachine.OpenSubKey(@"HARDWARE\DESCRIPTION\System\CentralProcessor\0\");
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
                        {
                            if (array[0].Trim() == "model name")
                            {
                                return array[1].Trim();
                            }
                        }
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
                string result = p.StandardOutput.ReadToEnd();
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

    [DefaultDllImportSearchPaths(DllImportSearchPath.System32)]
    [DllImport("libc", EntryPoint = "chmod", SetLastError = true)]
    [SupportedOSPlatform("FreeBSD")]
    [SupportedOSPlatform("Linux")]
    [SupportedOSPlatform("MacOS")]
    static extern int Chmod(string path, int mode);

    /// <summary>
    /// 初始化 NLog 配置
    /// </summary>
    /// <returns></returns>
    static LoggingConfiguration InitNLogConfig()
    {
        // https://github.com/NLog/NLog/wiki/Getting-started-with-ASP.NET-Core-6

        #region https://github.com/NLog/NLog/wiki/File-target

        /// <summary>
        /// 日志文件自动存档的字节大小
        /// </summary>
        const long archiveAboveSize = 10485760;

        /// <summary>
        /// 应保留的最大存档文件数。如果值小于或等于0，则不会删除旧文件
        /// </summary>
        const int maxArchiveFiles = 10;

        /// <summary>
        /// 应保留的存档文件的最长期限。当 archiveNumbering 无效时。如果值小于或等于 0，则不会删除旧文件
        /// </summary>
        const int maxArchiveDays = 15;

        #endregion

        if (OperatingSystem.IsLinux() || OperatingSystem.IsMacOS() || OperatingSystem.IsFreeBSD())
        {
            var logsPath = Path.Combine(AppContext.BaseDirectory, "logs");
            if (!Directory.Exists(logsPath))
                Directory.CreateDirectory(logsPath);
            _ = Chmod(logsPath, 666);
        }

        InternalLogger.LogFile = $"logs{Path.DirectorySeparatorChar}internal-nlog.txt";
        InternalLogger.LogLevel =
#if DEBUG
            NLogLevel.Info;
#else
            NLogLevel.Error;
#endif
        // enable asp.net core layout renderers
        LogManager.Setup().SetupExtensions(s => s.RegisterAssembly("NLog.Web.AspNetCore"));

        var objConfig = new LoggingConfiguration();
        // File Target for all log messages with basic details
        var allfile = new FileTarget("allfile")
        {
            FileName = $"logs{Path.DirectorySeparatorChar}nlog-all-${{shortdate}}.log",
            Layout = "${longdate}|${event-properties:item=EventId_Id}|${uppercase:${level}}|${logger}|${message} ${exception:format=tostring}",
            ArchiveAboveSize = archiveAboveSize,
            MaxArchiveFiles = maxArchiveFiles,
            MaxArchiveDays = maxArchiveDays,
        };
        objConfig.AddTarget(allfile);
        // File Target for own log messages with extra web details using some ASP.NET core renderers
        var ownFile_web = new FileTarget("ownFile-web")
        {
            FileName = $"logs{Path.DirectorySeparatorChar}nlog-own-${{shortdate}}.log",
            Layout = "${longdate}|${event-properties:item=EventId_Id}|${uppercase:${level}}|${logger}|${message} ${exception:format=tostring}|url: ${aspnet-request-url}|action: ${aspnet-mvc-action}",
            ArchiveAboveSize = archiveAboveSize,
            MaxArchiveFiles = maxArchiveFiles,
            MaxArchiveDays = maxArchiveDays,
        };
        objConfig.AddTarget(ownFile_web);
        // Console Target for hosting lifetime messages to improve Docker / Visual Studio startup detection
        var lifetimeConsole = new ConsoleTarget("lifetimeConsole")
        {
            Layout = "${level:truncate=4:tolower=true}\\: ${logger}[0]${newline}      ${message}${exception:format=tostring}",
        };
        objConfig.AddTarget(lifetimeConsole);

        var ruleMinLevel =
#if DEBUG
            NLogLevel.Trace;
#else
            NLogLevel.Error;
#endif

        // All logs, including from Microsoft
        objConfig.AddRule(ruleMinLevel, NLogLevel.Fatal, allfile, "*");
        objConfig.AddRule(ruleMinLevel, NLogLevel.Fatal, ownFile_web, "*");

        foreach (var target in objConfig.AllTargets)
        {
            // Skip non-critical Microsoft logs and so log only own logs (BlackHole)
            objConfig.AddRule(NLogLevel.Error, NLogLevel.Fatal, target, "Microsoft.*", true);
            objConfig.AddRule(NLogLevel.Error, NLogLevel.Fatal, target, "System.Net.Http.*", true);
        }

        foreach (var target in new Target[] { lifetimeConsole, ownFile_web })
        {
            // Output hosting lifetime messages to console target for faster startup detection
            objConfig.AddRule(NLogLevel.Error, NLogLevel.Fatal, target, "Microsoft.Hosting.Lifetime", true);
        }

        return objConfig;

        //        var xmlConfigStr =
        //          "<?xml version=\"1.0\" encoding=\"utf-8\" ?>" +
        //          "<nlog xmlns=\"http://www.nlog-project.org/schemas/NLog.xsd\"" +
        //          "      xmlns:xsi=\"http://www.w3.org/2001/XMLSchema-instance\"" +
        //          "      autoReload=\"true\"" +
        //          "      internalLogLevel=\"" +
        //#if DEBUG
        //          "Info"
        //#else
        //          "Error"
        //#endif
        //          + "\"" +
        //          "      internalLogFile=\"logs" + Path.DirectorySeparatorChar + "internal-nlog.txt\">" +
        //          // enable asp.net core layout renderers
        //          "  <extensions>" +
        //          "    <add assembly=\"NLog.Web.AspNetCore\"/>" +
        //          "  </extensions>" +
        //          // the targets to write to
        //          "  <targets>" +
        //          // write logs to file
        //          "    <target xsi:type=\"File\" name=\"allfile\" fileName=\"logs" + Path.DirectorySeparatorChar + "nlog-all-${shortdate}.log\"" +
        //          "            layout=\"${longdate}|${event-properties:item=EventId_Id}|${uppercase:${level}}|${logger}|${message} ${exception:format=tostring}\"/>" +
        //          // another file log, only own logs. Uses some ASP.NET core renderers
        //          "    <target xsi:type=\"File\" name=\"ownFile-web\" fileName=\"logs" + Path.DirectorySeparatorChar + "nlog-own-${shortdate}.log\"" +
        //          "            layout=\"${longdate}|${event-properties:item=EventId_Id}|${uppercase:${level}}|${logger}|${message} ${exception:format=tostring}|url: ${aspnet-request-url}|action: ${aspnet-mvc-action}\"/>" +
        //          // Console Target for hosting lifetime messages to improve Docker / Visual Studio startup detection
        //          "    <target xsi:type=\"Console\" name=\"lifetimeConsole\" layout=\"${level:truncate=4:tolower=true}\\: ${logger}[0]${newline}      ${message}${exception:format=tostring}\" />" +
        //          "  </targets>" +
        //          // rules to map from logger name to target
        //          "  <rules>" +
        //          // All logs, including from Microsoft
        //          "    <logger name=\"*\" minlevel=\"" +
        //#if DEBUG
        //          "Trace"
        //#else
        //          "Error"
        //#endif
        //          + "\" writeTo=\"allfile\"/>" +
        //          // Output hosting lifetime messages to console target for faster startup detection
        //          "    <logger name=\"Microsoft.Hosting.Lifetime\" minlevel=\"Info\" writeTo=\"lifetimeConsole, ownFile-web\" final=\"true\" />" +
        //          // Skip non-critical Microsoft logs and so log only own logs
        //          "    <logger name=\"Microsoft.*\" maxLevel=\"Info\" final=\"true\"/>" +
        //          "    <logger name=\"System.Net.Http.*\" maxlevel=\"Info\" final=\"true\" />" +
        //          // BlackHole without writeTo
        //          "    <logger name=\"*\" minlevel=\"" +
        //#if DEBUG
        //          "Trace"
        //#else
        //          "Error"
        //#endif
        //          + "\" writeTo=\"ownFile-web\"/>" +
        //          "  </rules>" +
        //          "</nlog>";

        //        var xmlConfig = XmlLoggingConfiguration.CreateFromXmlString(xmlConfigStr);

        //        return xmlConfig;
    }
}

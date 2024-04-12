namespace BD.Common8.AspNetCore.Helpers;

public static partial class ProgramHelper
{
    /// <summary>
    /// 当前 Program 版本号
    /// </summary>
    public static string Version { get; private set; } = string.Empty;

    /// <summary>
    /// 适用于 ASP.NET Core 6.0+ 中新的最小托管模型的代码
    /// </summary>
    /// <param name="projectName"></param>
    /// <param name="args"></param>
    /// <param name="configureServices"></param>
    /// <param name="configure"></param>
    /// <param name="builder"></param>
    /// <param name="jsonTypeInfoResolver"></param>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static unsafe void Main(
       string projectName,
       string[] args,
       delegate* managed<WebApplicationBuilder, void> configureServices = default,
       delegate* managed<WebApplication, void> configure = default,
       WebApplicationBuilder? builder = default,
       IJsonTypeInfoResolver? jsonTypeInfoResolver = default)
    {
        Assembly? callingAssembly = default;
        try
        {
            callingAssembly = Assembly.GetCallingAssembly();
        }
        catch
        {
        }

        var logger = NLogManager.Setup()
                                .RegisterNLogWeb()
                                .LoadConfiguration(InitNLogConfig())
                                .GetCurrentClassLogger();
        try
        {
            // https://github.com/NLog/NLog/wiki/Getting-started-with-ASP.NET-Core-6
            logger.Debug("init main");
            builder ??= WebApplication.CreateBuilder(new WebApplicationOptions
            {
                Args = args,
                ContentRootPath = AppContext.BaseDirectory,
                WebRootPath = Path.Combine(AppContext.BaseDirectory, "wwwroot"),
            });
            builder.WebHost.ConfigureKestrel(static o =>
            {
                o.AddServerHeader = false;
            });
            if (jsonTypeInfoResolver != default)
            {
                builder.Services.ConfigureHttpJsonOptions(options =>
                {
                    options.SerializerOptions.TypeInfoResolverChain.Insert(0, jsonTypeInfoResolver);
                });
            }
            builder.Host.UseNLog();
            if (configureServices != default)
                configureServices(builder);
            var app = builder.Build();
            InitFileSystem(app.Environment);
            Ioc.ConfigureServices(app.Services);
            if (configure != default)
                configure(app);

            Console.OutputEncoding = Encoding.Unicode; // 使用 UTF16 编码输出控制台文字
            Version = callingAssembly?.GetName()?.Version?.ToString() ?? string.Empty;

            #region 项目代号和版本信息

            Console.Write("Project ");
            Console.Write(projectName.TrimStart("Project"));
            const string version_f = $" [{nameof(Version)} ";
            Console.Write(version_f);
            Console.Write(Version);
            Console.Write(" / Runtime ");
            Console.Write(Environment.Version);
            Console.Write(']');
            Console.WriteLine();
            Console.WriteLine();

            #endregion

            #region 当前运行的计算机 CPU 显示名称

            if (!string.IsNullOrEmpty(CentralProcessorName))
            {
                Console.Write("CentralProcessorName: ");
                Console.Write(CentralProcessorName);
                Console.Write(" x");
                Console.Write(Environment.ProcessorCount);
                Console.WriteLine();
            }

            #endregion

            #region 本地时间与当前系统设置区域

            Console.Write("LocalTime: ");
            Console.Write(DateTimeOffset.Now.ToLocalTime());
            Console.WriteLine();

            Console.Write("CurrentCulture: ");
            Console.Write(CultureInfo.CurrentCulture.Name);
            Console.Write(' ');
            Console.Write(CultureInfo.CurrentCulture.EnglishName);
            Console.WriteLine();

            #endregion

            #region ShowInfo

            Console.Write("BaseDirectory: ");
            Console.WriteLine(AppContext.BaseDirectory);

            Console.Write("OSArchitecture: ");
            Console.WriteLine(RuntimeInformation.OSArchitecture);

            Console.Write("ProcessArchitecture: ");
            Console.WriteLine(RuntimeInformation.ProcessArchitecture);

            Console.Write("ProcessId: ");
            Console.WriteLine(Environment.ProcessId);

            Console.Write("ProcessorCount: ");
            Console.WriteLine(Environment.ProcessorCount);

            Console.Write("CurrentManagedThreadId: ");
            Console.WriteLine(Environment.CurrentManagedThreadId);

            Console.Write("RuntimeVersion: ");
            Console.WriteLine(Environment.Version);

            Console.Write("OSVersion: ");
            Console.WriteLine(Environment.OSVersion.Version);

            Console.Write("OSVersionString: ");
            Console.WriteLine(Environment.OSVersion.VersionString);

            Console.Write("UserInteractive: ");
            Console.WriteLine(Environment.UserInteractive);

            Console.Write("MachineName: ");
            Console.WriteLine(Environment.MachineName);

            Console.Write("UserName: ");
            Console.WriteLine(Environment.UserName);

            Console.Write("UserDomainName: ");
            Console.WriteLine(Environment.UserDomainName);

            Console.Write("IsPrivilegedProcess: ");
            Console.WriteLine(Environment.IsPrivilegedProcess);

            Console.Write("Is64BitOperatingSystem: ");
            Console.WriteLine(Environment.Is64BitOperatingSystem);

            Console.Write("Is64BitProcess: ");
            Console.WriteLine(Environment.Is64BitProcess);

            #endregion

            Console.WriteLine();

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
            NLogManager.Shutdown();
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

    [DefaultDllImportSearchPaths(DllImportSearchPath.System32)]
    [LibraryImport("libc", EntryPoint = "chmod", SetLastError = true, StringMarshalling = StringMarshalling.Utf16)]
    [SupportedOSPlatform("FreeBSD")]
    [SupportedOSPlatform("Linux")]
    [SupportedOSPlatform("MacOS")]
    private static partial int Chmod(string path, int mode);

    [Flags]
    [SupportedOSPlatform("FreeBSD")]
    [SupportedOSPlatform("Linux")]
    [SupportedOSPlatform("MacOS")]
    enum EUnixPermission : ushort
    {
        OtherExecute = 0x1,
        OtherWrite = 0x2,
        OtherRead = 0x4,
        GroupExecute = 0x8,
        GroupWrite = 0x10,
        GroupRead = 0x20,
        UserExecute = 0x40,
        UserWrite = 0x80,
        UserRead = 0x100,
        Combined777 = UserRead | UserWrite | UserExecute | GroupRead | GroupWrite | GroupExecute | OtherRead | OtherWrite | OtherExecute,
    }

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
    const int maxArchiveDays = 31;

    #endregion

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    static void CreateDirectory(string dirPath)
    {
        if (!Directory.Exists(dirPath))
            Directory.CreateDirectory(dirPath);
        if (OperatingSystem.IsLinux() || OperatingSystem.IsMacOS() || OperatingSystem.IsFreeBSD())
        {
            _ = Chmod(dirPath, (int)(EUnixPermission.UserRead | EUnixPermission.UserWrite | EUnixPermission.GroupRead | EUnixPermission.GroupWrite | EUnixPermission.OtherRead | EUnixPermission.OtherWrite));
        }
    }

    /// <summary>
    /// 初始化 NLog 配置
    /// </summary>
    /// <returns></returns>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    static LoggingConfiguration InitNLogConfig()
    {
        // https://github.com/NLog/NLog/wiki/Getting-started-with-ASP.NET-Core-6

        var logsPath = Path.Combine(AppContext.BaseDirectory, "logs");
        CreateDirectory(logsPath);

        NInternalLogger.LogFile = $"logs{Path.DirectorySeparatorChar}internal-nlog.txt";
        NInternalLogger.LogLevel =
#if DEBUG
            NLogLevel.Info;
#else
            NLogLevel.Error;
#endif
        // enable asp.net core layout renderers
        NLogManager.Setup().SetupExtensions(s => s.RegisterAssembly("NLog.Web.AspNetCore"));

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
            // Output hosting lifetime messages to console target for faster startup detection
            objConfig.AddRule(NLogLevel.Error, NLogLevel.Fatal, target, "Microsoft.Hosting.Lifetime", true);

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

    /// <inheritdoc cref="IOPath.InitFileSystem(Func{string}, Func{string})"/>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static void InitFileSystem(IWebHostEnvironment environment)
    {
        var appDataDirectory = Path.Combine(environment.ContentRootPath, "AppData");
        CreateDirectory(appDataDirectory);
        var cacheDirectory = Path.Combine(environment.ContentRootPath, "Cache");
        CreateDirectory(cacheDirectory);
        IOPath.InitFileSystem(() => appDataDirectory, () => cacheDirectory);
    }
}

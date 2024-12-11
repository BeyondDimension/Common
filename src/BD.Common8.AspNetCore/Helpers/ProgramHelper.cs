namespace BD.Common8.AspNetCore.Helpers;

using Http_JsonOptions = Microsoft.AspNetCore.Http.Json.JsonOptions;
using Mvc_JsonOptions = Microsoft.AspNetCore.Mvc.JsonOptions;

public static partial class ProgramHelper
{
    /// <summary>
    /// https://github.com/dotnet/aspnetcore/blob/v9.0.0/src/Http/Http.Extensions/src/JsonOptions.cs#L34
    /// </summary>
    /// <param name="this"></param>
    /// <param name="value"></param>
    static void SetSerializerOptions(Http_JsonOptions @this, SystemTextJsonSerializerOptions value)
    {
        ref var source = ref UnsafeAccessJsonSerializerOptions(@this);

        source = value;
        //@this.GetType().GetField("<SerializerOptions>k__BackingField", BindingFlags.NonPublic | BindingFlags.Instance)!.SetValue(@this, value);
    }

    /// <summary>
    /// https://github.com/dotnet/aspnetcore/blob/v9.0.0/src/Mvc/Mvc.Core/src/JsonOptions.cs#L35
    /// </summary>
    /// <param name="this"></param>
    /// <param name="value"></param>
    static void SetSerializerOptions(Mvc_JsonOptions @this, SystemTextJsonSerializerOptions value)
    {
        ref var source = ref UnsafeAccessJsonSerializerOptions(@this);

        source = value;

        //@this.GetType().GetField("<JsonSerializerOptions>k__BackingField", BindingFlags.NonPublic | BindingFlags.Instance)!.SetValue(@this, value);
    }

    [UnsafeAccessor(UnsafeAccessorKind.Field, Name = "<SerializerOptions>k__BackingField")]
    static extern ref SystemTextJsonSerializerOptions UnsafeAccessJsonSerializerOptions(Http_JsonOptions opt);

    [UnsafeAccessor(UnsafeAccessorKind.Field, Name = "<JsonSerializerOptions>k__BackingField")]
    static extern ref SystemTextJsonSerializerOptions UnsafeAccessJsonSerializerOptions(Mvc_JsonOptions opt);

    /// <summary>
    /// 适用于 ASP.NET Core 6.0+ 中新的最小托管模型的代码
    /// </summary>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static unsafe void Main(
       string projectName,
       string[] args,
       delegate* managed<WebApplicationBuilder, void> configureServices = default,
       delegate* managed<WebApplication, void> configure = default,
       WebApplicationBuilder? builder = default,
       IJsonTypeInfoResolver? jsonTypeInfoResolver = default,
       bool consoleWriteInfo = false,
       bool camelCase = false)
    {
        if (!string.IsNullOrWhiteSpace(projectName))
            ProjectName = projectName;

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
            if (jsonTypeInfoResolver != null)
            {
                if (jsonTypeInfoResolver is SystemTextJsonSerializerContext jsc)
                {
                    // camelCase 默认值为 false，将 JSON 属性名设置为大写开头与 C# 保持一致，历史问题，之前的前端 JS/TS 部分已经使用大写硬编码
                    // 如果需按现在 Web 规范，将此值设置为 true，则 JSON 属性名将会使用小写开头驼峰命名法
                    var jsonSerializerOptions = Serializable.CreateOptions(jsc.Options,
                        camelCase: camelCase);
                    builder.Services.ConfigureHttpJsonOptions(options =>
                    {
                        // 替换 WebApi 的 Json 序列化选项
                        SetSerializerOptions(options, jsonSerializerOptions);
                    });
                    // https://github.com/dotnet/aspnetcore/blob/v9.0.0/src/Mvc/Mvc.Core/src/DependencyInjection/MvcCoreMvcBuilderExtensions.cs#L51
                    builder.Services.Configure<Mvc_JsonOptions>(options =>
                    {
                        // 替换 Mvc 的 Json 序列化选项
                        SetSerializerOptions(options, jsonSerializerOptions);
                    });
                }
                else
                {
                    builder.Services.ConfigureHttpJsonOptions(options =>
                    {
                        options.SerializerOptions.TypeInfoResolverChain.Insert(0, jsonTypeInfoResolver);
                    });
                }
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
            CalcVersion(callingAssembly);

            if (consoleWriteInfo)
                ConsoleWriteInfo(projectName);

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
    private enum EUnixPermission : ushort
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

    #endregion https://github.com/NLog/NLog/wiki/File-target

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
#pragma warning disable CA1050 // 在命名空间中声明类型
#pragma warning disable IDE0005 // Using directive is unnecessary.
#pragma warning disable CS0649 // Field 'field' is never assigned to, and will always have its default value 'value'

global using static ProjectUtils;

/// <summary>
/// 项目工具类
/// </summary>
public static partial class ProjectUtils
{
    #region ProjPath/ROOT_ProjPath

#if !NET35
    static readonly Lazy<string[]> _ProjPath = new(static () =>
    {
        var lProjPath = GetProjectPath();
        var lROOT_ProjPath = lProjPath;
#if (NETFRAMEWORK && NET40_OR_GREATER) || !NETFRAMEWORK
        if (!string.IsNullOrWhiteSpace(lROOT_ProjPath))
        {
            var mROOT_ProjPath = lROOT_ProjPath;
            var mROOT_ProjPath2 = mROOT_ProjPath;
            while (true)
            {
                mROOT_ProjPath = Path.Combine(mROOT_ProjPath, "..");
                mROOT_ProjPath = GetProjectPath(mROOT_ProjPath);
                if (string.IsNullOrWhiteSpace(mROOT_ProjPath))
                {
                    lROOT_ProjPath = mROOT_ProjPath2;
                    break;
                }
                mROOT_ProjPath2 = mROOT_ProjPath;
            }
        }
#endif
        return [lProjPath, lROOT_ProjPath];
    }, LazyThreadSafetyMode.ExecutionAndPublication);
#endif

    /// <summary>
    /// 当前项目绝对路径
    /// </summary>
#if NET35
    public static readonly string ProjPath = GetProjectPath();
#else
    public static string ProjPath => _ProjPath.Value[0];
#endif

    /// <summary>
    /// 当前项目的顶级绝对路径（通常作为子模块返回仓库的项目路径）
    /// </summary>
    public static string ROOT_ProjPath =>
#if NET35
        ProjPath;
#else
        _ProjPath.Value[1];
#endif

    #endregion

    #region IsCI/DataPath

    /// <summary>
    /// 判断当前是否在 CI 中运行
    /// </summary>
    /// <returns></returns>
    public static bool IsCI()
#if (NETFRAMEWORK && NET40_OR_GREATER) || !NETFRAMEWORK
        => _DataPath.Value.IsCI;

    static readonly Lazy<(string DataPath, bool IsCI)> _DataPath = new(static () =>
    {
        string mDataPath = "";
        bool contains_actions_runner = false;
        if (!string.IsNullOrWhiteSpace(ROOT_ProjPath))
        {
            contains_actions_runner = ROOT_ProjPath.Contains("actions-runner");
            mDataPath = contains_actions_runner ? Path.Combine(ROOT_ProjPath, "..", "..") : Path.Combine(ROOT_ProjPath, "..");
            mDataPath = Path.GetFullPath(mDataPath);
        }

        // https://docs.github.com/en/actions/learn-github-actions/variables#default-environment-variables
        var isCI = contains_actions_runner || (bool.TryParse(Environment.GetEnvironmentVariable("CI"), out var result) && result);
        return (mDataPath, isCI);
    }, LazyThreadSafetyMode.ExecutionAndPublication);

    /// <summary>
    /// 用于测试的数据存储的路径
    /// </summary>
    public static string DataPath => _DataPath.Value.DataPath;
#else
        => false;
#endif

    #endregion

    #region tfm/tfm_

#pragma warning disable SA1307 // Accessible fields should begin with upper-case letter
    /// <summary>
    /// 当前目标框架 TFM
    /// </summary>
    public static readonly string tfm =
#pragma warning restore SA1307 // Accessible fields should begin with upper-case letter
        $"net{Environment.Version.Major}.{Environment.Version.Minor}{tfm_}";

    /// <summary>
    /// 当前目标框架 TFM 后缀
    /// </summary>
    public const string tfm_ =
#if WINDOWS
    "-windows10.0.19041.0";
#elif LINUX
    "";
#elif MACCATALYST
    "-maccatalyst";
#elif MACOS
    "-macos";
#else
    "";
#endif

    #endregion

    #region Methods

    /// <summary>
    /// 获取当前项目绝对路径(.sln文件所在目录)
    /// </summary>
    /// <param name="path"></param>
    /// <returns></returns>
    public static string GetProjectPath(string? path = null)
    {
        path ??=
#if NET46_OR_GREATER || NETCOREAPP
        AppContext.BaseDirectory;
#else
        AppDomain.CurrentDomain.BaseDirectory;
#endif
        try
        {
#pragma warning disable IDE0079 // 请删除不必要的忽略
#pragma warning disable RS1035 // 不要使用禁用于分析器的 API
#pragma warning disable SA1003 // Symbols should be spaced correctly
#pragma warning disable SA1008 // Opening parenthesis should be spaced correctly
#pragma warning disable SA1110 // Opening parenthesis or bracket should be on declaration line
            if (!
#pragma warning restore SA1003 // Symbols should be spaced correctly
#if NET35
                Directory.GetFiles
#else
                Directory.EnumerateFiles
#endif
                (path, "*.sln").Any())
            {
                var parent = Directory.GetParent(path);
                if (parent == null)
                    return string.Empty;
                return GetProjectPath(parent.FullName);
            }
#pragma warning restore SA1110 // Opening parenthesis or bracket should be on declaration line
#pragma warning restore SA1008 // Opening parenthesis should be spaced correctly
#pragma warning restore RS1035 // 不要使用禁用于分析器的 API
#pragma warning restore IDE0079 // 请删除不必要的忽略
        }
        catch
        {
            return string.Empty;
        }
        return path;
    }

#if NET7_0_OR_GREATER && !NO_REF_COMMON8_BCL
    /// <summary>
    /// 根据类型生成随机值，用于模拟的假数据
    /// </summary>
    /// <param name="type"></param>
    /// <returns></returns>
    public static object? GeneratorRandomValueByType(Type type)
    {
        if (type == typeof(int))
        {
            return Random.Shared.Next(int.MaxValue);
        }
        else if (type == typeof(char))
        {
            return (char)Random.Shared.Next(char.MinValue, char.MaxValue + 1);
        }
        else if (type == typeof(byte))
        {
            return (byte)Random.Shared.Next(byte.MinValue, byte.MaxValue + 1);
        }
        else if (type == typeof(sbyte))
        {
            return (sbyte)Random.Shared.Next(sbyte.MinValue, sbyte.MaxValue + 1);
        }
        else if (type == typeof(DateOnly))
        {
            return DateOnly.FromDateTime(DateTime.Today);
        }
        else if (type == typeof(DateTime))
        {
            return DateTime.Now;
        }
        else if (type == typeof(DateTimeOffset))
        {
            return DateTimeOffset.Now;
        }
        else if (type == typeof(decimal))
        {
            return (decimal)Random.Shared.NextDouble();
        }
        else if (type == typeof(double))
        {
            return Random.Shared.NextDouble();
        }
        else if (type.IsEnum)
        {
            var enums = Enum.GetValues(type);
            return enums.GetValue(Random.Shared.Next(enums.Length));
        }
        else if (type == typeof(Guid))
        {
            return Guid.NewGuid();
        }
        else if (type == typeof(short))
        {
            return (short)Random.Shared.Next(short.MinValue, short.MaxValue + 1);
        }
        else if (type == typeof(int))
        {
            return (int)Random.Shared.NextInt64(int.MinValue, int.MaxValue + 1L);
        }
        else if (type == typeof(long))
        {
            return Random.Shared.NextInt64(long.MinValue, long.MaxValue);
        }
        else if (type == typeof(float))
        {
            return Random.Shared.NextSingle();
        }
        else if (type == typeof(TimeOnly))
        {
            return TimeOnly.FromDateTime(DateTime.Now);
        }
        else if (type == typeof(TimeSpan))
        {
            return TimeSpan.FromSeconds(Random.Shared.Next(30, ushort.MaxValue));
        }
        else if (type == typeof(ushort))
        {
            return (ushort)Random.Shared.Next(ushort.MinValue, ushort.MaxValue + 1);
        }
        else if (type == typeof(uint))
        {
            return (uint)Random.Shared.NextInt64(uint.MinValue, uint.MaxValue + 1L);
        }
        else if (type == typeof(ulong))
        {
            return (ulong)Random.Shared.NextInt64(0, long.MaxValue);
        }
        else if (type == typeof(Uri))
        {
            return new Uri($"http://{Random2.GenerateRandomString()}.com");
        }
        else if (type == typeof(Version))
        {
            return new Version($"{Random2.GenerateRandomNum(1, true)}.{Random2.GenerateRandomNum(1)}.{Random2.GenerateRandomNum(5)}");
        }
        else if (type == typeof(CancellationToken))
        {
            return CancellationToken.None;
        }
        else if (type == typeof(string))
        {
            return Random2.GenerateRandomString();
        }
        else
        {
            if (type.IsClass)
            {
                return null;
            }
            else
            {
                try
                {
                    return Activator.CreateInstance(type);
                }
                catch
                {
                    return null;
                }
            }
        }
    }
#endif

    #endregion
}
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

    static IEnumerable<string> EnumerateSlnFiles(string path)
    {
#if NET35
        return Directory.GetFiles(path, "*.sln").Concat(Directory.GetFiles(path, "*.slnx"));
#else
        return Directory.EnumerateFiles(path, "*.sln").Concat(Directory.EnumerateFiles(path, "*.slnx"));
#endif
    }

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
            if (!EnumerateSlnFiles(path).Any())
#pragma warning restore SA1003 // Symbols should be spaced correctly
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

#if NET7_0_OR_GREATER

#if !NET6_0_OR_GREATER
    [ThreadStatic]
    static Random? _local;

    /// <summary>
    /// 提供可从任何线程并发使用的线程安全 Random 实例
    /// </summary>
    public static Random Shared()
    {
        var inst = _local;
        if (inst == null)
        {
            //byte[] buffer = new byte[4];
            //_global.GetBytes(buffer);
            //_local = inst = new Random(BitConverter.ToInt32(buffer, 0));

            // GUID 生成随机数性能比 RNGCryptoServiceProvider 更好
            _local = inst = new Random(Guid.NewGuid().GetHashCode());
        }
        return inst;
    }

#else

    // https://github.com/dotnet/runtime/blob/v6.0.6/src/libraries/System.Private.CoreLib/src/System/Random.cs#L52
    // https://github.com/dotnet/runtime/blob/v6.0.6/src/libraries/System.Private.CoreLib/src/System/Random.cs#L220

    /// <inheritdoc cref="Random.Shared"/>
    public static Random Shared() => Random.Shared;

#endif

    /// <summary>
    /// 数字
    /// </summary>
    public const string Digits = "0123456789";

    /// <summary>
    /// 大写字母
    /// </summary>
    public const string UpperCaseLetters = "ABCDEFGHIJKLMNOPQRSTUVWXYZ";

    /// <summary>
    /// 小写字母
    /// </summary>
    public const string LowerCaseLetters = "abcdefghijklmnopqrstuvwxyz";

    /// <summary>
    /// 字母
    /// </summary>
    public const string Letters = LowerCaseLetters + UpperCaseLetters;

    /// <summary>
    /// 数字与字母
    /// </summary>
    public const string DigitsLetters = Digits + Letters;

    /// <summary>
    /// 生成随机字符串，长度为固定传入字符串
    /// </summary>
    /// <param name="length">要生成的字符串长度</param>
    /// <param name="randomChars">随机字符串字符集</param>
    /// <returns></returns>
    public static string GenerateRandomString(int length = 6,
        string randomChars = DigitsLetters)
    {
        var random = Shared();
        var result = new char[length];
        if (random.Next(256) % 2 == 0)
            for (var i = length - 1; i >= 0; i--) // 5 4 3 2 1 0
                EachGenerate(i);
        else
            for (var i = 0; i < length; i++) // 0 1 2 3 4 5
                EachGenerate(i);
        return new string(result);
        void EachGenerate(int i)
        {
            var index = random.Next(0, randomChars.Length);
            var temp = RandomCharAt(randomChars, index);
            static char RandomCharAt(string s, int index)
            {
                if (index == s.Length) index = 0;
                else if (index > s.Length) index %= s.Length;
                return s[index];
            }
            result[i] = temp;
        }
    }

    /// <summary>
    /// 生成随机数字，长度为固定传入参数
    /// </summary>
    /// <param name="length">要生成的字符串长度</param>
    /// <param name="endIsZero">生成的数字最后一位是否能够为0，默认不能为0( <see langword="false"/> )</param>
    /// <returns></returns>
    public static int GenerateRandomNum(int length = 6, bool endIsZero = false)
    {
        if (length > 11) length = 11;
        var random = Shared();
        var result = 0;
        var lastNum = 0;
        if (random.Next(256) % 2 == 0)
            for (int i = length - 1; i >= 0; i--) // 5 4 3 2 1 0
                EachGenerate(i);
        else
            for (int i = 0; i < length; i++) // 0 1 2 3 4 5
                EachGenerate(i);
        return result;
        void EachGenerate(int i)
        {
            var bit = (int)(i == 0 ? 1 : Math.Pow(10, i));
            // 100,000  10,000  1,000   100     10      1
            // 1        10      100     1,000   10,000  100,000
            var current = random.Next(lastNum + 1, lastNum + 10);
            lastNum = current % 10;
            if (lastNum == 0)
            {
                // i != 0 &&  i!=5 末尾和开头不能有零
                if ((i != 0 || endIsZero) && i != length - 1)
                    return;
                lastNum = random.Next(1, 10);
            }
            result += lastNum * bit;
        }
    }

    /// <summary>
    /// 根据类型生成随机值，用于模拟的假数据
    /// </summary>
    /// <param name="type"></param>
    /// <returns></returns>
    public static object? GeneratorRandomValueByType(Type type)
    {
        if (type == typeof(int))
        {
            return Shared().Next(int.MaxValue);
        }
        else if (type == typeof(char))
        {
            return (char)Shared().Next(char.MinValue, char.MaxValue + 1);
        }
        else if (type == typeof(byte))
        {
            return (byte)Shared().Next(byte.MinValue, byte.MaxValue + 1);
        }
        else if (type == typeof(sbyte))
        {
            return (sbyte)Shared().Next(sbyte.MinValue, sbyte.MaxValue + 1);
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
            return (decimal)Shared().NextDouble();
        }
        else if (type == typeof(double))
        {
            return Shared().NextDouble();
        }
        else if (type.IsEnum)
        {
            var enums = Enum.GetValues(type);
            return enums.GetValue(Shared().Next(enums.Length));
        }
        else if (type == typeof(Guid))
        {
            return Guid.NewGuid();
        }
        else if (type == typeof(short))
        {
            return (short)Shared().Next(short.MinValue, short.MaxValue + 1);
        }
        else if (type == typeof(int))
        {
            return (int)Shared().NextInt64(int.MinValue, int.MaxValue + 1L);
        }
        else if (type == typeof(long))
        {
            return Shared().NextInt64(long.MinValue, long.MaxValue);
        }
        else if (type == typeof(float))
        {
            return Shared().NextSingle();
        }
        else if (type == typeof(TimeOnly))
        {
            return TimeOnly.FromDateTime(DateTime.Now);
        }
        else if (type == typeof(TimeSpan))
        {
            return TimeSpan.FromSeconds(Shared().Next(30, ushort.MaxValue));
        }
        else if (type == typeof(ushort))
        {
            return (ushort)Shared().Next(ushort.MinValue, ushort.MaxValue + 1);
        }
        else if (type == typeof(uint))
        {
            return (uint)Shared().NextInt64(uint.MinValue, uint.MaxValue + 1L);
        }
        else if (type == typeof(ulong))
        {
            return (ulong)Shared().NextInt64(0, long.MaxValue);
        }
        else if (type == typeof(Uri))
        {
            return new Uri($"http://{GenerateRandomString()}.com");
        }
        else if (type == typeof(Version))
        {
            return new Version($"{GenerateRandomNum(1, true)}.{GenerateRandomNum(1)}.{GenerateRandomNum(5)}");
        }
        else if (type == typeof(CancellationToken))
        {
            return CancellationToken.None;
        }
        else if (type == typeof(string))
        {
            return GenerateRandomString();
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
                    // Test/Debug Only
#pragma warning disable IL2067 // Target parameter argument does not satisfy 'DynamicallyAccessedMembersAttribute' in call to target method. The parameter of method does not have matching annotations.
                    return Activator.CreateInstance(type);
#pragma warning restore IL2067 // Target parameter argument does not satisfy 'DynamicallyAccessedMembersAttribute' in call to target method. The parameter of method does not have matching annotations.
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
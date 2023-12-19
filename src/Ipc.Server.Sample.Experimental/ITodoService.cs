#pragma warning disable CA1050 // 在命名空间中声明类型
#pragma warning disable SA1600 // Elements should be documented

/// <summary>
/// 模拟业务服务
/// </summary>
public partial interface ITodoService
{
    /// <summary>
    /// 获取所有模型数据
    /// </summary>
    /// <param name="cancellationToken"></param>
    /// <returns></returns>
    Task<ApiRspImpl<Todo[]?>> All(CancellationToken cancellationToken = default);

    /// <summary>
    /// 根据 Id 获取模型数据
    /// </summary>
    /// <param name="id"></param>
    /// <param name="cancellationToken"></param>
    /// <returns></returns>
    Task<ApiRspImpl<Todo?>> GetById(int id, CancellationToken cancellationToken = default);

    /// <summary>
    /// 测试 https://learn.microsoft.com/zh-cn/aspnet/core/mvc/models/model-binding?view=aspnetcore-8.0#simple-types 路由绑定简单类型
    /// <list type="bullet">
    /// <item>日期时间类型需要使用往返（“O”、“o”）格式</item>
    /// <item>https://learn.microsoft.com/zh-cn/dotnet/standard/base-types/standard-date-and-time-format-strings#the-round-trip-o-o-format-specifier</item>
    /// <item>“O”或“o”标准格式说明符表示使用保留时区信息的模式的自定义日期和时间格式字符串，并发出符合 ISO8601 的结果字符串。</item>
    /// <item>对于 DateTime 值，“O”或“o”标准格式说明符对应于“yyyy'-'MM'-'dd'T'HH':'mm':'ss'.'fffffffK”自定义格式字符串，对于 DateTimeOffset 值，“O”或“o”标准格式说明符则对应于“yyyy'-'MM'-'dd'T'HH':'mm':'ss'.'fffffffzzz”自定义格式字符串。 在此字符串中，分隔各个字符（例如连字符、冒号和字母“T”）的单引号标记对指示各个字符是不能更改的文本。 撇号不会出现在输出字符串中。</item>
    /// <item>System.Text.Json 中的 DateTime 和 DateTimeOffset 支持 ISO 8601-1:2019 格式</item>
    /// <item>https://learn.microsoft.com/zh-cn/dotnet/standard/datetime/system-text-json-support</item>
    /// <item>https://github.com/dotnet/runtime/blob/v8.0.0/src/libraries/System.Text.Json/src/System/Text/Json/JsonHelpers.Date.cs</item>
    /// </list>
    /// </summary>
    /// <param name="p0"></param>
    /// <param name="p1"></param>
    /// <param name="p2"></param>
    /// <param name="p3"></param>
    /// <param name="p4"></param>
    /// <param name="p5"></param>
    /// <param name="p6"></param>
    /// <param name="p7"></param>
    /// <param name="p8"></param>
    /// <param name="p9"></param>
    /// <param name="p10"></param>
    /// <param name="p11"></param>
    /// <param name="p12"></param>
    /// <param name="p13"></param>
    /// <param name="p14"></param>
    /// <param name="p15"></param>
    /// <param name="p16"></param>
    /// <param name="p17"></param>
    /// <param name="p18"></param>
    /// <param name="p19"></param>
    /// <param name="p20"></param>
    /// <param name="p21"></param>
    /// <param name="cancellationToken"></param>
    /// <returns></returns>
    Task<ApiRspImpl> SimpleTypes(bool p0, byte p1, sbyte p2,
        char p3, DateOnly p4, DateTime p5,
        DateTimeOffset p6, decimal p7, double p8,
        ProcessorArchitecture p9, Guid p10, short p11,
        int p12, long p13, float p14,
        TimeOnly p15, TimeSpan p16, ushort p17,
        uint p18, ulong p19, Uri p20,
        Version p21, CancellationToken cancellationToken = default);

    /// <summary>
    /// 使用 Request Body 的接口案例
    /// </summary>
    /// <param name="todo"></param>
    /// <param name="cancellationToken"></param>
    /// <returns></returns>
    /// <exception cref="NotImplementedException"></exception>
    Task<ApiRspImpl> BodyTest(Todo todo, CancellationToken cancellationToken = default)
    {
        throw new NotImplementedException();
    }

    /// <summary>
    /// 异步迭代器案例
    /// </summary>
    /// <param name="len"></param>
    /// <param name="cancellationToken"></param>
    /// <returns></returns>
    IAsyncEnumerable<Todo> AsyncEnumerable(int len, CancellationToken cancellationToken = default);

    /// <summary>
    /// 测试使用 <see cref="Tuple{T1, T2, T3, T4, T5, T6, T7, TRest}"/>
    /// </summary>
    /// <param name="p0"></param>
    /// <param name="p1"></param>
    /// <param name="p2"></param>
    /// <param name="p3"></param>
    /// <param name="p4"></param>
    /// <param name="p5"></param>
    /// <param name="p6"></param>
    /// <param name="p7"></param>
    /// <param name="p8"></param>
    /// <param name="p9"></param>
    /// <param name="p10"></param>
    /// <param name="p11"></param>
    /// <param name="p12"></param>
    /// <param name="p13"></param>
    /// <param name="p14"></param>
    /// <param name="p15"></param>
    /// <param name="p16"></param>
    /// <param name="p17"></param>
    /// <param name="p18"></param>
    /// <param name="p19"></param>
    /// <param name="p20"></param>
    /// <param name="p21"></param>
    /// <param name="cancellationToken"></param>
    /// <returns></returns>
    Task<ApiRspImpl> Tuple(bool p0, byte p1, sbyte p2,
        char p3, DateOnly p4, DateTime p5,
        DateTimeOffset p6, decimal p7, double p8,
        ProcessorArchitecture[] p9, Guid p10, short p11,
        int p12, long p13, float p14,
        TimeOnly p15, TimeSpan p16, ushort p17,
        uint p18, ulong[] p19, Uri p20, CancellationToken cancellationToken = default);
}

/// <summary>
/// 模拟的业务模型类
/// </summary>
/// <param name="Id"></param>
/// <param name="Title"></param>
/// <param name="DueBy"></param>
/// <param name="IsComplete"></param>
public record Todo(int Id, string? Title, DateOnly? DueBy = null, bool IsComplete = false);

/// <summary>
/// 示例路径助手类
/// </summary>
public static class SamplePathHelper
{
    /// <summary>
    /// 定义缓存文件夹
    /// </summary>
    public static readonly string TempDirPath =
        Path.Combine(ProjPath, "src", "artifacts", "temp", "Ipc.Sample.Experimental");

    public const string ConnectionStringsFileName = "IpcAppConnectionStrings.tmp";

    public static string ConnectionStringsFilePath =>
        Path.Combine(TempDirPath, ConnectionStringsFileName);

    public static IpcAppConnectionString[]? GetConnectionStrings()
    {
        try
        {
            var buffer = File.ReadAllBytes(ConnectionStringsFilePath);
            var buffer2 = Serializable.DMP2<byte[][]>(buffer);
            return buffer2.ThrowIsNull().Select(static x =>
            {
                IpcAppConnectionString? r = x;
                return r;
            }).Where(static x => x.HasValue).Select(static x => x!.Value).ToArray();
        }
        catch
        {
            return null;
        }
    }

    public static void SetConnectionStrings(params IpcAppConnectionString[] connectionStrings)
    {
        var buffer2 = connectionStrings.Select(static x =>
        {
            byte[] r = x;
            return r;
        }).ToArray();
        byte[] buffer = Serializable.SMP2(buffer2);
        IOPath.DirCreateByNotExists(TempDirPath);
        File.WriteAllBytes(ConnectionStringsFilePath, buffer);
    }

    const string X500DistinguishedCNName = "BeyondDimension Console Test Certificate";

    const string X500DistinguishedName =
       $"C=CN, O=BeyondDimension, OU=Technical Department, CN={X500DistinguishedCNName}";

    static readonly Lazy<X509Certificate2> _RootCertificate = new(() =>
    {
        X509Certificate2? certificate = null;
        var filePath = Path.Combine(TempDirPath, "IpcRootCertificate.pfx");

        if (Directory.Exists(TempDirPath))
        {
            if (File.Exists(filePath))
            {
                try
                {
                    certificate = new X509Certificate2(filePath);
                    return certificate;
                }
                catch
                {
                    IOPath.FileTryDelete(filePath);
                }
            }
        }
        else
        {
            Directory.CreateDirectory(TempDirPath);
        }

        certificate = X509Certificate2Generator.CreateRootCertificate(new()
        {
            DistinguishedNameString = X500DistinguishedName,
            SubjectAlternativeName = X500DistinguishedCNName,
        });

        File.WriteAllBytes(filePath, certificate.Export(X509ContentType.Pfx));
        return certificate;
    });

    /// <summary>
    /// Ipc 使用的根证书，每次由后端启动时生成新的，退出时删除缓存文件，可加上证书密码作为鉴权
    /// </summary>
    public static X509Certificate2 RootCertificate => _RootCertificate.Value;

    static readonly Lazy<X509Certificate2> _ServerCertificate = new(() =>
    {
        X509Certificate2? certificate = null;
        var filePath = Path.Combine(TempDirPath, "IpcServerCertificate.pfx");

        if (Directory.Exists(TempDirPath))
        {
            if (File.Exists(filePath))
            {
                try
                {
                    certificate = new X509Certificate2(filePath);
                    return certificate;
                }
                catch
                {
                    IOPath.FileTryDelete(filePath);
                }
            }
        }
        else
        {
            Directory.CreateDirectory(TempDirPath);
        }

        certificate = X509Certificate2Generator.CreateServerCertificate(new()
        {
            RootCertificate = RootCertificate,
            DistinguishedNameString = "CN=localhost",
            SubjectAlternativeName = "localhost",
            ExtraDnsNameOrIpAddresses = [
                X509Certificate2Generator.DnsNameOrIpAddress.GetMachineName(),
                IPAddress.Loopback,
                IPAddress.IPv6Loopback,
            ],
        });

        File.WriteAllBytes(filePath, certificate.Export(X509ContentType.Pfx));
        return certificate;
    });

    /// <summary>
    /// Ipc 用的服务器证书，每次由后端启动时生成新的，退出时删除缓存文件，可加上证书密码作为鉴权
    /// </summary>
    public static X509Certificate2 ServerCertificate => _ServerCertificate.Value;

    public static void Dispose()
    {
        IOPath.DirTryDelete(TempDirPath);
    }

    /// <summary>
    /// 根据类型生成随机值，用于模拟的假数据
    /// </summary>
    /// <param name="type"></param>
    /// <returns></returns>
    public static object? GetRandomValue(Type type)
    {
        if (type == typeof(int))
        {
            return Random.Shared.Next(int.MaxValue);
        }
        else if (type == typeof(Todo))
        {
            return new Todo(Random.Shared.Next(3, 9), Random2.GenerateRandomString(64));
        }
        else if (type == typeof(char))
        {
            return 'A';
        }
        else if (type == typeof(byte))
        {
            return (byte)25;
        }
        else if (type == typeof(sbyte))
        {
            return (sbyte)26;
        }
        else if (type == typeof(DateOnly))
        {
            return DateOnly.FromDateTime(DateTime.Today);
        }
        else if (type == typeof(DateTime))
        {
            return DateTime.UtcNow;
        }
        else if (type == typeof(DateTimeOffset))
        {
            return DateTimeOffset.Now;
        }
        else if (type == typeof(decimal))
        {
            return 27.1m;
        }
        else if (type == typeof(double))
        {
            return 27.1d;
        }
        else if (type == typeof(ProcessorArchitecture))
        {
            return ProcessorArchitecture.MSIL;
        }
        else if (type == typeof(Guid))
        {
            return Guid.NewGuid();
        }
        else if (type == typeof(short))
        {
            return (short)28;
        }
        else if (type == typeof(int))
        {
            return 29;
        }
        else if (type == typeof(long))
        {
            return 9223372036854775806L;
        }
        else if (type == typeof(float))
        {
            return 30.9f;
        }
        else if (type == typeof(TimeOnly))
        {
            return TimeOnly.FromDateTime(DateTime.Now);
        }
        else if (type == typeof(TimeSpan))
        {
            return TimeSpan.FromHours(3);
        }
        else if (type == typeof(ushort))
        {
            return (ushort)31;
        }
        else if (type == typeof(uint))
        {
            return 32u;
        }
        else if (type == typeof(ulong))
        {
            return 33ul;
        }
        else if (type == typeof(Uri))
        {
            return new Uri("http://github.com/BeyondDimension");
        }
        else if (type == typeof(Version))
        {
            return new Version("10.0.10240");
        }
        else if (type == typeof(CancellationToken))
        {
            return CancellationToken.None;
        }
        else
        {
            if (type.IsClass)
            {
                return null;
            }
            else
            {
                return Activator.CreateInstance(type);
            }
        }
    }
}

/// <summary>
/// <see cref="ITodoService"/> 的 Json 源生成类型信息
/// </summary>
[JsonSerializable(typeof(ApiRspImpl<Todo[]>))]
[JsonSerializable(typeof(ApiRspImpl<Todo>))]
[JsonSerializable(typeof(ApiRspImpl))]
[JsonSerializable(typeof(ApiRspImpl<nil>))]
[JsonSourceGenerationOptions(
    AllowTrailingCommas = true, // 忽略最后的逗号
    PropertyNameCaseInsensitive = true // 忽略大小写键名
    )]
internal sealed partial class SampleJsonSerializerContext : SystemTextJsonSerializerContext
{
}
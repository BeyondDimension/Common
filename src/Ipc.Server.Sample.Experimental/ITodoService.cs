#pragma warning disable SA1600 // Elements should be documented
#pragma warning disable CA1050 // 在命名空间中声明类型

[ServiceContract]
public partial interface ITodoService
{
    record Todo(int Id, string? Title, DateOnly? DueBy = null, bool IsComplete = false);

    Task<ApiRspImpl<Todo[]?>> All(CancellationToken cancellationToken = default);

    Task<ApiRspImpl<Todo?>> GetById(int id, CancellationToken cancellationToken = default);

    /// <summary>
    /// 测试 https://learn.microsoft.com/en-us/aspnet/core/mvc/models/model-binding?view=aspnetcore-8.0#simple-types 路由绑定简单类型
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

    Task<ApiRspImpl> SimpleTypes2(bool p0, byte p1, sbyte p2,
        /*DateOnly p4,*/ /*DateTime p5,*/
        /*DateTimeOffset p6,*/ decimal p7, double p8,
        ProcessorArchitecture p9, Guid p10, short p11,
        int p12, long p13, float p14,
        TimeOnly p15, TimeSpan p16, ushort p17,
        uint p18, ulong p19, Uri p20,
        Version p21, CancellationToken cancellationToken = default);

    Task<ApiRspImpl> BodyTest(Todo todo, CancellationToken cancellationToken = default)
    {
        throw new NotImplementedException();
    }

    IAsyncEnumerable<Todo> AsyncEnumerable(int len, CancellationToken cancellationToken = default);
}

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
                IpcAppConnectionString r = x;
                return r;
            }).ToArray();
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
        var filePath = Path.Combine(TempDirPath, "RootCertificate.pfx");

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
    /// 测试用的根证书，每次由服务端启动时生成新的，退出时删除缓存文件，可加上证书密码作为鉴权
    /// </summary>
    public static X509Certificate2 RootCertificate => _RootCertificate.Value;

    static readonly Lazy<X509Certificate2> _ServerCertificate = new(() =>
    {
        X509Certificate2? certificate = null;
        var filePath = Path.Combine(TempDirPath, "ServerCertificate.pfx");

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
    /// 测试用的服务器证书，每次由服务端启动时生成新的，退出时删除缓存文件，可加上证书密码作为鉴权
    /// </summary>
    public static X509Certificate2 ServerCertificate => _ServerCertificate.Value;

    public static void Dispose()
    {
        IOPath.DirTryDelete(TempDirPath);
    }
}

[JsonSerializable(typeof(ApiRspImpl<ITodoService.Todo[]>))]
[JsonSerializable(typeof(ApiRspImpl<ITodoService.Todo>))]
[JsonSerializable(typeof(ApiRspImpl))]
[JsonSerializable(typeof(ApiRspImpl<nil>))]
internal sealed partial class SampleJsonSerializerContext : SystemTextJsonSerializerContext
{
}
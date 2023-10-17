// https://learn.microsoft.com/zh-cn/aspnet/core/fundamentals/native-aot?view=aspnetcore-8.0#the-web-api-native-aot-template
// https://learn.microsoft.com/zh-cn/aspnet/core/fundamentals/minimal-apis/openapi?view=aspnetcore-8.0

#pragma warning disable SA1600 // Elements should be documented
#pragma warning disable CA1050 // 在命名空间中声明类型

//var builder = WebApplication.CreateSlimBuilder(args);
var builder = WebApplication.CreateEmptyBuilder(new WebApplicationOptions());
builder.Services.AddRoutingCore();
builder.WebHost.UseKestrelCore();

builder.Services.ConfigureHttpJsonOptions(options =>
{
    options.SerializerOptions.TypeInfoResolverChain.Insert(0, AppJsonSerializerContext.Default);
});

using var caCert = CertGenerator.GetCACert();
using var sslCert = CertGenerator.GetSslCert(caCert);

builder.WebHost.ConfigureKestrel(options =>
{
    static int GetRandomUnusedPort(IPAddress address)
    {
        using var listener = new TcpListener(address, 0);
        listener.Start();
        var port = ((IPEndPoint)listener.LocalEndpoint).Port;
        return port;
    }
    int port = GetRandomUnusedPort(IPAddress.Loopback);
    Console.WriteLine($"https://localhost:{port}/todos");
    options.ListenLocalhost(port, listenOptions =>
    {
        listenOptions.Protocols = HttpProtocols.Http2;
        listenOptions.UseHttps(sslCert);
    });
});

var app = builder.Build();

var sampleTodos = new Todo[] {
    new(1, "Walk the dog"),
    new(2, "Do the dishes", DateOnly.FromDateTime(DateTime.Now)),
    new(3, "Do the laundry", DateOnly.FromDateTime(DateTime.Now.AddDays(1))),
    new(4, "Clean the bathroom"),
    new(5, "Clean the car", DateOnly.FromDateTime(DateTime.Now.AddDays(2))),
};

var todosApi = app.MapGroup("/todos");
todosApi.MapGet("/", () => sampleTodos);
todosApi.MapGet("/{id}", (int id) =>
    sampleTodos.FirstOrDefault(a => a.Id == id) is { } todo
        ? Results.Ok(todo)
        : Results.NotFound());

app.Run();

public record Todo(int Id, string? Title, DateOnly? DueBy = null, bool IsComplete = false);

[JsonSerializable(typeof(Todo[]))]
internal partial class AppJsonSerializerContext : JsonSerializerContext
{
}

static class CertGenerator
{
    const int rsaKeySizeInBits = 2048;

    static readonly Oid tlsServerOid = new("1.3.6.1.5.5.7.3.1");
    static readonly Oid tlsClientOid = new("1.3.6.1.5.5.7.3.2");

    public static X509Certificate2 GetCACert()
    {
        using var rsa = RSA.Create(rsaKeySizeInBits);
        var request = new CertificateRequest(
            new X500DistinguishedName(
                "C=CN, O=BeyondDimension, OU=Technical Department, CN=SteamTools Certificate"),
            rsa, HashAlgorithmName.SHA256, RSASignaturePadding.Pkcs1);
        var basicConstraints = new X509BasicConstraintsExtension(true, true, 1, true);
        request.CertificateExtensions.Add(basicConstraints);

        var keyUsage = new X509KeyUsageExtension(X509KeyUsageFlags.DigitalSignature | X509KeyUsageFlags.CrlSign | X509KeyUsageFlags.KeyCertSign, true);
        request.CertificateExtensions.Add(keyUsage);

        var oids = new OidCollection { tlsServerOid, tlsClientOid };
        var enhancedKeyUsage = new X509EnhancedKeyUsageExtension(oids, true);
        request.CertificateExtensions.Add(enhancedKeyUsage);

        var dnsBuilder = new SubjectAlternativeNameBuilder();
        dnsBuilder.AddDnsName("SteamTools Certificate");
        request.CertificateExtensions.Add(dnsBuilder.Build());

        var subjectKeyId = new X509SubjectKeyIdentifierExtension(request.PublicKey, false);
        request.CertificateExtensions.Add(subjectKeyId);

        var notBefore = DateTime.Today.AddDays(-1);
        var notAfter = DateTime.Today.AddDays(300);
        var caCert = request.CreateSelfSigned(notBefore, notAfter);
        return caCert;
    }

    public static X509Certificate2 GetSslCert(X509Certificate2 issuerCertificate)
    {
        var subjectName = new X500DistinguishedName("CN=localhost");
        using var rsa = RSA.Create(rsaKeySizeInBits);
        var request = new CertificateRequest(subjectName, rsa, HashAlgorithmName.SHA256, RSASignaturePadding.Pkcs1);

        var basicConstraints = new X509BasicConstraintsExtension(false, false, 0, true);
        request.CertificateExtensions.Add(basicConstraints);

        var keyUsage = new X509KeyUsageExtension(X509KeyUsageFlags.DigitalSignature | X509KeyUsageFlags.KeyEncipherment, true);
        request.CertificateExtensions.Add(keyUsage);

        var oids = new OidCollection { tlsServerOid, tlsClientOid };
        var enhancedKeyUsage = new X509EnhancedKeyUsageExtension(oids, true);
        request.CertificateExtensions.Add(enhancedKeyUsage);

        var extension = new X509SubjectKeyIdentifierExtension(issuerCertificate.PublicKey, false);
        var authorityKeyId = X509AuthorityKeyIdentifierExtension.CreateFromSubjectKeyIdentifier(extension);
        request.CertificateExtensions.Add(authorityKeyId);

        var subjectKeyId = new X509SubjectKeyIdentifierExtension(request.PublicKey, false);
        request.CertificateExtensions.Add(subjectKeyId);

        var dnsBuilder = new SubjectAlternativeNameBuilder();
        dnsBuilder.AddDnsName(subjectName.Name[3..]);

        dnsBuilder.AddDnsName(Environment.MachineName.ToString());
        dnsBuilder.AddIpAddress(IPAddress.Loopback);
        dnsBuilder.AddIpAddress(IPAddress.IPv6Loopback);

        var dnsNames = dnsBuilder.Build();
        request.CertificateExtensions.Add(dnsNames);

        var serialNumber = BitConverter.GetBytes(Random.Shared.NextInt64());
        using var certOnly = request.Create(issuerCertificate, issuerCertificate.NotBefore, issuerCertificate.NotAfter, serialNumber);
        var serverCert = certOnly.CopyWithPrivateKey(rsa);
        var serverCertPfx = serverCert.Export(X509ContentType.Pfx);
        // 将生成的证书导出后重新创建一个
        return new X509Certificate2(serverCertPfx);
    }
}
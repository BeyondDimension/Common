using MessagePack.Resolvers;
using Splat;
using SDColor = System.Drawing.Color;

namespace BD.Common.UnitTest;

public sealed class SerializationTest
{
    [SetUp]
    public void Setup()
    {

    }

    static CookieCollection GetCookieCollection()
    {
        var cookie = new CookieContainer();
        cookie.Add(new Cookie("c1", "v1", "/", "steampp.net"));
        cookie.Add(new Cookie("c2", "v2", "/", "steampp.net"));
        cookie.Add(new Cookie("c3", "v3", "/", "steampp.net"));
        cookie.Add(new Cookie("c4", "v4", "/", "steampp.net"));
        return cookie.GetAllCookies();
    }

    static CookiesModel GetCookiesModel() => new()
    {
        Cookies = GetCookieCollection(),
        SDColor = SDColor.Purple,
        SplatColor = SplatColor.Purple,
        SDColor2 = SDColor.HotPink,
        SplatColor2 = SplatColor.HotPink,
    };

    static void TestCookieCollection(CookieCollection cookies)
    {
        cookies.Add(new Cookie("c5", "v5", "/", "steampp.net"));
        var cookie = new CookieContainer();
        cookie.Add(cookies);
        var cookieHeader = cookie.GetCookieHeader(new("https://steampp.net"));
        Assert.IsTrue(cookieHeader.Contains("c1") && cookieHeader.Contains("c3"));
        TestContext.WriteLine(cookieHeader);
    }

    /// <summary>
    /// 使用 MessagePack 测试对 <see cref="CookieCollection"/> 序列化与反序列化
    /// </summary>
    [Test]
    public void CookieCollection_MessagePack()
    {
        // 1. 在模型类上使用 [MessagePackFormatter(typeof(CookieFormatter))]
        var bytes = Serializable.SMP(GetCookiesModel());
        var m = Serializable.DMP<CookiesModel>(bytes)!;
        TestCookieCollection(m.Cookies!);

        // 2. 通过自定义 Options 直接反序列化 CookieCollection
        var options = MessagePackSerializerOptions.Standard
            .WithCompression(MessagePackCompression.Lz4BlockArray)
            .WithResolver(CompositeResolver.Create(
                new IMessagePackFormatter[] { new CookieFormatter() },
                new IFormatterResolver[] { StandardResolver.Instance }));
        bytes = MessagePackSerializer.Serialize(GetCookieCollection(), options);
        m.Cookies = MessagePackSerializer.Deserialize<CookieCollection>(bytes, options);
        TestCookieCollection(m.Cookies!);
    }

    /// <summary>
    /// 使用 MemoryPack 测试对 <see cref="CookieCollection"/> 序列化与反序列化
    /// </summary>
    [Test]
    public void CookieCollection_MemoryPack()
    {
        // 1. 在模型类上使用 [CookieCollectionFormatter]
        var bytes = Serializable.SMP2(GetCookiesModel());
        var m = Serializable.DMP2<CookiesModel>(bytes)!;
        TestCookieCollection(m.Cookies!);

        // 2. 通过 MemoryPackFormatterProvider.Register 注册全局格式化
        MemoryPackFormatterProvider.Register(CookieCollectionFormatterAttribute.Formatter.Default);
        bytes = Serializable.SMP2(GetCookieCollection());
        m.Cookies = Serializable.DMP2<CookieCollection>(bytes);
        TestCookieCollection(m.Cookies!);
    }

    const string X500DistinguishedNameValue = $"C=CN, O=BeyondDimension, OU=Technical Department, CN=Common Certificate";

    [Test]
    public void X509Certificate_MemoryPack()
    {
        using var rsa = RSA.Create();
        CertificateRequest request = new(new X500DistinguishedName(X500DistinguishedNameValue), rsa, HashAlgorithmName.SHA256, RSASignaturePadding.Pkcs1);
        var cert = request.CreateSelfSigned(DateTimeOffset.Now, DateTimeOffset.MaxValue);
        var model = new X509CertificateModel
        {
            X509Certificate = cert,
            X509Certificate2 = cert,
            NullableX509CertificatePackable2 =
                X509CertificatePackable.CreateX509Certificate2(
                    cert.GetRawCertData()),
            X509CertificatePackable =
                X509CertificatePackable.CreateX509Certificate2(
                    cert.GetRawCertData()),
        };
        var bytes = Serializable.SMP2(model);
        var m = Serializable.DMP2<X509CertificateModel>(bytes)!;

        MemoryPackFormatterProvider.Register<MemoryPackFormatters>();

        bytes = Serializable.SMP2(cert);
        model.X509Certificate2 = Serializable.DMP2<X509Certificate2>(bytes);

        bytes = Serializable.SMP2(model.NullableX509CertificatePackable2);
        model.NullableX509CertificatePackable2 = Serializable.DMP2<X509CertificatePackable?>(bytes);

        bytes = Serializable.SMP2(model.X509CertificatePackable);
        model.X509CertificatePackable = Serializable.DMP2<X509CertificatePackable>(bytes);
    }
}
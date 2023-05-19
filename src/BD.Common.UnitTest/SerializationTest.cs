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
}

[MPObj, MP2Obj(SerializeLayout.Explicit)]
public sealed partial class CookiesModel
{
    [MPKey(0), MP2Key(0)]
    [MessagePackFormatter(typeof(CookieFormatter))]
    [CookieCollectionFormatter]
    public CookieCollection? Cookies { get; set; }

    [MPKey(1), MP2Key(1)]
    [MessagePackFormatter(typeof(ColorFormatter))]
    [ColorFormatter]
    public SDColor SDColor { get; set; }

    [MPKey(2), MP2Key(2)]
    [MessagePackFormatter(typeof(ColorFormatter))]
    [SplatColorFormatter]
    public SplatColor SplatColor { get; set; }

    [MPKey(3), MP2Key(3)]
    [MessagePackFormatter(typeof(ColorFormatter))]
    [NullableColorFormatter]
    public SDColor? SDColor2 { get; set; }

    [MPKey(4), MP2Key(4)]
    [MessagePackFormatter(typeof(ColorFormatter))]
    [NullableSplatColorFormatter]
    public SplatColor? SplatColor2 { get; set; }

    [MPKey(5), MP2Key(5)]
    [MessagePackFormatter(typeof(ColorFormatter))]
    [NullableColorFormatter]
    public SDColor? SDColor3 { get; set; }

    [MPKey(6), MP2Key(6)]
    [MessagePackFormatter(typeof(ColorFormatter))]
    [NullableSplatColorFormatter]
    public SplatColor? SplatColor3 { get; set; }
}
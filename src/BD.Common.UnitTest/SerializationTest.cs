using MessagePack.Resolvers;
using System.Runtime.Serialization.Formatters;

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
        cookie.Add(new Cookie("c1", "v1", "/", "httpbin.org"));
        cookie.Add(new Cookie("c2", "v2", "/", "httpbin.org"));
        cookie.Add(new Cookie("c3", "v3", "/", "httpbin.org"));
        cookie.Add(new Cookie("c4", "v4", "/", "httpbin.org"));
        return cookie.GetAllCookies();
    }

    static async Task TestCookieCollection(CookieCollection cookies)
    {
        cookies.Add(new Cookie("c5", "v5", "/", "httpbin.org"));
        var cookie = new CookieContainer();
        cookie.Add(cookies);
        var client = new HttpClient(new HttpClientHandler
        {
            UseCookies = true,
            CookieContainer = cookie,
        });
        var rsp = await client.GetStringAsync("https://httpbin.org/cookies");
        Assert.IsTrue(rsp.Contains("c1") && rsp.Contains("c3"));
        TestContext.WriteLine(rsp);
    }

    /// <summary>
    /// 使用 MessagePack 测试对 <see cref="CookieCollection"/> 序列化与反序列化
    /// </summary>
    /// <returns></returns>
    [Test]
    public async Task CookieCollection_MessagePack()
    {
        // 1. 在模型类上使用 [MessagePackFormatter(typeof(CookieFormatter))]
        var bytes = Serializable.SMP(new CookiesModel
        {
            Cookies = GetCookieCollection(),
        });
        var m = Serializable.DMP<CookiesModel>(bytes)!;
        await TestCookieCollection(m.Cookies!);

        // 2. 通过自定义 Options 直接反序列化 CookieCollection
        var options = MessagePackSerializerOptions.Standard
            .WithCompression(MessagePackCompression.Lz4BlockArray)
            .WithResolver(CompositeResolver.Create(
                new IMessagePackFormatter[] { new CookieFormatter() },
                new IFormatterResolver[] { StandardResolver.Instance }));
        bytes = MessagePackSerializer.Serialize(GetCookieCollection(), options);
        m.Cookies = MessagePackSerializer.Deserialize<CookieCollection>(bytes, options);
        await TestCookieCollection(m.Cookies!);
    }

    /// <summary>
    /// 使用 MemoryPack 测试对 <see cref="CookieCollection"/> 序列化与反序列化
    /// </summary>
    /// <returns></returns>
    [Test]
    public async Task CookieCollection_MemoryPack()
    {
        // 1. 在模型类上使用 [CookieCollectionFormatter]
        var bytes = Serializable.SMP2(new CookiesModel
        {
            Cookies = GetCookieCollection(),
        });
        var m = Serializable.DMP2<CookiesModel>(bytes)!;
        await TestCookieCollection(m.Cookies!);

        // 2. 通过 MemoryPackFormatterProvider.Register 注册全局格式化
        MemoryPackFormatterProvider.Register(CookieCollectionFormatterAttribute.Formatter.Default);
        bytes = Serializable.SMP2(GetCookieCollection());
        m.Cookies = Serializable.DMP2<CookieCollection>(bytes);
        await TestCookieCollection(m.Cookies!);
    }
}

[MPObj, MP2Obj(SerializeLayout.Explicit)]
public sealed partial class CookiesModel
{
    [MPKey(0), MP2Key(0)]
    [MessagePackFormatter(typeof(CookieFormatter))]
    [CookieCollectionFormatter]
    public CookieCollection? Cookies { get; set; }
}
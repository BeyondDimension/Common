using MessagePack;
using MessagePack.Formatters;
using MessagePack.Resolvers;
using System.Runtime.Serialization.Formatters;
using KeyAttribute = MessagePack.KeyAttribute;

namespace BD.Common.UnitTest;

public class SerializationTest
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

    [MessagePackObject]
    public sealed class Model
    {
        [Key(0)]
        [MessagePackFormatter(typeof(CookieFormatter))]
        public CookieCollection? Cookies { get; set; }
    }

    static async Task CookieCollection(CookieCollection cookies)
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

    [Test]
    public async Task CookieCollection()
    {
        // 在模型类上使用 [MessagePackFormatter(typeof(CookieFormatter))]
        var bytes = Serializable.SMP(new Model
        {
            Cookies = GetCookieCollection(),
        });
        var m = Serializable.DMP<Model>(bytes)!;
        await CookieCollection(m.Cookies!);

        // 通过自定义 Options 直接反序列化 CookieCollection
        var options = MessagePackSerializerOptions.Standard
            .WithCompression(MessagePackCompression.Lz4BlockArray)
            .WithResolver(CompositeResolver.Create(
                new IMessagePackFormatter[] { new CookieFormatter() },
                new IFormatterResolver[] { StandardResolver.Instance }));
        bytes = MessagePackSerializer.Serialize(GetCookieCollection(), options);
        m.Cookies = MessagePackSerializer.Deserialize<CookieCollection>(bytes, options);
        await CookieCollection(m.Cookies!);
    }
}
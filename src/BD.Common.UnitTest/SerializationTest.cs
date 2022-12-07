using MessagePack;
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

    [Test]
    public async Task CookieCollection()
    {
        var bytes = Serializable.SMP(new Model
        {
            Cookies = GetCookieCollection(),
        });
        var m = Serializable.DMP<Model>(bytes)!;
        m.Cookies!.Add(new Cookie("c5", "v5", "/", "httpbin.org"));
        var cookie = new CookieContainer();
        cookie.Add(m.Cookies);
        var client = new HttpClient(new HttpClientHandler
        {
            UseCookies = true,
            CookieContainer = cookie,
        });
        var rsp = await client.GetStringAsync("https://httpbin.org/cookies");
        Assert.IsTrue(rsp.Contains("c1") && rsp.Contains("c3"));
        TestContext.WriteLine(rsp);
    }
}
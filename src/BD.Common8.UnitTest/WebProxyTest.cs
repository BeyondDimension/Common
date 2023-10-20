namespace BD.Common8.UnitTest;

/// <summary>
/// <see cref="IWebProxy"/> 测试
/// </summary>
public sealed class WebProxyTest
{
#if WINDOWS7_0_OR_GREATER
    /// <summary>
    /// <see cref="DynamicHttpWindowsProxy"/>
    /// </summary>
    [Test]
    public void DynamicHttpWindowsProxyTest()
    {
        using var handler = new HttpClientHandler
        {
            UseProxy = true,
            Proxy = DynamicHttpWindowsProxy.Instance,
        };
        using var client = new HttpClient(handler);
        client.Timeout = TimeSpan.FromMicroseconds(1d);

        Uri uri = new("https://github.com");
        var proxyUri = handler.Proxy.GetProxy(uri);
        TestContext.WriteLine(proxyUri);
    }
#endif
}
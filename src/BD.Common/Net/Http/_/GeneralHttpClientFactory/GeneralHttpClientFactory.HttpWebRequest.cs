// ReSharper disable once CheckNamespace
namespace System.Net.Http;

partial class GeneralHttpClientFactory
{
    [Obsolete("WebRequest, HttpWebRequest, ServicePoint, and WebClient are obsolete. Use HttpClient instead.", DiagnosticId = "SYSLIB0014", UrlFormat = "https://aka.ms/dotnet-warnings/{0}")]
    public static HttpWebRequest Create(string requestUriString)
    {
        var request = WebRequest.CreateHttp(requestUriString);
        ConfigHttpWebRequest(request);
        return request;
    }

    [Obsolete("WebRequest, HttpWebRequest, ServicePoint, and WebClient are obsolete. Use HttpClient instead.", DiagnosticId = "SYSLIB0014", UrlFormat = "https://aka.ms/dotnet-warnings/{0}")]
    public static HttpWebRequest Create(Uri requestUri)
    {
        var request = WebRequest.CreateHttp(requestUri);
        ConfigHttpWebRequest(request);
        return request;
    }

    static void ConfigHttpWebRequest(HttpWebRequest request)
    {
        request.AllowAutoRedirect = true;
        request.MaximumAutomaticRedirections = 1000;
        request.Timeout = DefaultTimeoutMilliseconds;
#if NETSTANDARD
        // .NET Core 3+ 上已由 HttpClient.DefaultProxy 生效
        // https://docs.microsoft.com/zh-cn/dotnet/api/system.net.http.httpclient.defaultproxy?view=net-6.0
        var proxy = DefaultProxy;
        if (proxy != null)
        {
            request.Proxy = proxy;
        }
#endif
    }
}

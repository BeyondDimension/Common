using BD.Common.Services.Implementation;
using Fusillade;
using Microsoft.Extensions.Logging;

namespace BD.Common.UnitTest;

public sealed class ImageHttpClientTest
{
    [SetUp]
    public void Setup()
    {
        static void ConfigureServices(IServiceCollection s)
        {
            s.AddLogging(l => l.AddConsole());
            s.AddSingleton<IHttpPlatformHelperService, HttpPlatformHelperImpl>();
            s.AddSingleton<IHttpClientFactory, FusilladeHttpClientFactory>();
            s.AddSingleton<IImageHttpClientService, ImageHttpClientService>();
        }

        Ioc.ConfigureServices(ConfigureServices);
        NetCache.RequestCache = new RequestCache();
    }

    sealed class RequestCache : IRequestCache
    {
        async Task<byte[]> IRequestCache.Fetch(HttpRequestMessage request, string key, CancellationToken ct)
        {
            TestContext.WriteLine($"key: {key}");
            TestContext.WriteLine($"key2: {UniqueKeyForRequest(request)}");
            using var client = new HttpClient();
            using var req = new HttpRequestMessage(request.Method, request.RequestUri);
            using var rsp = await client.SendAsync(req, ct);
            var r = await rsp.Content.ReadAsByteArrayAsync(ct);
            return r;
        }

        /// <summary>
        /// Generates a unique key for a <see cref="HttpRequestMessage"/>.
        /// This assists with the caching.
        /// </summary>
        /// <param name="request">The request to generate a unique key for.</param>
        /// <returns>The unique key.</returns>
        public static string UniqueKeyForRequest(HttpRequestMessage request)
        {
            if (request is null)
            {
                throw new ArgumentNullException(nameof(request));
            }

            var ret = new[]
            {
                request.RequestUri!.ToString(),
                request.Method.Method,
                request.Headers.Accept.ConcatenateAll(x => x.CharSet + x.MediaType),
                request.Headers.AcceptEncoding.ConcatenateAll(x => x.Value),
                (request.Headers.Referrer ?? new Uri("http://example")).AbsoluteUri,
                request.Headers.UserAgent.ConcatenateAll(x => x.Product != null ? x.Product.ToString() : x.Comment!),
            }.Aggregate(
                new StringBuilder(),
                (acc, x) =>
                {
                    acc.AppendLine(x);
                    return acc;
                });

            if (request.Headers.Authorization != null)
            {
                ret.Append(request.Headers.Authorization.Parameter).AppendLine(request.Headers.Authorization.Scheme);
            }

            var ret_ = ret.ToString();
            return ret_;
            //return "HttpSchedulerCache_" + ret_.GetHashCode().ToString("x", CultureInfo.InvariantCulture);
        }

        Task IRequestCache.Save(HttpRequestMessage request, HttpResponseMessage response, string key, CancellationToken ct)
        {
            return Task.CompletedTask;
        }
    }

    sealed class HttpPlatformHelperImpl : HttpPlatformHelperService
    {
        public static new string DefaultUserAgent { get; internal set; } = "Mozilla/5.0 (Windows NT 10.0; Win64; x64) AppleWebKit/537.36 (KHTML, like Gecko) Chrome/90.0.4430.93 Safari/537.36 Edg/90.0.818.51";

        public override string UserAgent => DefaultUserAgent;
    }

    [Test]
    public async Task GetImage()
    {
        const string url = "https://steampp.mossimo.net:8800/komaasharu/images/2080735b-f36b-1410-8e27-0099cc325616";

        var imgStream = await Ioc.Get<IImageHttpClientService>().GetImageMemoryStreamAsync(url, cache: true, cacheFirst: true);

        TestContext.WriteLine(imgStream);
    }
}

static class ConcatenateMixin
{
    public static string ConcatenateAll<T>(this IEnumerable<T> enumerables, Func<T, string> selector, char separator = '|')
    {
        return enumerables.Aggregate(new StringBuilder(), (acc, x) =>
        {
            acc.Append(selector(x)).Append(separator);
            return acc;
        }).ToString();
    }
}

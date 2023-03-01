using static System.Net.Http.IImageHttpClientService;

namespace System.Net.Http;

public sealed partial class ImageHttpClientService : GeneralHttpClientFactory, IImageHttpClientService
{
    public ImageHttpClientService(
            ILoggerFactory loggerFactory,
            IHttpPlatformHelperService http_helper,
            IHttpClientFactory clientFactory)
            : base(loggerFactory.CreateLogger(TAG), http_helper, clientFactory)
    {

    }
}

using Polly;
using static System.Net.Http.IImageHttpClientService;

// ReSharper disable once CheckNamespace
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

    public async Task<MemoryStream?> GetImageMemoryStreamAsync(
        string requestUri,
        bool isPolly = true,
        bool cache = false,
        HttpHandlerCategory category = DefaultHttpHandlerCategory,
        CancellationToken cancellationToken = default)
    {
        if (!String2.IsHttpUrl(requestUri))
            return default;

        if (!cache)
        {
            category = HttpHandlerCategory.Default;
        }
        else
        {
            isPolly = false;
        }

        MemoryStream? response = null;
        try
        {
            if (isPolly)
            {
                response = await Policy.HandleResult<MemoryStream?>(x => x == null)
                    .WaitAndRetryAsync(numRetries, PollyRetryAttempt)
                    .ExecuteAsync(_GetImageMemoryStreamCoreAsync, cancellationToken);
            }
            else
            {
                response = await _GetImageMemoryStreamCoreAsync(cancellationToken);
            }
        }
        catch (Exception e)
        {
            logger.LogWarning(e,
                $"{nameof(GetImageMemoryStreamAsync)} fail, category: {{category}}.", category);
        }

        if (response == null && category != HttpHandlerCategory.Offline && cache)
        {
            try
            {
                category = HttpHandlerCategory.Offline;
                response = await _GetImageMemoryStreamCoreAsync(cancellationToken);
            }
            catch (Exception e)
            {
                logger.LogWarning(e,
                    $"{nameof(GetImageMemoryStreamAsync)} fail, category: {{category}}.", category);
            }
        }

        return response;

        Task<MemoryStream?> _GetImageMemoryStreamCoreAsync(CancellationToken cancellationToken)
            => GetImageMemoryStreamCoreAsync(
                requestUri,
                category,
                cancellationToken);
    }

    async Task<MemoryStream?> GetImageMemoryStreamCoreAsync(
        string requestUri,
        HttpHandlerCategory category = DefaultHttpHandlerCategory,
        CancellationToken cancellationToken = default)
    {
        var request = new HttpRequestMessage(HttpMethod.Get, requestUri);
        request.Headers.Accept.ParseAdd(http_helper.AcceptImages);
        request.Headers.UserAgent.ParseAdd(http_helper.UserAgent);
        var client = CreateClient(TAG, category);
        var response = await client.SendAsync(request,
            HttpCompletionOption.ResponseHeadersRead,
            cancellationToken)
            .ConfigureAwait(false);
        if (response.IsSuccessStatusCode)
        {
            //var imageStream = await response.Content.ReadAsStreamAsync(cancellationToken);
            //var memoryStream = new MemoryStream();
            //await imageStream.CopyToAsync(memoryStream, cancellationToken);
            //memoryStream.Position = 0;
            //return memoryStream;
            var imageBytes = await response.Content.ReadAsByteArrayAsync(cancellationToken);
            return new MemoryStream(imageBytes, false);
        }

        return default;
    }

    #region Polly

    const int numRetries = 3;

    static TimeSpan PollyRetryAttempt(int attemptNumber)
    {
        var powY = attemptNumber % numRetries;
        var timeSpan = TimeSpan.FromMilliseconds(Math.Pow(2, powY));
        int addS = attemptNumber / numRetries;
        if (addS > 0) timeSpan = timeSpan.Add(TimeSpan.FromSeconds(addS));
        return timeSpan;
    }

    #endregion
}

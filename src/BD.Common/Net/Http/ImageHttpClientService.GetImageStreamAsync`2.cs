namespace System.Net.Http;

partial class ImageHttpClientService
{
    public async Task<Stream?> GetImageStreamAsync(string requestUri, CancellationToken cancellationToken = default)
    {
        if (!String2.IsHttpUrl(requestUri)) return default;
        try
        {
            var request = new HttpRequestMessage(HttpMethod.Get, requestUri);
            request.Headers.Accept.ParseAdd(http_helper.AcceptImages);
            request.Headers.UserAgent.ParseAdd(http_helper.UserAgent);
            var client = CreateClient();
            var response = await client.SendAsync(request,
                HttpCompletionOption.ResponseHeadersRead,
                cancellationToken)
                .ConfigureAwait(false);
            if (response.IsSuccessStatusCode)
            {
                var imageStream = await response.Content.ReadAsByteArrayAsync(cancellationToken);
                var ms = new MemoryStream(imageStream);
                return ms;
            }
        }
        catch (Exception e)
        {
            logger.LogWarning(e, "GetImageStreamAsync fail.");
        }
        return default;
    }
}

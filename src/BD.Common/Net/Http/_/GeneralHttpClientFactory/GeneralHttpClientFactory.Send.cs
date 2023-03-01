using Newtonsoft.Json;
using System.Xml.Serialization;
using JsonSerializer = Newtonsoft.Json.JsonSerializer;
using SJsonSerializer = System.Text.Json.JsonSerializer;

// ReSharper disable once CheckNamespace
namespace System.Net.Http;

partial class GeneralHttpClientFactory
{
    static async Task<T?> SendAsync<T>(
       HttpClient client,
       ILogger logger,
       JsonSerializer? jsonSerializer,
       bool isCheckHttpUrl,
       string? requestUri,
       Func<HttpRequestMessage> requestFactory,
       string? accept,
       //bool enableForward,
       CancellationToken cancellationToken,
       Action<HttpResponseMessage>? handlerResponse = null,
       Action<HttpResponseMessage>? handlerResponseByIsNotSuccessStatusCode = null,
       Serializable.JsonImplType jsonImplType = Serializable.JsonImplType.NewtonsoftJson) where T : notnull
    {
        HttpRequestMessage? request = null;
        bool requestIsSend = false;
        HttpResponseMessage? response = null;
        bool notDisposeResponse = false;
        try
        {
            request = requestFactory();
            requestUri ??= request.RequestUri?.ToString();

            if (!isCheckHttpUrl && !String2.IsHttpUrl(requestUri)) return default;

            //if (enableForward && IsAllowUrl(requestUri))
            //{
            //    try
            //    {
            //        requestIsSend = true;
            //        response = await Csc.Forward(request,
            //            HttpCompletionOption.ResponseHeadersRead,
            //            cancellationToken);
            //    }
            //    catch (Exception e)
            //    {
            //        logger.LogWarning(e, "CloudService Forward Fail, requestUri: {0}", requestUri);
            //        response = null;
            //    }
            //}

            if (response == null)
            {
                if (requestIsSend)
                {
                    request.Dispose();
                    request = requestFactory();
                }
                response = await client.SendAsync(request,
                  HttpCompletionOption.ResponseHeadersRead,
                  cancellationToken).ConfigureAwait(false);
            }

            handlerResponse?.Invoke(response);

            if (response.IsSuccessStatusCode)
            {
                if (response.Content != null)
                {
                    var rspContentClrType = typeof(T);
                    if (rspContentClrType == typeof(string))
                    {
                        return (T)(object)await response.Content.ReadAsStringAsync();
                    }
                    else if (rspContentClrType == typeof(byte[]))
                    {
                        return (T)(object)await response.Content.ReadAsByteArrayAsync();
                    }
                    else if (rspContentClrType == typeof(Stream))
                    {
                        notDisposeResponse = true;
                        return (T)(object)await response.Content.ReadAsStreamAsync();
                    }
                    var mime = response.Content.Headers.ContentType?.MediaType ?? accept;
                    switch (mime)
                    {
                        case MediaTypeNames.XML:
                        case MediaTypeNames.XML_APP:
                            {
                                using var stream = await response.Content.ReadAsStreamAsync().ConfigureAwait(false);
                                using var reader = new StreamReader(stream, Encoding.UTF8);
                                var xmlSerializer = new XmlSerializer(typeof(T));
                                var result = (T?)xmlSerializer.Deserialize(reader);
                                return result;
                            }
                        case MediaTypeNames.JSON:
                            {
                                if (jsonImplType == Serializable.JsonImplType.SystemTextJson || jsonSerializer == null)
                                {
                                    using var stream = await response.Content.ReadAsStreamAsync().ConfigureAwait(false);
                                    var result = await SJsonSerializer.DeserializeAsync<T>(stream);
                                    return result;
                                }
                                else
                                {
                                    using var stream = await response.Content.ReadAsStreamAsync().ConfigureAwait(false);
                                    using var reader = new StreamReader(stream, Encoding.UTF8);
                                    using var json = new JsonTextReader(reader);
                                    var result = jsonSerializer.Deserialize<T>(json);
                                    return result;
                                }
                            }
                    }
                }
            }
            else
            {
                handlerResponseByIsNotSuccessStatusCode?.Invoke(response);
            }
        }
        catch (Exception e)
        {
            var knownType = e.GetKnownType();
            if (knownType == ExceptionKnownType.Unknown)
            {
                logger.LogError(e, "SendAsync fail, requestUri: {requestUri}", requestUri);
            }
        }
        finally
        {
            request?.Dispose();
            if (!notDisposeResponse)
            {
                response?.Dispose();
            }
        }
        return default;
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static Task<T?> SendAsync<T>(
        HttpClient client,
        ILogger logger,
        JsonSerializer? jsonSerializer,
        string? requestUri,
        Func<HttpRequestMessage> requestFactory,
        string? accept,
        //bool enableForward,
        CancellationToken cancellationToken,
        Action<HttpResponseMessage>? handlerResponse = null,
        Action<HttpResponseMessage>? handlerResponseByIsNotSuccessStatusCode = null,
        Serializable.JsonImplType jsonImplType = Serializable.JsonImplType.NewtonsoftJson) where T : notnull => SendAsync<T>(client,
            logger,
            jsonSerializer,
            isCheckHttpUrl: false,
            requestUri,
            requestFactory,
            accept,
            //enableForward,
            cancellationToken,
            handlerResponse,
            handlerResponseByIsNotSuccessStatusCode,
            jsonImplType);

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static Task<T?> GetAsync<T>(
        HttpClient client,
        ILogger logger,
        string requestUri,
        string accept = MediaTypeNames.JSON,
        CancellationToken cancellationToken = default,
        string? cookie = null,
        string? userAgent = null,
        JsonSerializer? jsonSerializer = null,
        Serializable.JsonImplType jsonImplType = Serializable.JsonImplType.NewtonsoftJson) where T : notnull
    {
        if (!String2.IsHttpUrl(requestUri))
            return Task.FromResult(default(T?));
        return SendAsync<T>(client, logger, jsonSerializer, isCheckHttpUrl: true, requestUri, () =>
        {
            var request = new HttpRequestMessage(HttpMethod.Get, requestUri);
            if (cookie != null)
            {
                request.Headers.Add("Cookie", cookie);
            }
            request.Headers.Accept.ParseAdd(accept);
            if (userAgent != null)
                request.Headers.UserAgent.ParseAdd(userAgent);
            return request;
        }, accept/*, true*/, cancellationToken, jsonImplType: jsonImplType);
    }
}
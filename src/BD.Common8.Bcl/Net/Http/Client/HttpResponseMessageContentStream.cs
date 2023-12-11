namespace System.Net.Http.Client;

/// <summary>
/// 包装 Http 响应正文流，流释放时候跟随释放响应消息
/// </summary>
/// <param name="responseMessage"></param>
/// <param name="inner"></param>
public sealed class HttpResponseMessageContentStream(HttpResponseMessage responseMessage, Stream inner) : DelegatingStream(inner)
{
    HttpResponseMessage? responseMessage = responseMessage;

    /// <inheritdoc/>
    protected override void Dispose(bool disposing)
    {
        if (disposing)
        {
            // 释放托管状态(托管对象)
            responseMessage?.Dispose();
        }

        // 释放未托管的资源(未托管的对象)并重写终结器
        // 将大型字段设置为 null
        responseMessage = null;
    }

    /// <inheritdoc cref="HttpStatusCode"/>
    public HttpStatusCode StatusCode => responseMessage is null ? default : responseMessage.StatusCode;

    /// <inheritdoc cref="HttpContent.ReadAsStreamAsync(CancellationToken)"/>
    public static async Task<Stream> ReadAsStreamAsync(
        HttpResponseMessage responseMessage,
        CancellationToken cancellationToken = default)
    {
        var stream = await responseMessage.Content.ReadAsStreamAsync(cancellationToken);
        var result = new HttpResponseMessageContentStream(responseMessage, stream);
        return result;
    }

    /// <inheritdoc cref="HttpContent.ReadAsStream(CancellationToken)"/>
    public static Stream ReadAsStream(
        HttpResponseMessage responseMessage,
        CancellationToken cancellationToken = default)
    {
        var stream = responseMessage.Content.ReadAsStream(cancellationToken);
        var result = new HttpResponseMessageContentStream(responseMessage, stream);
        return result;
    }
}
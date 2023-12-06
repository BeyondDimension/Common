namespace System.Net.Http.Client;

#pragma warning disable SA1600 // Elements should be documented

public sealed class HttpResponseMessageContentAsyncEnumerable<T>(IAsyncEnumerable<T> enumerable, HttpResponseMessage httpResponseMessage) : IAsyncEnumerable<T>
{
    readonly IAsyncEnumerable<T> enumerable = enumerable;
    readonly HttpResponseMessage httpResponseMessage = httpResponseMessage;

    public IAsyncEnumerator<T> GetAsyncEnumerator(CancellationToken cancellationToken = default)
    {
        return new HttpResponseMessageContentAsyncEnumerator<T>(
            enumerable.GetAsyncEnumerator(cancellationToken), httpResponseMessage);
    }
}

sealed class HttpResponseMessageContentAsyncEnumerator<T>(IAsyncEnumerator<T> enumerator, HttpResponseMessage httpResponseMessage) : IAsyncEnumerator<T>
{
    readonly IAsyncEnumerator<T> enumerator = enumerator;
    HttpResponseMessage httpResponseMessage = httpResponseMessage;

    /// <inheritdoc />
    public T Current => enumerator.Current;

    /// <inheritdoc />
    public async ValueTask DisposeAsync()
    {
        await enumerator.DisposeAsync();
        if (httpResponseMessage != null)
        {
            httpResponseMessage.Dispose();
            httpResponseMessage = null!;
        }
    }

    /// <inheritdoc />
    public ValueTask<bool> MoveNextAsync() => enumerator.MoveNextAsync();
}
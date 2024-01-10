namespace BD.Common8.Ipc.Services.Implementation;

#pragma warning disable SA1600 // Elements should be documented

partial class IpcClientService
{
    /// <inheritdoc/>
    public virtual async Task<TResponseBody?> HubSendAsync<[DynamicallyAccessedMembers(DynamicallyAccessedMemberTypes.All)] TResponseBody>(
        string? hubUrl,
        string methodName,
        object?[]? args = null,
        CancellationToken cancellationToken = default)
        where TResponseBody : notnull
    {
        HubConnection? conn = null;
        try
        {
            conn = await GetHubConnAsync(hubUrl);
            var result = await conn.InvokeCoreAsync<TResponseBody>(methodName,
                args ?? [],
                cancellationToken);
            return result!;
        }
        catch (Exception e)
        {
            var result = OnError<TResponseBody>(e, conn);
            return result!;
        }
    }

    /// <inheritdoc/>
    public virtual async IAsyncEnumerable<TResponseBody?> HubSendAsAsyncEnumerable<[DynamicallyAccessedMembers(DynamicallyAccessedMemberTypes.All)] TResponseBody>(
        string? hubUrl,
        string methodName,
        object?[]? args = null,
        [EnumeratorCancellation] CancellationToken cancellationToken = default)
        where TResponseBody : notnull
    {
        IAsyncEnumerable<TResponseBody>? result = default;
        HubConnection? conn = null;
        try
        {
            conn = await GetHubConnAsync(hubUrl);
            result = conn.StreamAsyncCore<TResponseBody>(methodName,
                args ?? [],
                cancellationToken);
        }
        catch (Exception e)
        {
            OnError<nil>(e, conn);
        }
        if (result != default)
        {
            await using var enumerator = result.GetAsyncEnumerator(cancellationToken);
            TResponseBody? item = default!;
            bool hasItem = true;
            while (hasItem)
            {
                try
                {
                    hasItem = await enumerator.MoveNextAsync().ConfigureAwait(false);
                    if (hasItem)
                    {
                        item = enumerator.Current;
                    }
                    else
                    {
                        result = default;
                    }
                }
                catch (Exception e)
                {
                    OnError<nil>(e, conn);
                    break;
                }
                if (hasItem)
                    yield return item;
            }
        }
    }
}
namespace System.Threading;

public interface IRequestAbortedProvider
{
    CancellationToken RequestAborted => default;
}
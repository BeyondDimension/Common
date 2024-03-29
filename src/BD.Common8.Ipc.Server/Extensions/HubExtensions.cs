namespace Microsoft.AspNetCore.SignalR;

public static partial class HubExtensions
{
    /// <summary>
    /// 根据 <see cref="Hub"/> 获取 <see cref="HttpContext.RequestAborted"/>
    /// </summary>
    /// <param name="hub"></param>
    /// <returns></returns>
    public static CancellationToken RequestAborted(this Hub hub)
    {
        if (hub is IRequestAbortedProvider provider)
            return provider.RequestAborted;

        var httpContext = hub.Context.GetHttpContext();
        if (httpContext == null) return default;
        return httpContext.RequestAborted;
    }
}

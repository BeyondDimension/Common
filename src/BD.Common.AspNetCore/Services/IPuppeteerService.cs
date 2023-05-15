using PuppeteerSharp;

namespace BD.Common.Services;

public interface IPuppeteerService
{
    /// <summary>
    /// 使用 Puppeteer 获取 Html 字符串
    /// </summary>
    Task<HttpResponseWithStringContent> GetHtmlAsync(string url, CookieContainer? cookieContainer = null, CancellationToken cancellationToken = default);

    /// <summary>
    /// Http 响应与 String 内容
    /// </summary>
    /// <param name="RequestUrl">请求的 Url</param>
    /// <param name="ResponseContent">响应内容</param>
    /// <param name="StatusCode">状态码</param>
    /// <param name="StepTrace">调试用的步骤跟踪</param>
    /// <param name="PageUrl">执行完 GET 请求后当前页面的地址，例如请求的 Url 触发了重定向等操作</param>
    record struct HttpResponseWithStringContent(
        string RequestUrl,
        string? ResponseContent = null,
        //IReadOnlyDictionary<string, string>? ResponseHeaders = null,
        int? StatusCode = null,
        string? StepTrace = null,
        string? PageUrl = null);

    const int shortTimeout = 35000;
    const int longTimeout = 55000;
    const int btnClickDelay = 3200; // 等待按钮点击事件执行

    /// <summary>
    /// 选择页面元素在等待的一个时间内返回结果，要么成功获取到元素 <see cref="IElementHandle"/>，要么产生异常 <see cref="WaitTaskTimeoutException"/>
    /// </summary>
    /// <param name="Element"></param>
    /// <param name="Message"></param>
    record struct WaitForSelectorResult(IElementHandle? Element = default, string? Message = default)
    {
        public static implicit operator WaitForSelectorResult(WaitTaskTimeoutException exception) => new(null, exception.Message);

        public bool IsSuccess => Element != null;
    }

    record struct BrowserWrap(IBrowser Browser, string PathUserDataCache) : IDisposable, IAsyncDisposable
    {
        public void DeleteCaches()
        {
            if (!string.IsNullOrWhiteSpace(PathUserDataCache))
            {
                try
                {
                    IOPath.DirTryDelete(PathUserDataCache);
                }
                catch
                {
                }
            }
        }

        public void Dispose()
        {
            Browser.Dispose();
            DeleteCaches();
        }

        public async ValueTask DisposeAsync()
        {
            await Browser.DisposeAsync();
            DeleteCaches();
        }
    }
}
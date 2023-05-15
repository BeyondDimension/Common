using Nito.AsyncEx;
using Polly;
using PuppeteerSharp;
using static BD.Common.Services.IPuppeteerService;
using static System.Net.Http.HttpClientUseCookiesWithProxyServiceImpl;

namespace BD.Common.Services.Implementation;

public class PuppeteerService : ConsoleRandomGetUserAgentServiceImpl, IPuppeteerService, IDisposable
{
    protected readonly ILoggerFactory loggerFactory;
    protected readonly ILogger<PuppeteerService> logger;
    protected readonly IServiceProvider s;
    protected readonly string pathUserData;
    bool disposedValue;
    readonly AsyncLock initBrowserMutex = new();
#if !CONSOLE_TEST_STEAM_LOGIN
    readonly IDisposable? options_disposable;
#endif
    protected string webProxy = WebProxySettings.LocalClash;

    public PuppeteerService(IServiceProvider s, ILoggerFactory loggerFactory)
    {
        this.loggerFactory = loggerFactory;
        logger = loggerFactory.CreateLogger<PuppeteerService>();
        this.s = s;
#if !CONSOLE_TEST_STEAM_LOGIN
        var o = s.GetService<IOptionsMonitor<IAppSettings>>();
        var options = o == null ? default : o.CurrentValue;
        webProxy = GetWebProxy(options);
        options_disposable = o?.OnChange(o =>
        {
            webProxy = GetWebProxy(o);
        });
#endif
        pathUserData = Path.Combine(Path.GetTempPath(), "BD.Common", $"Puppeteer_{Environment.ProcessId}");
        logger.LogInformation("浏览器用户数据缓存路径：{pathUserData}", pathUserData);
    }

    protected virtual string GetWebProxy(IAppSettings? o)
    {
        if (o != null)
        {
            return $"{o.WebProxyAddress}:{o.WebProxyPort}";
        }
        return WebProxySettings.LocalClash;
    }

    /// <summary>
    /// 初始化浏览器
    /// <para>注意事项：</para>
    /// <list type="bullet">
    /// <item><see cref="GetBrowserExecutablePath"/> 在 Win 上使用已安装的 Edge 或 Chrome，因此不需要调用此函数</item>
    /// <item>在 Linux 容器中已通过 Dockerfile 已配置 Chrome，因此不需要调用此函数</item>
    /// </list>
    /// </summary>
    /// <param name="revision"></param>
    public async Task InitBrowserAsync(string revision = BrowserFetcher.DefaultChromiumRevision)
    {
        using (await initBrowserMutex.LockAsync())
        {
            using var browserFetcher = new BrowserFetcher();
            logger.LogInformation("正在初始化浏览器，Revision：{revision}", revision);
            // 下载或使用已下载的浏览器文件，目录默认位于程序根目录下
            var result = await browserFetcher.DownloadAsync(revision);
            logger.LogInformation("初始化浏览器完成，Platform：{Platform}，FolderPath：{FolderPath}", result.Platform, result.FolderPath);
        }
    }

    /// <summary>
    /// 获取浏览器执行文件目录
    /// </summary>
    /// <returns></returns>
    static string? GetBrowserExecutablePath()
    {
        if (OperatingSystem.IsWindows())
        {
            var programFiles = new HashSet<string>
            {
                Environment.GetFolderPath(Environment.SpecialFolder.ProgramFiles),
                Environment.GetFolderPath(Environment.SpecialFolder.ProgramFilesX86),
            };

            foreach (var item in programFiles)
            {
                var chromePath = Path.Combine(item, "Google", "Chrome", "Application", "chrome.exe");
                if (File.Exists(chromePath)) return chromePath;
            }

            foreach (var item in programFiles)
            {
                var edgePath = Path.Combine(item, "Microsoft", "Edge", "Application", "msedge.exe");
                if (File.Exists(edgePath)) return edgePath;
            }

        }
        return null;
    }

    /// <summary>
    /// 获取随机用户数据文件夹
    /// </summary>
    /// <returns></returns>
    string GetRandomUserDataDir()
    {
        var userDataDir = Path.Combine(pathUserData,
            $"{Guid.NewGuid().ToStringN()}_{DateTimeOffset.Now:yyMMddHHmmssfffffff}_{Environment.CurrentManagedThreadId}_{Random2.GenerateRandomString(5)}");
        IOPath.DirCreateByNotExists(userDataDir);
        return userDataDir;
    }

    public virtual bool Headless { get; set; } = true;

    /// <summary>
    /// 获取启动浏览器选项
    /// </summary>
    /// <returns></returns>
    protected virtual LaunchOptions GetLaunchBrowserOptions()
    {
        LaunchOptions options = new()
        {
            //#if DEBUG
            //            Headless = default, // Debug 模式下浏览器显示窗口
            //#endif
            UserDataDir = GetRandomUserDataDir(), // 使用随机的用户数据文件夹进行数据隔离
            Headless = Headless,
        };

        var executablePath = GetBrowserExecutablePath();
        if (executablePath != default) options.ExecutablePath = executablePath;

        var args = GetLaunchBrowserArgs();
        if (args.Any()) options.Args = args.ToArray();

        return options;
    }

    /// <summary>
    /// 获取启动浏览器参数
    /// </summary>
    /// <returns></returns>
    protected virtual IEnumerable<string> GetLaunchBrowserArgs()
    {
        if (UseWebProxy)
        {
            var webProxy = GetWebProxy();
            if (webProxy != null)
                yield return $"--proxy-server={webProxy}";
        }
        if (OperatingSystem.IsLinux())
        {
            // 在 Docker 容器中运行时关闭沙盒
            yield return "--no-sandbox";
            yield return "--disable-setuid-sandbox";
        }
        //if (!ImagesEnabled)
        //{
        //    yield return "--blink-settings=imagesEnabled=false";
        //}
        // https://github.com/litmus/HeadlessChromium.Puppeteer.Lambda.Dotnet/blob/c5127c1dfe5b5cdc815b5a1cde51e872413e5447/src/HeadlessChromium.Puppeteer.Lambda.Dotnet/HeadlessChromiumPuppeteerLauncher.cs
        //yield return "--disable-background-timer-throttling";
        //yield return "--disable-breakpad";
        //yield return "--disable-client-side-phishing-detection";
        //yield return "--disable-cloud-import";
        //yield return "--disable-default-apps";
        //yield return "--disable-dev-shm-usage";
        yield return "--disable-extensions";
        //yield return "--disable-gesture-typing";
        //yield return "--disable-hang-monitor";
        //yield return "--disable-infobars";
        //yield return "--disable-notifications";
        //yield return "--disable-offer-store-unmasked-wallet-cards";
        //yield return "--disable-offer-upload-credit-cards";
        //yield return "--disable-popup-blocking";
        //yield return "--disable-print-preview";
        //yield return "--disable-prompt-on-repost";
        //yield return "--disable-speech-api";
        //yield return "--disable-sync";
        //yield return "--disable-tab-for-desktop-share";
        //yield return "--disable-translate";
        //yield return "--disable-voice-input";
        //yield return "--disable-wake-on-wifi";
        //yield return "--disk-cache-size=33554432";
        //yield return "--enable-async-dns";
        //yield return "--enable-simple-cache-backend";
        //yield return "--enable-tcp-fast-open";
        //yield return "--enable-webgl";
        //yield return "--hide-scrollbars";
        //yield return "--ignore-gpu-blacklist";
        //yield return "--media-cache-size=33554432";
        //yield return "--metrics-recording-only";
        //yield return "--mute-audio";
        //yield return "--no-default-browser-check";
        //yield return "--no-first-run";
        //yield return "--no-pings";
        //yield return "--no-zygote";
        //yield return "--password-store=basic";
        //yield return "--prerender-from-omnibox=disabled";
        //yield return "--use-gl=swiftshader";
        //yield return "--use-mock-keychain";
        //yield return "--single-process";
    }

    /// <summary>
    /// 获取 Web 代理
    /// </summary>
    /// <returns></returns>
    protected virtual string? GetWebProxy() => webProxy;

    public void SetWebProxy(string value) => webProxy = value;

    /// <summary>
    /// 是否启用 Web 代理
    /// </summary>
    public virtual bool UseWebProxy { get; set; } = true;

    /// <summary>
    /// 是否加载图像
    /// </summary>
    public virtual bool ImagesEnabled { get; set; }

    /// <summary>
    /// 获取浏览器实例
    /// </summary>
    /// <returns></returns>
    public async Task<BrowserWrap> GetBrowserAsync()
    {
        var options = GetLaunchBrowserOptions();
        var b = await Puppeteer.LaunchAsync(options, loggerFactory);
        return new(b, options.UserDataDir);
    }

    static readonly TimeSpan timeoutWaitPageLoaded = TimeSpan.FromSeconds(150d);

    static readonly string[] LoadingTitles = new[] {
        "Human Verification", // 亚马逊人机验证
        "Just a moment...", // Cloudflare CDN
    };

    /// <summary>
    /// 等待页面加载完成
    /// </summary>
    /// <param name="page"></param>
    /// <param name="cancellationToken"></param>
    /// <returns></returns>
    public static async Task WaitPageLoaded(IPage page, CancellationToken cancellationToken = default)
    {
        // 亚马逊人机验证，通过标题判断
        // Policy.Timeout 超时时将抛出 TimeoutRejectedException
        var timeoutPolicy = Policy.TimeoutAsync(timeoutWaitPageLoaded);
        await timeoutPolicy.ExecuteAsync(WaitPageLoadedCore, cancellationToken);

        async Task WaitPageLoadedCore(CancellationToken cancellationToken)
        {
            while (!cancellationToken.IsCancellationRequested)
            {
                try
                {
                    var isLoaded = false;
                    try
                    {
                        var handle = await page.EvaluateExpressionHandleAsync(document_readyState);
                        var value = await handle.JsonValueAsync();
                        if (value is string str && str == document_readyState_complete)
                        {
                            isLoaded = true;
                        }
                    }
                    catch
                    {

                    }

                    if (isLoaded)
                    {
                        var title = await page.GetTitleAsync();
                        if (!string.IsNullOrWhiteSpace(title) &&
                            LoadingTitles.Contains(title))
                        {

                            // 等待页面渲染完成
                            await Task.Delay(1500, cancellationToken);
                            break;
                        }
                        else
                        {
                            break;
                        }
                    }
                }
                catch
                {

                }
                // 等待跳转
                await Task.Delay(1000, cancellationToken);
            }
        }
    }

    public async Task<HttpResponseWithStringContent> GetHtmlAsync(string url, CookieContainer? cookieContainer = null, CancellationToken cancellationToken = default)
    {
        HttpResponseWithStringContent rspResult = new(url);
        try
        {
            await using var lwnp = await LaunchWithNewPageCoreAsync(cancellationToken);
            var browser = lwnp.Browser.Browser;
            var page = lwnp.Page;

            await page.SetViewportAsync(new() { Width = 1366, Height = 768 });

            rspResult.StepTrace = "NewPageAfter";

            if (!UseBrowserUserAgent)
                await page.SetUserAgentAsync(GetUserAgent());

            cancellationToken.ThrowIfCancellationRequested();
            var useCookie = cookieContainer != null;
            if (useCookie)
            {
                // 从 Cookie 容器中读取当前 Url 的 Cookies 写入浏览器
                var cookies = cookieContainer!.GetCookies(new(url));
                await SetCookiesAsync(page, cookies);
            }

            rspResult.StepTrace = "SetReqCookiesAfter";

            cancellationToken.ThrowIfCancellationRequested();
            var rsp = await page.GoToAsync(url, timeout: 60000);
            rspResult.StatusCode = (int)rsp.Status;
            //rspResult.ResponseHeaders = rsp.Headers;

            rspResult.StepTrace = "GoToAfter";

            await WaitPageLoaded(page, cancellationToken);

            await Task.Delay(6000, cancellationToken); // 等待可能的 JS 执行

            rspResult.StepTrace = "WaitPageLoadedAfter";

            var maxTries = 3;
            for (int i = 1; i <= maxTries; i++)
            {
                rspResult.ResponseContent = await page.GetContentAsync();
                if (string.IsNullOrWhiteSpace(rspResult.ResponseContent))
                {
                    if (i == maxTries) return rspResult;
                    await Task.Delay(1000, cancellationToken);
                }
            }

            rspResult.StepTrace = "GetContentAfter";

            cancellationToken.ThrowIfCancellationRequested();
            if (useCookie)
            {
                // 从浏览器中获取当前 Url 的 Cookies 写入 Cookie 容器
                var cookies = await page.GetCookiesAsync(url);
                cancellationToken.ThrowIfCancellationRequested();
                SetCookies(cookieContainer!, cookies);
            }

            rspResult.StepTrace = "SetRspCookiesAfter";

            rspResult.PageUrl = page.Url;

            //#if !DEBUG
            //        await page.CloseAsync();
            //#endif

        }
        catch (Exception ex)
        {
            if (ex is OperationCanceledException) throw;
            logger.LogError(ex, "获取 Html 失败");
        }
        return rspResult;
    }

    #region Cookies

    /// <summary>
    /// 设置 Cookies，2 分钟
    /// </summary>
    static readonly TimeSpan timeoutSetCookies = TimeSpan.FromMinutes(2d);

    public async Task SetCookiesWithTimeoutAsync(IPage page, IEnumerable<Cookie>? cookies)
    {
        var timeoutPolicy = Policy.TimeoutAsync(timeoutSetCookies);
        await timeoutPolicy.ExecuteAsync(SetCookiesWithTimeoutCoreAsync);
        async Task SetCookiesWithTimeoutCoreAsync() => await SetCookiesAsync(page, cookies);
    }

    public virtual async Task SetCookiesAsync(IPage page, IEnumerable<Cookie>? cookies)
    {
        if (cookies == null || !cookies.Any()) return;
        var cookieParams = cookies.Select(Parse).ToArray();
        await page.SetCookieAsync(cookieParams);
    }

    public virtual void SetCookies(CookieContainer cookieContainer, IEnumerable<CookieParam>? cookieParams)
    {
        if (cookieParams == null || !cookieParams.Any()) return;
        foreach (var cookieParam in cookieParams)
        {
            var cookie = Parse(cookieParam);
            cookieContainer.Add(cookie);
        }
    }

    static CookieParam Parse(Cookie cookie)
    {
        var cookieParam = new CookieParam()
        {
            Name = cookie.Name,
            Value = cookie.Value,
            Domain = cookie.Domain,
            Path = cookie.Path,
            Expires = cookie.Expires == DateTime.MinValue ? null : cookie.Expires.ToUnixTimeSeconds(),
            HttpOnly = cookie.HttpOnly,
            Secure = cookie.Secure,
        };
        return cookieParam;
    }

    static Cookie Parse(CookieParam cookieParam)
    {
        var cookie = new Cookie()
        {
            Name = cookieParam.Name,
            Value = cookieParam.Value,
            Domain = cookieParam.Domain,
            Path = cookieParam.Path,
            HttpOnly = cookieParam.HttpOnly ?? false,
            Secure = cookieParam.Secure ?? false,
        };
        if (cookieParam.Expires.HasValue && cookieParam.Expires.Value != -1d)
        {
            cookie.Expires = Convert.ToInt64(
                Math.Floor(cookieParam.Expires.Value))
                    .ToDateTime(UnixTimestampType.Seconds);
        }
        else if (!cookie.Expired)
        {
            // chrome 的 Cookie 无值则永不过期
            cookie.Expires = DateTime.MaxValue;
        }
        return cookie;
    }

    #endregion

    #region UserAgent

    /// <summary>
    /// 是否使用浏览器 UA
    /// <para><see cref="LaunchOptions.Headless"/> 为 <see langword="true"/> 时，不显示浏览器窗口，会在 UA 中有标识，所以不应使用浏览器默认值</para>
    /// </summary>
    public virtual bool UseBrowserUserAgent { get; set; }

    //public async Task<string> GetBrowserUserAgentAsync()
    //{
    //    using var browser = await GetBrowserAsync();
    //    using var page = await browser.NewPageAsync();

    //    var handle = await page.EvaluateExpressionHandleAsync(navigator_userAgent);
    //    var value = await handle.JsonValueAsync();
    //    if (value is string str) return str;

    //    return IRandomGetUserAgentService.Win10ChromeLatest;
    //}

    //readonly object lockUserAgent = new();

    public sealed override string GetUserAgent()
    {
        //if (UseBrowserUserAgent)
        //{
        //    lock (lockUserAgent)
        //    {
        //        if (UserAgent == default)
        //        {
        //            UserAgent = GetBrowserUserAgentAsync().GetAwaiter().GetResult();
        //            logger.LogInformation("UserAgent：{UserAgent}", UserAgent);
        //        }
        //    }
        //    return UserAgent;
        //}
        //else
        //{
        UserAgent ??= IRandomGetUserAgentService.Win10ChromeLatest;
        return UserAgent;
        //}
    }

    public sealed override void RefreshUserAgent() { }

    #endregion

    public Exception? DeleteCaches()
    {
        try
        {
            IOPath.DirTryDelete(pathUserData);
            try
            {
                logger.LogInformation("已删除缓存：{pathUserData}", pathUserData);
            }
            catch (Exception e)
            {
                return e;
            }
        }
        catch (Exception e)
        {
            return e;
        }
        return null;
    }

    protected virtual void Dispose(bool disposing)
    {
        if (!disposedValue)
        {
            if (disposing)
            {
                // 释放托管状态(托管对象)
#if !CONSOLE_TEST_STEAM_LOGIN
                options_disposable?.Dispose();
#endif
                DeleteCaches();
            }

            // 释放未托管的资源(未托管的对象)并重写终结器
            // 将大型字段设置为 null
            disposedValue = true;
        }
    }

    public void Dispose()
    {
        // 不要更改此代码。请将清理代码放入“Dispose(bool disposing)”方法中
        Dispose(disposing: true);
        GC.SuppressFinalize(this);
    }

    const string navigator_userAgent = "navigator.userAgent";

    /// <summary>
    /// https://developer.mozilla.org/zh-CN/docs/Web/API/Document/readyState
    /// </summary>
    const string document_readyState = "document.readyState";

    const string document_readyState_complete = "complete";

    /// <summary>
    /// 启动浏览器建立新页面超时时间，5 分钟
    /// </summary>
    static readonly TimeSpan timeoutLaunchWithNewPage = TimeSpan.FromMinutes(5d);

    public sealed record class LaunchWithNewPageResult(BrowserWrap Browser, IPage Page) : /*IDisposable,*/ IAsyncDisposable
    {
        //bool disposedValue;

        //private async void Dispose(bool disposing)
        //{
        //    if (!disposedValue)
        //    {
        //        if (disposing)
        //        {
        //            // 释放托管状态(托管对象)
        //            Page.Dispose();
        //            Browser.Dispose();
        //        }

        //        // 释放未托管的资源(未托管的对象)并重写终结器
        //        // 将大型字段设置为 null
        //        disposedValue = true;
        //    }
        //}

        //public void Dispose()
        //{
        //    // 不要更改此代码。请将清理代码放入“Dispose(bool disposing)”方法中
        //    Dispose(disposing: true);
        //    GC.SuppressFinalize(this);
        //}

        public async ValueTask DisposeAsync()
        {
            try
            {
                await Page.DisposeAsync();
            }
            catch
            {

            }
            await Browser.DisposeAsync();
        }
    }

    public async Task<LaunchWithNewPageResult> LaunchWithNewPageAsync(CancellationToken cancellationToken = default)
    {
        logger.LogInformation("lockLaunchWithNewPageAsync.LockAsync");
        //using (await lockLaunchWithNewPageAsync.LockAsync(cancellationToken))
        //{
        var timeoutPolicy = Policy.TimeoutAsync(timeoutLaunchWithNewPage);
        logger.LogInformation("timeoutPolicy.ExecuteAsync");
        var result = await timeoutPolicy.ExecuteAsync(LaunchWithNewPageCoreAsync, cancellationToken);
        return result;
        //}
    }

    //static readonly AsyncLock lockLaunchWithNewPageAsync = new();

    async Task<LaunchWithNewPageResult> LaunchWithNewPageCoreAsync(CancellationToken cancellationToken = default)
    {
        logger.LogInformation("GetBrowserAsync");
        var browserW = await GetBrowserAsync();
        var browser = browserW.Browser;
        logger.LogInformation("NewPageAsync");
        var page = await browser.NewPageAsync();

        logger.LogInformation("SetViewportAsync");
        await page.SetViewportAsync(new() { Width = 1366, Height = 768 });

        if (!UseBrowserUserAgent)
        {
            logger.LogInformation("SetUserAgentAsync");
            await page.SetUserAgentAsync(GetUserAgent());
        }

        return new(browserW, page);
    }
}
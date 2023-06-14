// https://www.cnblogs.com/lindexi/p/16706558.html
// https://blog.lindexi.com/post/dotnet-6-%E4%B8%BA%E4%BB%80%E4%B9%88%E7%BD%91%E7%BB%9C%E8%AF%B7%E6%B1%82%E4%B8%8D%E8%B7%9F%E9%9A%8F%E7%B3%BB%E7%BB%9F%E7%BD%91%E7%BB%9C%E4%BB%A3%E7%90%86%E5%8F%98%E5%8C%96%E8%80%8C%E5%8A%A8%E6%80%81%E5%88%87%E6%8D%A2%E4%BB%A3%E7%90%86.html

#if NETFRAMEWORK || WINDOWS7_0_OR_GREATER

using System.Collections.Concurrent;
using System.Net.NetworkInformation;
using Microsoft.Win32.SafeHandles;
using static System.String;

// ReSharper disable once CheckNamespace
namespace System.Net;

[SupportedOSPlatform("Windows")]
public sealed class DynamicHttpWindowsProxy : IWebProxy, IDisposable
{
    public static readonly DynamicHttpWindowsProxy Instance = new();

    DynamicHttpWindowsProxy()
    {
        if (HttpWindowsProxy.TryCreate(out var proxy))
        {
            _innerProxy = proxy;
        }
        else
        {
            _innerProxy = HttpNoProxy.Instance;
        }

        RegistryMonitor = new RegistryMonitor_(RegistryHive.CurrentUser, @"Software\Microsoft\Windows\CurrentVersion\Internet Settings\Connections");
        RegistryMonitor.RegChanged += RegistryMonitor_RegChanged;
        RegistryMonitor.Start();
    }

    void RegistryMonitor_RegChanged(object? sender, EventArgs e)
    {
        if (HttpWindowsProxy.TryCreate(out var proxy))
        {
            InnerProxy = proxy;
        }
        else
        {
            InnerProxy = HttpNoProxy.Instance;
        }
    }

    RegistryMonitor_? RegistryMonitor { get; set; }

    IWebProxy InnerProxy
    {
        get => _innerProxy;
        set
        {
            if (ReferenceEquals(_innerProxy, value))
            {
                return;
            }

            if (_innerProxy is IDisposable disposable)
            {
                disposable.Dispose();
            }

            _innerProxy = value;
        }
    }

    public ICredentials? Credentials
    {
        get => InnerProxy.Credentials;
        set => InnerProxy.Credentials = value;
    }

    public Uri? GetProxy(Uri destination)
    {
        return InnerProxy.GetProxy(destination);
    }

    public bool IsBypassed(Uri host)
    {
        return InnerProxy.IsBypassed(host);
    }

    bool disposedValue;

    void Dispose(bool disposing)
    {
        if (!disposedValue)
        {
            if (disposing)
            {
                // TODO: 释放托管状态(托管对象)
                if (_innerProxy is IDisposable disposable)
                {
                    disposable.Dispose();
                }

                RegistryMonitor?.Dispose();
            }

            // TODO: 释放未托管的资源(未托管的对象)并重写终结器
            // TODO: 将大型字段设置为 null
            disposedValue = true;
        }
    }

    public void Dispose()
    {
        // 不要更改此代码。请将清理代码放入“Dispose(bool disposing)”方法中
        Dispose(disposing: true);
        GC.SuppressFinalize(this);
    }

    private IWebProxy _innerProxy;

    [SupportedOSPlatform("Windows")]
    sealed class HttpWindowsProxy : IWebProxy, IDisposable
    {
        private readonly MultiProxy _insecureProxy;    // URI of the http system proxy if set
        private readonly MultiProxy _secureProxy;      // URI of the https system proxy if set
        private readonly FailedProxyCache _failedProxies = new();
        private readonly List<string>? _bypass;         // list of domains not to proxy
        private readonly bool _bypassLocal;    // we should bypass domain considered local
        private readonly List<IPAddress>? _localIp;
        private ICredentials? _credentials;
        private readonly WinInetProxyHelper _proxyHelper;
        private Vanara.PInvoke.WinHTTP.SafeHINTERNET? _sessionHandle;
        private bool _disposed;

        public static bool TryCreate([NotNullWhen(true)] out IWebProxy? proxy)
        {
            // This will get basic proxy setting from system using existing
            // WinInetProxyHelper functions. If no proxy is enabled, it will return null.
            Vanara.PInvoke.WinHTTP.SafeHINTERNET? sessionHandle = null;
            proxy = null;

            WinInetProxyHelper proxyHelper = new();
            if (!proxyHelper.ManualSettingsOnly && !proxyHelper.AutoSettingsUsed)
            {
                return false;
            }

            if (proxyHelper.AutoSettingsUsed)
            {
                sessionHandle = Vanara.PInvoke.WinHTTP.WinHttpOpen(null,
                    Vanara.PInvoke.WinHTTP.WINHTTP_ACCESS_TYPE.WINHTTP_ACCESS_TYPE_NO_PROXY,
                    Interop.WinHttp.WINHTTP_NO_PROXY_NAME,
                    Interop.WinHttp.WINHTTP_NO_PROXY_BYPASS,
                    Vanara.PInvoke.WinHTTP.WINHTTP_OPEN_FLAG.WINHTTP_FLAG_ASYNC
                );

                if (sessionHandle.IsInvalid)
                {
                    // Proxy failures are currently ignored by managed handler.
                    //if (NetEventSource.Log.IsEnabled()) NetEventSource.Error(proxyHelper, $"{nameof(Interop.WinHttp.WinHttpOpen)} returned invalid handle");
                    return false;
                }
            }

            proxy = new HttpWindowsProxy(proxyHelper, sessionHandle);
            return true;
        }

        private HttpWindowsProxy(WinInetProxyHelper proxyHelper, Vanara.PInvoke.WinHTTP.SafeHINTERNET? sessionHandle)
        {
            _proxyHelper = proxyHelper;
            _sessionHandle = sessionHandle;

            if (proxyHelper.ManualSettingsUsed)
            {
                _secureProxy = MultiProxy.Parse(_failedProxies, proxyHelper.Proxy, true);
                _insecureProxy = MultiProxy.Parse(_failedProxies, proxyHelper.Proxy, false);

                if (!IsNullOrWhiteSpace(proxyHelper.ProxyBypass))
                {
                    int idx = 0;
                    string? tmp;

                    // Process bypass list for manual setting.
                    // Initial list size is best guess based on string length assuming each entry is at least 5 characters on average.
                    _bypass = new List<string>(proxyHelper.ProxyBypass.Length / 5);

                    while (idx < proxyHelper.ProxyBypass.Length)
                    {
                        // Strip leading spaces and scheme if any.
                        while (idx < proxyHelper.ProxyBypass.Length && proxyHelper.ProxyBypass[idx] == ' ') { idx += 1; }
                        if (Compare(proxyHelper.ProxyBypass, idx, "http://", 0, 7, StringComparison.OrdinalIgnoreCase) == 0)
                        {
                            idx += 7;
                        }
                        else if (Compare(proxyHelper.ProxyBypass, idx, "https://", 0, 8, StringComparison.OrdinalIgnoreCase) == 0)
                        {
                            idx += 8;
                        }

                        if (idx < proxyHelper.ProxyBypass.Length && proxyHelper.ProxyBypass[idx] == '[')
                        {
                            // Strip [] from IPv6 so we can use IdnHost laster for matching.
                            idx += 1;
                        }

                        int start = idx;
                        while (idx < proxyHelper.ProxyBypass.Length && proxyHelper.ProxyBypass[idx] != ' ' && proxyHelper.ProxyBypass[idx] != ';' && proxyHelper.ProxyBypass[idx] != ']') { idx += 1; }

                        if (idx == start)
                        {
                            // Empty string.
                            tmp = null;
                        }
                        else if (Compare(proxyHelper.ProxyBypass, start, "<local>", 0, 7, StringComparison.OrdinalIgnoreCase) == 0)
                        {
                            _bypassLocal = true;
                            tmp = null;
                        }
                        else
                        {
                            tmp = proxyHelper.ProxyBypass[start..idx];
                        }

                        // Skip trailing characters if any.
                        if (idx < proxyHelper.ProxyBypass.Length && proxyHelper.ProxyBypass[idx] != ';')
                        {
                            // Got stopped at space or ']'. Strip until next ';' or end.
                            while (idx < proxyHelper.ProxyBypass.Length && proxyHelper.ProxyBypass[idx] != ';') { idx += 1; }
                        }
                        if (idx < proxyHelper.ProxyBypass.Length && proxyHelper.ProxyBypass[idx] == ';')
                        {
                            idx++;
                        }
                        if (tmp == null)
                        {
                            continue;
                        }

                        _bypass.Add(tmp);
                    }
                    if (_bypass.Count == 0)
                    {
                        // Bypass string only had garbage we did not parse.
                        _bypass = null;
                    }
                }

                if (_bypassLocal)
                {
                    _localIp = new List<IPAddress>();
                    foreach (NetworkInterface netInterface in NetworkInterface.GetAllNetworkInterfaces())
                    {
                        IPInterfaceProperties ipProps = netInterface.GetIPProperties();
                        foreach (UnicastIPAddressInformation addr in ipProps.UnicastAddresses)
                        {
                            _localIp.Add(addr.Address);
                        }
                    }
                }
            }
        }

        public void Dispose()
        {
            if (!_disposed)
            {
                _disposed = true;

                if (_sessionHandle != null && !_sessionHandle.IsInvalid)
                {
                    _sessionHandle.Dispose();
                    _sessionHandle = null;
                }
            }
        }

        /// <summary>
        /// Gets the proxy URI. (IWebProxy interface)
        /// </summary>
        public Uri? GetProxy(Uri uri)
        {
            GetMultiProxy(uri).ReadNext(out Uri? proxyUri, out _);
            return proxyUri;
        }

        /// <summary>
        /// Gets the proxy URIs.
        /// </summary>
        public MultiProxy GetMultiProxy(Uri uri)
        {
            // We need WinHTTP to detect and/or process a PAC (JavaScript) file. This maps to
            // "Automatically detect settings" and/or "Use automatic configuration script" from IE
            // settings. But, calling into WinHTTP can be slow especially when it has to call into
            // the out-of-process service to discover, load, and run the PAC file. So, we skip
            // calling into WinHTTP if there was a recent failure to detect a PAC file on the network.
            // This is a common error. The default IE settings on a Windows machine consist of the
            // single checkbox for "Automatically detect settings" turned on and most networks
            // won't actually discover a PAC file on the network since WPAD protocol isn't configured.
            if (_proxyHelper.AutoSettingsUsed && !_proxyHelper.RecentAutoDetectionFailure)
            {
                Vanara.PInvoke.WinHTTP.WINHTTP_PROXY_INFO proxyInfo = default;
                try
                {
                    if (_proxyHelper.GetProxyForUrl(_sessionHandle, uri, out proxyInfo))
                    {
                        // If WinHTTP just specified a Proxy with no ProxyBypass list, then
                        // we can return the Proxy uri directly.
                        if (proxyInfo.lpszProxyBypass == IntPtr.Zero)
                        {
                            if (proxyInfo.lpszProxy != IntPtr.Zero)
                            {
                                string proxyStr = proxyInfo.lpszProxy;
                                return MultiProxy.CreateLazy(_failedProxies, proxyStr, IsSecureUri(uri));
                            }
                            else
                            {
                                return MultiProxy.Empty;
                            }
                        }

                        // A bypass list was also specified. This means that WinHTTP has fallen back to
                        // using the manual IE settings specified and there is a ProxyBypass list also.
                        // Since we're not really using the full WinHTTP stack, we need to use HttpSystemProxy
                        // to do the computation of the final proxy uri merging the information from the Proxy
                        // and ProxyBypass strings.
                    }
                    else
                    {
                        return MultiProxy.Empty;
                    }
                }
                finally
                {
                    proxyInfo.FreeMemory();
                }
            }

            // Fallback to manual settings if present.
            if (_proxyHelper.ManualSettingsUsed)
            {
                if (_bypassLocal)
                {
                    IPAddress? address;

                    if (uri.IsLoopback)
                    {
                        // This is optimization for loopback addresses.
                        // Unfortunately this does not work for all local addresses.
                        return MultiProxy.Empty;
                    }

                    // Pre-Check if host may be IP address to avoid parsing.
                    if (uri.HostNameType == UriHostNameType.IPv6 || uri.HostNameType == UriHostNameType.IPv4)
                    {
                        // RFC1123 allows labels to start with number.
                        // Leading number may or may not be IP address.
                        // IPv6 [::1] notation. '[' is not valid character in names.
                        if (IPAddress.TryParse(uri.IdnHost, out address))
                        {
                            // Host is valid IP address.
                            // Check if it belongs to local system.
                            foreach (IPAddress a in _localIp!)
                            {
                                if (a.Equals(address))
                                {
                                    return MultiProxy.Empty;
                                }
                            }
                        }
                    }
                    if (uri.HostNameType != UriHostNameType.IPv6 && !uri.IdnHost.Contains('.'))
                    {
                        // Not address and does not have a dot.
                        // Hosts without FQDN are considered local.
                        return MultiProxy.Empty;
                    }
                }

                // Check if we have other rules for bypass.
                if (_bypass != null)
                {
                    foreach (string entry in _bypass)
                    {
                        // IdnHost does not have [].
                        if (SimpleRegex.IsMatchWithStarWildcard(uri.IdnHost, entry))
                        {
                            return MultiProxy.Empty;
                        }
                    }
                }

                // We did not find match on bypass list.
                return IsSecureUri(uri) ? _secureProxy : _insecureProxy;
            }

            return MultiProxy.Empty;
        }

        private static bool IsSecureUri(Uri uri)
        {
            return uri.Scheme == UriScheme.Https || uri.Scheme == UriScheme.Wss;
        }

        /// <summary>
        /// Checks if URI is subject to proxy or not.
        /// </summary>
        public bool IsBypassed(Uri uri)
        {
            // This HttpSystemProxy class is only consumed by SocketsHttpHandler and is not exposed outside of
            // SocketsHttpHandler. The current pattern for consumption of IWebProxy is to call IsBypassed first.
            // If it returns false, then the caller will call GetProxy. For this proxy implementation, computing
            // the return value for IsBypassed is as costly as calling GetProxy. We want to avoid doing extra
            // work. So, this proxy implementation for the IsBypassed method can always return false. Then the
            // GetProxy method will return non-null for a proxy, or null if no proxy should be used.
            return false;
        }

        public ICredentials? Credentials
        {
            get => _credentials;
            set => _credentials = value;
        }

        // Access function for unit tests.
        internal List<string>? BypassList => _bypass;
    }

    /// <summary>
    /// A collection of proxies.
    /// </summary>
    struct MultiProxy
    {
        private static readonly char[] proxyDelimiters = { ';', ' ', '\n', '\r', '\t' };
        private readonly FailedProxyCache? _failedProxyCache;
        private readonly Uri[]? _uris;
        private readonly string? _proxyConfig;
        private readonly bool _secure;
        private int _currentIndex;
        private Uri? _currentUri;

        public static MultiProxy Empty => new(null, Array.Empty<Uri>());

        private MultiProxy(FailedProxyCache? failedProxyCache, Uri[] uris)
        {
            _failedProxyCache = failedProxyCache;
            _uris = uris;
            _proxyConfig = null;
            _secure = default;
            _currentIndex = 0;
            _currentUri = null;
        }

        private MultiProxy(FailedProxyCache failedProxyCache, string proxyConfig, bool secure)
        {
            _failedProxyCache = failedProxyCache;
            _uris = null;
            _proxyConfig = proxyConfig;
            _secure = secure;
            _currentIndex = 0;
            _currentUri = null;
        }

        /// <summary>
        /// Parses a WinHTTP proxy config into a MultiProxy instance.
        /// </summary>
        /// <param name="failedProxyCache">The cache of failed proxy requests to employ.</param>
        /// <param name="proxyConfig">The WinHTTP proxy config to parse.</param>
        /// <param name="secure">If true, return proxies suitable for use with a secure connection. If false, return proxies suitable for an insecure connection.</param>
        public static MultiProxy Parse(FailedProxyCache failedProxyCache, string? proxyConfig, bool secure)
        {
            Debug.Assert(failedProxyCache != null);

            Uri[] uris = Array.Empty<Uri>();

            ReadOnlySpan<char> span = proxyConfig;
            while (TryParseProxyConfigPart(span, secure, out Uri? uri, out int charactersConsumed))
            {
                int idx = uris.Length;

                // Assume that we will typically not have more than 1...3 proxies, so just
                // grow by 1. This method is currently only used once per process, so the
                // case of an abnormally large config will not be much of a concern anyway.
                Array.Resize(ref uris, idx + 1);
                uris[idx] = uri;

                span = span[charactersConsumed..];
            }

            return new MultiProxy(failedProxyCache, uris);
        }

        /// <summary>
        /// Initializes a MultiProxy instance that lazily parses a given WinHTTP configuration string.
        /// </summary>
        /// <param name="failedProxyCache">The cache of failed proxy requests to employ.</param>
        /// <param name="proxyConfig">The WinHTTP proxy config to parse.</param>
        /// <param name="secure">If true, return proxies suitable for use with a secure connection. If false, return proxies suitable for an insecure connection.</param>
        public static MultiProxy CreateLazy(FailedProxyCache failedProxyCache, string proxyConfig, bool secure)
        {
            Debug.Assert(failedProxyCache != null);

            return IsNullOrEmpty(proxyConfig) == false ?
                new MultiProxy(failedProxyCache, proxyConfig, secure) :
                Empty;
        }

        /// <summary>
        /// Reads the next proxy URI from the MultiProxy.
        /// </summary>
        /// <param name="uri">The next proxy to use for the request.</param>
        /// <param name="isFinalProxy">If true, indicates there are no further proxies to read from the config.</param>
        /// <returns>If there is a proxy available, true. Otherwise, false.</returns>
        public bool ReadNext([NotNullWhen(true)] out Uri? uri, out bool isFinalProxy)
        {
            // Enumerating indicates the previous proxy has failed; mark it as such.
            if (_currentUri != null)
            {
                Debug.Assert(_failedProxyCache != null);
                _failedProxyCache.SetProxyFailed(_currentUri);
            }

            // If no more proxies to read, return out quickly.
            if (!ReadNextHelper(out uri, out isFinalProxy))
            {
                _currentUri = null;
                return false;
            }

            // If this is the first ReadNext() and all proxies are marked as failed, return the proxy that is closest to renewal.
            Uri? oldestFailedProxyUri = null;
            long oldestFailedProxyTicks = long.MaxValue;

            do
            {
                Debug.Assert(_failedProxyCache != null);
                long renewTicks = _failedProxyCache.GetProxyRenewTicks(uri);

                // Proxy hasn't failed recently, return for use.
                if (renewTicks == FailedProxyCache.Immediate)
                {
                    _currentUri = uri;
                    return true;
                }

                if (renewTicks < oldestFailedProxyTicks)
                {
                    oldestFailedProxyUri = uri;
                    oldestFailedProxyTicks = renewTicks;
                }
            }
            while (ReadNextHelper(out uri, out isFinalProxy));

            // All the proxies in the config have failed; in this case, return the proxy that is closest to renewal.
            if (_currentUri == null)
            {
                uri = oldestFailedProxyUri;
                _currentUri = oldestFailedProxyUri;

                if (oldestFailedProxyUri != null)
                {
                    Debug.Assert(uri != null);
                    _failedProxyCache.TryRenewProxy(uri, oldestFailedProxyTicks);
                    return true;
                }
            }

            return false;
        }

        /// <summary>
        /// Reads the next proxy URI from the MultiProxy, either via parsing a config string or from an array.
        /// </summary>
        private bool ReadNextHelper([NotNullWhen(true)] out Uri? uri, out bool isFinalProxy)
        {
            Debug.Assert(_uris != null || _proxyConfig != null, $"{nameof(ReadNext)} must not be called on a default-initialized {nameof(MultiProxy)}.");

            if (_uris != null)
            {
                if (_currentIndex == _uris.Length)
                {
                    uri = default;
                    isFinalProxy = default;
                    return false;
                }

                uri = _uris[_currentIndex++];
                isFinalProxy = _currentIndex == _uris.Length;
                return true;
            }

            Debug.Assert(_proxyConfig != null);
            if (_currentIndex < _proxyConfig.Length)
            {
                bool hasProxy = TryParseProxyConfigPart(_proxyConfig.AsSpan(_currentIndex), _secure, out uri!, out int charactersConsumed);

                _currentIndex += charactersConsumed;
                Debug.Assert(_currentIndex <= _proxyConfig.Length);

                isFinalProxy = _currentIndex == _proxyConfig.Length;

                return hasProxy;
            }

            uri = default;
            isFinalProxy = default;
            return false;
        }

        /// <summary>
        /// This method is used to parse WinINet Proxy strings, a single proxy at a time.
        /// </summary>
        /// <remarks>
        /// The strings are a semicolon or whitespace separated list, with each entry in the following format:
        /// ([&lt;scheme&gt;=][&lt;scheme&gt;"://"]&lt;server&gt;[":"&lt;port&gt;])
        /// </remarks>
        private static bool TryParseProxyConfigPart(ReadOnlySpan<char> proxyString, bool secure, [NotNullWhen(true)] out Uri? uri, out int charactersConsumed)
        {
            const int SECURE_FLAG = 1;
            const int INSECURE_FLAG = 2;

            int wantedFlag = secure ? SECURE_FLAG : INSECURE_FLAG;
            int originalLength = proxyString.Length;

            while (true)
            {
                // Skip any delimiters.
                int iter = 0;
                while (iter < proxyString.Length && Array.IndexOf(proxyDelimiters, proxyString[iter]) >= 0)
                {
                    ++iter;
                }

                if (iter == proxyString.Length)
                {
                    break;
                }

                proxyString = proxyString[iter..];

                // Determine which scheme this part is for.
                // If no schema is defined, use both.
                int proxyType = SECURE_FLAG | INSECURE_FLAG;

                if (proxyString.StartsWith("http="))
                {
                    proxyType = INSECURE_FLAG;
                    proxyString = proxyString["http=".Length..];
                }
                else if (proxyString.StartsWith("https="))
                {
                    proxyType = SECURE_FLAG;
                    proxyString = proxyString["https=".Length..];
                }

                if (proxyString.StartsWith("http://"))
                {
                    proxyType = INSECURE_FLAG;
                    proxyString = proxyString["http://".Length..];
                }
                else if (proxyString.StartsWith("https://"))
                {
                    proxyType = SECURE_FLAG;
                    proxyString = proxyString["https://".Length..];
                }

                // Find the next delimiter, or end of string.
                iter = proxyString.IndexOfAny(proxyDelimiters);
                if (iter < 0)
                {
                    iter = proxyString.Length;
                }

                // Return URI if it's a match to what we want.
                if ((proxyType & wantedFlag) != 0 && Uri.TryCreate(Concat("http://", proxyString[..iter]), UriKind.Absolute, out uri))
                {
                    charactersConsumed = originalLength - proxyString.Length + iter;
                    Debug.Assert(charactersConsumed > 0);

                    return true;
                }

                proxyString = proxyString[iter..];
            }

            uri = null;
            charactersConsumed = originalLength;
            return false;
        }
    }

    /// <summary>
    /// Holds a cache of failing proxies and manages when they should be retried.
    /// </summary>
    sealed class FailedProxyCache
    {
        /// <summary>
        /// When returned by <see cref="GetProxyRenewTicks"/>, indicates a proxy is immediately usable.
        /// </summary>
        public const long Immediate = 0;

        // If a proxy fails, time out 30 minutes. WinHTTP and Firefox both use this.
        private const int FailureTimeoutInMilliseconds = 1000 * 60 * 30;

        // Scan through the failures and flush any that have expired every 5 minutes.
        private const int FlushFailuresTimerInMilliseconds = 1000 * 60 * 5;

        // _failedProxies will only be flushed (rare but somewhat expensive) if we have more than this number of proxies in our dictionary. See Cleanup() for details.
        private const int LargeProxyConfigBoundary = 8;

        // Value is the Environment.TickCount64 to remove the proxy from the failure list.
        private readonly ConcurrentDictionary<Uri, long> _failedProxies = new();

        // When Environment.TickCount64 >= _nextFlushTicks, cause a flush.
        private long _nextFlushTicks = Environment.TickCount64 + FlushFailuresTimerInMilliseconds;

        // This lock can be folded into _nextFlushTicks for space optimization, but
        // this class should only have a single instance so would rather have clarity.
        private SpinLock _flushLock = new(enableThreadOwnerTracking: false); // mutable struct; do not make this readonly

        /// <summary>
        /// Checks when a proxy will become usable.
        /// </summary>
        /// <param name="uri">The <see cref="Uri"/> of the proxy to check.</param>
        /// <returns>If the proxy can be used, <see cref="Immediate"/>. Otherwise, the next <see cref="Environment.TickCount64"/> that it should be used.</returns>
        public long GetProxyRenewTicks(Uri uri)
        {
            Cleanup();

            // If not failed, ready immediately.
            if (!_failedProxies.TryGetValue(uri, out long renewTicks))
            {
                return Immediate;
            }

            // If we haven't reached out renew time, the proxy can't be used.
            if (Environment.TickCount64 < renewTicks)
            {
                return renewTicks;
            }

            // Renew time reached, we can remove the proxy from the cache.
            if (TryRenewProxy(uri, renewTicks))
            {
                return Immediate;
            }

            // Another thread updated the cache before we could remove it.
            // We can't know if this is a removal or an update, so check again.
            return _failedProxies.TryGetValue(uri, out renewTicks) ? renewTicks : Immediate;
        }

        /// <summary>
        /// Sets a proxy as failed, to avoid trying it again for some time.
        /// </summary>
        /// <param name="uri">The URI of the proxy.</param>
        public void SetProxyFailed(Uri uri)
        {
            _failedProxies[uri] = Environment.TickCount64 + FailureTimeoutInMilliseconds;
            Cleanup();
        }

        /// <summary>
        /// Renews a proxy prior to its period expiring. Used when all proxies are failed to renew the proxy closest to being renewed.
        /// </summary>
        /// <param name="uri">The <paramref name="uri"/> of the proxy to renew.</param>
        /// <param name="renewTicks">The current renewal time for the proxy. If the value has changed from this, the proxy will not be renewed.</param>
        public bool TryRenewProxy(Uri uri, long renewTicks) =>
            _failedProxies.TryRemove(new KeyValuePair<Uri, long>(uri, renewTicks));

        /// <summary>
        /// Cleans up any old proxies that should no longer be marked as failing.
        /// </summary>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        private void Cleanup()
        {
            if (_failedProxies.Count > LargeProxyConfigBoundary && Environment.TickCount64 >= Interlocked.Read(ref _nextFlushTicks))
            {
                CleanupHelper();
            }
        }

        /// <summary>
        /// Cleans up any old proxies that should no longer be marked as failing.
        /// </summary>
        /// <remarks>
        /// I expect this to never be called by <see cref="Cleanup"/> in a production system. It is only needed in the case
        /// that a system has a very large number of proxies that the PAC script cycles through. It is moderately expensive,
        /// so it's only run periodically and is disabled until we exceed <see cref="LargeProxyConfigBoundary"/> failed proxies.
        /// </remarks>
        [MethodImpl(MethodImplOptions.NoInlining)]
        private void CleanupHelper()
        {
            bool lockTaken = false;
            try
            {
                _flushLock.TryEnter(ref lockTaken);
                if (!lockTaken)
                {
                    return;
                }

                long curTicks = Environment.TickCount64;

                foreach (KeyValuePair<Uri, long> kvp in _failedProxies)
                {
                    if (curTicks >= kvp.Value)
                    {
                        ((ICollection<KeyValuePair<Uri, long>>)_failedProxies).Remove(kvp);
                    }
                }
            }
            finally
            {
                if (lockTaken)
                {
                    Interlocked.Exchange(ref _nextFlushTicks, Environment.TickCount64 + FlushFailuresTimerInMilliseconds);
                    _flushLock.Exit(false);
                }
            }
        }
    }

    /// <summary>
    /// <b>RegistryMonitor</b> allows you to monitor specific registry key.
    /// </summary>
    /// https://www.codeproject.com/Articles/4502/RegistryMonitor-a-NET-wrapper-class-for-RegNotifyC
    /// <remarks>
    /// If a monitored registry key changes, an event is fired. You can subscribe to these
    /// events by adding a delegate to <see cref="RegChanged"/>.
    /// <para>The Windows API provides a function
    /// <a href="http://msdn.microsoft.com/library/en-us/sysinfo/base/regnotifychangekeyvalue.asp">
    /// RegNotifyChangeKeyValue</a>, which is not covered by the
    /// <see cref="RegistryKey"/> class. <see cref="RegistryMonitor_"/> imports
    /// that function and encapsulates it in a convenient manner.
    /// </para>
    /// </remarks>
    /// <example>
    /// This sample shows how to monitor <c>HKEY_CURRENT_USER\Environment</c> for changes:
    /// <code>
    /// public class MonitorSample
    /// {
    ///     static void Main() 
    ///     {
    ///         RegistryMonitor monitor = new RegistryMonitor(RegistryHive.CurrentUser, "Environment");
    ///         monitor.RegChanged += new EventHandler(OnRegChanged);
    ///         monitor.Start();
    ///
    ///         while(true);
    /// 
    /// 			monitor.Stop();
    ///     }
    ///
    ///     private void OnRegChanged(object sender, EventArgs e)
    ///     {
    ///         Console.WriteLine("registry key has changed");
    ///     }
    /// }
    /// </code>
    /// </example>
    [SupportedOSPlatform("Windows")]
    sealed class RegistryMonitor_ : IDisposable
    {
        #region P/Invoke

        private const int KEY_QUERY_VALUE = 0x0001;
        private const int KEY_NOTIFY = 0x0010;
        private const int STANDARD_RIGHTS_READ = 0x00020000;

        #endregion

        #region Event handling

        /// <summary>
        /// Occurs when the specified registry key has changed.
        /// </summary>
        public event EventHandler? RegChanged;

        /// <summary>
        /// Raises the <see cref="RegChanged"/> event.
        /// </summary>
        /// <remarks>
        /// <p>
        /// <b>OnRegChanged</b> is called when the specified registry key has changed.
        /// </p>
        /// <note type="inheritinfo">
        /// When overriding <see cref="OnRegChanged"/> in a derived class, be sure to call
        /// the base class's <see cref="OnRegChanged"/> method.
        /// </note>
        /// </remarks>
        private void OnRegChanged()
        {
            RegChanged?.Invoke(this, EventArgs.Empty);
        }

        /// <summary>
        /// Occurs when the access to the registry fails.
        /// </summary>
        public event ErrorEventHandler? Error;

        /// <summary>
        /// Raises the <see cref="Error"/> event.
        /// </summary>
        /// <param name="e">The <see cref="Exception"/> which occured while watching the registry.</param>
        /// <remarks>
        /// <p>
        /// <b>OnError</b> is called when an exception occurs while watching the registry.
        /// </p>
        /// <note type="inheritinfo">
        /// When overriding <see cref="OnError"/> in a derived class, be sure to call
        /// the base class's <see cref="OnError"/> method.
        /// </note>
        /// </remarks>
        private void OnError(Exception e)
        {
            Error?.Invoke(this, new ErrorEventArgs(e));
        }

        #endregion

        #region Private member variables

        private SafeRegistryHandle? _registryHive;
        private string? _registrySubName;
        private readonly object _threadLock = new();
        private Thread? _thread;
        private bool _disposed = false;
        private readonly ManualResetEvent _eventTerminate = new(false);

        private PInvoke.AdvApi32.RegNotifyFilter _regFilter = PInvoke.AdvApi32.RegNotifyFilter.REG_NOTIFY_CHANGE_NAME | PInvoke.AdvApi32.RegNotifyFilter.REG_NOTIFY_CHANGE_ATTRIBUTES |
                                                   PInvoke.AdvApi32.RegNotifyFilter.REG_NOTIFY_CHANGE_LAST_SET | PInvoke.AdvApi32.RegNotifyFilter.REG_NOTIFY_CHANGE_SECURITY;

        #endregion

        /// <summary>
        /// Initializes a new instance of the <see cref="RegistryMonitor_"/> class.
        /// </summary>
        /// <param name="registryHive">The registry hive.</param>
        /// <param name="subKey">The sub key.</param>
        public RegistryMonitor_(RegistryHive registryHive, string subKey)
        {
            InitRegistryKey(registryHive, subKey);
        }

        /// <summary>
        /// Disposes this object.
        /// </summary>
        public void Dispose()
        {
            Stop();
            _disposed = true;
            GC.SuppressFinalize(this);
        }

        /// <summary>
        /// Gets or sets the <see cref="RegChangeNotifyFilter">RegChangeNotifyFilter</see>.
        /// </summary>
        public PInvoke.AdvApi32.RegNotifyFilter RegChangeNotifyFilter
        {
            get => _regFilter;
            set
            {
                lock (_threadLock)
                {
                    if (IsMonitoring)
                        throw new InvalidOperationException("Monitoring thread is already running");

                    _regFilter = value;
                }
            }
        }

        #region Initialization

        private void InitRegistryKey(RegistryHive hive, string name)
        {
            _registryHive = hive switch
            {
                RegistryHive.ClassesRoot => Registry.ClassesRoot.Handle,
                RegistryHive.CurrentConfig => Registry.CurrentConfig.Handle,
                RegistryHive.CurrentUser => Registry.CurrentUser.Handle,
                RegistryHive.LocalMachine => Registry.LocalMachine.Handle,
                RegistryHive.PerformanceData => Registry.PerformanceData.Handle,
                RegistryHive.Users => Registry.Users.Handle,
                _ => throw new InvalidEnumArgumentException("hive", (int)hive, typeof(RegistryHive)),
            };
            _registrySubName = name;
        }

        #endregion

        /// <summary>
        /// <b>true</b> if this <see cref="RegistryMonitor_"/> object is currently monitoring;
        /// otherwise, <b>false</b>.
        /// </summary>
        public bool IsMonitoring
        {
            get { return _thread != null; }
        }

        /// <summary>
        /// Start monitoring.
        /// </summary>
        public void Start()
        {
            if (_disposed)
                throw new ObjectDisposedException(null, "This instance is already disposed");

            lock (_threadLock)
            {
                if (!IsMonitoring)
                {
                    _eventTerminate.Reset();
                    _thread = new Thread(new ThreadStart(MonitorThread))
                    {
                        Name = "RegistryMonitor",
                        IsBackground = true,
                    };
                    _thread.Start();
                }
            }
        }

        /// <summary>
        /// Stops the monitoring thread.
        /// </summary>
        public void Stop()
        {
            if (_disposed)
                throw new ObjectDisposedException(null, "This instance is already disposed");

            lock (_threadLock)
            {
                Thread? thread = _thread;
                if (thread != null)
                {
                    _eventTerminate.Set();
                    thread.Join();
                }
            }
        }

        private void MonitorThread()
        {
            try
            {
                ThreadLoop();
            }
            catch (Exception e)
            {
                OnError(e);
            }
            _thread = null;
        }

        private void ThreadLoop()
        {
            SafeRegistryHandle? registryKey = null;
            AutoResetEvent? eventNotify = null;
            try
            {
                var result = PInvoke.AdvApi32.RegOpenKeyEx(_registryHive, _registrySubName, 0, STANDARD_RIGHTS_READ | KEY_QUERY_VALUE | KEY_NOTIFY, out registryKey);
                if (result != PInvoke.Win32ErrorCode.ERROR_SUCCESS)
                    throw new Win32Exception((int)result);

                eventNotify = new AutoResetEvent(false);
                WaitHandle[] waitHandles = new WaitHandle[] { eventNotify, _eventTerminate };
                while (!_eventTerminate.WaitOne(0, true))
                {
                    result = PInvoke.AdvApi32.RegNotifyChangeKeyValue(registryKey, true, _regFilter, eventNotify.SafeWaitHandle, true);
                    if (result != 0)
                        throw new Win32Exception((int)result);

                    if (WaitHandle.WaitAny(waitHandles) == 0)
                    {
                        OnRegChanged();
                    }
                }
            }
            finally
            {
                registryKey?.Dispose();
                eventNotify?.Dispose();
            }
        }
    }

    [SupportedOSPlatform("Windows")]
    static partial class Interop
    {
        internal static partial class WinHttp
        {
            public const uint ERROR_WINHTTP_LOGIN_FAILURE = 12015;
            public const uint ERROR_WINHTTP_AUTODETECTION_FAILED = 12180;

            public const string WINHTTP_NO_PROXY_NAME = null;
            public const string WINHTTP_NO_PROXY_BYPASS = null;
        }
    }

    // This class is only used on OS versions where WINHTTP_ACCESS_TYPE_AUTOMATIC_PROXY
    // is not supported (i.e. before Win8.1/Win2K12R2) in the WinHttpOpen() function.
    [SupportedOSPlatform("Windows")]
    sealed class WinInetProxyHelper
    {
        private const int RecentAutoDetectionInterval = 120_000; // 2 minutes in milliseconds.
        private readonly string? _autoConfigUrl;
        private readonly string? _proxy;
        private readonly string? _proxyBypass;
        private readonly bool _autoDetect;
        private readonly bool _useProxy;
        private bool _autoDetectionFailed;
        private int _lastTimeAutoDetectionFailed; // Environment.TickCount units (milliseconds).

        public WinInetProxyHelper()
        {
            Vanara.PInvoke.WinHTTP.WINHTTP_CURRENT_USER_IE_PROXY_CONFIG proxyConfig = default;

            try
            {
                if (Vanara.PInvoke.WinHTTP.WinHttpGetIEProxyConfigForCurrentUser(out proxyConfig))
                {
                    _autoConfigUrl = proxyConfig.lpszAutoConfigUrl;
                    _autoDetect = proxyConfig.fAutoDetect;
                    _proxy = proxyConfig.lpszProxy!;
                    _proxyBypass = proxyConfig.lpszProxyBypass;

                    //if (NetEventSource.Log.IsEnabled())
                    //{
                    //    NetEventSource.Info(this, $"AutoConfigUrl={AutoConfigUrl}, AutoDetect={AutoDetect}, Proxy={Proxy}, ProxyBypass={ProxyBypass}");
                    //}

                    _useProxy = true;
                }
                else
                {
                    // We match behavior of WINHTTP_ACCESS_TYPE_AUTOMATIC_PROXY and ignore errors.
                    //int lastError = Marshal.GetLastWin32Error();
                    //if (NetEventSource.Log.IsEnabled()) NetEventSource.Error(this, $"error={lastError}");
                }

                //if (NetEventSource.Log.IsEnabled()) NetEventSource.Info(this, $"_useProxy={_useProxy}");
            }
            finally
            {
                // FreeHGlobal already checks for null pointer before freeing the memory.
                proxyConfig.FreeMemory();
            }
        }

        public string? AutoConfigUrl => _autoConfigUrl;

        public bool AutoDetect => _autoDetect;

        public bool AutoSettingsUsed => AutoDetect || !IsNullOrEmpty(AutoConfigUrl);

        public bool ManualSettingsUsed => !IsNullOrEmpty(Proxy);

        public bool ManualSettingsOnly => !AutoSettingsUsed && ManualSettingsUsed;

        public string? Proxy => _proxy;

        public string? ProxyBypass => _proxyBypass;

        public bool RecentAutoDetectionFailure =>
            _autoDetectionFailed &&
            Environment.TickCount - _lastTimeAutoDetectionFailed <= RecentAutoDetectionInterval;

        public bool GetProxyForUrl(
            Vanara.PInvoke.WinHTTP.SafeHINTERNET? sessionHandle,
            Uri uri,
            out Vanara.PInvoke.WinHTTP.WINHTTP_PROXY_INFO proxyInfo)
        {
            proxyInfo.dwAccessType = Vanara.PInvoke.WinHTTP.WINHTTP_ACCESS_TYPE.WINHTTP_ACCESS_TYPE_NO_PROXY;
            proxyInfo.lpszProxy = IntPtr.Zero;
            proxyInfo.lpszProxyBypass = IntPtr.Zero;

            if (!_useProxy)
            {
                return false;
            }

            bool useProxy = false;

            Vanara.PInvoke.WinHTTP.WINHTTP_AUTOPROXY_OPTIONS autoProxyOptions;
            autoProxyOptions.lpszAutoConfigUrl = AutoConfigUrl;
            autoProxyOptions.dwAutoDetectFlags = AutoDetect ?
                (Vanara.PInvoke.WinHTTP.WINHTTP_AUTO_DETECT_TYPE.WINHTTP_AUTO_DETECT_TYPE_DHCP | Vanara.PInvoke.WinHTTP.WINHTTP_AUTO_DETECT_TYPE.WINHTTP_AUTO_DETECT_TYPE_DNS_A) : 0;
            autoProxyOptions.fAutoLogonIfChallenged = false;
            autoProxyOptions.dwFlags =
                (AutoDetect ? Vanara.PInvoke.WinHTTP.WINHTTP_AUTOPROXY.WINHTTP_AUTOPROXY_AUTO_DETECT : 0) |
                (!IsNullOrEmpty(AutoConfigUrl) ? Vanara.PInvoke.WinHTTP.WINHTTP_AUTOPROXY.WINHTTP_AUTOPROXY_CONFIG_URL : 0);
            autoProxyOptions.lpvReserved = IntPtr.Zero;
            autoProxyOptions.dwReserved = 0;

            // AutoProxy Cache.
            // https://docs.microsoft.com/en-us/windows/desktop/WinHttp/autoproxy-cache
            // If the out-of-process service is active when WinHttpGetProxyForUrl is called, the cached autoproxy
            // URL and script are available to the whole computer. However, if the out-of-process service is used,
            // and the fAutoLogonIfChallenged flag in the pAutoProxyOptions structure is true, then the autoproxy
            // URL and script are not cached. Therefore, calling WinHttpGetProxyForUrl with the fAutoLogonIfChallenged
            // member set to TRUE results in additional overhead operations that may affect performance.
            // The following steps can be used to improve performance:
            // 1. Call WinHttpGetProxyForUrl with the fAutoLogonIfChallenged parameter set to false. The autoproxy
            //    URL and script are cached for future calls to WinHttpGetProxyForUrl.
            // 2. If Step 1 fails, with ERROR_WINHTTP_LOGIN_FAILURE, then call WinHttpGetProxyForUrl with the
            //    fAutoLogonIfChallenged member set to TRUE.
            //
            // We match behavior of WINHTTP_ACCESS_TYPE_AUTOMATIC_PROXY and ignore errors.

#pragma warning disable CA1845 // file is shared with a build that lacks string.Concat for spans
            // Underlying code does not understand WebSockets so we need to convert it to http or https.
            string destination = uri.AbsoluteUri;
            if (uri.Scheme == UriScheme.Wss)
            {
                destination = UriScheme.Https + destination[UriScheme.Wss.Length..];
            }
            else if (uri.Scheme == UriScheme.Ws)
            {
                destination = UriScheme.Http + destination[UriScheme.Ws.Length..];
            }
#pragma warning restore CA1845

            var repeat = false;
            do
            {
                _autoDetectionFailed = false;
                if (Vanara.PInvoke.WinHTTP.WinHttpGetProxyForUrl(
                    sessionHandle!,
                    destination,
                    in autoProxyOptions,
                    out proxyInfo))
                {
                    //if (NetEventSource.Log.IsEnabled()) NetEventSource.Info(this, "Using autoconfig proxy settings");
                    useProxy = true;

                    break;
                }
                else
                {
                    var lastError = Marshal.GetLastWin32Error();
                    //if (NetEventSource.Log.IsEnabled()) NetEventSource.Error(this, $"error={lastError}");

                    if (lastError == Interop.WinHttp.ERROR_WINHTTP_LOGIN_FAILURE)
                    {
                        if (repeat)
                        {
                            // We don't retry more than once.
                            break;
                        }
                        else
                        {
                            repeat = true;
                            autoProxyOptions.fAutoLogonIfChallenged = true;
                        }
                    }
                    else
                    {
                        if (lastError == Interop.WinHttp.ERROR_WINHTTP_AUTODETECTION_FAILED)
                        {
                            _autoDetectionFailed = true;
                            _lastTimeAutoDetectionFailed = Environment.TickCount;
                        }

                        break;
                    }
                }
            } while (repeat);

            // Fall back to manual settings if available.
            if (!useProxy && !IsNullOrEmpty(Proxy))
            {
                proxyInfo.dwAccessType = Vanara.PInvoke.WinHTTP.WINHTTP_ACCESS_TYPE.WINHTTP_ACCESS_TYPE_NAMED_PROXY;
                proxyInfo.lpszProxy = Marshal.StringToHGlobalUni(Proxy);
                proxyInfo.lpszProxyBypass = IsNullOrEmpty(ProxyBypass) ?
                    IntPtr.Zero : Marshal.StringToHGlobalUni(ProxyBypass);

                //if (NetEventSource.Log.IsEnabled()) NetEventSource.Info(this, $"Fallback to Proxy={Proxy}, ProxyBypass={ProxyBypass}");
                useProxy = true;
            }

            //if (NetEventSource.Log.IsEnabled()) NetEventSource.Info(this, $"useProxy={useProxy}");

            return useProxy;
        }
    }

    static class SimpleRegex
    {
        // Based on wildcmp written by Jack Handy - <A href="mailto:jakkhandy@hotmail.com">jakkhandy@hotmail.com</A>
        // https://www.codeproject.com/Articles/1088/Wildcard-string-compare-globbing

        /// <summary>
        /// Perform a match between an input string and a pattern in which the only special character
        /// is an asterisk, which can map to zero or more of any character in the input.
        /// </summary>
        /// <param name="input">The input to match.</param>
        /// <param name="pattern">The pattern to match against.</param>
        /// <returns>true if the input matches the pattern; otherwise, false.</returns>
        public static bool IsMatchWithStarWildcard(ReadOnlySpan<char> input, ReadOnlySpan<char> pattern)
        {
            int inputPos = 0, inputPosSaved = -1;
            int patternPos = 0, patternPosSaved = -1;

            // Loop through each character in the input.
            while (inputPos < input.Length)
            {
                if (patternPos < pattern.Length && pattern[patternPos] == '*')
                {
                    // If we're currently positioned on a wildcard in the pattern,
                    // move past it and remember where we are to backtrack to.
                    inputPosSaved = inputPos;
                    patternPosSaved = ++patternPos;
                }
                else if (patternPos < pattern.Length &&
                         (pattern[patternPos] == input[inputPos] ||
                          char.ToUpperInvariant(pattern[patternPos]) == char.ToUpperInvariant(input[inputPos])))
                {
                    // If the characters in the pattern and the input match, advance both.
                    inputPos++;
                    patternPos++;
                }
                else if (patternPosSaved == -1)
                {
                    // If we're not on a wildcard and the current characters don't match and we don't have
                    // any wildcard to backtrack to, this is not a match.
                    return false;
                }
                else
                {
                    // Otherwise, this is not a wildcard, the characters don't match, but we do have a
                    // wildcard saved, so backtrack to it and use it to consume the next input character.
                    inputPos = ++inputPosSaved;
                    patternPos = patternPosSaved;
                }
            }

            // We've reached the end of the input.  Eat all wildcards immediately after where we are
            // in the pattern, as if they're at the end, they'll all just map to nothing (and if it
            // turns out there's something after them, eating them won't matter).
            while (patternPos < pattern.Length && pattern[patternPos] == '*')
            {
                patternPos++;
            }

            // If we are in fact at the end of the pattern, then we successfully matched.
            // If there's anything left, it's not a wildcard, so it doesn't match.
            Debug.Assert(patternPos <= pattern.Length);
            return patternPos == pattern.Length;
        }
    }

    static class UriScheme
    {
        public const string File = "file";
        public const string Ftp = "ftp";
        public const string Gopher = "gopher";
        public const string Http = "http";
        public const string Https = "https";
        public const string News = "news";
        public const string NetPipe = "net.pipe";
        public const string NetTcp = "net.tcp";
        public const string Nntp = "nntp";
        public const string Mailto = "mailto";
        public const string Ws = "ws";
        public const string Wss = "wss";

        public const string SchemeDelimiter = "://";
    }
}

#endif
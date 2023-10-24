#if ANDROID
using Android.Content;
using Android.Net;
using Application = Android.App.Application;
using Context = Android.Content.Context;
#elif WINDOWS
using Windows.Networking.Connectivity;
#elif IOS
using CoreTelephony;
#endif
#if IOS || MACCATALYST || MACOS
using CoreFoundation;
using SystemConfiguration;
#endif
using ConnectionProfileEnum = BD.Common8.Essentials.Enums.ConnectionProfile;

namespace BD.Common8.Essentials.Services.Implementation;

#pragma warning disable SA1600 // Elements should be documented

/// <summary>
/// https://github.com/dotnet/maui/tree/8.0.0-rc.2.9373/src/Essentials/src/Connectivity
/// </summary>
sealed class ConnectivityPlatformServiceImpl : IConnectivityPlatformService
{
    event EventHandler<ConnectivityChangedEventArgs>? ConnectivityChangedInternal;

    // a cache so that events aren't fired unnecessarily
    // this is mainly an issue on Android, but we can stiil do this everywhere
    NetworkAccess currentAccess;
    List<ConnectionProfileEnum> currentProfiles = [];

    public event EventHandler<ConnectivityChangedEventArgs> ConnectivityChanged
    {
        add
        {
            if (ConnectivityChangedInternal is null)
            {
                SetCurrent();
                StartListeners();
            }
            ConnectivityChangedInternal += value;
        }

        remove
        {
            ConnectivityChangedInternal -= value;
            if (ConnectivityChangedInternal is null)
                StopListeners();
        }
    }

    void SetCurrent()
    {
        currentAccess = NetworkAccess;
        currentProfiles = new List<ConnectionProfileEnum>(ConnectionProfiles);
    }

    void OnConnectivityChanged(NetworkAccess access, IEnumerable<ConnectionProfileEnum> profiles)
            => OnConnectivityChanged(new ConnectivityChangedEventArgs(access, profiles));

    void OnConnectivityChanged()
        => OnConnectivityChanged(NetworkAccess, ConnectionProfiles);

    void OnConnectivityChanged(ConnectivityChangedEventArgs e)
    {
        if (currentAccess != e.NetworkAccess || !currentProfiles.SequenceEqual(e.ConnectionProfiles))
        {
            SetCurrent();
            MainThread2.BeginInvokeOnMainThread(() => ConnectivityChangedInternal?.Invoke(null, e));
        }
    }

    /// <summary>
    /// The current connectivity information from the change event.
    /// </summary>
    /// <remarks>
    /// Initializes a new instance of the <see cref="ConnectivityChangedEventArgs"/> class.
    /// </remarks>
    /// <param name="access">The current access of the network.</param>
    /// <param name="connectionProfiles">The connection profiles changing correspondingto this event.</param>
    public class ConnectivityChangedEventArgs(NetworkAccess access, IEnumerable<ConnectionProfileEnum> connectionProfiles) : EventArgs
    {
        /// <summary>
        /// Gets the current state of network access.
        /// </summary>
        /// <remarks>
        /// <para>Even when <see cref="NetworkAccess.Internet"/> is returned, full internet access is not guaranteed.</para>
        /// <para>Can throw <see cref="PermissionException"/> on Android if <c>ACCESS_NETWORK_STATE</c> is not set in manifest.</para>
        /// </remarks>
        public NetworkAccess NetworkAccess { get; } = access;

        /// <summary>
        /// Gets the active connectivity profiles for the device.
        /// </summary>
        public IEnumerable<ConnectionProfileEnum> ConnectionProfiles { get; } = connectionProfiles;

        /// <summary>
        /// Returns a string representation of the current values of <see cref="ConnectivityChangedEventArgs"/>.
        /// </summary>
        /// <returns>A string representation of this instance in the format of <c>NetworkAccess: {value}, ConnectionProfiles: [{value1}, {value2}]</c>.</returns>
        public override string ToString() =>
            $"{nameof(NetworkAccess)}: {NetworkAccess}, " +
            $"{nameof(ConnectionProfiles)}: [{string.Join(", ", ConnectionProfiles)}]";
    }

#if ANDROID
    /// <summary>
    /// Unique identifier for the connectivity changed action on Android.
    /// </summary>
    public const string ConnectivityChangedAction = "com.maui.essentials.ESSENTIALS_CONNECTIVITY_CHANGED";
    static readonly Intent connectivityIntent = new(ConnectivityChangedAction);

    static ConnectivityManager? connectivityManager;

    static ConnectivityManager ConnectivityManager =>
        connectivityManager ??= (Application.Context.GetSystemService(Context.ConnectivityService) as ConnectivityManager)!;

    ConnectivityBroadcastReceiver? conectivityReceiver;
    EssentialsNetworkCallback? networkCallback;

    void StartListeners()
    {
        Permissions2.EnsureDeclared<Permissions2.NetworkState>();

        var filter = new IntentFilter();

        if (OperatingSystem.IsAndroidVersionAtLeast(24))
        {
            RegisterNetworkCallback();
            filter.AddAction(ConnectivityChangedAction);
        }
        else
        {
            filter.AddAction(ConnectivityManager.ConnectivityAction);
        }

        conectivityReceiver = new ConnectivityBroadcastReceiver(OnConnectivityChanged);

        Application.Context.RegisterReceiver(conectivityReceiver, filter);
    }

    void StopListeners()
    {
        if (conectivityReceiver == null)
            return;

        try
        {
            UnregisterNetworkCallback();
        }
        catch
        {
            Debug.WriteLine("Connectivity receiver already unregistered. Disposing of it.");
        }

        try
        {
            Application.Context.UnregisterReceiver(conectivityReceiver);
        }
        catch (Java.Lang.IllegalArgumentException)
        {
            Debug.WriteLine("Connectivity receiver already unregistered. Disposing of it.");
        }

        conectivityReceiver = null;
    }

    void RegisterNetworkCallback()
    {
        if (!OperatingSystem.IsAndroidVersionAtLeast(24))
            return;

        var manager = ConnectivityManager;
        if (manager == null)
            return;

        var request = new NetworkRequest.Builder().Build()!;
        networkCallback = new EssentialsNetworkCallback();
        manager.RegisterNetworkCallback(request, networkCallback);
    }

    void UnregisterNetworkCallback()
    {
        if (!OperatingSystem.IsAndroidVersionAtLeast(24))
            return;

        var manager = ConnectivityManager;
        if (manager == null || networkCallback == null)
            return;

        manager.UnregisterNetworkCallback(networkCallback);

        networkCallback = null;
    }

    sealed class EssentialsNetworkCallback : ConnectivityManager.NetworkCallback
    {
        public override void OnAvailable(Network network) =>
            Application.Context.SendBroadcast(connectivityIntent);

        public override void OnLost(Network network) =>
            Application.Context.SendBroadcast(connectivityIntent);

        public override void OnCapabilitiesChanged(Network network, NetworkCapabilities networkCapabilities) =>
            Application.Context.SendBroadcast(connectivityIntent);

        public override void OnUnavailable() =>
            Application.Context.SendBroadcast(connectivityIntent);

        public override void OnLinkPropertiesChanged(Network network, LinkProperties linkProperties) =>
            Application.Context.SendBroadcast(connectivityIntent);

        public override void OnLosing(Network network, int maxMsToLive) =>
            Application.Context.SendBroadcast(connectivityIntent);
    }

    static NetworkAccess IsBetterAccess(NetworkAccess currentAccess, NetworkAccess newAccess) =>
        newAccess > currentAccess ? newAccess : currentAccess;

    public NetworkAccess NetworkAccess
    {
        get
        {
            Permissions2.EnsureDeclared<Permissions2.NetworkState>();

            try
            {
                var currentAccess = NetworkAccess.None;
                var manager = ConnectivityManager;

#pragma warning disable CS0618 // Type or member is obsolete
#pragma warning disable CA1416 // Validate platform compatibility
#pragma warning disable CA1422 // Validate platform compatibility
                var networks = manager.GetAllNetworks();
#pragma warning restore CA1422 // Validate platform compatibility
#pragma warning restore CA1416 // Validate platform compatibility
#pragma warning restore CS0618 // Type or member is obsolete

                // some devices running 21 and 22 only use the older api.
                if (networks.Length == 0 && !OperatingSystem.IsAndroidVersionAtLeast(23))
                {
                    ProcessAllNetworkInfo();
                    return currentAccess;
                }

                foreach (var network in networks)
                {
                    try
                    {
                        var capabilities = manager.GetNetworkCapabilities(network);

                        if (capabilities == null)
                            continue;

#pragma warning disable CS0618 // Type or member is obsolete
#pragma warning disable CA1416 // Validate platform compatibility
#pragma warning disable CA1422 // Validate platform compatibility
                        var info = manager.GetNetworkInfo(network);

                        if (info == null || !info.IsAvailable)
                            continue;
#pragma warning restore CS0618 // Type or member is obsolete

                        // Check to see if it has the internet capability
                        if (!capabilities.HasCapability(NetCapability.Internet))
                        {
                            // Doesn't have internet, but local is possible
                            currentAccess = IsBetterAccess(currentAccess, NetworkAccess.Local);
                            continue;
                        }

                        ProcessNetworkInfo(info);
                    }
                    catch
                    {
                        // there is a possibility, but don't worry
                    }
                }

                void ProcessAllNetworkInfo()
                {
#pragma warning disable CS0618 // Type or member is obsolete
                    foreach (var info in manager.GetAllNetworkInfo())
#pragma warning restore CS0618 // Type or member is obsolete
                    {
                        ProcessNetworkInfo(info);
                    }
                }

#pragma warning disable CS0618 // Type or member is obsolete
                void ProcessNetworkInfo(NetworkInfo info)
                {
                    if (info == null || !info.IsAvailable)
                        return;

                    if (info.IsConnected)
                        currentAccess = IsBetterAccess(currentAccess, NetworkAccess.Internet);
                    else if (info.IsConnectedOrConnecting)
                        currentAccess = IsBetterAccess(currentAccess, NetworkAccess.ConstrainedInternet);
#pragma warning restore CA1422 // Validate platform compatibility
#pragma warning restore CA1416 // Validate platform compatibility
#pragma warning restore CS0618 // Type or member is obsolete
                }

                return currentAccess;
            }
            catch (Exception e)
            {
                // TODO add Logging here
                Debug.WriteLine("Unable to get connected state - do you have ACCESS_NETWORK_STATE permission? - error: {0}", e);
                return NetworkAccess.Unknown;
            }
        }
    }

    public IEnumerable<ConnectionProfileEnum> ConnectionProfiles
    {
        get
        {
            Permissions2.EnsureDeclared<Permissions2.NetworkState>();

            var manager = ConnectivityManager;
#pragma warning disable CS0618 // Type or member is obsolete
#pragma warning disable CA1416 // Validate platform compatibility
#pragma warning disable CA1422 // Validate platform compatibility
            var networks = manager.GetAllNetworks();
#pragma warning restore CS0618 // Type or member is obsolete
            foreach (var network in networks)
            {
#pragma warning disable CS0618 // Type or member is obsolete
                NetworkInfo? info = null;
                try
                {
                    info = manager.GetNetworkInfo(network);
                }
                catch
                {
                    // there is a possibility, but don't worry about it
                }
#pragma warning restore CS0618 // Type or member is obsolete

                var p = ProcessNetworkInfo(info);
                if (p.HasValue)
                    yield return p.Value;
            }

#pragma warning disable CS0618 // Type or member is obsolete
            static ConnectionProfileEnum? ProcessNetworkInfo(NetworkInfo? info)
            {
                if (info == null || !info.IsAvailable || !info.IsConnectedOrConnecting)
                    return null;

                return GetConnectionType(info.Type, info.TypeName);
            }
#pragma warning restore CA1422 // Validate platform compatibility
#pragma warning restore CA1416 // Validate platform compatibility
#pragma warning restore CS0618 // Type or member is obsolete
        }
    }

    internal static ConnectionProfileEnum GetConnectionType(ConnectivityType connectivityType, string? typeName)
    {
        switch (connectivityType)
        {
            case ConnectivityType.Ethernet:
                return ConnectionProfileEnum.Ethernet;
            case ConnectivityType.Wifi:
                return ConnectionProfileEnum.WiFi;
            case ConnectivityType.Bluetooth:
                return ConnectionProfileEnum.Bluetooth;
            case ConnectivityType.Wimax:
            case ConnectivityType.Mobile:
            case ConnectivityType.MobileDun:
            case ConnectivityType.MobileHipri:
            case ConnectivityType.MobileMms:
                return ConnectionProfileEnum.Cellular;
            case ConnectivityType.Dummy:
                return ConnectionProfileEnum.Unknown;
            default:
                if (string.IsNullOrWhiteSpace(typeName))
                    return ConnectionProfileEnum.Unknown;

                if (typeName.Contains("mobile", StringComparison.OrdinalIgnoreCase))
                    return ConnectionProfileEnum.Cellular;

                if (typeName.Contains("wimax", StringComparison.OrdinalIgnoreCase))
                    return ConnectionProfileEnum.Cellular;

                if (typeName.Contains("wifi", StringComparison.OrdinalIgnoreCase))
                    return ConnectionProfileEnum.WiFi;

                if (typeName.Contains("ethernet", StringComparison.OrdinalIgnoreCase))
                    return ConnectionProfileEnum.Ethernet;

                if (typeName.Contains("bluetooth", StringComparison.OrdinalIgnoreCase))
                    return ConnectionProfileEnum.Bluetooth;

                return ConnectionProfileEnum.Unknown;
        }
    }

    [BroadcastReceiver(Enabled = true, Exported = false, Label = "Essentials Connectivity Broadcast Receiver")]
    sealed class ConnectivityBroadcastReceiver : BroadcastReceiver
    {
        readonly Action? onChanged;

        /// <summary>
        /// Initializes a new instance of the <see cref="ConnectivityBroadcastReceiver"/> class.
        /// </summary>
        public ConnectivityBroadcastReceiver()
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="ConnectivityBroadcastReceiver"/> class.
        /// </summary>
        /// <param name="onChanged">The action that is triggered whenever the connectivity changes.</param>
        public ConnectivityBroadcastReceiver(Action onChanged) =>
            this.onChanged = onChanged;

        public override async void OnReceive(Context? context, Intent? intent)
        {
            if (intent == null || (intent.Action != ConnectivityManager.ConnectivityAction && intent.Action != ConnectivityChangedAction))
                return;

            // await 1500ms to ensure that the the connection manager updates
            await Task.Delay(1500);
            onChanged?.Invoke();
        }
    }
#elif WINDOWS
    void StartListeners() =>
        NetworkInformation.NetworkStatusChanged += NetworkStatusChanged;

    void StopListeners() =>
        NetworkInformation.NetworkStatusChanged -= NetworkStatusChanged;

    void NetworkStatusChanged(object sender) =>
        OnConnectivityChanged();

    public NetworkAccess NetworkAccess
    {
        get
        {
            var profile = NetworkInformation.GetInternetConnectionProfile();
            if (profile == null)
                return NetworkAccess.Unknown;

            var level = profile.GetNetworkConnectivityLevel();
            return level switch
            {
                NetworkConnectivityLevel.LocalAccess => NetworkAccess.Local,
                NetworkConnectivityLevel.InternetAccess => NetworkAccess.Internet,
                NetworkConnectivityLevel.ConstrainedInternetAccess => NetworkAccess.ConstrainedInternet,
                _ => NetworkAccess.None,
            };
        }
    }

    public IEnumerable<ConnectionProfileEnum> ConnectionProfiles
    {
        get
        {
            var networkInterfaceList = NetworkInformation.GetConnectionProfiles();
            foreach (var interfaceInfo in networkInterfaceList.Where(nii => nii.GetNetworkConnectivityLevel() != NetworkConnectivityLevel.None))
            {
                var type = ConnectionProfileEnum.Unknown;

                try
                {
                    var adapter = interfaceInfo.NetworkAdapter;
                    if (adapter == null)
                        continue;

                    // http://www.iana.org/assignments/ianaiftype-mib/ianaiftype-mib
                    switch (adapter.IanaInterfaceType)
                    {
                        case 6:
                            type = ConnectionProfileEnum.Ethernet;
                            break;
                        case 71:
                            type = ConnectionProfileEnum.WiFi;
                            break;
                        case 243:
                        case 244:
                            type = ConnectionProfileEnum.Cellular;
                            break;

                        // xbox wireless, can skip
                        case 281:
                            continue;
                    }
                }
                catch (Exception ex)
                {
                    // TODO Add Logging?
                    Debug.WriteLine($"Unable to get Network Adapter, returning Unknown: {ex.Message}");
                }

                yield return type;
            }
        }
    }
#elif IOS || MACCATALYST || MACOS
#if !(MACCATALYST || MACOS)
    // TODO: Use NWPathMonitor on > iOS 12
#pragma warning disable BI1234, CA1416 // Analyzer bug https://github.com/dotnet/roslyn-analyzers/issues/5938
    static readonly Lazy<CTCellularData> cellularData = new(() => new CTCellularData());

    internal static CTCellularData CellularData => cellularData.Value;
#pragma warning restore BI1234, CA1416
#endif

    static ReachabilityListener? listener;

    void StartListeners()
    {
        listener = new ReachabilityListener();
        listener.ReachabilityChanged += OnConnectivityChanged;
    }

    void StopListeners()
    {
        if (listener == null)
            return;

        listener.ReachabilityChanged -= OnConnectivityChanged;
        listener.Dispose();
        listener = null;
    }

    public NetworkAccess NetworkAccess
    {
        get
        {
            var restricted = false;
#if !(MACCATALYST || MACOS)
            // TODO: Use NWPathMonitor on > iOS 12
#pragma warning disable CA1416 // 验证平台兼容性
            restricted = CellularData.RestrictedState == CTCellularDataRestrictedState.Restricted;
#pragma warning restore CA1416 // 验证平台兼容性
#endif
            var internetStatus = Reachability.InternetConnectionStatus();
            if ((internetStatus == NetworkStatus.ReachableViaCarrierDataNetwork && !restricted) || internetStatus == NetworkStatus.ReachableViaWiFiNetwork)
                return NetworkAccess.Internet;

            var remoteHostStatus = Reachability.RemoteHostStatus();
            if ((remoteHostStatus == NetworkStatus.ReachableViaCarrierDataNetwork && !restricted) || remoteHostStatus == NetworkStatus.ReachableViaWiFiNetwork)
                return NetworkAccess.Internet;

            return NetworkAccess.None;
        }
    }

    public IEnumerable<ConnectionProfileEnum> ConnectionProfiles
    {
        get
        {
            var statuses = Reachability.GetActiveConnectionType();
            foreach (var status in statuses)
            {
                switch (status)
                {
                    case NetworkStatus.ReachableViaCarrierDataNetwork:
                        yield return ConnectionProfileEnum.Cellular;
                        break;
                    case NetworkStatus.ReachableViaWiFiNetwork:
                        yield return ConnectionProfileEnum.WiFi;
                        break;
                    default:
                        yield return ConnectionProfileEnum.Unknown;
                        break;
                }
            }
        }
    }

    enum NetworkStatus : byte
    {
        NotReachable,
        ReachableViaCarrierDataNetwork,
        ReachableViaWiFiNetwork,
    }

    static class Reachability
    {
        internal const string HostName = "www.microsoft.com";

        internal static NetworkStatus RemoteHostStatus()
        {
            using (var remoteHostReachability = new NetworkReachability(HostName))
            {
                var reachable = remoteHostReachability.TryGetFlags(out var flags);

                if (!reachable)
                    return NetworkStatus.NotReachable;

                if (!IsReachableWithoutRequiringConnection(flags))
                    return NetworkStatus.NotReachable;

#if IOS
                if ((flags & NetworkReachabilityFlags.IsWWAN) != 0)
                    return NetworkStatus.ReachableViaCarrierDataNetwork;
#endif

                return NetworkStatus.ReachableViaWiFiNetwork;
            }
        }

        internal static NetworkStatus InternetConnectionStatus()
        {
            var status = NetworkStatus.NotReachable;

            var defaultNetworkAvailable = IsNetworkAvailable(out var flags);

#if IOS
            // If it's a WWAN connection..
            if ((flags & NetworkReachabilityFlags.IsWWAN) != 0)
                status = NetworkStatus.ReachableViaCarrierDataNetwork;
#endif

            // If the connection is reachable and no connection is required, then assume it's WiFi
            if (defaultNetworkAvailable)
            {
                status = NetworkStatus.ReachableViaWiFiNetwork;
            }

            // If the connection is on-demand or on-traffic and no user intervention
            // is required, then assume WiFi.
            if (((flags & NetworkReachabilityFlags.ConnectionOnDemand) != 0 || (flags & NetworkReachabilityFlags.ConnectionOnTraffic) != 0) &&
                 (flags & NetworkReachabilityFlags.InterventionRequired) == 0)
            {
                status = NetworkStatus.ReachableViaWiFiNetwork;
            }

            return status;
        }

        internal static IEnumerable<NetworkStatus> GetActiveConnectionType()
        {
            var status = new List<NetworkStatus>();

            var defaultNetworkAvailable = IsNetworkAvailable(out var flags);

#if IOS
            // If it's a WWAN connection.
            if ((flags & NetworkReachabilityFlags.IsWWAN) != 0)
            {
                status.Add(NetworkStatus.ReachableViaCarrierDataNetwork);
            }
            else if (defaultNetworkAvailable)
#else
            // If the connection is reachable and no connection is required, then assume it's WiFi
            if (defaultNetworkAvailable)
#endif
            {
                status.Add(NetworkStatus.ReachableViaWiFiNetwork);
            }
            else if (((flags & NetworkReachabilityFlags.ConnectionOnDemand) != 0 || (flags & NetworkReachabilityFlags.ConnectionOnTraffic) != 0) &&
                     (flags & NetworkReachabilityFlags.InterventionRequired) == 0)
            {
                // If the connection is on-demand or on-traffic and no user intervention
                // is required, then assume WiFi.
                status.Add(NetworkStatus.ReachableViaWiFiNetwork);
            }

            return status;
        }

        internal static bool IsNetworkAvailable(out NetworkReachabilityFlags flags)
        {
            var ip = new IPAddress(0);
            using var defaultRouteReachability = new NetworkReachability(ip);
            if (!defaultRouteReachability.TryGetFlags(out flags))
                return false;

            return IsReachableWithoutRequiringConnection(flags);
        }

        internal static bool IsReachableWithoutRequiringConnection(NetworkReachabilityFlags flags)
        {
            // Is it reachable with the current network configuration?
            var isReachable = (flags & NetworkReachabilityFlags.Reachable) != 0;

            // Do we need a connection to reach it?
            var noConnectionRequired = (flags & NetworkReachabilityFlags.ConnectionRequired) == 0;

#if IOS
            // Since the network stack will automatically try to get the WAN up,
            // probe that
            if ((flags & NetworkReachabilityFlags.IsWWAN) != 0)
                noConnectionRequired = true;
#endif

            return isReachable && noConnectionRequired;
        }
    }

    sealed class ReachabilityListener : IDisposable
    {
        NetworkReachability? defaultRouteReachability;
        NetworkReachability? remoteHostReachability;

        internal ReachabilityListener()
        {
            var ip = new IPAddress(0);
            defaultRouteReachability = new NetworkReachability(ip);
            defaultRouteReachability.SetNotification(OnChange);
            defaultRouteReachability.Schedule(CFRunLoop.Main, CFRunLoop.ModeDefault);

            remoteHostReachability = new NetworkReachability(Reachability.HostName);

            // Need to probe before we queue, or we wont get any meaningful values
            // this only happens when you create NetworkReachability from a hostname
            remoteHostReachability.TryGetFlags(out var flags);

            remoteHostReachability.SetNotification(OnChange);
            remoteHostReachability.Schedule(CFRunLoop.Main, CFRunLoop.ModeDefault);

#if !(MACCATALYST || MACOS)
#pragma warning disable BI1234, CA1416 // Analyzer bug https://github.com/dotnet/roslyn-analyzers/issues/5938
            CellularData.RestrictionDidUpdateNotifier = new Action<CTCellularDataRestrictedState>(OnRestrictedStateChanged);
#pragma warning restore BI1234, CA1416
#endif
        }

        internal event Action? ReachabilityChanged;

        void IDisposable.Dispose() => Dispose();

        internal void Dispose()
        {
            defaultRouteReachability?.Dispose();
            defaultRouteReachability = null;
            remoteHostReachability?.Dispose();
            remoteHostReachability = null;

#if !(MACCATALYST || MACOS)
#pragma warning disable CA1416 // Analyzer bug https://github.com/dotnet/roslyn-analyzers/issues/5938
            CellularData.RestrictionDidUpdateNotifier = null;
#pragma warning restore CA1416
#endif
        }

#if !(MACCATALYST || MACOS)
#pragma warning disable BI1234
        void OnRestrictedStateChanged(CTCellularDataRestrictedState state)
        {
            ReachabilityChanged?.Invoke();
        }
#pragma warning restore BI1234
#endif

        async void OnChange(NetworkReachabilityFlags flags)
        {
            // Add in artifical delay so the connection status has time to change
            // else it will return true no matter what.
            await Task.Delay(100);

            ReachabilityChanged?.Invoke();
        }
    }
#else
    public NetworkAccess NetworkAccess => NetworkAccess.Unknown;

    public IEnumerable<ConnectionProfileEnum> ConnectionProfiles => [];

    static void StartListeners() { }

    static void StopListeners() { }
#endif
}

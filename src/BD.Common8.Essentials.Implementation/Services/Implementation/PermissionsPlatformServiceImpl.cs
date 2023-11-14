#if ANDROID
using Android;
using Android.Content.PM;
using Android.OS;
using AndroidX.Core.App;
using AndroidX.Core.Content;
#elif WINDOWS
using Windows.ApplicationModel.Contacts;
using Windows.Devices.Enumeration;
using Windows.Devices.Geolocation;
#elif MACOS
using CoreLocation;
#elif IOS || MACCATALYST
using AVFoundation;
using CoreLocation;
using CoreMotion;
using EventKit;
using MediaPlayer;
using Speech;
#endif
#if IOS || MACCATALYST || MACOS
using Photos;
#endif

namespace BD.Common8.Essentials.Services.Implementation;

#pragma warning disable SA1600 // Elements should be documented

/// <summary>
/// https://github.com/dotnet/maui/tree/8.0.0-rc.2.9373/src/Essentials/src/Permissions
/// </summary>
[SupportedOSPlatform("android")]
[SupportedOSPlatform("macos")]
[SupportedOSPlatform("maccatalyst")]
[SupportedOSPlatform("ios")]
[SupportedOSPlatform("windows")]
public class PermissionsPlatformServiceImpl : IPermissionsPlatformService
{
    Task<PermissionStatus> IPermissionsPlatformService.CheckStatusAsync<TPermission>()
        => GetRequiredService(typeof(TPermission).Name).CheckStatusAsync();

    void IPermissionsPlatformService.EnsureDeclared<TPermission>()
        => GetRequiredService(typeof(TPermission).Name).EnsureDeclared();

    Task<PermissionStatus> IPermissionsPlatformService.RequestAsync<TPermission>()
        => GetRequiredService(typeof(TPermission).Name).RequestAsync();

    bool IPermissionsPlatformService.ShouldShowRationale<TPermission>()
        => GetRequiredService(typeof(TPermission).Name).ShouldShowRationale();

    protected virtual IBasePermission GetRequiredService(string serviceName) => serviceName switch
    {
        nameof(Permissions2.PhoneNumber) => new PhoneNumber(),
        nameof(Permissions2.Battery) => new Battery(),
        nameof(Permissions2.Bluetooth) => new Bluetooth(),
        nameof(Permissions2.CalendarRead) => new CalendarRead(),
        nameof(Permissions2.CalendarWrite) => new CalendarWrite(),
        nameof(Permissions2.Camera) => new Camera(),
        nameof(Permissions2.ContactsRead) => new ContactsRead(),
        nameof(Permissions2.ContactsWrite) => new ContactsWrite(),
        nameof(Permissions2.Flashlight) => new Flashlight(),
        nameof(Permissions2.LaunchApp) => new LaunchApp(),
        nameof(Permissions2.LocationWhenInUse) => new LocationWhenInUse(),
        nameof(Permissions2.LocationAlways) => new LocationAlways(),
        nameof(Permissions2.Maps) => new Maps(),
        nameof(Permissions2.Media) => new Media(),
        nameof(Permissions2.Microphone) => new Microphone(),
        nameof(Permissions2.NearbyWifiDevices) => new NearbyWifiDevices(),
        nameof(Permissions2.NetworkState) => new NetworkState(),
        nameof(Permissions2.Phone) => new Phone(),
        nameof(Permissions2.Photos) => new Photos(),
        nameof(Permissions2.Reminders) => new Reminders(),
        nameof(Permissions2.Sensors) => new Sensors(),
        nameof(Permissions2.Sms) => new Sms(),
        nameof(Permissions2.Speech) => new Speech(),
        nameof(Permissions2.StorageRead) => new StorageRead(),
        nameof(Permissions2.StorageWrite) => new StorageWrite(),
        nameof(Permissions2.Vibrate) => new Vibrate(),
        _ => throw ThrowHelper.GetArgumentOutOfRangeException(serviceName),
    };

#if ANDROID
    public static bool IsDeclaredInManifest(string permission)
    {
        var context = Application.Context;
        var packageInfo = context.PackageManager!.GetPackageInfo(context.PackageName!, PackageInfoFlags.Permissions);
        var requestedPermissions = packageInfo?.RequestedPermissions;

        return requestedPermissions?.Any(r => r.Equals(permission, StringComparison.OrdinalIgnoreCase)) ?? false;
    }

    internal static void OnRequestPermissionsResult(int requestCode, string[] permissions, Permission[] grantResults)
        => BasePlatformPermission.OnRequestPermissionsResult(requestCode, permissions, grantResults);

    public partial class PermissionResult(string[] permissions, Permission[] grantResults)
    {
        public string[] Permissions { get; } = permissions;

        public Permission[] GrantResults { get; } = grantResults;
    }

    public abstract partial class BasePlatformPermission : IBasePermission
    {
        static readonly Dictionary<int, TaskCompletionSource<PermissionResult>> requests = [];

        static readonly object locker = new();
        static int requestCode;

        public virtual (string androidPermission, bool isRuntime)[]? RequiredPermissions { get; }

        public virtual Task<PermissionStatus> CheckStatusAsync() => CheckStatusAsync(RequiredPermissions);

        public virtual Task<PermissionStatus> CheckStatusAsync((string androidPermission, bool isRuntime)[]? requiredPermissions)
        {
            if (requiredPermissions == null || requiredPermissions.Length <= 0)
                return Task.FromResult(PermissionStatus.Granted);

            foreach (var (androidPermission, isRuntime) in requiredPermissions)
            {
                var ap = androidPermission;
                if (!IsDeclaredInManifest(ap))
                    throw new PermissionException($"You need to declare using the permission: `{androidPermission}` in your AndroidManifest.xml");

                var status = DoCheck(ap);
                if (status != PermissionStatus.Granted)
                    return Task.FromResult(PermissionStatus.Denied);
            }

            return Task.FromResult(PermissionStatus.Granted);
        }

        public virtual async Task<PermissionStatus> RequestAsync()
        {
            var requiredPermissions = RequiredPermissions;
            // Check status before requesting first
            if (await CheckStatusAsync(requiredPermissions) == PermissionStatus.Granted)
                return PermissionStatus.Granted;

            var runtimePermissions = requiredPermissions!.Where(p => p.isRuntime)
                ?.Select(p => p.androidPermission)?.ToArray();

            // We may have no runtime permissions required, in this case
            // knowing they all exist in the manifest from the Check call above is sufficient
            if (runtimePermissions == null || runtimePermissions.Length == 0)
                return PermissionStatus.Granted;

            var permissionResult = await DoRequest(runtimePermissions);
            if (permissionResult.GrantResults.Any(g => g == Permission.Denied))
                return PermissionStatus.Denied;

            return PermissionStatus.Granted;
        }

        protected virtual async Task<PermissionResult> DoRequest(string[] permissions)
        {
            TaskCompletionSource<PermissionResult> tcs;

            lock (locker)
            {
                tcs = new TaskCompletionSource<PermissionResult>();

#pragma warning disable CA1416 // 验证平台兼容性
                requestCode = PlatformUtils.NextRequestCode();
#pragma warning restore CA1416 // 验证平台兼容性

                requests.Add(requestCode, tcs);
            }

            if (!MainThread2.IsMainThread())
                throw new PermissionException("Permission request must be invoked on main thread.");

#pragma warning disable CA1416 // 验证平台兼容性
            ActivityCompat.RequestPermissions(ActivityStateManager.GetCurrentActivity(true), [.. permissions], requestCode);
#pragma warning restore CA1416 // 验证平台兼容性

            var result = await tcs.Task;
            return result;
        }

        public virtual void EnsureDeclared()
        {
            if (RequiredPermissions == null || RequiredPermissions.Length <= 0)
                return;

            foreach (var (androidPermission, isRuntime) in RequiredPermissions)
            {
                var ap = androidPermission;
                if (!IsDeclaredInManifest(ap))
                    throw new PermissionException($"You need to declare using the permission: `{androidPermission}` in your AndroidManifest.xml");
            }
        }

        public virtual bool ShouldShowRationale()
        {
            if (RequiredPermissions == null || RequiredPermissions.Length <= 0)
                return false;

#pragma warning disable CA1416 // 验证平台兼容性
            var activity = ActivityStateManager.GetCurrentActivity(true);
#pragma warning restore CA1416 // 验证平台兼容性
            foreach (var (androidPermission, isRuntime) in RequiredPermissions)
            {
                if (isRuntime && ActivityCompat.ShouldShowRequestPermissionRationale(activity, androidPermission))
                    return true;
            }

            return false;
        }

        protected virtual PermissionStatus DoCheck(string androidPermission)
        {
            var context = Application.Context;
            var targetsMOrHigher = context.ApplicationInfo!.TargetSdkVersion >= BuildVersionCodes.M;

            if (!IsDeclaredInManifest(androidPermission))
                throw new PermissionException($"You need to declare using the permission: `{androidPermission}` in your AndroidManifest.xml");

            PermissionStatus status;
            if (targetsMOrHigher)
            {
                status = ContextCompat.CheckSelfPermission(context, androidPermission) switch
                {
                    Permission.Granted => PermissionStatus.Granted,
                    Permission.Denied => PermissionStatus.Denied,
                    _ => PermissionStatus.Unknown,
                };
            }
            else
            {
                status = PermissionChecker.CheckSelfPermission(context, androidPermission) switch
                {
                    PermissionChecker.PermissionGranted => PermissionStatus.Granted,
                    PermissionChecker.PermissionDenied => PermissionStatus.Denied,
                    PermissionChecker.PermissionDeniedAppOp => PermissionStatus.Denied,
                    _ => PermissionStatus.Unknown,
                };
            }
            return status;
        }

        internal static void OnRequestPermissionsResult(int requestCode, string[] permissions, Permission[] grantResults)
        {
            lock (locker)
            {
                if (requests.TryGetValue(requestCode, out var value))
                {
                    var result = new PermissionResult(permissions, grantResults);
                    value.TrySetResult(result);
                    requests.Remove(requestCode);
                }
            }
        }
    }
#elif WINDOWS
    /// <summary>
    /// Checks if the capability specified in <paramref name="capabilityName"/> is declared in the application's <c>AppxManifest.xml</c> file.
    /// </summary>
    /// <param name="capabilityName">The capability to check for specification in the <c>AppxManifest.xml</c> file.</param>
    /// <returns><see langword="true"/> when the capability is specified, otherwise <see langword="false"/>.</returns>
    [SupportedOSPlatform("windows10.0.10240.0")]
    public static bool IsCapabilityDeclared(string capabilityName)
    {
        var docPath = FileSystemUtils.PlatformGetFullAppPackageFilePath(PlatformUtils.AppManifestFilename);
        var doc = XDocument.Load(docPath, LoadOptions.None);
        var reader = doc.CreateReader();
        var namespaceManager = new XmlNamespaceManager(reader.NameTable);
        namespaceManager.AddNamespace("x", PlatformUtils.AppManifestXmlns);
        namespaceManager.AddNamespace("uap", PlatformUtils.AppManifestUapXmlns);

        // If the manifest doesn't contain a capability we need, throw
        var docRoot = doc.Root;
        docRoot.ThrowIsNull();
        return (docRoot.XPathSelectElements($"//x:DeviceCapability[@Name='{capabilityName}']", namespaceManager)?.Any() ?? false) ||
            (docRoot.XPathSelectElements($"//x:Capability[@Name='{capabilityName}']", namespaceManager)?.Any() ?? false) ||
            (docRoot.XPathSelectElements($"//uap:Capability[@Name='{capabilityName}']", namespaceManager)?.Any() ?? false);
    }

    /// <summary>
    /// Represents the platform-specific abstract base class for all permissions on this platform.
    /// </summary>
    public abstract partial class BasePlatformPermission : IBasePermission
    {
        /// <summary>
        /// Gets the required entries that need to be present in the application's <c>AppxManifest.xml</c> file for this permission.
        /// </summary>
        protected virtual Func<IEnumerable<string>> RequiredDeclarations { get; } = Array.Empty<string>;

        /// <inheritdoc/>
        public virtual Task<PermissionStatus> CheckStatusAsync()
        {
            EnsureDeclared();
            return Task.FromResult(PermissionStatus.Granted);
        }

        /// <inheritdoc/>
        public virtual Task<PermissionStatus> RequestAsync()
            => CheckStatusAsync();

        /// <inheritdoc/>
        public virtual void EnsureDeclared()
        {
            foreach (var d in RequiredDeclarations())
            {
#pragma warning disable CA1416 // 验证平台兼容性
                if (!IsCapabilityDeclared(d))
                    throw new PermissionException($"You need to declare the capability `{d}` in your AppxManifest.xml file");
#pragma warning restore CA1416 // 验证平台兼容性
            }
        }

        /// <inheritdoc/>
        public virtual bool ShouldShowRationale() => false;
    }
#elif MACOS
    public static bool IsKeyDeclaredInInfoPlist(string usageKey) =>
        NSBundle.MainBundle.InfoDictionary.ContainsKey(new NSString(usageKey));

    public static TimeSpan LocationTimeout { get; set; } = TimeSpan.FromSeconds(10);

    public abstract class BasePlatformPermission : IBasePermission
    {
        protected virtual Func<IEnumerable<string>>? RecommendedInfoPlistKeys { get; }

        protected virtual Func<IEnumerable<string>>? RequiredInfoPlistKeys { get; }

        public virtual Task<PermissionStatus> CheckStatusAsync() =>
            Task.FromResult(PermissionStatus.Granted);

        public virtual Task<PermissionStatus> RequestAsync() =>
            Task.FromResult(PermissionStatus.Granted);

        public virtual bool ShouldShowRationale() => false;

        public virtual void EnsureDeclared()
        {
            var plistKeys = RequiredInfoPlistKeys?.Invoke();
            if (plistKeys != null)
            {
                foreach (var requiredKey in plistKeys)
                {
                    if (!IsKeyDeclaredInInfoPlist(requiredKey))
                        throw new PermissionException($"You must set `{requiredKey}` in your Info.plist file to use the Permission: {GetType().Name}.");
                }
            }

            plistKeys = RecommendedInfoPlistKeys?.Invoke();
            if (plistKeys != null)
            {
                foreach (var recommendedKey in plistKeys)
                {
                    // NOTE: This is not a problem as macOS has a "default" message. But, this is still something
                    //       the developer must do. We use a Console instead of a Debug because we always want to
                    //       print this message.
                    if (!IsKeyDeclaredInInfoPlist(recommendedKey))
                        Console.WriteLine($"You must set `{recommendedKey}` in your Info.plist file to enable a user-friendly message for the Permission: {GetType().Name}.");
                }
            }
        }

#pragma warning disable CA1822 // 将成员标记为 static
        internal void EnsureMainThread()
#pragma warning restore CA1822 // 将成员标记为 static
        {
            if (!MainThread2.IsMainThread())
                throw new PermissionException("Permission request must be invoked on main thread.");
        }
    }
#elif IOS || MACCATALYST
    internal static class EventPermission
    {
        internal static PermissionStatus CheckPermissionStatus(EKEntityType entityType)
        {
#pragma warning disable CA1416 // 验证平台兼容性
            var status = EKEventStore.GetAuthorizationStatus(entityType);
#pragma warning restore CA1416 // 验证平台兼容性
            return status switch
            {
                EKAuthorizationStatus.Authorized => PermissionStatus.Granted,
                EKAuthorizationStatus.Denied => PermissionStatus.Denied,
                EKAuthorizationStatus.Restricted => PermissionStatus.Restricted,
                _ => PermissionStatus.Unknown,
            };
        }

        internal static async Task<PermissionStatus> RequestPermissionAsync(EKEntityType entityType)
        {
            var eventStore = new EKEventStore();

#pragma warning disable CA1416 // 验证平台兼容性
#pragma warning disable CA1422 // 验证平台兼容性
            var results = await eventStore.RequestAccessAsync(entityType);
#pragma warning restore CA1422 // 验证平台兼容性
#pragma warning restore CA1416 // 验证平台兼容性

            return results.Item1 ? PermissionStatus.Granted : PermissionStatus.Denied;
        }
    }

    /// <summary>
    /// Checks if the key specified in <paramref name="usageKey"/> is declared in the application's <c>Info.plist</c> file.
    /// </summary>
    /// <param name="usageKey">The key to check for declaration in the <c>Info.plist</c> file.</param>
    /// <returns><see langword="true"/> when the key is declared, otherwise <see langword="false"/>.</returns>
    public static bool IsKeyDeclaredInInfoPlist(string usageKey) =>
        NSBundle.MainBundle.InfoDictionary.ContainsKey(new NSString(usageKey));

    /// <summary>
    /// Gets or sets the timeout that is used when the location permission is requested.
    /// </summary>
    public static TimeSpan LocationTimeout { get; set; } = TimeSpan.FromSeconds(10);

    /// <summary>
    /// Represents the platform-specific abstract base class for all permissions on this platform.
    /// </summary>
    public abstract class BasePlatformPermission : IBasePermission
    {
        /// <summary>
        /// Gets the required entries that need to be present in the application's <c>Info.plist</c> file for this permission.
        /// </summary>
        protected virtual Func<IEnumerable<string>>? RequiredInfoPlistKeys { get; }

        /// <inheritdoc/>
        public virtual Task<PermissionStatus> CheckStatusAsync() =>
            Task.FromResult(PermissionStatus.Granted);

        /// <inheritdoc/>
        public virtual Task<PermissionStatus> RequestAsync() =>
            Task.FromResult(PermissionStatus.Granted);

        /// <inheritdoc/>
        public virtual void EnsureDeclared()
        {
            if (RequiredInfoPlistKeys == null)
                return;

            var plistKeys = RequiredInfoPlistKeys?.Invoke();

            if (plistKeys == null)
                return;

            foreach (var requiredInfoPlistKey in plistKeys)
            {
                if (!IsKeyDeclaredInInfoPlist(requiredInfoPlistKey))
                    throw new PermissionException($"You must set `{requiredInfoPlistKey}` in your Info.plist file to use the Permission: {GetType().Name}.");
            }
        }

        /// <inheritdoc/>
        public virtual bool ShouldShowRationale() => false;

#pragma warning disable CA1822 // 将成员标记为 static
        internal void EnsureMainThread()
#pragma warning restore CA1822 // 将成员标记为 static
        {
            if (!MainThread2.IsMainThread())
                throw new PermissionException("Permission request must be invoked on main thread.");
        }
    }
#else
    public abstract class BasePlatformPermission : IBasePermission
    {
        public virtual Task<PermissionStatus> CheckStatusAsync()
        {
            return Task.FromResult(PermissionStatus.Granted);
        }

        public virtual void EnsureDeclared()
        {
        }

        public virtual Task<PermissionStatus> RequestAsync()
        {
            return Task.FromResult(PermissionStatus.Granted);
        }

        public virtual bool ShouldShowRationale()
        {
            return default;
        }
    }
#endif

#if IOS || MACCATALYST
    internal static partial class AVPermissions
    {
#pragma warning disable CA1416 // https://github.com/xamarin/xamarin-macios/issues/14619
        internal static PermissionStatus CheckPermissionsStatus(AVAuthorizationMediaType mediaType)
        {
            var status = AVCaptureDevice.GetAuthorizationStatus(mediaType);
            return status switch
            {
                AVAuthorizationStatus.Authorized => PermissionStatus.Granted,
                AVAuthorizationStatus.Denied => PermissionStatus.Denied,
                AVAuthorizationStatus.Restricted => PermissionStatus.Restricted,
                _ => PermissionStatus.Unknown,
            };
        }

        internal static async Task<PermissionStatus> RequestPermissionAsync(AVAuthorizationMediaType mediaType)
        {
            try
            {
                var auth = await AVCaptureDevice.RequestAccessForMediaTypeAsync(mediaType);
                return auth ? PermissionStatus.Granted : PermissionStatus.Denied;
            }
            catch (Exception ex)
            {
                Debug.WriteLine($"Unable to get {mediaType} permission: " + ex);
                return PermissionStatus.Unknown;
            }
        }
#pragma warning restore CA1416
    }
#endif

    public partial class PhoneNumber : BasePlatformPermission, Permissions2.PhoneNumber
    {
    }

    public partial class Battery : BasePlatformPermission, Permissions2.Battery
    {
#if ANDROID
        public override (string androidPermission, bool isRuntime)[] RequiredPermissions =>
            new (string, bool)[] { (Manifest.Permission.BatteryStats, false) };

        public override Task<PermissionStatus> CheckStatusAsync() =>
            Task.FromResult(IsDeclaredInManifest(Manifest.Permission.BatteryStats) ? PermissionStatus.Granted : PermissionStatus.Denied);
#endif
    }

    public partial class Bluetooth : BasePlatformPermission, Permissions2.Bluetooth
    {
#if ANDROID
        public override (string androidPermission, bool isRuntime)[] RequiredPermissions
        {
            get
            {
                var permissions = new List<(string, bool)>();

                // When targeting Android 11 or lower, AccessFineLocation is required for Bluetooth.
                // For Android 12 and above, it is optional.
                if (Application.Context.ApplicationInfo!.TargetSdkVersion <= BuildVersionCodes.R || IsDeclaredInManifest(Manifest.Permission.AccessFineLocation))
                    permissions.Add((Manifest.Permission.AccessFineLocation, true));

#if __ANDROID_31__
                if (OperatingSystem.IsAndroidVersionAtLeast(31) && Application.Context.ApplicationInfo.TargetSdkVersion >= BuildVersionCodes.S)
                {
                    // new runtime permissions on Android 12
                    if (IsDeclaredInManifest(Manifest.Permission.BluetoothScan))
                        permissions.Add((Manifest.Permission.BluetoothScan, true));
                    if (IsDeclaredInManifest(Manifest.Permission.BluetoothConnect))
                        permissions.Add((Manifest.Permission.BluetoothConnect, true));
                    if (IsDeclaredInManifest(Manifest.Permission.BluetoothAdvertise))
                        permissions.Add((Manifest.Permission.BluetoothAdvertise, true));
                }
#endif

                return permissions.ToArray();
            }
        }
#endif
    }

    public partial class CalendarRead : BasePlatformPermission, Permissions2.CalendarRead
    {
#if ANDROID
        public override (string androidPermission, bool isRuntime)[] RequiredPermissions =>
            new (string, bool)[] { (Manifest.Permission.ReadCalendar, true) };
#elif IOS || MACCATALYST
        /// <inheritdoc/>
        protected override Func<IEnumerable<string>> RequiredInfoPlistKeys =>
            () => new string[] { "NSCalendarsUsageDescription" };

        /// <inheritdoc/>
        public override Task<PermissionStatus> CheckStatusAsync()
        {
            EnsureDeclared();

            return Task.FromResult(EventPermission.CheckPermissionStatus(EKEntityType.Event));
        }

        /// <inheritdoc/>
        public override Task<PermissionStatus> RequestAsync()
        {
            EnsureDeclared();

            var status = EventPermission.CheckPermissionStatus(EKEntityType.Event);
            if (status == PermissionStatus.Granted)
                return Task.FromResult(status);

            return EventPermission.RequestPermissionAsync(EKEntityType.Event);
        }
#endif
    }

    public partial class CalendarWrite : BasePlatformPermission, Permissions2.CalendarWrite
    {
#if ANDROID
        public override (string androidPermission, bool isRuntime)[] RequiredPermissions =>
            new (string, bool)[] { (Manifest.Permission.WriteCalendar, true) };
#elif IOS || MACCATALYST
        /// <inheritdoc/>
        protected override Func<IEnumerable<string>> RequiredInfoPlistKeys =>
            () => new string[] { "NSCalendarsUsageDescription" };

        /// <inheritdoc/>
        public override Task<PermissionStatus> CheckStatusAsync()
        {
            EnsureDeclared();

            return Task.FromResult(EventPermission.CheckPermissionStatus(EKEntityType.Event));
        }

        /// <inheritdoc/>
        public override Task<PermissionStatus> RequestAsync()
        {
            EnsureDeclared();

            var status = EventPermission.CheckPermissionStatus(EKEntityType.Event);
            if (status == PermissionStatus.Granted)
                return Task.FromResult(status);

            return EventPermission.RequestPermissionAsync(EKEntityType.Event);
        }
#endif
    }

    public partial class Camera : BasePlatformPermission, Permissions2.Camera
    {
#if ANDROID
        public override (string androidPermission, bool isRuntime)[] RequiredPermissions =>
            new (string, bool)[] { (Manifest.Permission.Camera, true) };
#elif IOS || MACCATALYST
#pragma warning disable CA1416 // 验证平台兼容性
        /// <inheritdoc/>
        protected override Func<IEnumerable<string>> RequiredInfoPlistKeys =>
            () => new string[] { "NSCameraUsageDescription" };

        /// <inheritdoc/>
        public override Task<PermissionStatus> CheckStatusAsync()
        {
            EnsureDeclared();

            return Task.FromResult(AVPermissions.CheckPermissionsStatus(AVAuthorizationMediaType.Video));
        }

        /// <inheritdoc/>
        public override async Task<PermissionStatus> RequestAsync()
        {
            EnsureDeclared();

            var status = AVPermissions.CheckPermissionsStatus(AVAuthorizationMediaType.Video);
            if (status == PermissionStatus.Granted)
                return status;

            EnsureMainThread();

            return await AVPermissions.RequestPermissionAsync(AVAuthorizationMediaType.Video);
        }
#pragma warning restore CA1416 // 验证平台兼容性
#endif
    }

    public partial class ContactsRead : BasePlatformPermission, Permissions2.ContactsRead
    {
#if ANDROID
        public override (string androidPermission, bool isRuntime)[] RequiredPermissions =>
            new (string, bool)[] { (Manifest.Permission.ReadContacts, true) };
#elif WINDOWS
        /// <inheritdoc/>
        public override async Task<PermissionStatus> CheckStatusAsync()
        {
            EnsureDeclared();
#pragma warning disable CA1416 // 验证平台兼容性
            var accessStatus = await ContactManager.RequestStoreAsync(ContactStoreAccessType.AppContactsReadWrite);
#pragma warning restore CA1416 // 验证平台兼容性

            if (accessStatus == null)
                return PermissionStatus.Denied;

            return PermissionStatus.Granted;
        }
#elif IOS || MACCATALYST
#pragma warning disable CA1416 // 验证平台兼容性
        /// <inheritdoc/>
        protected override Func<IEnumerable<string>> RequiredInfoPlistKeys =>
            () => new string[] { "NSContactsUsageDescription" };

        /// <inheritdoc/>
        public override Task<PermissionStatus> CheckStatusAsync()
        {
            EnsureDeclared();

            return Task.FromResult(GetAddressBookPermissionStatus());
        }

        /// <inheritdoc/>
        public override Task<PermissionStatus> RequestAsync()
        {
            EnsureDeclared();

            var status = GetAddressBookPermissionStatus();
            if (status == PermissionStatus.Granted)
                return Task.FromResult(status);

            EnsureMainThread();

            return RequestAddressBookPermission();
        }

        internal static PermissionStatus GetAddressBookPermissionStatus()
        {
            var status = Contacts.CNContactStore.GetAuthorizationStatus(Contacts.CNEntityType.Contacts);
            return status switch
            {
                Contacts.CNAuthorizationStatus.Authorized => PermissionStatus.Granted,
                Contacts.CNAuthorizationStatus.Denied => PermissionStatus.Denied,
                Contacts.CNAuthorizationStatus.Restricted => PermissionStatus.Restricted,
                _ => PermissionStatus.Unknown,
            };
        }

        internal static async Task<PermissionStatus> RequestAddressBookPermission()
        {
            var contactStore = new Contacts.CNContactStore();
            var result = await contactStore.RequestAccessAsync(Contacts.CNEntityType.Contacts);

            if (result.Item2 != null)
                return PermissionStatus.Denied;

            return result.Item1 ? PermissionStatus.Granted : PermissionStatus.Denied;
        }
#pragma warning restore CA1416 // 验证平台兼容性
#endif
    }

    public partial class ContactsWrite : BasePlatformPermission, Permissions2.ContactsWrite
    {
#if ANDROID
        public override (string androidPermission, bool isRuntime)[] RequiredPermissions =>
            new (string, bool)[] { (Manifest.Permission.WriteContacts, true) };
#elif WINDOWS
        /// <inheritdoc/>
        public override async Task<PermissionStatus> CheckStatusAsync()
        {
            EnsureDeclared();
#pragma warning disable CA1416 // 验证平台兼容性
            var accessStatus = await ContactManager.RequestStoreAsync(ContactStoreAccessType.AppContactsReadWrite);
#pragma warning restore CA1416 // 验证平台兼容性

            if (accessStatus == null)
                return PermissionStatus.Denied;

            return PermissionStatus.Granted;
        }
#elif IOS || MACCATALYST
        /// <inheritdoc/>
        protected override Func<IEnumerable<string>> RequiredInfoPlistKeys =>
            () => new string[] { "NSContactsUsageDescription" };

        /// <inheritdoc/>
        public override Task<PermissionStatus> CheckStatusAsync()
        {
            EnsureDeclared();

            return Task.FromResult(ContactsRead.GetAddressBookPermissionStatus());
        }

        /// <inheritdoc/>
        public override Task<PermissionStatus> RequestAsync()
        {
            EnsureDeclared();

            var status = ContactsRead.GetAddressBookPermissionStatus();
            if (status == PermissionStatus.Granted)
                return Task.FromResult(status);

            EnsureMainThread();

            return ContactsRead.RequestAddressBookPermission();
        }
#endif
    }

    public partial class Flashlight : BasePlatformPermission, Permissions2.Flashlight
    {
#if ANDROID
        public override (string androidPermission, bool isRuntime)[] RequiredPermissions =>
            new (string, bool)[]
            {
                    (Manifest.Permission.Camera, true),
                    (Manifest.Permission.Flashlight, false),
            };
#endif
    }

    public partial class LaunchApp : BasePlatformPermission, Permissions2.LaunchApp
    {
    }

    public partial class LocationWhenInUse : BasePlatformPermission, Permissions2.LocationWhenInUse
    {
#if ANDROID
        public override (string androidPermission, bool isRuntime)[] RequiredPermissions =>
            new (string, bool)[]
            {
                    (Manifest.Permission.AccessCoarseLocation, true),
                    (Manifest.Permission.AccessFineLocation, true),
            };

        public override async Task<PermissionStatus> RequestAsync()
        {
            // Check status before requesting first
            if (await CheckStatusAsync() == PermissionStatus.Granted)
                return PermissionStatus.Granted;

            var permissionResult = await DoRequest([Manifest.Permission.AccessCoarseLocation, Manifest.Permission.AccessFineLocation]);

            // when requesting fine location, user can decline and set coarse instead
            var count = permissionResult.GrantResults.Count(x => x == Permission.Granted);
            return count switch
            {
                2 => PermissionStatus.Granted,
                1 => PermissionStatus.Restricted,
                _ => PermissionStatus.Denied,
            };
        }
#elif WINDOWS
        /// <inheritdoc/>
        public override Task<PermissionStatus> CheckStatusAsync()
        {
            EnsureDeclared();
            return RequestLocationPermissionAsync();
        }

        internal static async Task<PermissionStatus> RequestLocationPermissionAsync()
        {
            if (!MainThread2.IsMainThread())
                throw new PermissionException("Permission request must be invoked on main thread.");

#pragma warning disable CA1416 // 验证平台兼容性
            var accessStatus = await Geolocator.RequestAccessAsync();
            return accessStatus switch
            {
                GeolocationAccessStatus.Allowed => PermissionStatus.Granted,
                GeolocationAccessStatus.Unspecified => PermissionStatus.Unknown,
                _ => PermissionStatus.Denied,
            };
#pragma warning restore CA1416 // 验证平台兼容性
        }
#elif MACOS
        protected override Func<IEnumerable<string>> RecommendedInfoPlistKeys =>
                () => new string[] { "NSLocationWhenInUseUsageDescription" };

        public override Task<PermissionStatus> CheckStatusAsync()
        {
            EnsureDeclared();

            return Task.FromResult(GetLocationStatus());
        }

        public override async Task<PermissionStatus> RequestAsync()
        {
            EnsureDeclared();

            var status = GetLocationStatus();
            if (status == PermissionStatus.Granted || status == PermissionStatus.Disabled)
                return status;

            EnsureMainThread();

            return await RequestLocationAsync();
        }

        internal static PermissionStatus GetLocationStatus()
        {
            if (!CLLocationManager.LocationServicesEnabled)
                return PermissionStatus.Disabled;

#pragma warning disable CA1416 // 验证平台兼容性
#pragma warning disable CA1422 // 验证平台兼容性
            var status = CLLocationManager.Status;

            return status switch
            {
                CLAuthorizationStatus.AuthorizedAlways => PermissionStatus.Granted,
                CLAuthorizationStatus.AuthorizedWhenInUse => PermissionStatus.Granted,
                CLAuthorizationStatus.Denied => PermissionStatus.Denied,
                CLAuthorizationStatus.Restricted => PermissionStatus.Restricted,
                _ => PermissionStatus.Unknown,
            };
#pragma warning restore CA1416 // 验证平台兼容性
#pragma warning restore CA1422 // 验证平台兼容性
        }

        static CLLocationManager? locationManager;

        internal static Task<PermissionStatus> RequestLocationAsync()
        {
            locationManager = new CLLocationManager();

            var tcs = new TaskCompletionSource<PermissionStatus>(locationManager);

#pragma warning disable CA1416 // 验证平台兼容性
#pragma warning disable CA1422 // 验证平台兼容性
            var previousState = CLLocationManager.Status;

            locationManager.AuthorizationChanged += LocationAuthCallback;
            locationManager.StartUpdatingLocation();
            locationManager.StopUpdatingLocation();

            return tcs.Task;

            void LocationAuthCallback(object? sender, CLAuthorizationChangedEventArgs e)
            {
                if (e.Status == CLAuthorizationStatus.NotDetermined)
                    return;

                locationManager!.AuthorizationChanged -= LocationAuthCallback;
                tcs.TrySetResult(GetLocationStatus());
                locationManager.Dispose();
                locationManager = null;
            }
#pragma warning restore CA1416 // 验证平台兼容性
#pragma warning restore CA1422 // 验证平台兼容性
        }
#elif IOS || MACCATALYST
        /// <inheritdoc/>
        protected override Func<IEnumerable<string>> RequiredInfoPlistKeys =>
            () => new string[] { "NSLocationWhenInUseUsageDescription" };

        /// <inheritdoc/>
        public override Task<PermissionStatus> CheckStatusAsync()
        {
            EnsureDeclared();

            return Task.FromResult(GetLocationStatus(true));
        }

        /// <inheritdoc/>
        public override async Task<PermissionStatus> RequestAsync()
        {
            EnsureDeclared();

            var status = GetLocationStatus(true);
            if (status == PermissionStatus.Granted || status == PermissionStatus.Disabled)
                return status;

            EnsureMainThread();

#pragma warning disable CA1416 // https://github.com/xamarin/xamarin-macios/issues/14619
            return await RequestLocationAsync(true, lm => lm.RequestWhenInUseAuthorization());
#pragma warning restore CA1416
        }

        internal static PermissionStatus GetLocationStatus(bool whenInUse)
        {
            if (!CLLocationManager.LocationServicesEnabled)
                return PermissionStatus.Disabled;

#pragma warning disable CA1416 // TODO: CLLocationManager.Status has [UnsupportedOSPlatform("ios14.0")], [UnsupportedOSPlatform("macos11.0")], [UnsupportedOSPlatform("tvos14.0")], [UnsupportedOSPlatform("watchos7.0")]
#pragma warning disable CA1422 // Validate platform compatibility
            var status = CLLocationManager.Status;

            return status switch
            {
                CLAuthorizationStatus.AuthorizedAlways => PermissionStatus.Granted,
                CLAuthorizationStatus.AuthorizedWhenInUse => whenInUse ? PermissionStatus.Granted : PermissionStatus.Denied,
                CLAuthorizationStatus.Denied => PermissionStatus.Denied,
                CLAuthorizationStatus.Restricted => PermissionStatus.Restricted,
                _ => PermissionStatus.Unknown,
            };
#pragma warning restore CA1422 // Validate platform compatibility
#pragma warning restore CA1416
        }

        static CLLocationManager? locationManager;

        internal static Task<PermissionStatus> RequestLocationAsync(bool whenInUse, Action<CLLocationManager> invokeRequest)
        {
            if (!CLLocationManager.LocationServicesEnabled)
                return Task.FromResult(PermissionStatus.Disabled);

            locationManager = new CLLocationManager();
#pragma warning disable CA1416 // TODO: CLLocationManager.Status has [UnsupportedOSPlatform("ios14.0")], [UnsupportedOSPlatform("macos11.0")], [UnsupportedOSPlatform("tvos14.0")], [UnsupportedOSPlatform("watchos7.0")]
            var previousState = locationManager.GetAuthorizationStatus();

            var tcs = new TaskCompletionSource<PermissionStatus>(locationManager);

            var del = new ManagerDelegate();
            del.AuthorizationStatusChanged += LocationAuthCallback;
            locationManager.Delegate = del;

            invokeRequest(locationManager);

            return tcs.Task;

            void LocationAuthCallback(object? sender, CLAuthorizationChangedEventArgs e)
            {
                if (e.Status == CLAuthorizationStatus.NotDetermined)
                    return;

                try
                {
                    if (previousState == CLAuthorizationStatus.AuthorizedWhenInUse && !whenInUse)
                    {
                        if (e.Status == CLAuthorizationStatus.AuthorizedWhenInUse)
                        {
                            Utils.WithTimeout(tcs.Task, LocationTimeout).ContinueWith(t =>
                            {
                                try
                                {
                                    // Wait for a timeout to see if the check is complete
                                    if (tcs != null && !tcs.Task.IsCompleted)
                                    {
                                        del.AuthorizationStatusChanged -= LocationAuthCallback;
                                        tcs.TrySetResult(GetLocationStatus(whenInUse));
                                    }
                                }
                                catch (Exception ex)
                                {
                                    // TODO change this to Logger?
                                    Debug.WriteLine($"Exception processing location permission: {ex.Message}");
                                    tcs?.TrySetException(ex);
                                }
                                finally
                                {
                                    locationManager?.Dispose();
                                    locationManager = null;
                                }
                            });
                            return;
                        }
                    }

                    del.AuthorizationStatusChanged -= LocationAuthCallback;
                    locationManager?.Dispose();
                    locationManager = null;
                    tcs.TrySetResult(GetLocationStatus(whenInUse));
                }
                catch (Exception ex)
                {
                    Debug.WriteLine($"Exception processing location permission: {ex.Message}");
                    tcs?.TrySetException(ex);
                    locationManager?.Dispose();
                    locationManager = null;
                }
            }
#pragma warning restore CA1416
        }

        sealed class ManagerDelegate : NSObject, ICLLocationManagerDelegate
        {
            public event EventHandler<CLAuthorizationChangedEventArgs>? AuthorizationStatusChanged;

            [Export("locationManager:didChangeAuthorizationStatus:")]
#pragma warning disable IDE0060 // 删除未使用的参数
            public void AuthorizationChanged(CLLocationManager manager, CLAuthorizationStatus status) =>
                AuthorizationStatusChanged?.Invoke(this, new CLAuthorizationChangedEventArgs(status));
#pragma warning restore IDE0060 // 删除未使用的参数

#pragma warning disable CA1416 // 验证平台兼容性
            [Export("locationManagerDidChangeAuthorization:")]
            public void DidChangeAuthorization(CLLocationManager manager) =>
                AuthorizationStatusChanged?.Invoke(this, new CLAuthorizationChangedEventArgs(manager.GetAuthorizationStatus()));
#pragma warning restore CA1416 // 验证平台兼容性
        }
#endif
    }

    public partial class LocationAlways : BasePlatformPermission, Permissions2.LocationAlways
    {
#if ANDROID
        public override (string androidPermission, bool isRuntime)[] RequiredPermissions
        {
            get
            {
                var permissions = new List<(string, bool)>();
#if __ANDROID_29__
                // Check if running and targeting Q
                if (OperatingSystem.IsAndroidVersionAtLeast(29) && Application.Context.ApplicationInfo!.TargetSdkVersion >= BuildVersionCodes.Q)
                    permissions.Add((Manifest.Permission.AccessBackgroundLocation, true));
#endif

                permissions.Add((Manifest.Permission.AccessCoarseLocation, true));
                permissions.Add((Manifest.Permission.AccessFineLocation, true));

                return permissions.ToArray();
            }
        }

#if __ANDROID_29__
        public override async Task<PermissionStatus> RequestAsync()
        {
            // Check status before requesting first
            if (await CheckStatusAsync() == PermissionStatus.Granted)
                return PermissionStatus.Granted;

            if (OperatingSystem.IsAndroidVersionAtLeast(30))
            {
                var permissionResult = await new LocationWhenInUse().RequestAsync();
                if (permissionResult == PermissionStatus.Denied)
                    return PermissionStatus.Denied;

                var result = await DoRequest([Manifest.Permission.AccessBackgroundLocation]);
                if (!result.GrantResults.All(x => x == Permission.Granted))
                    permissionResult = PermissionStatus.Restricted;

                return permissionResult;
            }

            return await base.RequestAsync();
        }
#endif
#elif WINDOWS
        /// <inheritdoc/>
        public override Task<PermissionStatus> CheckStatusAsync()
        {
            EnsureDeclared();
            return LocationWhenInUse.RequestLocationPermissionAsync();
        }
#elif IOS || MACCATALYST
        /// <inheritdoc/>
        protected override Func<IEnumerable<string>> RequiredInfoPlistKeys =>
            () => new string[]
            {
                    "NSLocationAlwaysAndWhenInUseUsageDescription",
                    "NSLocationAlwaysUsageDescription",
            };

        /// <inheritdoc/>
        public override Task<PermissionStatus> CheckStatusAsync()
        {
            EnsureDeclared();

            return Task.FromResult(LocationWhenInUse.GetLocationStatus(false));
        }

        /// <inheritdoc/>
        public override async Task<PermissionStatus> RequestAsync()
        {
            EnsureDeclared();

            var status = LocationWhenInUse.GetLocationStatus(false);
            if (status == PermissionStatus.Granted)
                return status;

            EnsureMainThread();

#pragma warning disable CA1416 // https://github.com/xamarin/xamarin-macios/issues/14619
            return await LocationWhenInUse.RequestLocationAsync(false, lm => lm.RequestAlwaysAuthorization());
#pragma warning restore CA1416
        }
#endif
    }

    public partial class Maps : BasePlatformPermission, Permissions2.Maps
    {
    }

    public partial class Media : BasePlatformPermission, Permissions2.Media
    {
#if IOS || MACCATALYST
#pragma warning disable CA1416 // 验证平台兼容性
        /// <inheritdoc/>
        protected override Func<IEnumerable<string>> RequiredInfoPlistKeys =>
            () => new string[] { "NSAppleMusicUsageDescription" };

        /// <inheritdoc/>
        public override Task<PermissionStatus> CheckStatusAsync()
        {
            EnsureDeclared();

            return Task.FromResult(GetMediaPermissionStatus());
        }

        /// <inheritdoc/>
        public override Task<PermissionStatus> RequestAsync()
        {
            EnsureDeclared();

            var status = GetMediaPermissionStatus();
            if (status == PermissionStatus.Granted)
                return Task.FromResult(status);

            EnsureMainThread();

            return RequestMediaPermission();
        }

        internal static PermissionStatus GetMediaPermissionStatus()
        {
            var status = MPMediaLibrary.AuthorizationStatus;
            return status switch
            {
                MPMediaLibraryAuthorizationStatus.Authorized => PermissionStatus.Granted,
                MPMediaLibraryAuthorizationStatus.Denied => PermissionStatus.Denied,
                MPMediaLibraryAuthorizationStatus.Restricted => PermissionStatus.Restricted,
                _ => PermissionStatus.Unknown,
            };
        }

        internal static Task<PermissionStatus> RequestMediaPermission()
        {
            var tcs = new TaskCompletionSource<PermissionStatus>();

            MPMediaLibrary.RequestAuthorization(s =>
            {
                switch (s)
                {
                    case MPMediaLibraryAuthorizationStatus.Authorized:
                        tcs.TrySetResult(PermissionStatus.Granted);
                        break;
                    case MPMediaLibraryAuthorizationStatus.Denied:
                        tcs.TrySetResult(PermissionStatus.Denied);
                        break;
                    case MPMediaLibraryAuthorizationStatus.Restricted:
                        tcs.TrySetResult(PermissionStatus.Restricted);
                        break;
                    default:
                        tcs.TrySetResult(PermissionStatus.Unknown);
                        break;
                }
            });

            return tcs.Task;
        }
#pragma warning restore CA1416 // 验证平台兼容性
#endif
    }

    public partial class Microphone : BasePlatformPermission, Permissions2.Microphone
    {
#if ANDROID
        public override (string androidPermission, bool isRuntime)[] RequiredPermissions =>
            new (string, bool)[] { (Manifest.Permission.RecordAudio, true) };
#elif IOS || MACCATALYST
#pragma warning disable CA1416 // 验证平台兼容性
        /// <inheritdoc/>
        protected override Func<IEnumerable<string>> RequiredInfoPlistKeys =>
            () => new string[] { "NSMicrophoneUsageDescription" };

        /// <inheritdoc/>
        public override Task<PermissionStatus> CheckStatusAsync()
        {
            EnsureDeclared();

            return Task.FromResult(AVPermissions.CheckPermissionsStatus(AVAuthorizationMediaType.Audio));
        }

        /// <inheritdoc/>
        public override Task<PermissionStatus> RequestAsync()
        {
            EnsureDeclared();

            var status = AVPermissions.CheckPermissionsStatus(AVAuthorizationMediaType.Audio);
            if (status == PermissionStatus.Granted)
                return Task.FromResult(status);

            EnsureMainThread();

            return AVPermissions.RequestPermissionAsync(AVAuthorizationMediaType.Audio);
        }
#pragma warning restore CA1416 // 验证平台兼容性
#endif
    }

    public partial class NearbyWifiDevices : BasePlatformPermission, Permissions2.NearbyWifiDevices
    {
#if ANDROID
        public override (string androidPermission, bool isRuntime)[] RequiredPermissions
        {
            get
            {
                var permissions = new List<(string, bool)>();
                // When targeting Android 12 or lower, AccessFineLocation is required for several WiFi APIs.
                // For Android 13 and above, it is optional.
                if (Application.Context.ApplicationInfo!.TargetSdkVersion < BuildVersionCodes.Tiramisu || IsDeclaredInManifest(Manifest.Permission.AccessFineLocation))
                    permissions.Add((Manifest.Permission.AccessFineLocation, true));

#if __ANDROID_33__
                if (OperatingSystem.IsAndroidVersionAtLeast(33) && Application.Context.ApplicationInfo.TargetSdkVersion >= BuildVersionCodes.Tiramisu)
                {
                    // new runtime permission on Android 13
                    if (IsDeclaredInManifest(Manifest.Permission.NearbyWifiDevices))
                        permissions.Add((Manifest.Permission.NearbyWifiDevices, true));
                }
#endif

                return permissions.ToArray();
            }
        }
#endif
    }

    public partial class NetworkState : BasePlatformPermission, Permissions2.NetworkState
    {
#if ANDROID
        public override (string androidPermission, bool isRuntime)[] RequiredPermissions
        {
            get
            {
                var permissions = new List<(string, bool)>
                    {
                        (Manifest.Permission.AccessNetworkState, false),
                    };

                if (IsDeclaredInManifest(Manifest.Permission.ChangeNetworkState))
                    permissions.Add((Manifest.Permission.ChangeNetworkState, true));

                return permissions.ToArray();
            }
        }
#endif
    }

    public partial class Phone : BasePlatformPermission, Permissions2.Phone
    {
#if ANDROID
        public override (string androidPermission, bool isRuntime)[] RequiredPermissions
        {
            get
            {
                var permissions = new List<(string, bool)>
                    {
                        (Manifest.Permission.ReadPhoneState, true),
                    };

                if (IsDeclaredInManifest(Manifest.Permission.CallPhone))
                    permissions.Add((Manifest.Permission.CallPhone, true));
                if (IsDeclaredInManifest(Manifest.Permission.ReadCallLog))
                    permissions.Add((Manifest.Permission.ReadCallLog, true));
                if (IsDeclaredInManifest(Manifest.Permission.WriteCallLog))
                    permissions.Add((Manifest.Permission.WriteCallLog, true));
                if (IsDeclaredInManifest(Manifest.Permission.AddVoicemail))
                    permissions.Add((Manifest.Permission.AddVoicemail, true));
                if (IsDeclaredInManifest(Manifest.Permission.UseSip))
                    permissions.Add((Manifest.Permission.UseSip, true));
                if (OperatingSystem.IsAndroidVersionAtLeast(26))
                {
                    if (IsDeclaredInManifest(Manifest.Permission.AnswerPhoneCalls))
                        permissions.Add((Manifest.Permission.AnswerPhoneCalls, true));
                }

#pragma warning disable IDE0079 // 请删除不必要的忽略
#pragma warning disable CS0618 // Type or member is obsolete
#pragma warning disable CA1416 // Validate platform compatibility
#pragma warning disable CA1422 // Validate platform compatibility
                if (IsDeclaredInManifest(Manifest.Permission.ProcessOutgoingCalls))
                {
                    if (OperatingSystem.IsAndroidVersionAtLeast((int)BuildVersionCodes.Q))
                        System.Diagnostics.Debug.WriteLine($"{Manifest.Permission.ProcessOutgoingCalls} is deprecated in Android 10");
                    permissions.Add((Manifest.Permission.ProcessOutgoingCalls, true));
                }
#pragma warning restore CA1422 // Validate platform compatibility
#pragma warning restore CA1416 // Validate platform compatibility
#pragma warning restore CS0618 // Type or member is obsolete
#pragma warning restore IDE0079 // 请删除不必要的忽略

                return permissions.ToArray();
            }
        }
#endif
    }

    public partial class Photos : BasePlatformPermission, Permissions2.Photos
    {
#if IOS || MACCATALYST
        /// <inheritdoc/>
        protected override Func<IEnumerable<string>> RequiredInfoPlistKeys =>
            () => new string[] { "NSPhotoLibraryUsageDescription" };

        /// <inheritdoc/>
        public override Task<PermissionStatus> CheckStatusAsync()
        {
            EnsureDeclared();

#pragma warning disable CA1416 // 验证平台兼容性
            return Task.FromResult(GetPhotoPermissionStatus(PHAccessLevel.ReadWrite));
#pragma warning restore CA1416 // 验证平台兼容性
        }

        /// <inheritdoc/>
        public override async Task<PermissionStatus> RequestAsync()
        {
            EnsureDeclared();

#pragma warning disable CA1416 // 验证平台兼容性
            var status = GetPhotoPermissionStatus(PHAccessLevel.ReadWrite);
#pragma warning restore CA1416 // 验证平台兼容性
            if (status == PermissionStatus.Granted)
            {
                return status;
            }
            else if (OperatingSystem.IsIOSVersionAtLeast(14) && status == PermissionStatus.Limited)
            {
                PhotosUI.PHPhotoLibrary_PhotosUISupport.PresentLimitedLibraryPicker(
                    PHPhotoLibrary.SharedPhotoLibrary,
                    WindowStateManager.GetCurrentUIViewController(true));
                return status;
            }

            EnsureMainThread();

#pragma warning disable CA1416 // 验证平台兼容性
            return await RequestPhotoPermissionStatus(PHAccessLevel.ReadWrite);
#pragma warning restore CA1416 // 验证平台兼容性
        }
#endif
    }

    public partial class PhotosAddOnly : BasePlatformPermission, Permissions2.PhotosAddOnly
    {
#if IOS || MACCATALYST || MACOS
        /// <inheritdoc/>
        protected override Func<IEnumerable<string>> RequiredInfoPlistKeys =>
                () => new string[] { "NSPhotoLibraryAddUsageDescription" };

        /// <inheritdoc/>
        public override Task<PermissionStatus> CheckStatusAsync()
        {
            EnsureDeclared();

#pragma warning disable CA1416 // 验证平台兼容性
            return Task.FromResult(GetPhotoPermissionStatus(PHAccessLevel.AddOnly));
#pragma warning restore CA1416 // 验证平台兼容性
        }

        /// <inheritdoc/>
        public override async Task<PermissionStatus> RequestAsync()
        {
            EnsureDeclared();

#pragma warning disable CA1416 // 验证平台兼容性
            var status = GetPhotoPermissionStatus(PHAccessLevel.AddOnly);
#pragma warning restore CA1416 // 验证平台兼容性
            if (status == PermissionStatus.Granted)
                return status;

            EnsureMainThread();

#pragma warning disable CA1416 // 验证平台兼容性
            return await RequestPhotoPermissionStatus(PHAccessLevel.AddOnly);
#pragma warning restore CA1416 // 验证平台兼容性
        }
#endif
    }

#if IOS || MACCATALYST || MACOS
#pragma warning disable CA1416 // 验证平台兼容性
#pragma warning disable CA1422 // 验证平台兼容性
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    static PermissionStatus GetPhotoPermissionStatus(PHAccessLevel level)
            => Convert(CheckOSVersionForPhotos()
                ? PHPhotoLibrary.GetAuthorizationStatus(level)
                : PHPhotoLibrary.AuthorizationStatus);

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    static async Task<PermissionStatus> RequestPhotoPermissionStatus(PHAccessLevel level)
        => Convert(CheckOSVersionForPhotos()
            ? await PHPhotoLibrary.RequestAuthorizationAsync(level)
            : await PHPhotoLibrary.RequestAuthorizationAsync());

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    static PermissionStatus Convert(PHAuthorizationStatus status)
        => status switch
        {
            PHAuthorizationStatus.Authorized => PermissionStatus.Granted,
            PHAuthorizationStatus.Limited => PermissionStatus.Limited,
            PHAuthorizationStatus.Denied => PermissionStatus.Denied,
            PHAuthorizationStatus.Restricted => PermissionStatus.Restricted,
            _ => PermissionStatus.Unknown,
        };

    [SupportedOSPlatformGuard("iOS14.0")]
    [SupportedOSPlatformGuard("macOS11.0")]
    [SupportedOSPlatformGuard("tvOS14.0")]
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    static bool CheckOSVersionForPhotos()
    {
        return OperatingSystem.IsIOSVersionAtLeast(14, 0) ||
            OperatingSystem.IsMacOSVersionAtLeast(11, 0) ||
            OperatingSystem.IsTvOSVersionAtLeast(14, 0);
    }
#pragma warning restore CA1422 // 验证平台兼容性
#pragma warning restore CA1416 // 验证平台兼容性
#endif

    public partial class Reminders : BasePlatformPermission, Permissions2.Reminders
    {
#if IOS || MACCATALYST
        /// <inheritdoc/>
        protected override Func<IEnumerable<string>> RequiredInfoPlistKeys =>
            () => new string[] { "NSRemindersUsageDescription" };

        /// <inheritdoc/>
        public override Task<PermissionStatus> CheckStatusAsync()
        {
            EnsureDeclared();

            return Task.FromResult(EventPermission.CheckPermissionStatus(EKEntityType.Reminder));
        }

        /// <inheritdoc/>
        public override Task<PermissionStatus> RequestAsync()
        {
            EnsureDeclared();

            var status = EventPermission.CheckPermissionStatus(EKEntityType.Reminder);
            if (status == PermissionStatus.Granted)
                return Task.FromResult(status);

            return EventPermission.RequestPermissionAsync(EKEntityType.Reminder);
        }
#endif
    }

    public partial class Sensors : BasePlatformPermission, Permissions2.Sensors
    {
#if ANDROID
        public override (string androidPermission, bool isRuntime)[] RequiredPermissions =>
            new (string, bool)[] { (Manifest.Permission.BodySensors, true) };
#elif WINDOWS
        static readonly Guid activitySensorClassId = new("9D9E0118-1807-4F2E-96E4-2CE57142E196");

        /// <inheritdoc/>
        public override Task<PermissionStatus> CheckStatusAsync()
        {
            // Determine if the user has allowed access to activity sensors
#pragma warning disable CA1416 // 验证平台兼容性
            var deviceAccessInfo = DeviceAccessInformation.CreateFromDeviceClassId(activitySensorClassId);
            return deviceAccessInfo.CurrentStatus switch
            {
                DeviceAccessStatus.Allowed => Task.FromResult(PermissionStatus.Granted),
                DeviceAccessStatus.DeniedBySystem or DeviceAccessStatus.DeniedByUser => Task.FromResult(PermissionStatus.Denied),
                _ => Task.FromResult(PermissionStatus.Unknown),
            };
#pragma warning restore CA1416 // 验证平台兼容性
        }
#elif IOS || MACCATALYST
        /// <inheritdoc/>
        protected override Func<IEnumerable<string>> RequiredInfoPlistKeys =>
            () => new string[] { "NSMotionUsageDescription" };

        /// <inheritdoc/>
        public override Task<PermissionStatus> CheckStatusAsync()
        {
            EnsureDeclared();

            return Task.FromResult(GetSensorPermissionStatus());
        }

        /// <inheritdoc/>
        public override Task<PermissionStatus> RequestAsync()
        {
            EnsureDeclared();

            var status = GetSensorPermissionStatus();
            if (status == PermissionStatus.Granted)
                return Task.FromResult(status);

            EnsureMainThread();

            return RequestSensorPermission();
        }

        internal static PermissionStatus GetSensorPermissionStatus()
        {
            // Check if it's available
#pragma warning disable CA1416 // 验证平台兼容性
            if (!CMMotionActivityManager.IsActivityAvailable)
                return PermissionStatus.Disabled;
#pragma warning restore CA1416 // 验证平台兼容性

            if (OperatingSystem.IsIOSVersionAtLeast(11, 0) || OperatingSystem.IsWatchOSVersionAtLeast(4, 0))
            {
#pragma warning disable CA1416 // https://github.com/xamarin/xamarin-macios/issues/14619
                switch (CMMotionActivityManager.AuthorizationStatus)
                {
                    case CMAuthorizationStatus.Authorized:
                        return PermissionStatus.Granted;
                    case CMAuthorizationStatus.Denied:
                        return PermissionStatus.Denied;
                    case CMAuthorizationStatus.NotDetermined:
                        return PermissionStatus.Unknown;
                    case CMAuthorizationStatus.Restricted:
                        return PermissionStatus.Restricted;
                }
#pragma warning restore CA1416
            }

            return PermissionStatus.Unknown;
        }

        internal static async Task<PermissionStatus> RequestSensorPermission()
        {
#pragma warning disable CA1416 // 验证平台兼容性
            var activityManager = new CMMotionActivityManager();

            try
            {
                var results = await activityManager.QueryActivityAsync(NSDate.DistantPast, NSDate.DistantFuture, NSOperationQueue.MainQueue);
                if (results != null)
                    return PermissionStatus.Granted;
            }
            catch (Exception ex)
            {
                Debug.WriteLine("Unable to query activity manager: " + ex.Message);
                return PermissionStatus.Denied;
            }

            return PermissionStatus.Unknown;
#pragma warning restore CA1416 // 验证平台兼容性
        }
#endif
    }

    public partial class Sms : BasePlatformPermission, Permissions2.Sms
    {
#if ANDROID
        public override (string androidPermission, bool isRuntime)[] RequiredPermissions
        {
            get
            {
                var permissions = new List<(string, bool)>
                {
                };

                if (IsDeclaredInManifest(Manifest.Permission.SendSms))
                    permissions.Add((Manifest.Permission.SendSms, true));
                if (IsDeclaredInManifest(Manifest.Permission.ReadSms))
                    permissions.Add((Manifest.Permission.ReadSms, true));
                if (IsDeclaredInManifest(Manifest.Permission.ReceiveWapPush))
                    permissions.Add((Manifest.Permission.ReceiveWapPush, true));
                if (IsDeclaredInManifest(Manifest.Permission.ReceiveMms))
                    permissions.Add((Manifest.Permission.ReceiveMms, true));

                return permissions.ToArray();
            }
        }
#endif
    }

    public partial class Speech : BasePlatformPermission, Permissions2.Speech
    {
#if ANDROID
        public override (string androidPermission, bool isRuntime)[] RequiredPermissions =>
            new (string, bool)[] { (Manifest.Permission.RecordAudio, true) };
#elif IOS || MACCATALYST
        /// <inheritdoc/>
        protected override Func<IEnumerable<string>> RequiredInfoPlistKeys =>
            () => new string[] { "NSSpeechRecognitionUsageDescription" };

        /// <inheritdoc/>
        public override Task<PermissionStatus> CheckStatusAsync()
        {
            EnsureDeclared();

            return Task.FromResult(GetSpeechPermissionStatus());
        }

        /// <inheritdoc/>
        public override Task<PermissionStatus> RequestAsync()
        {
            EnsureDeclared();

            var status = GetSpeechPermissionStatus();
            if (status == PermissionStatus.Granted)
                return Task.FromResult(status);

            EnsureMainThread();

            return RequestSpeechPermission();
        }

#pragma warning disable CA1416 // https://github.com/xamarin/xamarin-macios/issues/14619
        internal static PermissionStatus GetSpeechPermissionStatus()
        {
            var status = SFSpeechRecognizer.AuthorizationStatus;
            return status switch
            {
                SFSpeechRecognizerAuthorizationStatus.Authorized => PermissionStatus.Granted,
                SFSpeechRecognizerAuthorizationStatus.Denied => PermissionStatus.Denied,
                SFSpeechRecognizerAuthorizationStatus.Restricted => PermissionStatus.Restricted,
                _ => PermissionStatus.Unknown,
            };
        }

        internal static Task<PermissionStatus> RequestSpeechPermission()
        {
            var tcs = new TaskCompletionSource<PermissionStatus>();

            SFSpeechRecognizer.RequestAuthorization(s =>
            {
                switch (s)
                {
                    case SFSpeechRecognizerAuthorizationStatus.Authorized:
                        tcs.TrySetResult(PermissionStatus.Granted);
                        break;
                    case SFSpeechRecognizerAuthorizationStatus.Denied:
                        tcs.TrySetResult(PermissionStatus.Denied);
                        break;
                    case SFSpeechRecognizerAuthorizationStatus.Restricted:
                        tcs.TrySetResult(PermissionStatus.Restricted);
                        break;
                    default:
                        tcs.TrySetResult(PermissionStatus.Unknown);
                        break;
                }
            });

            return tcs.Task;
        }
#pragma warning restore CA1416 // 验证平台兼容性
#endif
    }

    public partial class StorageRead : BasePlatformPermission, Permissions2.StorageRead
    {
#if ANDROID
        public override (string androidPermission, bool isRuntime)[] RequiredPermissions =>
            new (string, bool)[] { (Manifest.Permission.ReadExternalStorage, true) };
#endif
    }

    public partial class StorageWrite : BasePlatformPermission, Permissions2.StorageWrite
    {
#if ANDROID
        public override (string androidPermission, bool isRuntime)[] RequiredPermissions =>
            new (string, bool)[] { (Manifest.Permission.WriteExternalStorage, true) };
#endif
    }

    public partial class Vibrate : BasePlatformPermission, Permissions2.Vibrate
    {
#if ANDROID
        public override (string androidPermission, bool isRuntime)[] RequiredPermissions =>
            new (string, bool)[] { (Manifest.Permission.Vibrate, false) };
#endif
    }
}

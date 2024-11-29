#if ANDROID
using Android;
using Android.Content.PM;
using Android.OS;
using AndroidX.Core.App;
using AndroidX.Core.Content;
using Application = Android.App.Application;
using BuildVersionCodes = Android.OS.BuildVersionCodes;
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

/// <summary>
/// 权限平台服务实现
/// https://github.com/dotnet/maui/tree/8.0.0-rc.2.9373/src/Essentials/src/Permissions
/// </summary>
[SupportedOSPlatform("android")]
[SupportedOSPlatform("macos")]
[SupportedOSPlatform("maccatalyst")]
[SupportedOSPlatform("ios")]
[SupportedOSPlatform("windows")]
public class PermissionsPlatformServiceImpl : IPermissionsPlatformService
{
    /// <inheritdoc/>
    Task<PermissionStatus> IPermissionsPlatformService.CheckStatusAsync<TPermission>()
        => GetRequiredService(typeof(TPermission).Name).CheckStatusAsync();

    /// <inheritdoc/>
    void IPermissionsPlatformService.EnsureDeclared<TPermission>()
        => GetRequiredService(typeof(TPermission).Name).EnsureDeclared();

    /// <inheritdoc/>
    Task<PermissionStatus> IPermissionsPlatformService.RequestAsync<TPermission>()
        => GetRequiredService(typeof(TPermission).Name).RequestAsync();

    /// <inheritdoc/>
    bool IPermissionsPlatformService.ShouldShowRationale<TPermission>()
        => GetRequiredService(typeof(TPermission).Name).ShouldShowRationale();

    /// <summary>
    /// 获取所需服务实例
    /// </summary>
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
    /// <summary>
    /// 检查权限是否在应用程序清单中声明
    /// </summary>
    public static bool IsDeclaredInManifest(string permission)
    {
        var context = Application.Context;
        var packageInfo = context.PackageManager!.GetPackageInfo(context.PackageName!, PackageInfoFlags.Permissions);
        var requestedPermissions = packageInfo?.RequestedPermissions;

        return requestedPermissions?.Any(r => r.Equals(permission, StringComparison.OrdinalIgnoreCase)) ?? false;
    }

    /// <summary>
    /// 处理权限请求结果
    /// </summary>
    /// <param name="requestCode">请求码</param>
    /// <param name="permissions">权限数组</param>
    /// <param name="grantResults">授权结果数组</param>
    internal static void OnRequestPermissionsResult(int requestCode, string[] permissions, Permission[] grantResults)
        => BasePlatformPermission.OnRequestPermissionsResult(requestCode, permissions, grantResults);

    /// <summary>
    /// 权限结果类用于存储权限请求的结果信息
    /// </summary>
    public partial class PermissionResult(string[] permissions, Permission[] grantResults)
    {
        /// <summary>
        /// 获取请求的权限数组
        /// </summary>
        public string[] Permissions { get; } = permissions;

        /// <summary>
        /// 获取授权结果数组
        /// </summary>
        public Permission[] GrantResults { get; } = grantResults;
    }

    /// <summary>
    /// 表示所有权限的抽象基类，实现 <see cref="IBasePermission"/> 接口
    /// </summary>
    public abstract partial class BasePlatformPermission : IBasePermission
    {
        /// <summary>
        /// 权限请求的字典，用于存储每个请求的请求代码和对应的任务完成源
        /// </summary>
        static readonly Dictionary<int, TaskCompletionSource<PermissionResult>> requests = [];

        /// <summary>
        /// 用于请求字典的锁对象
        /// </summary>
        static readonly object locker = new();

        /// <summary>
        /// 当前的请求代码
        /// </summary>
        static int requestCode;

        /// <summary>
        /// 获取所需的权限数组
        /// </summary>
        public virtual (string androidPermission, bool isRuntime)[]? RequiredPermissions { get; }

        /// <inheritdoc/>
        public virtual Task<PermissionStatus> CheckStatusAsync() => CheckStatusAsync(RequiredPermissions);

        /// <summary>
        /// 检索此权限的当前状态
        /// </summary>
        /// <exception cref="PermissionException">如果权限没有在 AndroidManifest.xml 中声明，则引发</exception>
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

        /// <inheritdoc/>
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

        /// <summary>
        /// 执行权限请求并返回结果
        /// </summary>
        /// <param name="permissions">需要请求的权限数组</param>
        /// <returns>权限请求的结果</returns>
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

        /// <inheritdoc/>
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

        /// <inheritdoc/>
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

        /// <summary>
        /// 执行权限检查
        /// </summary>
        /// <param name="androidPermission">Android 权限字符串</param>
        /// <returns>权限状态</returns>
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

        /// <summary>
        /// 处理权限请求结果
        /// </summary>
        /// <param name="requestCode">请求代码</param>
        /// <param name="permissions">请求的权限数组</param>
        /// <param name="grantResults">授权结果数组</param>
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
    /// 检查 <paramref name="capabilityName"/> 中指定的功能是否在应用程序的 <c>AppxManifest.xml</c> 文件中声明
    /// </summary>
    /// <param name="capabilityName">在 <c>AppxManifest.xml</c> 文件中检查规范的功能</param>
    /// <returns><see langword="true"/>当指定了该功能时，否则 <see langword="false"/></returns>
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
    /// 表示所有权限的抽象基类，实现 <see cref="IBasePermission"/> 接口
    /// </summary>
    public abstract partial class BasePlatformPermission : IBasePermission
    {
        /// <summary>
        /// 获取权限所需声明
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

    /// <summary>
    /// 检查指定的键是否在 Info.plist 文件中声明
    /// </summary>
    /// <param name="usageKey">要检查的键名</param>
    /// <returns>如果键存在于 Info.plist 中，则返回 <see langword="true"/>；否则返回 <see langword="false"/></returns>
    public static bool IsKeyDeclaredInInfoPlist(string usageKey) =>
        NSBundle.MainBundle.InfoDictionary.ContainsKey(new NSString(usageKey));

    /// <summary>
    /// 获取或设置位置超时时间，默认为 10 秒
    /// </summary>
    public static TimeSpan LocationTimeout { get; set; } = TimeSpan.FromSeconds(10);

    /// <summary>
    /// 表示所有权限的抽象基类，实现 <see cref="IBasePermission"/> 接口
    /// </summary>
    public abstract class BasePlatformPermission : IBasePermission
    {
        /// <summary>
        /// 获取推荐的 Info.plist 键
        /// </summary>
        protected virtual Func<IEnumerable<string>>? RecommendedInfoPlistKeys { get; }

        /// <summary>
        /// 获取必需的 Info.plist 键
        /// </summary>
        protected virtual Func<IEnumerable<string>>? RequiredInfoPlistKeys { get; }

        /// <inheritdoc/>
        public virtual Task<PermissionStatus> CheckStatusAsync() =>
            Task.FromResult(PermissionStatus.Granted);

        /// <inheritdoc/>
        public virtual Task<PermissionStatus> RequestAsync() =>
            Task.FromResult(PermissionStatus.Granted);

        /// <inheritdoc/>
        public virtual bool ShouldShowRationale() => false;

        /// <inheritdoc/>
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

        /// <summary>
        /// 检查在主线程调用
        /// </summary>
        /// <exception cref="PermissionException">不在主线程上调用权限请求，则引发</exception>
#pragma warning disable CA1822 // 将成员标记为 static
        internal void EnsureMainThread()
#pragma warning restore CA1822 // 将成员标记为 static
        {
            if (!MainThread2.IsMainThread())
                throw new PermissionException("Permission request must be invoked on main thread.");
        }
    }
#elif IOS || MACCATALYST
    /// <summary>
    /// 访问系统事件权限类
    /// </summary>
    internal static class EventPermission
    {
        /// <summary>
        /// 检查权限状态
        /// </summary>
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

        /// <summary>
        /// 异步请求对事件的访问权限
        /// </summary>
        /// <returns>权限状态</returns>
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
    /// 检查 <paramref name="usageKey"/> 中指定的密钥是否在应用程序的 <c>Info.plist</c> 文件中声明
    /// </summary>
    /// <param name="usageKey">检查 <c>Info.plist</c> 文件中声明的键</param>
    /// <returns><see langword="true"/> 当键被声明时，否则 <see langword="false"/></returns>
    public static bool IsKeyDeclaredInInfoPlist(string usageKey) =>
        NSBundle.MainBundle.InfoDictionary.ContainsKey(new NSString(usageKey));

    /// <summary>
    /// 获取或设置请求位置权限时使用的超时
    /// </summary>
    public static TimeSpan LocationTimeout { get; set; } = TimeSpan.FromSeconds(10);

    /// <summary>
    /// 表示所有权限的抽象基类，实现 <see cref="IBasePermission"/> 接口
    /// </summary>
    public abstract class BasePlatformPermission : IBasePermission
    {
        /// <summary>
        /// 获取此权限需要存在于应用程序的 Info.plist 文件中的必需项
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
        /// <summary>
        /// 检查在主线程调用
        /// </summary>
        /// <exception cref="PermissionException">不在主线程上调用权限请求，则引发</exception>
        internal void EnsureMainThread()
#pragma warning restore CA1822 // 将成员标记为 static
        {
            if (!MainThread2.IsMainThread())
                throw new PermissionException("Permission request must be invoked on main thread.");
        }
    }
#else

    /// <summary>
    /// 表示所有权限的抽象基类，实现 <see cref="IBasePermission"/> 接口
    /// </summary>
    public abstract class BasePlatformPermission : IBasePermission
    {
        /// <inheritdoc/>
        public virtual Task<PermissionStatus> CheckStatusAsync()
        {
            return Task.FromResult(PermissionStatus.Granted);
        }

        /// <inheritdoc/>
        public virtual void EnsureDeclared()
        {
        }

        /// <inheritdoc/>
        public virtual Task<PermissionStatus> RequestAsync()
        {
            return Task.FromResult(PermissionStatus.Granted);
        }

        /// <inheritdoc/>
        public virtual bool ShouldShowRationale()
        {
            return default;
        }
    }
#endif

#if IOS || MACCATALYST

    /// <summary>
    /// 用于检查和请求 <see cref="AVCaptureDevice"/> 权限的辅助类
    /// </summary>
    internal static partial class AVPermissions
    {
#pragma warning disable CA1416 // https://github.com/xamarin/xamarin-macios/issues/14619
        /// <summary>
        /// 检查媒体类型的权限状态
        /// </summary>
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

        /// <summary>
        /// 异步请求媒体类型的权限
        /// </summary>
        /// <param name="mediaType">媒体类型</param>
        /// <returns>权限状态</returns>
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

    /// <summary>
    /// 电话号码权限类，继承自 <see cref="BasePlatformPermission"/> 和 <see cref="Permissions2.PhoneNumber"/> 类
    /// </summary>
    public partial class PhoneNumber : BasePlatformPermission, Permissions2.PhoneNumber
    {
    }

    /// <summary>
    /// 电池权限类，继承自 <see cref="BasePlatformPermission"/> 和 <see cref="Permissions2.Battery"/> 类
    /// </summary>
    public partial class Battery : BasePlatformPermission, Permissions2.Battery
    {
#if ANDROID
        /// <inheritdoc/>
        public override (string androidPermission, bool isRuntime)[] RequiredPermissions =>
            new (string, bool)[] { (Manifest.Permission.BatteryStats, false) };

        /// <inheritdoc/>
        public override Task<PermissionStatus> CheckStatusAsync() =>
            Task.FromResult(IsDeclaredInManifest(Manifest.Permission.BatteryStats) ? PermissionStatus.Granted : PermissionStatus.Denied);
#endif
    }

    /// <summary>
    /// 蓝牙权限类，继承自 <see cref="BasePlatformPermission"/> 和 <see cref="Permissions2.Bluetooth"/> 类
    /// </summary>
    public partial class Bluetooth : BasePlatformPermission, Permissions2.Bluetooth
    {
#if ANDROID
        /// <inheritdoc/>
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

    /// <summary>
    /// 读取设备日历信息权限类，继承自 <see cref="BasePlatformPermission"/> 和 <see cref="Permissions2.CalendarRead"/> 类
    /// </summary>
    public partial class CalendarRead : BasePlatformPermission, Permissions2.CalendarRead
    {
#if ANDROID
        /// <inheritdoc/>
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

    /// <summary>
    /// 写入设备日历数据权限类，继承自 <see cref="BasePlatformPermission"/> 和 <see cref="Permissions2.CalendarWrite"/> 类
    /// </summary>
    public partial class CalendarWrite : BasePlatformPermission, Permissions2.CalendarWrite
    {
#if ANDROID
        /// <inheritdoc/>
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

    /// <summary>
    /// 访问设备摄像头的权限类，继承自 <see cref="BasePlatformPermission"/> 和 <see cref="Permissions2.Camera"/> 类
    /// </summary>
    public partial class Camera : BasePlatformPermission, Permissions2.Camera
    {
#if ANDROID
        /// <inheritdoc/>
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

    /// <summary>
    /// 读取设备联系人信息的权限类，继承自 <see cref="BasePlatformPermission"/> 和 <see cref="Permissions2.ContactsRead"/> 类
    /// </summary>
    public partial class ContactsRead : BasePlatformPermission, Permissions2.ContactsRead
    {
#if ANDROID
        /// <inheritdoc/>
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

        /// <summary>
        /// 获取通讯录权限状态的内部静态方法
        /// </summary>
        /// <returns>权限状态</returns>
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

        /// <summary>
        /// 请求通讯录权限的内部异步方法
        /// </summary>
        /// <returns>权限状态</returns>
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

    /// <summary>
    /// 写入设备联系人数据的权限类，继承自 <see cref="BasePlatformPermission"/> 和 <see cref="Permissions2.ContactsWrite"/> 类
    /// </summary>
    public partial class ContactsWrite : BasePlatformPermission, Permissions2.ContactsWrite
    {
#if ANDROID
        /// <inheritdoc/>
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

    /// <summary>
    /// 访问设备手电筒的权限类，继承自 <see cref="BasePlatformPermission"/> 和 <see cref="Permissions2.Flashlight"/> 类
    /// </summary>
    public partial class Flashlight : BasePlatformPermission, Permissions2.Flashlight
    {
#if ANDROID
        /// <inheritdoc/>
        public override (string androidPermission, bool isRuntime)[] RequiredPermissions =>
            new (string, bool)[]
            {
                    (Manifest.Permission.Camera, true),
                    (Manifest.Permission.Flashlight, false),
            };
#endif
    }

    /// <summary>
    /// 在设备上启动其他应用程序的权限，继承自 <see cref="BasePlatformPermission"/> 和 <see cref="Permissions2.LaunchApp"/> 类
    /// </summary>
    public partial class LaunchApp : BasePlatformPermission, Permissions2.LaunchApp
    {
    }

    /// <summary>
    /// 仅当应用程序正在使用时访问设备位置的权限类，继承自 <see cref="BasePlatformPermission"/> 和 <see cref="Permissions2.LaunchApp"/> 类
    /// </summary>
    public partial class LocationWhenInUse : BasePlatformPermission, Permissions2.LocationWhenInUse
    {
#if ANDROID
        /// <inheritdoc/>
        public override (string androidPermission, bool isRuntime)[] RequiredPermissions =>
            new (string, bool)[]
            {
                    (Manifest.Permission.AccessCoarseLocation, true),
                    (Manifest.Permission.AccessFineLocation, true),
            };

        /// <inheritdoc/>
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

        /// <summary>
        /// 请求位置权限内部方法，必须在主线程调用权限请求，否则引发异常 <see cref="PermissionException"/>
        /// </summary>
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
        /// <inheritdoc/>
        protected override Func<IEnumerable<string>> RecommendedInfoPlistKeys =>
                () => new string[] { "NSLocationWhenInUseUsageDescription" };

        /// <inheritdoc/>
        public override Task<PermissionStatus> CheckStatusAsync()
        {
            EnsureDeclared();

            return Task.FromResult(GetLocationStatus());
        }

        /// <inheritdoc/>
        public override async Task<PermissionStatus> RequestAsync()
        {
            EnsureDeclared();

            var status = GetLocationStatus();
            if (status == PermissionStatus.Granted || status == PermissionStatus.Disabled)
                return status;

            EnsureMainThread();

            return await RequestLocationAsync();
        }

        /// <summary>
        /// 获取位置权限状态
        /// </summary>
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

        /// <summary>
        /// 异步请求位置权限方法
        /// </summary>
        /// <returns></returns>
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

        /// <summary>
        /// 根据给定的参数获取位置权限状态
        /// </summary>
        /// <param name="whenInUse">是否是在使用期间请求权限</param>
        /// <returns>位置权限状态</returns>
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

        /// <summary>
        /// 异步请求用户位置权限的方法
        /// </summary>
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

    /// <summary>
    /// 表示访问设备位置的权限，继承自 <see cref="BasePlatformPermission"/> 和 <see cref="Permissions2.LocationAlways"/> 类
    /// </summary>
    public partial class LocationAlways : BasePlatformPermission, Permissions2.LocationAlways
    {
#if ANDROID
        /// <inheritdoc/>
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
        /// <inheritdoc/>
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

    /// <summary>
    /// 表示设备访问地图的权限类，继承自 <see cref="BasePlatformPermission"/> 和 <see cref="Permissions2.Maps"/> 类
    /// </summary>
    public partial class Maps : BasePlatformPermission, Permissions2.Maps
    {
    }

    /// <summary>
    /// 从设备库访问媒体的权限类，继承自 <see cref="BasePlatformPermission"/> 和 <see cref="Permissions2.Media"/> 类
    /// </summary>
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

        /// <summary>
        /// 获取媒体权限状态的内部静态方法
        /// </summary>
        /// <returns>返回权限状态</returns>
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

        /// <summary>
        /// 请求媒体权限的内部静态方法
        /// </summary>
        /// <returns>返回权限状态的任务</returns>
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

    /// <summary>
    /// 访问设备麦克风的权限类，继承自 <see cref="BasePlatformPermission"/> 和 <see cref="Permissions2.Microphone"/> 类
    /// </summary>
    public partial class Microphone : BasePlatformPermission, Permissions2.Microphone
    {
#if ANDROID
        /// <inheritdoc/>
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

    /// <summary>
    /// 访问附近 WiFi 设备的权限类，继承自 <see cref="BasePlatformPermission"/> 和 <see cref="Permissions2.NearbyWifiDevices"/> 类
    /// </summary>
    public partial class NearbyWifiDevices : BasePlatformPermission, Permissions2.NearbyWifiDevices
    {
#if ANDROID
        /// <inheritdoc/>
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

    /// <summary>
    /// 表示访问设备网络状态信息的权限类，继承自 <see cref="BasePlatformPermission"/> 和 <see cref="Permissions2.NetworkState"/> 类
    /// </summary>
    public partial class NetworkState : BasePlatformPermission, Permissions2.NetworkState
    {
#if ANDROID
        /// <inheritdoc/>
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

    /// <summary>
    /// 表示访问设备电话数据的权限类，继承自 <see cref="BasePlatformPermission"/> 和 <see cref="Permissions2.Phone"/> 类
    /// </summary>
    public partial class Phone : BasePlatformPermission, Permissions2.Phone
    {
#if ANDROID
        /// <inheritdoc/>
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

    /// <summary>
    /// 表示访问设备库中照片的权限类，继承自 <see cref="BasePlatformPermission"/> 和 <see cref="Permissions2.Photos"/> 类
    /// </summary>
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

    /// <summary>
    /// 表示向设备库添加照片（非读取）的权限类，继承自 <see cref="BasePlatformPermission"/> 和 <see cref="Permissions2.Photos"/> 类
    /// </summary>
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

    /// <summary>
    /// 提醒事项权限类，继承自 <see cref="BasePlatformPermission"/> 和 <see cref="Permissions2.Reminders"/> 类
    /// </summary>
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

    /// <summary>
    /// 表示访问设备传感器的权限类，继承自 <see cref="BasePlatformPermission"/> 和 <see cref="Permissions2.Sensors"/> 类
    /// </summary>
    public partial class Sensors : BasePlatformPermission, Permissions2.Sensors
    {
#if ANDROID
        /// <inheritdoc/>
        public override (string androidPermission, bool isRuntime)[] RequiredPermissions =>
            new (string, bool)[] { (Manifest.Permission.BodySensors, true) };
#elif WINDOWS
        /// <summary>
        /// 表示活动传感器类的标识符
        /// </summary>
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

        /// <summary>
        /// 获取传感器权限的状态
        /// </summary>
        /// <returns>返回传感器权限状态。</returns>
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

        /// <summary>
        /// 请求传感器权限的方法
        /// </summary>
        /// <returns>表示传感器权限状态的枚举值。</returns>
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

    /// <summary>
    /// 表示访问设备 SMS 数据的权限类，继承自 <see cref="BasePlatformPermission"/> 和 <see cref="Permissions2.Sms"/> 类
    /// </summary>
    public partial class Sms : BasePlatformPermission, Permissions2.Sms
    {
#if ANDROID
        /// <inheritdoc/>
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

    /// <summary>
    /// 表示访问设备语音功能的权限类，继承自 <see cref="BasePlatformPermission"/> 和 <see cref="Permissions2.Speech"/> 类
    /// </summary>
    public partial class Speech : BasePlatformPermission, Permissions2.Speech
    {
#if ANDROID
        /// <inheritdoc/>
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
        /// <summary>
        /// 获取语音权限的状态
        /// </summary>
        /// <returns>权限状态</returns>
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

        /// <summary>
        /// 请求语音权限
        /// </summary>
        /// <returns>权限状态</returns>
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

    /// <summary>
    /// 表示读取设备存储的权限，继承自 <see cref="BasePlatformPermission"/> 和 <see cref="Permissions2.StorageRead"/> 类
    /// </summary>
    public partial class StorageRead : BasePlatformPermission, Permissions2.StorageRead
    {
#if ANDROID
        /// <inheritdoc/>
        public override (string androidPermission, bool isRuntime)[] RequiredPermissions =>
            new (string, bool)[] { (Manifest.Permission.ReadExternalStorage, true) };
#endif
    }

    /// <summary>
    /// 表示对设备存储的写入权限，继承自 <see cref="BasePlatformPermission"/> 和 <see cref="Permissions2.StorageWrite"/> 类
    /// </summary>
    public partial class StorageWrite : BasePlatformPermission, Permissions2.StorageWrite
    {
#if ANDROID
        /// <inheritdoc/>
        public override (string androidPermission, bool isRuntime)[] RequiredPermissions =>
            new (string, bool)[] { (Manifest.Permission.WriteExternalStorage, true) };
#endif
    }

    /// <summary>
    /// 表示访问设备震动的权限，，继承自 <see cref="BasePlatformPermission"/> 和 <see cref="Permissions2.Vibrate"/> 类
    /// </summary>
    public partial class Vibrate : BasePlatformPermission, Permissions2.Vibrate
    {
#if ANDROID
        /// <inheritdoc/>
        public override (string androidPermission, bool isRuntime)[] RequiredPermissions =>
            new (string, bool)[] { (Manifest.Permission.Vibrate, false) };
#endif
    }
}

#if ANDROID
using Android.Content.Res;
using Android.OS;
using Android.Provider;
#elif WINDOWS
using Windows.Security.ExchangeActiveSyncProvisioning;
using Windows.System.Profile;
#elif IOS
using ObjCRuntime;
#endif

namespace BD.Common8.Essentials.Services.Implementation;

/// <summary>
/// https://github.com/dotnet/maui/tree/8.0.0-rc.2.9373/src/Essentials/src/DeviceInfo
/// </summary>
public partial class DeviceInfoPlatformServiceImpl : IDeviceInfoPlatformService
{
#if WINDOWS
    readonly EasClientDeviceInformation deviceInfo;
    DeviceIdiom currentIdiom = DeviceIdiom.Unknown;
    DeviceType currentType = DeviceType.Unknown;
    string? systemProductName;

    static readonly int SM_CONVERTIBLESLATEMODE = 0x2003;
    static readonly int SM_TABLETPC = 0x56;

    [LibraryImport("user32.dll", SetLastError = true)]
    private static partial int GetSystemMetrics(int nIndex);

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    static bool GetIsInTabletMode()
    {
        var supportsTablet = GetSystemMetrics(SM_TABLETPC) != 0;
        var inTabletMode = GetSystemMetrics(SM_CONVERTIBLESLATEMODE) == 0;
        return inTabletMode && supportsTablet;
    }
#elif MACOS
    [LibraryImport(ObjCRuntime.Constants.SystemConfigurationLibrary)]
    private static partial nint SCDynamicStoreCopyComputerName(nint store, nint encoding);
#endif

    /// <summary>
    /// 初始化 <see cref="DeviceInfoPlatformServiceImpl"/> 类的新实例
    /// </summary>
    public DeviceInfoPlatformServiceImpl()
    {
#if WINDOWS
        deviceInfo = new EasClientDeviceInformation();
        try
        {
            systemProductName = deviceInfo.SystemProductName;
        }
        catch (Exception ex)
        {
            Debug.WriteLine($"Unable to get system product name. {ex.Message}");
        }
#endif
    }

    /// <inheritdoc/>
    public virtual string Model
    {
        get
        {
#if ANDROID
            return Build.Model ?? string.Empty;
#elif WINDOWS
            return deviceInfo.SystemProductName ?? string.Empty;
#elif MACOS
            return IOKit.GetPlatformExpertPropertyValue<NSData>("model")?.ToString() ?? string.Empty;
#elif IOS || MACCATALYST || __WATCHOS__ || __TVOS__
            try
            {
                var r = PlatformUtils.GetSystemLibraryProperty("hw.machine");
                return r ?? string.Empty;
            }
            catch (Exception)
            {
                Debug.WriteLine("Unable to query hardware model, returning current device model.");
            }
            return UIDevice.CurrentDevice.Model ?? string.Empty;
#else
            return string.Empty;
#endif
        }
    }

    /// <inheritdoc/>
    public virtual string Manufacturer
    {
        get
        {
#if ANDROID
            return Build.Manufacturer ?? string.Empty;
#elif WINDOWS
            return deviceInfo.SystemManufacturer ?? string.Empty;
#elif IOS || MACCATALYST || MACOS
            return "Apple";
#else
            return string.Empty;
#endif
        }
    }

    /// <inheritdoc/>
    public virtual string Name
    {
        get
        {
#if ANDROID
            // DEVICE_NAME added in System.Global in API level 25
            // https://developer.android.com/reference/android/provider/Settings.Global#DEVICE_NAME
            var name = GetSystemSetting("device_name", true);
            if (string.IsNullOrWhiteSpace(name))
                name = Model;
            return name!;
#elif WINDOWS
            return deviceInfo.FriendlyName ?? string.Empty;
#elif MACOS
            var computerNameHandle = SCDynamicStoreCopyComputerName(IntPtr.Zero, IntPtr.Zero);

            if (computerNameHandle == IntPtr.Zero)
                return string.Empty;

            try
            {
#pragma warning disable CS0618 // Type or member is obsolete
                string nsstr = NSString.FromHandle(computerNameHandle);
#pragma warning restore CS0618 // Type or member is obsolete
                return nsstr ?? string.Empty;
            }
            finally
            {
                IOKit.CFRelease(computerNameHandle);
            }
#elif IOS || MACCATALYST || __WATCHOS__ || __TVOS__
            return UIDevice.CurrentDevice.Name ?? string.Empty;
#else
            return string.Empty;
#endif
        }
    }

    /// <inheritdoc/>
    public virtual string VersionString
    {
        get
        {
#if ANDROID
            return Build.VERSION.Release ?? string.Empty;
#elif WINDOWS
            var version = AnalyticsInfo.VersionInfo.DeviceFamilyVersion;
            if (ulong.TryParse(version, out var v))
            {
                var v1 = (v & 0xFFFF000000000000L) >> 48;
                var v2 = (v & 0x0000FFFF00000000L) >> 32;
                var v3 = (v & 0x00000000FFFF0000L) >> 16;
                var v4 = v & 0x000000000000FFFFL;
                return $"{v1}.{v2}.{v3}.{v4}";
            }
            return Environment.OSVersion.Version.ToString() ?? string.Empty;
#elif MACOS
            using var info = new NSProcessInfo();
            return info.OperatingSystemVersion.ToString() ?? string.Empty;
#elif IOS || MACCATALYST || __WATCHOS__
            return UIDevice.CurrentDevice.SystemVersion ?? string.Empty;
#else
            return string.Empty;
#endif
        }
    }

    /// <inheritdoc/>
    public virtual DeviceType DeviceType
    {
        get
        {
#if ANDROID
            if (IsEmulator)
            {
                return DeviceType.Virtual;
            }
            return DeviceType.Physical;
#elif WINDOWS
            if (currentType != DeviceType.Unknown)
                return currentType;

            try
            {
                if (string.IsNullOrWhiteSpace(systemProductName))
                    systemProductName = deviceInfo.SystemProductName;

                var isVirtual = systemProductName.Contains("Virtual", StringComparison.Ordinal) || systemProductName == "HMV domU";

                currentType = isVirtual ? DeviceType.Virtual : DeviceType.Physical;
            }
            catch (Exception ex)
            {
                Debug.WriteLine($"Unable to get device type. {ex.Message}");
            }
            return currentType;
#elif MACOS || MACCATALYST
            return DeviceType.Physical;
#elif IOS
            return Runtime.Arch == Arch.DEVICE ? DeviceType.Physical : DeviceType.Virtual;
#else
            return DeviceType.Unknown;
#endif
        }
    }

    /// <inheritdoc/>
    public virtual DeviceIdiom Idiom
    {
        get
        {
#if ANDROID
            const int tabletCrossover = 600;
            var currentIdiom = DeviceIdiom.Unknown;

            // first try UIModeManager
            using var uiModeManager = UiModeManager.FromContext(Application.Context);

            try
            {
                var uiMode = uiModeManager?.CurrentModeType ?? UiMode.TypeUndefined;
                currentIdiom = DetectIdiom(uiMode);
                static DeviceIdiom DetectIdiom(UiMode uiMode)
                {
                    if (uiMode == UiMode.TypeNormal)
                        return DeviceIdiom.Unknown;
                    else if (uiMode == UiMode.TypeTelevision)
                        return DeviceIdiom.TV;
                    else if (uiMode == UiMode.TypeDesk)
                        return DeviceIdiom.Desktop;
                    else if (uiMode == UiMode.TypeWatch)
                        return DeviceIdiom.Watch;

                    return DeviceIdiom.Unknown;
                }
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"Unable to detect using UiModeManager: {ex.Message}");
            }

            // then try Configuration
            if (currentIdiom == DeviceIdiom.Unknown)
            {
                var configuration = Application.Context.Resources?.Configuration;
                if (configuration != null)
                {
                    var minWidth = configuration.SmallestScreenWidthDp;
                    var isWide = minWidth >= tabletCrossover;
                    currentIdiom = isWide ? DeviceIdiom.Tablet : DeviceIdiom.Phone;
                }
                else
                {
                    // start clutching at straws
                    using var metrics = Application.Context.Resources?.DisplayMetrics;
                    if (metrics != null)
                    {
                        var minSize = Math.Min(metrics.WidthPixels, metrics.HeightPixels);
                        var isWide = minSize * metrics.Density >= tabletCrossover;
                        currentIdiom = isWide ? DeviceIdiom.Tablet : DeviceIdiom.Phone;
                    }
                }
            }

            // hope we got it somewhere
            return currentIdiom;

#elif WINDOWS
            switch (AnalyticsInfo.VersionInfo.DeviceFamily)
            {
                case "Windows.Mobile":
                    currentIdiom = DeviceIdiom.Phone;
                    break;
                case "Windows.Universal":
                case "Windows.Desktop":
                    currentIdiom = GetIsInTabletMode()
                        ? DeviceIdiom.Tablet
                        : DeviceIdiom.Desktop;
                    break;
                case "Windows.Xbox":
                case "Windows.Team":
                    currentIdiom = DeviceIdiom.TV;
                    break;
                case "Windows.IoT":
                default:
                    currentIdiom = DeviceIdiom.Unknown;
                    break;
            }

            return currentIdiom;
#elif MACOS || MACCATALYST
            return DeviceIdiom.Desktop;
#elif IOS
            switch (UIDevice.CurrentDevice.UserInterfaceIdiom)
            {
                case UIUserInterfaceIdiom.Pad:
                    return DeviceIdiom.Tablet;
                case UIUserInterfaceIdiom.Phone:
                    return DeviceIdiom.Phone;
                case UIUserInterfaceIdiom.TV:
                    return DeviceIdiom.TV;
                case UIUserInterfaceIdiom.CarPlay:
                case UIUserInterfaceIdiom.Unspecified:
                default:
                    return DeviceIdiom.Unknown;
            }
#elif __WATCHOS__
			return DeviceIdiom.Watch;
#elif __TVOS__
			return DeviceIdiom.TV;
#else
            return DeviceIdiom.Unknown;
#endif
        }
    }

    /// <inheritdoc/>
    public virtual bool IsChromeOS
    {
        get
        {
#if ANDROID
            return mIsChromeOS.Value;
#else
            return false;
#endif
        }
    }

#if ANDROID
    static int rating = -1;
    static readonly Lazy<bool> mIsEmulator = new(GetIsEmulator);

    static bool GetIsEmulator()
    {
        // 参考 https://github.com/gingo/android-emulator-detector/blob/master/EmulatorDetectorProject/EmulatorDetector/src/main/java/net/skoumal/emulatordetector/EmulatorDetector.java
        return _();
#pragma warning disable SA1312 // Variable names should begin with lower-case letter
#pragma warning disable SA1300 // Element should begin with upper-case letter
        static bool contains(string l, string r)
        {
            return l.Contains(r, StringComparison.OrdinalIgnoreCase);
        }
        static bool equals(string l, string r) => string.Equals(l, r, StringComparison.OrdinalIgnoreCase);
        static bool _()
        {
            var newRating = 0;
            if (rating < 0)
            {
                var PRODUCT = Build.Product;
                if (PRODUCT == null ||
                    contains(PRODUCT, "sdk") ||
                    contains(PRODUCT, "Andy") ||
                    contains(PRODUCT, "ttVM_Hdragon") ||
                    contains(PRODUCT, "google_sdk") ||
                    contains(PRODUCT, "Droid4X") ||
                    contains(PRODUCT, "nox") ||
                    contains(PRODUCT, "sdk_x86") ||
                    contains(PRODUCT, "sdk_google") ||
                    contains(PRODUCT, "vbox86p") ||
                    contains(PRODUCT, "emulator") ||
                    contains(PRODUCT, "simulator"))
                {
                    newRating++;
                }

                var MANUFACTURER = Build.Manufacturer;
                if (MANUFACTURER == null ||
                    equals(MANUFACTURER, "unknown") ||
                    equals(MANUFACTURER, "Genymotion") ||
                    contains(MANUFACTURER, "VS Emulator") ||
                    contains(MANUFACTURER, "Andy") ||
                    contains(MANUFACTURER, "MIT") ||
                    contains(MANUFACTURER, "nox") ||
                    contains(MANUFACTURER, "TiantianVM"))
                {
                    newRating++;
                }

                var BRAND = Build.Brand;
                if (BRAND == null ||
                    equals(BRAND, "generic") ||
                    equals(BRAND, "generic_x86") ||
                    equals(BRAND, "TTVM") ||
                    contains(BRAND, "Andy"))
                {
                    newRating++;
                }

                var DEVICE = Build.Device;
                if (DEVICE == null ||
                    contains(DEVICE, "generic") ||
                    contains(DEVICE, "generic_x86") ||
                    contains(DEVICE, "Andy") ||
                    contains(DEVICE, "ttVM_Hdragon") ||
                    contains(DEVICE, "Droid4X") ||
                    contains(DEVICE, "nox") ||
                    contains(DEVICE, "generic_x86_64") ||
                    contains(DEVICE, "vbox86p"))
                {
                    newRating++;
                }

                var MODEL = Build.Model;
                if (MODEL == null ||
                    equals(MODEL, "sdk") ||
                    equals(MODEL, "google_sdk") ||
                    contains(MODEL, "Droid4X") ||
                    contains(MODEL, "TiantianVM") ||
                    contains(MODEL, "Andy") ||
                    equals(MODEL, "Android SDK built for x86_64") ||
                    equals(MODEL, "Android SDK built for x86") ||
                    equals(MODEL, "Emulator"))
                {
                    newRating++;
                }

                var HARDWARE = Build.Hardware;
                if (HARDWARE == null ||
                    equals(HARDWARE, "goldfish") ||
                    equals(HARDWARE, "vbox86") ||
                    contains(HARDWARE, "nox") ||
                    contains(HARDWARE, "ttVM_x86") ||
                    contains(HARDWARE, "ranchu"))
                {
                    newRating++;
                }

                var FINGERPRINT = Build.Fingerprint;
                if (FINGERPRINT == null ||
                    contains(FINGERPRINT, "generic/sdk/generic") ||
                    contains(FINGERPRINT, "generic_x86/sdk_x86/generic_x86") ||
                    contains(FINGERPRINT, "Andy") ||
                    contains(FINGERPRINT, "ttVM_Hdragon") ||
                    contains(FINGERPRINT, "generic_x86_64") ||
                    contains(FINGERPRINT, "generic/google_sdk/generic") ||
                    contains(FINGERPRINT, "vbox86p") ||
                    contains(FINGERPRINT, "generic/vbox86p/vbox86p"))
                {
                    newRating++;
                }

                /* signal 11 (SIGSEGV), code 1 (SEGV_MAPERR), fault addr 0xdd0
                 * Cause: null pointer dereference
                 * backtrace:
                 * #00 pc 0000000000005ff4  /system/lib64/libGLESv1_CM.so (glGetString+52)
                 */
                //                    try
                //                    {
                //                        var opengl = Android.Opengl.GLES20.GlGetString(Android.Opengl.GLES20.GlRenderer);
                //                        if (!string.IsNullOrWhiteSpace(opengl))
                //                        {
                //#pragma warning disable CS8604 // 可能的 null 引用参数。
                //                            if (contains(opengl, "Bluestacks") ||
                //                                contains(opengl, "Translator") ||
                //                                contains(opengl, "youwave")
                //                            )
                //#pragma warning restore CS8604 // 可能的 null 引用参数。
                //                                newRating += 10;
                //                        }
                //                    }
                //                    catch
                //                    {
                //                    }

                try
                {
                    var esDir = Android.OS.Environment.ExternalStorageDirectory;
                    if (esDir != null)
                    {
                        var path = Path.Combine(esDir.ToString(), "windows", "BstSharedFolder");
                        if (File.Exists(path))
                        {
                            newRating += 10;
                        }
                    }
                }
                catch
                {
                }
                rating = newRating;
            }
            return rating > 3;
        }
#pragma warning restore SA1300 // Element should begin with upper-case letter
#pragma warning restore SA1312 // Variable names should begin with lower-case letter
    }

    /// <summary>
    /// 当前设备是否为模拟器
    /// </summary>
    public static bool IsEmulator => mIsEmulator.Value;

    static bool GetIsChromeOS()
    {
        // https://stackoverflow.com/questions/51489132/how-to-find-if-android-app-is-started-by-androidos-or-chromeoss-android-contain
        // https://github.com/maxim-saplin/xOPS-App/blob/1.1.2/Saplin.xOPS.Android/DeviceInfo.cs#L75
        try
        {
            return Android.App.Application.Context.PackageManager!.HasSystemFeature("org.chromium.arc.device_management");
        }
        catch
        {
            return false;
        }
    }

    static readonly Lazy<bool> mIsChromeOS = new(GetIsChromeOS);

    static string? GetSystemSetting(string name, bool isGlobal = false)
    {
        if (isGlobal && OperatingSystem.IsAndroidVersionAtLeast(25))
            return Settings.Global.GetString(Application.Context.ContentResolver, name);
        else
            return Settings.System.GetString(Application.Context.ContentResolver, name);
    }
#endif

    /// <inheritdoc/>
    public DevicePlatform2 Platform
    {
        get
        {
#if ANDROID
            var idiom = Idiom;
            return idiom switch
            {
                DeviceIdiom.Phone => DevicePlatform2.AndroidPhone,
                DeviceIdiom.Tablet => DevicePlatform2.AndroidTablet,
                DeviceIdiom.Desktop => DevicePlatform2.AndroidDesktop,
                DeviceIdiom.TV => DevicePlatform2.AndroidTV,
                DeviceIdiom.Watch => DevicePlatform2.AndroidWatch,
                _ => DevicePlatform2.AndroidUnknown,
            };
#elif WINDOWS
            return DevicePlatform2.Windows;
#elif IOS
            return DevicePlatform2.iOS;
#elif MACOS
            return DevicePlatform2.macOS;
#elif __WATCHOS__
            return DevicePlatform2.watchOS;
#elif __TVOS__
            return DevicePlatform2.tvOS;
#elif MACCATALYST
            if (OperatingSystem.IsIOS())
            {
                var idiom = Idiom;
                return idiom switch
                {
                    DeviceIdiom.Phone => DevicePlatform2.iOS,
                    _ => DevicePlatform2.iPadOS,
                };
            }
            else
            {
                return DevicePlatform2.macOS;
            }
#else
            if (OperatingSystem.IsWindows())
            {
                return DevicePlatform2.Windows;
            }
            else if (OperatingSystem.IsAndroid())
            {
                var idiom = Idiom;
                return idiom switch
                {
                    DeviceIdiom.Phone => DevicePlatform2.AndroidPhone,
                    DeviceIdiom.Tablet => DevicePlatform2.AndroidTablet,
                    DeviceIdiom.Desktop => DevicePlatform2.AndroidDesktop,
                    DeviceIdiom.TV => DevicePlatform2.AndroidTV,
                    DeviceIdiom.Watch => DevicePlatform2.AndroidWatch,
                    _ => DevicePlatform2.AndroidUnknown,
                };
            }
            else if (OperatingSystem.IsIOS())
            {
                var idiom = Idiom;
                return idiom switch
                {
                    DeviceIdiom.Phone => DevicePlatform2.iOS,
                    _ => DevicePlatform2.iPadOS,
                };
            }
            else if (OperatingSystem.IsMacOS())
            {
                return DevicePlatform2.macOS;
            }
            else if (OperatingSystem.IsTvOS())
            {
                return DevicePlatform2.tvOS;
            }
            else if (OperatingSystem.IsWatchOS())
            {
                return DevicePlatform2.watchOS;
            }
            else if (OperatingSystem.IsLinux())
            {
                return DevicePlatform2.Linux;
            }
            return default;
#endif
        }
    }
}

#if ANDROID
using Android.OS;
#endif

namespace BD.Common.Services.Implementation.Essentials;

public abstract class DeviceInfoPlatformServiceImpl : IDeviceInfoPlatformService
{
    public virtual string Model => DeviceInfo.Model ?? "";

    public virtual string Manufacturer => DeviceInfo.Manufacturer;

    public virtual string Name => DeviceInfo.Name ?? "";

    public virtual string VersionString => DeviceInfo.VersionString;

    public virtual DeviceType DeviceType => DeviceInfo.DeviceType.Convert();

    public virtual bool IsChromeOS => false;

#pragma warning disable IDE0079 // 请删除不必要的忽略
#pragma warning disable CS0618 // 类型或成员已过时
    public virtual bool IsUWP => DeviceInfo.Platform == DevicePlatform.UWP;
#pragma warning restore CS0618 // 类型或成员已过时
#pragma warning restore IDE0079 // 请删除不必要的忽略

    public virtual bool IsWinUI =>
#if MAUI
        DeviceInfo.Platform == DevicePlatform.WinUI;
#else
        false;
#endif

    public virtual DeviceIdiom Idiom => DeviceInfo.Idiom.Convert();
}

#if ANDROID
public sealed class AndroidDeviceInfoPlatformServiceImpl : DeviceInfoPlatformServiceImpl
{
    static int rating = -1;
    static readonly Lazy<bool> mIsEmulator = new(GetIsEmulator);

    static bool GetIsEmulator()
    {
        // 参考 https://github.com/gingo/android-emulator-detector/blob/master/EmulatorDetectorProject/EmulatorDetector/src/main/java/net/skoumal/emulatordetector/EmulatorDetector.java
        return DeviceInfo.DeviceType == XEDeviceType.Virtual || _();
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
                    contains(PRODUCT, "vbox86p"))
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
                    equals(MODEL, "Android SDK built for x86"))
                {
                    newRating++;
                }

                var HARDWARE = Build.Hardware;
                if (HARDWARE == null ||
                    equals(HARDWARE, "goldfish") ||
                    equals(HARDWARE, "vbox86") ||
                    contains(HARDWARE, "nox") ||
                    contains(HARDWARE, "ttVM_x86"))
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
#pragma warning disable CS0618 // 类型或成员已过时
                    var esDir = Android.OS.Environment.ExternalStorageDirectory;
#pragma warning restore CS0618 // 类型或成员已过时
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

    public override DeviceType DeviceType
    {
        get
        {
            if (IsEmulator)
            {
                return DeviceType.Virtual;
            }
            return base.DeviceType;
        }
    }

    public override bool IsChromeOS => mIsChromeOS.Value;

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
}
#else
public sealed class DefaultDeviceInfoPlatformServiceImpl : DeviceInfoPlatformServiceImpl
{

}
#endif
#if ANDROID
using Android.Content;
using Android.Content.PM;
using Application = Android.App.Application;
using Context = Android.Content.Context;

namespace BD.Common8.Essentials.Helpers;

#pragma warning disable SA1600 // Elements should be documented

/// <summary>
/// https://github.com/dotnet/maui/blob/8.0.0-rc.2.9373/src/Essentials/src/Platform/PlatformUtils.android.cs
/// </summary>
static partial class PlatformUtils
{
    [SupportedOSPlatform("android")]
    public const int requestCodeFilePicker = 11001;
    [SupportedOSPlatform("android")]
    public const int requestCodeMediaPicker = 11002;
    [SupportedOSPlatform("android")]
    public const int requestCodeMediaCapture = 11003;
    [SupportedOSPlatform("android")]
    public const int requestCodePickContact = 11004;

    [SupportedOSPlatform("android")]
    public const int requestCodeStart = 12000;

    static int requestCode = requestCodeStart;

    [SupportedOSPlatform("android")]
    public static int NextRequestCode()
    {
        if (++requestCode >= 12999)
            requestCode = requestCodeStart;

        return requestCode;
    }

    [SupportedOSPlatform("android")]
    public static bool IsIntentSupported(Intent intent)
    {
        if (Application.Context is not Context ctx || ctx.PackageManager is not PackageManager pm)
            return false;

        return intent.ResolveActivity(pm) is not null;
    }

    [SupportedOSPlatform("android")]
    public static bool IsIntentSupported(Intent intent, string expectedPackageName)
    {
        if (Application.Context is not Context ctx || ctx.PackageManager is not PackageManager pm)
            return false;

        return intent.ResolveActivity(pm) is ComponentName c && c.PackageName == expectedPackageName;
    }
}
#endif
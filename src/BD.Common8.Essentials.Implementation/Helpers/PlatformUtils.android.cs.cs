#if ANDROID
using Android.Content;
using Android.Content.PM;
using Application = Android.App.Application;
using Context = Android.Content.Context;

namespace BD.Common8.Essentials.Helpers;

/// <summary>
/// 提供用于处理平台相关功能
/// <para>https://github.com/dotnet/maui/blob/8.0.0-rc.2.9373/src/Essentials/src/Platform/PlatformUtils.android.cs</para>
/// </summary>
static partial class PlatformUtils
{
    /// <summary>
    /// 文件选择器请求代码
    /// </summary>
    [SupportedOSPlatform("android")]
    public const int requestCodeFilePicker = 11001;

    /// <summary>
    /// 媒体选择器请求代码
    /// </summary>
    [SupportedOSPlatform("android")]
    public const int requestCodeMediaPicker = 11002;

    /// <summary>
    /// 媒体捕获请求代码
    /// </summary>
    [SupportedOSPlatform("android")]
    public const int requestCodeMediaCapture = 11003;

    /// <summary>
    /// 选择联系人请求代码
    /// </summary>
    [SupportedOSPlatform("android")]
    public const int requestCodePickContact = 11004;

    /// <summary>
    /// 请求代码的初始值
    /// </summary>
    [SupportedOSPlatform("android")]
    public const int requestCodeStart = 12000;

    static int requestCode = requestCodeStart;

    /// <summary>
    /// 获取下一个请求代码
    /// </summary>
    [SupportedOSPlatform("android")]
    public static int NextRequestCode()
    {
        if (++requestCode >= 12999)
            requestCode = requestCodeStart;

        return requestCode;
    }

    /// <summary>
    /// 检查是否支持指定的 Intent，支持返回 <see langword="true"/>；否则为 <see langword="false"/>
    /// </summary>
    [SupportedOSPlatform("android")]
    public static bool IsIntentSupported(Intent intent)
    {
        if (Application.Context is not Context ctx || ctx.PackageManager is not PackageManager pm)
            return false;

        return intent.ResolveActivity(pm) is not null;
    }

    /// <summary>
    /// 检查是否支持指定的 Intent 以及 应用程序包
    /// </summary>
    [SupportedOSPlatform("android")]
    public static bool IsIntentSupported(Intent intent, string expectedPackageName)
    {
        if (Application.Context is not Context ctx || ctx.PackageManager is not PackageManager pm)
            return false;

        return intent.ResolveActivity(pm) is ComponentName c && c.PackageName == expectedPackageName;
    }
}
#endif
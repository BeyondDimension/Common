using EDeviceType = System.Runtime.Devices.DeviceType;
using EPlatform = System.Runtime.Devices.Platform;

namespace BD.Common;

public static class DeviceInfo2
{
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static string Model() => IDeviceInfoPlatformService.Interface?.Model ?? string.Empty;

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static string Manufacturer() => IDeviceInfoPlatformService.Interface?.Manufacturer ?? string.Empty;

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static string Name() => IDeviceInfoPlatformService.Interface?.Name ?? string.Empty;

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static string VersionString() => IDeviceInfoPlatformService.Interface?.VersionString ?? string.Empty;

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static EPlatform Platform() => IDeviceInfoPlatformService.Platform;

    public static DeviceIdiom Idiom()
    {
        var i = IDeviceInfoPlatformService.Interface;
        if (i != null)
        {
            var value = i.Idiom;
            if (value != DeviceIdiom.Unknown) return value;
        }
        return DeviceIdiom.Unknown;
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static string OSName() => OSNameValue().ToDisplayName();

    static readonly Lazy<ClientOSPlatform> _OSNameValue = new(() =>
    {
        if (OperatingSystem.IsWindows())
        {
            var i = IDeviceInfoPlatformService.Interface;
            if (i != null && i.IsUWP)
            {
                if (i.IsWinUI) return ClientOSPlatform.WinUI;
                if (i.IsUWP) return ClientOSPlatform.UWP;
            }
            if (DesktopBridge.IsRunningAsUwp)
            {
                return ClientOSPlatform.WindowsDesktopBridge;
            }
            else
            {
                return ClientOSPlatform.Windows;
            }
        }
        else if (OperatingSystem.IsAndroid())
        {
            if (OperatingSystem2.IsRunningOnWSA())
            {
                return ClientOSPlatform.WSA;
            }
            var i = IDeviceInfoPlatformService.Interface;
            if (i != null && i.IsChromeOS)
            {
                return ClientOSPlatform.ChromeOS;
            }
            else
            {
                if (DeviceType() == EDeviceType.Virtual)
                {
                    return ClientOSPlatform.AndroidVirtual;
                }
                return Idiom() switch
                {
                    DeviceIdiom.Phone => ClientOSPlatform.AndroidPhone,
                    DeviceIdiom.Tablet => ClientOSPlatform.AndroidTablet,
                    DeviceIdiom.Desktop => ClientOSPlatform.AndroidDesktop,
                    DeviceIdiom.TV => ClientOSPlatform.AndroidTV,
                    DeviceIdiom.Watch => ClientOSPlatform.AndroidWatch,
                    _ => ClientOSPlatform.AndroidUnknown,
                };
            }
        }
        else if (OperatingSystem.IsIOS())
        {
            if (Idiom() == DeviceIdiom.Tablet)
                return ClientOSPlatform.iPadOS;
            return ClientOSPlatform.iOS;
        }
        else if (OperatingSystem.IsMacOS())
        {
            return ClientOSPlatform.macOS;
        }
        else if (OperatingSystem.IsTvOS())
        {
            return ClientOSPlatform.tvOS;
        }
        else if (OperatingSystem.IsWatchOS())
        {
            return ClientOSPlatform.watchOS;
        }
        else if (OperatingSystem.IsLinux())
        {
            return ClientOSPlatform.Linux;
        }
        return default;
    });

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static ClientOSPlatform OSNameValue() => _OSNameValue.Value;

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static EDeviceType DeviceType()
    {
        var i = IDeviceInfoPlatformService.Interface;
        if (i != null) return i.DeviceType;
        return default;
    }
}
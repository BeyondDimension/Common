#if !LINUX
namespace BD.Common8.Essentials.Services.Implementation;

partial class DeviceInfoPlatformServiceImpl
{
    /// <inheritdoc/>
    public virtual partial string Model
    {
        get
        {
#if ANDROID
            var value = global::Android.OS.Build.Model;
            return value ?? string.Empty;
#else
            var value = global::Microsoft.Maui.Devices.DeviceInfo.Model;
            return value;
#endif
        }
    }

    /// <inheritdoc/>
    public virtual partial string Manufacturer
    {
        get
        {
#if ANDROID
            var value = global::Android.OS.Build.Manufacturer;
            return value ?? string.Empty;
#elif IOS || MACCATALYST || MACOS || __WATCHOS__ || __TVOS__
            return "Apple";
#else
            var value = global::Microsoft.Maui.Devices.DeviceInfo.Manufacturer;
            return value;
#endif
        }
    }

    /// <inheritdoc/>
    public virtual partial string Name
    {
        get
        {
#if IOS || MACCATALYST || __WATCHOS__ || __TVOS__
            var value = global::UIKit.UIDevice.CurrentDevice.Name;
            return value ?? string.Empty;
#else
            var value = global::Microsoft.Maui.Devices.DeviceInfo.Name;
            return value;
#endif
        }
    }

    /// <inheritdoc/>
    public virtual partial string VersionString
    {
        get
        {
#if ANDROID
            var value = global::Android.OS.Build.VERSION.Release;
            return value ?? string.Empty;
#elif WINDOWS
            var version = global::Windows.System.Profile.AnalyticsInfo.VersionInfo.DeviceFamilyVersion;
            if (ulong.TryParse(version, out var v))
            {
                var v1 = (v & 0xFFFF000000000000L) >> 48;
                var v2 = (v & 0x0000FFFF00000000L) >> 32;
                var v3 = (v & 0x00000000FFFF0000L) >> 16;
                var v4 = v & 0x000000000000FFFFL;
                return $"{v1}.{v2}.{v3}.{v4}";
            }
            else
            {
                var value = Environment.OSVersion.Version.ToString();
                return value ?? string.Empty;
            }
#else
            var value = global::Microsoft.Maui.Devices.DeviceInfo.VersionString;
            return value;
#endif
        }
    }

    /// <inheritdoc/>
    public virtual partial global::BD.Common8.Essentials.Enums.DeviceType DeviceType
    {
        get
        {
#if ANDROID
            var value = IsEmulator ?
                global::BD.Common8.Essentials.Enums.DeviceType.Virtual :
                global::BD.Common8.Essentials.Enums.DeviceType.Physical;
            return value;
#else
            var value = global::Microsoft.Maui.Devices.DeviceInfo.DeviceType;
            return unchecked((global::BD.Common8.Essentials.Enums.DeviceType)value);
#endif
        }
    }

    static global::BD.Common8.Essentials.Enums.DeviceIdiom Convert(global::Microsoft.Maui.Devices.DeviceIdiom value)
    {
        // struct => enum
        if (value == global::Microsoft.Maui.Devices.DeviceIdiom.Phone)
            return global::BD.Common8.Essentials.Enums.DeviceIdiom.Phone;
        else if (value == global::Microsoft.Maui.Devices.DeviceIdiom.Tablet)
            return global::BD.Common8.Essentials.Enums.DeviceIdiom.Tablet;
        else if (value == global::Microsoft.Maui.Devices.DeviceIdiom.Desktop)
            return global::BD.Common8.Essentials.Enums.DeviceIdiom.Desktop;
        else if (value == global::Microsoft.Maui.Devices.DeviceIdiom.TV)
            return global::BD.Common8.Essentials.Enums.DeviceIdiom.TV;
        else if (value == global::Microsoft.Maui.Devices.DeviceIdiom.Watch)
            return global::BD.Common8.Essentials.Enums.DeviceIdiom.Watch;
        else if (value == global::Microsoft.Maui.Devices.DeviceIdiom.Unknown)
            return global::BD.Common8.Essentials.Enums.DeviceIdiom.Unknown;
        return default;
    }

    /// <inheritdoc/>
    public virtual partial global::BD.Common8.Essentials.Enums.DeviceIdiom Idiom
    {
        get
        {
            var value = global::Microsoft.Maui.Devices.DeviceInfo.Idiom;
            return Convert(value);
        }
    }

    /// <inheritdoc/>
    public virtual partial bool IsChromeOS
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
}
#endif
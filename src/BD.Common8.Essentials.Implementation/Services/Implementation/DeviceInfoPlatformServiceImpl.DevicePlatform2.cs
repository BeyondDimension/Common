using BD.Common8.Essentials.Enums;

namespace BD.Common8.Essentials.Services.Implementation;

partial class DeviceInfoPlatformServiceImpl
{
    /// <inheritdoc/>
    public DevicePlatform2 Platform
    {
        get
        {
#if ANDROID
            if (OSHelper.IsRunningOnWSA())
            {
                return DevicePlatform2.WSA;
            }
            else if (IsChromeOS)
            {
                return DevicePlatform2.ChromeOS;
            }
            else if (DeviceType == DeviceType.Virtual)
            {
                return DevicePlatform2.AndroidVirtual;
            }
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
            if (OSHelper.IsPublishToStore)
            {
                return DevicePlatform2.WindowsDesktopBridge;
            }
            else
            {
                return DevicePlatform2.Windows;
            }
#elif IOS
            var idiom = Idiom;
            return idiom switch
            {
                DeviceIdiom.Phone => DevicePlatform2.iOS,
                _ => DevicePlatform2.iPadOS,
            };
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
#elif LINUX
            return DevicePlatform2.Linux;
#else
            if (OperatingSystem.IsWindows())
            {
                if (OSHelper.IsPublishToStore)
                {
                    return DevicePlatform2.WindowsDesktopBridge;
                }
                else
                {
                    return DevicePlatform2.Windows;
                }
            }
            else if (OperatingSystem.IsAndroid())
            {
                if (OSHelper.IsRunningOnWSA())
                {
                    return DevicePlatform2.WSA;
                }
                else if (IsChromeOS)
                {
                    return DevicePlatform2.ChromeOS;
                }
                else if (DeviceType == DeviceType.Virtual)
                {
                    return DevicePlatform2.AndroidVirtual;
                }
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

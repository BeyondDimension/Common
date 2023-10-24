#if MACOS

using CoreFoundation;
using ObjCRuntime;

namespace BD.Common8.Essentials.Helpers;

#pragma warning disable SA1600 // Elements should be documented

/// <summary>
/// https://github.com/dotnet/maui/blob/8.0.0-rc.2.9373/src/Essentials/src/Platform/PlatformUtils.macos.cs
/// </summary>
[SupportedOSPlatform("macos")]
static partial class IOKit
{
    const string IOKitLibrary = "/System/Library/Frameworks/IOKit.framework/IOKit";
    const string IOPlatformExpertDeviceClassName = "IOPlatformExpertDevice";

    const uint kIOPMAssertionLevelOff = 0;
    const uint kIOPMAssertionLevelOn = 255;

    const string kIOPMACPowerKey = "AC Power";
    const string kIOPMUPSPowerKey = "UPS Power";
    const string kIOPMBatteryPowerKey = "Battery Power";

    const string kIOPSCurrentCapacityKey = "Current Capacity";
    const string kIOPSMaxCapacityKey = "Max Capacity";
    const string kIOPSTypeKey = "Type";
    const string kIOPSInternalBatteryType = "InternalBattery";
    const string kIOPSIsPresentKey = "Is Present";
    const string kIOPSIsChargingKey = "Is Charging";
    const string kIOPSIsChargedKey = "Is Charged";
    const string kIOPSIsFinishingChargeKey = "Is Finishing Charge";

    static readonly CFString kIOPMAssertionTypePreventUserIdleDisplaySleep = "PreventUserIdleDisplaySleep";

    [LibraryImport(IOKitLibrary)]
    public static partial nint IOPSCopyPowerSourcesInfo();

    [LibraryImport(IOKitLibrary)]
    public static partial nint IOPSGetProvidingPowerSourceType(nint snapshot);

    [LibraryImport(IOKitLibrary)]
    public static partial nint IOPSCopyPowerSourcesList(nint blob);

    [LibraryImport(IOKitLibrary)]
    public static partial nint IOPSGetPowerSourceDescription(nint blob, nint ps);

    //[DllImport(IOKitLibrary)]
    //public static extern nint IOPSNotificationCreateRunLoopSource(IOPowerSourceCallback callback, nint context);

    [LibraryImport(IOKitLibrary)]
    public static partial uint IOServiceGetMatchingService(uint masterPort, nint matching);

    [DllImport(IOKitLibrary)]
    public static extern nint IOServiceMatching(string s);

    [LibraryImport(IOKitLibrary)]
    public static partial nint IORegistryEntryCreateCFProperty(uint entry, nint key, nint allocator, uint options);

    [LibraryImport(IOKitLibrary)]
    public static partial uint IOPMAssertionCreateWithName(nint type, uint level, nint name, out uint id);

    [LibraryImport(IOKitLibrary)]
    public static partial uint IOPMAssertionRelease(uint id);

    [LibraryImport(IOKitLibrary)]
    public static partial int IOObjectRelease(uint o);

    [LibraryImport(ObjCRuntime.Constants.CoreFoundationLibrary)]
    public static partial void CFRelease(nint obj);

    static bool TryGet<T>(this NSDictionary dic, string key, out T? value)
        where T : NSObject
    {
        if (dic != null && dic.TryGetValue((NSString)key, out var obj) && obj is T val)
        {
            value = val;
            return true;
        }

        value = default;
        return false;
    }

    internal static T? GetPlatformExpertPropertyValue<T>(CFString property)
        where T : NSObject
    {
        uint platformExpertRef = 0;
        try
        {
            platformExpertRef = IOServiceGetMatchingService(0, IOServiceMatching(IOPlatformExpertDeviceClassName));
            if (platformExpertRef == 0)
                return default;

            var propertyRef = IORegistryEntryCreateCFProperty(platformExpertRef, property.Handle, IntPtr.Zero, 0);
            if (propertyRef == IntPtr.Zero)
                return default;

            return Runtime.GetNSObject<T>(propertyRef, true);
        }
        finally
        {
            if (platformExpertRef != 0)
                _ = IOObjectRelease(platformExpertRef);
        }
    }

    internal static bool PreventUserIdleDisplaySleep(CFString name, out uint id)
    {
        var result = IOPMAssertionCreateWithName(
            kIOPMAssertionTypePreventUserIdleDisplaySleep.Handle,
            kIOPMAssertionLevelOn,
            name.Handle,
            out var newId);

        if (result == 0)
            id = newId;
        else
            id = 0;

        return result == 0;
    }

    internal static bool AllowUserIdleDisplaySleep(uint id)
    {
        var result = IOPMAssertionRelease(id);
        return result == 0;
    }
}
#endif
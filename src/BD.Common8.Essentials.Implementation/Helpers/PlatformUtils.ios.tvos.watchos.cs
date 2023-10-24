#if IOS || MACCATALYST || __WATCHOS__ || __TVOS__

using ObjCRuntime;

namespace BD.Common8.Essentials.Helpers;

#pragma warning disable SA1600 // Elements should be documented

static partial class PlatformUtils
{
#if __IOS__
    [LibraryImport(Constants.SystemLibrary, EntryPoint = "sysctlbyname")]
#else
    [LibraryImport(Constants.libSystemLibrary, EntryPoint = "sysctlbyname")]
#endif
    private static partial int SysctlByName([MarshalAs(UnmanagedType.LPStr)] string property, IntPtr output, IntPtr oldLen, IntPtr newp, uint newlen);

    [SupportedOSPlatform("maccatalyst")]
    [SupportedOSPlatform("ios")]
    [SupportedOSPlatform("tvos")]
    public static string? GetSystemLibraryProperty(string property)
    {
        var lengthPtr = Marshal.AllocHGlobal(sizeof(int));
        SysctlByName(property, IntPtr.Zero, lengthPtr, IntPtr.Zero, 0);

        var propertyLength = Marshal.ReadInt32(lengthPtr);

        if (propertyLength == 0)
        {
            Marshal.FreeHGlobal(lengthPtr);
            throw new InvalidOperationException("Unable to read length of property.");
        }

        var valuePtr = Marshal.AllocHGlobal(propertyLength);
        SysctlByName(property, valuePtr, lengthPtr, IntPtr.Zero, 0);

        var returnValue = Marshal.PtrToStringAnsi(valuePtr);

        Marshal.FreeHGlobal(lengthPtr);
        Marshal.FreeHGlobal(valuePtr);

        return returnValue;
    }

    [SupportedOSPlatform("maccatalyst")]
    [SupportedOSPlatform("ios")]
    [SupportedOSPlatform("tvos")]
    public static void BeginInvokeOnMainThread(Action action)
    {
        NSRunLoop.Main.BeginInvokeOnMainThread(action);
    }
}
#endif
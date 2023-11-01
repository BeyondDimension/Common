#if IOS || MACCATALYST || __WATCHOS__ || __TVOS__

using ObjCRuntime;

namespace BD.Common8.Essentials.Helpers;

static partial class PlatformUtils
{
    /// <summary>
    /// 获取有关系统和环境的信息
    /// </summary>
#if __IOS__
    [LibraryImport(Constants.SystemLibrary, EntryPoint = "sysctlbyname")]
#else
    [LibraryImport(Constants.libSystemLibrary, EntryPoint = "sysctlbyname")]
#endif
    private static partial int SysctlByName([MarshalAs(UnmanagedType.LPStr)] string property, IntPtr output, IntPtr oldLen, IntPtr newp, uint newlen);

    /// <summary>
    /// 通过指定的属性名称获取系统库的属性值
    /// </summary>
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

    /// <summary>
    /// 在主线程上调度指定的操作
    /// </summary>
    [SupportedOSPlatform("maccatalyst")]
    [SupportedOSPlatform("ios")]
    [SupportedOSPlatform("tvos")]
    public static void BeginInvokeOnMainThread(Action action)
    {
        NSRunLoop.Main.BeginInvokeOnMainThread(action);
    }
}
#endif
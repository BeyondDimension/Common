namespace MS.Win32;

internal static class ExternDll
{
    public const string Gdi32 = "gdi32.dll";
    public const string User32 = "user32.dll";
}

class UnsafeNativeMethods
{
    [ComImport, Guid("00000114-0000-0000-C000-000000000046"), InterfaceType(ComInterfaceType.InterfaceIsIUnknown)]
    public interface IOleWindow
    {
        [PreserveSig]
        int GetWindow([Out] out IntPtr hwnd);

        void ContextSensitiveHelp(int fEnterMode);
    }

    [DllImport(ExternDll.User32, ExactSpelling = true, CharSet = CharSet.Auto)]
    [ResourceExposure(ResourceScope.None)]
    public static extern IntPtr GetFocus();

    internal static IntPtr SetFocus(HandleRef hWnd)
    {
        IntPtr result = IntPtr.Zero;

        if (!TrySetFocus(hWnd, ref result))
        {
            throw new Win32Exception();
        }

        return result;
    }

    internal static bool TrySetFocus(HandleRef hWnd, ref IntPtr result)
    {
        result = NativeMethodsSetLastError.SetFocus(hWnd);
        int errorCode = Marshal.GetLastWin32Error();

        if (result == IntPtr.Zero && errorCode != 0)
        {
            return false;
        }

        return true;
    }

    internal static int GetWindowText(HandleRef hWnd, [Out] StringBuilder lpString, int nMaxCount)
    {
        int returnValue = NativeMethodsSetLastError.GetWindowText(hWnd, lpString, nMaxCount);
        if (returnValue == 0)
        {
            int win32Err = Marshal.GetLastWin32Error();
            if (win32Err != 0)
            {
                throw new Win32Exception(win32Err);
            }
        }
        return returnValue;
    }

    internal static int GetWindowTextLength(HandleRef hWnd)
    {
        int returnValue = NativeMethodsSetLastError.GetWindowTextLength(hWnd);
        if (returnValue == 0)
        {
            int win32Err = Marshal.GetLastWin32Error();
            if (win32Err != 0)
            {
                throw new Win32Exception(win32Err);
            }
        }
        return returnValue;
    }

    [DllImport(ExternDll.User32, ExactSpelling = true, CharSet = CharSet.Auto)]
    public static extern bool IsWindow(HandleRef hWnd);
}

internal static class NativeMethodsSetLastError
{
    private const string PresentationNativeDll = "PresentationNative_cor3.dll";

    [DllImport(PresentationNativeDll, EntryPoint = "SetFocusWrapper", SetLastError = true)]
    public static extern IntPtr SetFocus(HandleRef hWnd);

    [DllImport(PresentationNativeDll, EntryPoint = "GetWindowTextWrapper", CharSet = CharSet.Auto, BestFitMapping = false, SetLastError = true)]
    public static extern int GetWindowText(HandleRef hWnd, [Out] StringBuilder lpString, int nMaxCount);

    [DllImport(PresentationNativeDll, EntryPoint = "GetWindowTextLengthWrapper", CharSet = CharSet.Auto, SetLastError = true)]
    public static extern int GetWindowTextLength(HandleRef hWnd);
}
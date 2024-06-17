using HRESULT = global::Windows.Win32.Foundation.HRESULT;
using PInvoke = global::Windows.Win32.PInvoke;

namespace BD.Common8.NativeHost;

static class NativeHost
{
    #region C:\Program Files (x86)\Windows Kits\NETFXSDK\4.8\Include\um\metahost.h

    public static readonly Guid CLSID_CLRMetaHost = new(0x9280188d, 0xe8e, 0x4867, 0xb3, 0xc, 0x7f, 0xa8, 0x38, 0x84, 0xe8, 0xde);
    public static readonly Guid IID_ICLRMetaHost = new(0xD332DB9E, 0xB9B3, 0x4125, 0x82, 0x07, 0xA1, 0x48, 0x84, 0xF5, 0x32, 0x16);
    public static readonly Guid IID_ICLRRuntimeInfo = new(0xBD39D1D2, 0xBA2F, 0x486a, 0x89, 0xB0, 0xB4, 0xB0, 0xCB, 0x46, 0x68, 0x91);
    public static readonly Guid CLSID_CLRRuntimeHost = new(0x90F1A06E, 0x7712, 0x4762, 0x86, 0xB5, 0x7A, 0x5E, 0xBA, 0x6B, 0xDB, 0x02);
    public static readonly Guid IID_ICLRRuntimeHost = new(0x90F1A06C, 0x7712, 0x4762, 0x86, 0xB5, 0x7A, 0x5E, 0xBA, 0x6B, 0xDB, 0x02);

    #endregion

    public static int ExecuteInDefaultAppDomain(
        string pwzAssemblyPath, // "xxx.exe"
        string pwzTypeName, // "xxx.Program"
        string pwzMethodName,
        string? pwzArgument = null)
    {
        object? ppInterface = null;
        object? ppRuntime = null;
        object? ppHost = null;
        HRESULT hr = default;
        try
        {
            hr = PInvoke.CLRCreateInstance(CLSID_CLRMetaHost, IID_ICLRMetaHost, out ppInterface);
            Marshal.ThrowExceptionForHR(hr);

            var metaHost = (ICLRMetaHost)ppInterface;
            hr = new(unchecked((int)metaHost.GetRuntime("v4.0.30319", IID_ICLRRuntimeInfo, out ppRuntime)));
            Marshal.ThrowExceptionForHR(hr);

            var runtimeInfo = (IClrRuntimeInfo)ppRuntime;
            hr = new(unchecked((int)runtimeInfo.GetInterface(CLSID_CLRRuntimeHost, IID_ICLRRuntimeHost, out ppHost)));
            Marshal.ThrowExceptionForHR(hr);

            var runtimeHost = (ICLRRuntimeHost)ppHost;
            hr = new(unchecked((int)runtimeHost.Start()));
            Marshal.ThrowExceptionForHR(hr);

            hr = new(unchecked((int)runtimeHost.ExecuteInDefaultAppDomain(pwzAssemblyPath, pwzTypeName, pwzMethodName, null, out var result)));
            Marshal.ThrowExceptionForHR(hr);

            return result;
        }
        catch (Exception ex)
        {
            Console.Error.WriteLine(ex.ToString());
            return hr.Value;
        }
        finally
        {
            if (ppHost != null)
            {
                Marshal.FinalReleaseComObject(ppHost);
            }
            if (ppRuntime != null)
            {
                Marshal.FinalReleaseComObject(ppRuntime);
            }
            if (ppInterface != null)
            {
                Marshal.FinalReleaseComObject(ppInterface);
            }
        }
    }

    /// <summary>
    /// https://csharp.hotexamples.com/site/file?hash=0x30f1abd76a133e9c4b85cedb3c0ce182802bb28abf796d66573d6674ef0c2fdf
    /// </summary>
    [Guid("d332db9e-b9b3-4125-8207-a14884f53216")]
    [InterfaceType(ComInterfaceType.InterfaceIsIUnknown)]
    [ComImport]
    partial interface ICLRMetaHost
    {
        [MethodImpl(MethodImplOptions.InternalCall, MethodCodeType = MethodCodeType.Runtime)]
        IntPtr GetRuntime(
            string pwzVersion,
            [MarshalAs(UnmanagedType.LPStruct)] Guid iid,
            [MarshalAs(UnmanagedType.IUnknown)] out object ppRuntime);

        //[MethodImpl(MethodImplOptions.InternalCall, MethodCodeType = MethodCodeType.Runtime)]
        //void GetVersionFromFile(
        //    string pwzFilePath,
        //    IntPtr pwzBuffer,
        //    ref uint pcchBuffer);

        //[MethodImpl(MethodImplOptions.InternalCall, MethodCodeType = MethodCodeType.Runtime)]
        //IntPtr EnumerateInstalledRuntimes();          // IEnumUnknown

        //[MethodImpl(MethodImplOptions.InternalCall, MethodCodeType = MethodCodeType.Runtime)]
        //IntPtr EnumerateLoadedRuntimes(IntPtr hndProcess);          // IEnumUnknown

        //[MethodImpl(MethodImplOptions.InternalCall, MethodCodeType = MethodCodeType.Runtime)]
        //void RequestRuntimeLoadedNotification(IntPtr pCallbackFunction);

        //[MethodImpl(MethodImplOptions.InternalCall, MethodCodeType = MethodCodeType.Runtime)]
        //IntPtr QueryLegacyV2RuntimeBinding(Guid riid);

        //[MethodImpl(MethodImplOptions.InternalCall, MethodCodeType = MethodCodeType.Runtime)]
        //void ExitProcess(int iExitCode);
    }

    /// <summary>
    /// https://referencesource.microsoft.com/#ComSvcConfig/SafeNativeMethods.cs,312ee6a251e8a881
    /// </summary>
    [Guid("BD39D1D2-BA2F-486A-89B0-B4B0CB466891")]
    [InterfaceType(ComInterfaceType.InterfaceIsIUnknown)]
    [ComImport]
    partial interface IClrRuntimeInfo
    {
        //[MethodImpl(MethodImplOptions.InternalCall, MethodCodeType = MethodCodeType.Runtime)]
        //void GetVersionString(
        //    [Out, MarshalAs(UnmanagedType.LPWStr, SizeParamIndex = 1)] StringBuilder buffer,
        //    [In, Out, MarshalAs(UnmanagedType.U4)] ref int bufferLength);

        //[MethodImpl(MethodImplOptions.InternalCall, MethodCodeType = MethodCodeType.Runtime)]
        //void GetRuntimeDirectory(
        //    [Out, MarshalAs(UnmanagedType.LPWStr, SizeParamIndex = 1)] StringBuilder buffer,
        //    [In, Out, MarshalAs(UnmanagedType.U4)] ref int bufferLength);

        //[return: MarshalAs(UnmanagedType.Bool)]
        //[MethodImpl(MethodImplOptions.InternalCall, MethodCodeType = MethodCodeType.Runtime)]
        //bool IsLoaded(
        //    [In] IntPtr processHandle);

        //[MethodImpl(MethodImplOptions.InternalCall, MethodCodeType = MethodCodeType.Runtime), LCIDConversion(3)]
        //void LoadErrorString(
        //    [In, MarshalAs(UnmanagedType.U4)] int resourceId,
        //    [Out, MarshalAs(UnmanagedType.LPWStr, SizeParamIndex = 2)] StringBuilder buffer,
        //    [In, Out, MarshalAs(UnmanagedType.U4)] ref int bufferLength);

        ////@
        //[MethodImpl(MethodImplOptions.InternalCall, MethodCodeType = MethodCodeType.Runtime)]
        //IntPtr LoadLibrary(
        //    [In, MarshalAs(UnmanagedType.LPWStr)] string dllName);

        //[MethodImpl(MethodImplOptions.InternalCall, MethodCodeType = MethodCodeType.Runtime)]
        //IntPtr GetProcAddress(
        //    [In, MarshalAs(UnmanagedType.LPStr)] string procName);

        [return: MarshalAs(UnmanagedType.Interface)]
        [MethodImpl(MethodImplOptions.InternalCall, MethodCodeType = MethodCodeType.Runtime)]
        IntPtr GetInterface(
            [In, MarshalAs(UnmanagedType.LPStruct)] Guid coClassId,
            [In, MarshalAs(UnmanagedType.LPStruct)] Guid interfaceId,
            [MarshalAs(UnmanagedType.IUnknown)] out object ppUnk);
    }

    [Guid("90F1A06C-7712-4762-86B5-7A5EBA6BDB02")]
    [InterfaceType(ComInterfaceType.InterfaceIsIUnknown)]
    [ComImport]
    partial interface ICLRRuntimeHost
    {
        [MethodImpl(MethodImplOptions.InternalCall, MethodCodeType = MethodCodeType.Runtime)]
        IntPtr Start();

        [MethodImpl(MethodImplOptions.InternalCall, MethodCodeType = MethodCodeType.Runtime)]
        IntPtr Stop();

        [MethodImpl(MethodImplOptions.InternalCall, MethodCodeType = MethodCodeType.Runtime)]
        IntPtr ExecuteInDefaultAppDomain(
            string pwzAssemblyPath,
            string pwzTypeName,
            string pwzMethodName,
            string? pwzArgument,
            [Out, MarshalAs(UnmanagedType.U4)] out int pReturnValue);
    }
}

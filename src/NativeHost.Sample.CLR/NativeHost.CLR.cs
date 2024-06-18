namespace BD.Common8.NativeHost;

#pragma warning disable IDE1006 // 命名样式
#pragma warning disable SA1302 // Interface names should begin with I

[SupportedOSPlatform("windows")]
static unsafe partial class NativeHost
{
    #region C:\Program Files (x86)\Windows Kits\NETFXSDK\4.8\Include\um\metahost.h

    const string IID__Assembly = "17156360-2f1a-384a-bc52-fde93c215c5b";
    const string IID__AppDomain = "05f696dc-2b29-3663-ad8b-c4389cf2a713";
    const string IID__MethodInfo = "ffcc1b5d-ecb8-38dd-9b01-3dc8abc2aa5f";

    static readonly Guid CLSID_CLRMetaHost = new(0x9280188d, 0xe8e, 0x4867, 0xb3, 0xc, 0x7f, 0xa8, 0x38, 0x84, 0xe8, 0xde);
    static readonly Guid IID_ICLRMetaHost = new(0xD332DB9E, 0xB9B3, 0x4125, 0x82, 0x07, 0xA1, 0x48, 0x84, 0xF5, 0x32, 0x16);
    static readonly Guid IID_ICLRRuntimeInfo = new(0xBD39D1D2, 0xBA2F, 0x486a, 0x89, 0xB0, 0xB4, 0xB0, 0xCB, 0x46, 0x68, 0x91);
    static readonly Guid CLSID_CorRuntimeHost = new(0xcb2f6723, 0xab3a, 0x11d2, 0x9c, 0x40, 0x00, 0xc0, 0x4f, 0xa3, 0x0a, 0x3e);
    static readonly Guid IID_ICorRuntimeHost = new(0xcb2f6722, 0xab3a, 0x11d2, 0x9c, 0x40, 0x00, 0xc0, 0x4f, 0xa3, 0x0a, 0x3e);
    static readonly Guid IID_IUnknown = new("00000000-0000-0000-C000-000000000046");

    #endregion

    /// <summary>
    /// 在 CoreCLR 中加载 CLR 并且执行 .NET Framework 4.x 二进制程序集
    /// </summary>
    /// <param name="rawAssembly">带有入口点的 .NET Framework 4.x 二进制程序集</param>
    /// <param name="reverse">是否反转二进制数据</param>
    /// <returns></returns>
    public static int ExecuteInDefaultAppDomain(byte[] rawAssembly, bool reverse = false)
    {
        // https://0pen1.github.io/2022/02/09/net%E7%A8%8B%E5%BA%8F%E9%9B%86%E5%86%85%E5%AD%98%E5%8A%A0%E8%BD%BD%E6%89%A7%E8%A1%8C%E6%8A%80%E6%9C%AF/

        var rawAssemblySpan = rawAssembly.AsSpan();
        if (reverse)
            rawAssemblySpan.Reverse();
        object? ppInterface = null;
        object? ppRuntime = null;
        object? ppHost = null;
        ICorRuntimeHost? runtimeHost = null;
        HRESULT hr = default;
        try
        {
            // 初始化 ICLRMetaHost 接口
            hr = PInvoke.CLRCreateInstance(CLSID_CLRMetaHost, IID_ICLRMetaHost, out ppInterface);
            Marshal.ThrowExceptionForHR(hr);

            // 通过 ICLRMetaHost 获取 ICLRRuntimeInfo 接口
            var metaHost = (ICLRMetaHost)ppInterface;
            ppRuntime = metaHost.GetRuntime("v4.0.30319", IID_ICLRRuntimeInfo);

            // 通过 ICLRRuntimeInfo 将 CLR 加载到当前进程并返回运行时接口 ICorRuntimeHost 指针
            var runtimeInfo = (ICLRRuntimeInfo)ppRuntime;
            ppHost = runtimeInfo.GetInterface(CLSID_CorRuntimeHost, IID_ICorRuntimeHost);

            // 通过 ICLRRuntimeHost.Start() 初始化 CLR
            runtimeHost = (ICorRuntimeHost)ppHost;
            runtimeHost.Start();

            // 通过 ICLRRuntimeHost 获取 AppDomain 接口指针
            runtimeHost.GetDefaultDomain(out var pAppDomain);

            //  然后通过 AppDomain 接口的 QueryInterface 方法来查询默认应用程序域的实例指针
            var pAppDomainPtr = Marshal.GetIUnknownForObject(pAppDomain);
            Guid iid_IUnknown = IID_IUnknown;
            hr = new(Marshal.QueryInterface(pAppDomainPtr, ref iid_IUnknown, out var pDefaultAppDomain));
            Marshal.ThrowExceptionForHR(hr);

            // 定义 SAFEARRAYBOUND 结构体，设置维度为 1
            Windows.Win32.System.Com.SAFEARRAYBOUND saBound = new()
            {
                // 数组的长度
                cElements = unchecked((uint)rawAssembly.Length),
                // 设置第一维的起始下标
                lLbound = 0,
            };

            // 创建一个新的数组描述符，分配和初始化数组的数据，并返回一个指向新数组描述符的指针
            var pSafeArray = PInvoke.SafeArrayCreate(VARENUM.VT_UI1, 1, in saBound);

            // 增加数组的锁计数，并返回数组的指针。
            hr = PInvoke.SafeArrayAccessData(pSafeArray, out var pData);
            Marshal.ThrowExceptionForHR(hr);

            // 在缓冲区之间复制字节
            var pDataSpan = new Span<byte>(pData, rawAssembly.Length);
            rawAssemblySpan.CopyTo(pDataSpan);

            // 减少数组的锁计数，并释放通过 SafeArrayAccessData 返回的指针
            hr = PInvoke.SafeArrayUnaccessData(pSafeArray);
            Marshal.ThrowExceptionForHR(hr);

            // 通过默认应用程序域实例的 Load_3 方法加载安全 .NET Framework 程序集数组，并返回 Assembly 的实例对象指针
            var defaultAppDomain = (_AppDomain)Marshal.GetObjectForIUnknown(pDefaultAppDomain);
            hr = new(unchecked((int)defaultAppDomain.Load_3(pSafeArray, out var pAssembly)));
            Marshal.ThrowExceptionForHR(hr);

            //// 通过 Assembly 实例对象的 get_EntryPoint 方法获取描述入口点的 MethodInfo 实例对象
            //var assembly = (_Assembly)Marshal.GetTypedObjectForIUnknown(pAssembly, typeof(_Assembly));
            //hr = new(unchecked((int)assembly.get_EntryPoint(out var pMethodInfo)));
            //Marshal.ThrowExceptionForHR(hr);

            //var methodInfo = (_MethodInfo)Marshal.GetTypedObjectForIUnknown(pMethodInfo, typeof(_MethodInfo));

            //// 创建参数安全数组
            //TagVariant vPsa = new()
            //{
            //    vt = VARENUM.VT_ARRAY | VARENUM.VT_BSTR,
            //};
            //var args = PInvoke.SafeArrayCreateVector(VARENUM.VT_VARIANT, 0, 1);

            //TagVariant vObj = new()
            //{
            //    vt = VARENUM.VT_NULL,
            //};

            //// 通过描述入口点的 MethodInfo 实例对象的 Invoke 方法执行入口点
            //hr = new(unchecked((int)methodInfo.Invoke_3(vObj, (nint)args, out var pRetVal)));
            //Marshal.ThrowExceptionForHR(hr);

            //// https://referencesource.microsoft.com/#System.Windows.Forms/winforms/Managed/System/WinForms/NativeMethods.cs,4322
            //var result = Marshal.PtrToStructure<TagVariant>(pRetVal);
            //return unchecked((int)result.data1);

            return 0;
        }
        catch (Exception ex)
        {
            Console.Error.WriteLine(ex.ToString());
            return hr.Value;
        }
        finally
        {
            runtimeHost?.Stop();
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

    [Guid(IID__AppDomain)]
    [InterfaceType(ComInterfaceType.InterfaceIsIUnknown)]
    [ComImport]
    interface _AppDomain
    {
        unsafe HRESULT Load_3(Windows.Win32.System.Com.SAFEARRAY* rawAssembly, out _Assembly* pRetVal);
    }

    [Guid(IID__Assembly)]
    [InterfaceType(ComInterfaceType.InterfaceIsIUnknown)]
    [ComImport]
    interface _Assembly
    {
        nint get_EntryPoint(out nint pRetVal);
    }

    [Guid(IID__MethodInfo)]
    [InterfaceType(ComInterfaceType.InterfaceIsIUnknown)]
    [ComImport]
    interface _MethodInfo
    {
        nint Invoke_3(TagVariant obj, nint parameters, out nint pRetVal);
    }

    [StructLayout(LayoutKind.Sequential, CharSet = CharSet.Auto)]
    struct TagVariant
    {
        [MarshalAs(UnmanagedType.I2)]
        public VARENUM vt;
        [MarshalAs(UnmanagedType.I2)]
        public ushort reserved1;
        [MarshalAs(UnmanagedType.I2)]
        public ushort reserved2;
        [MarshalAs(UnmanagedType.I2)]
        public ushort reserved3;
        public IntPtr data1;
        public IntPtr data2;
    }
}

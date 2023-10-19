namespace BD.Common8.Security;

#pragma warning disable IDE1006 // 命名样式

partial class MachineUniqueIdentifier
{
    [LibraryImport("/System/Library/Frameworks/IOKit.framework/IOKit")]
    private static partial uint IOServiceGetMatchingService(uint masterPort, IntPtr matching);

    [LibraryImport("/System/Library/Frameworks/IOKit.framework/IOKit")]
    private static partial nint IOServiceMatching(nint s);

    static unsafe _NativeSafeHandle Utf8StringToPointer(ReadOnlySpan<byte> s)
    {
        var bufferIntPtr = Marshal.AllocHGlobal(s.Length);
        var nativeSpan = new Span<byte>(bufferIntPtr.ToPointer(), s.Length);
        s.CopyTo(nativeSpan);
        return new(bufferIntPtr);
    }

    [LibraryImport("/System/Library/Frameworks/IOKit.framework/IOKit")]
    private static partial nint IORegistryEntryCreateCFProperty(uint entry, IntPtr key, IntPtr allocator, uint options);

    [LibraryImport("/System/Library/Frameworks/IOKit.framework/IOKit")]
    private static partial int IOObjectRelease(uint o);

    sealed class _NativeSafeHandle(nint invalidHandleValue) : SafeHandle(invalidHandleValue, true)
    {
        public override bool IsInvalid => handle == default;

        protected override bool ReleaseHandle()
        {
            Marshal.FreeHGlobal(handle);
            return true;
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static implicit operator nint(_NativeSafeHandle handle) => handle.handle;
    }

    static partial class _Messaging
    {
        internal const string LIBOBJC_DYLIB = "/usr/lib/libobjc.dylib";

        [LibraryImport(LIBOBJC_DYLIB, EntryPoint = "objc_msgSend")]
        public static partial nint IntPtr_objc_msgSend(nint receiver, nint selector);

        [LibraryImport(LIBOBJC_DYLIB, EntryPoint = "objc_msgSend")]
        public static partial void void_objc_msgSend(nint receiver, nint selector);
    }

    static partial class _Selector
    {
        // objc/runtime.h
        // Selector.GetHandle is optimized by the AOT compiler, and the current implementation only supports IntPtr, so we can't switch to NativeHandle quite yet (the AOT compiler crashes).
        [LibraryImport(_Messaging.LIBOBJC_DYLIB, EntryPoint = "sel_registerName")]
        public static partial nint GetHandle(nint name);
    }

    static partial class _NSString
    {
        public static string? FromHandle(nint handle)
        {
            if (handle == default)
                return null;

            try
            {
                using var selUTF8String = Utf8StringToPointer("UTF8String"u8);
                return Marshal.PtrToStringAuto(_Messaging.IntPtr_objc_msgSend(handle, _Selector.GetHandle(selUTF8String)));
            }
            finally
            {
                DangerousRelease(handle);
            }
        }

        static void DangerousRelease(nint handle)
        {
            if (handle == default)
                return;

            using var release = Utf8StringToPointer("release"u8);
            _Messaging.void_objc_msgSend(handle, _Selector.GetHandle(release));
        }
    }

    [MethodImpl(MethodImplOptions.NoInlining)]
    static string GetIOPlatformSerialNumber()
    {
        // 禁止反射调用此函数
        var caller = new StackTrace().GetFrame(1)?.GetMethod()?.DeclaringType;
        if (typeof(MachineUniqueIdentifier).Assembly != caller?.Assembly)
            throw new InvalidOperationException("Direct invocation of this method is not allowed.");

        var keyName = "IOPlatformSerialNumber"u8;

        var value = string.Empty;
        using var iOPlatformExpertDevice = Utf8StringToPointer("IOPlatformExpertDevice"u8);
        nint matching = IOServiceMatching(iOPlatformExpertDevice);
        var platformExpert = IOServiceGetMatchingService(0, matching);
        if (platformExpert != 0)
        {
            using var keyName_ = Utf8StringToPointer(keyName);
            var valueIntPtr = IORegistryEntryCreateCFProperty(platformExpert, keyName_, default, default);
            if (valueIntPtr != default)
            {
                value = _NSString.FromHandle(valueIntPtr) ?? value;
            }
            _ = IOObjectRelease(platformExpert);
        }

        return value;
    }
}

namespace BD.Common8.Security;

partial class MachineUniqueIdentifier
{
    static partial class MacOS
    {
        [LibraryImport("/System/Library/Frameworks/IOKit.framework/IOKit")]
        private static partial uint IOServiceGetMatchingService(uint masterPort, nint matching);

        [LibraryImport("/System/Library/Frameworks/IOKit.framework/IOKit")]
        private static partial nint IOServiceMatching([MarshalAs(UnmanagedType.LPStr)] string s);

        [LibraryImport("/System/Library/Frameworks/IOKit.framework/IOKit")]
        private static partial nint IORegistryEntryCreateCFProperty(uint entry, nint key, nint allocator, uint options);

        [LibraryImport("/System/Library/Frameworks/IOKit.framework/IOKit")]
        private static partial int IOObjectRelease(uint o);

        static readonly nint allocSel = GetHandle("alloc");
        static readonly nint nsStringClass = objc_getClass("NSString");
        static readonly nint initWithCharactersSel = GetHandle("initWithCharacters:length:");

        [LibraryImport("/usr/lib/libobjc.dylib", EntryPoint = "sel_registerName")]
        private static partial nint GetHandle([MarshalAs(UnmanagedType.LPStr)] string name);

        [LibraryImport("/usr/lib/libobjc.dylib")]
        private static partial nint objc_getClass([MarshalAs(UnmanagedType.LPStr)] string className);

        [LibraryImport("/usr/lib/libobjc.dylib", EntryPoint = "objc_msgSend")]
        private static partial nint intptr_objc_msgSend(nint basePtr, nint selector);

        [LibraryImport("/usr/lib/libobjc.dylib", EntryPoint = "objc_msgSend")]
        private static partial nint IntPtr_objc_msgSend_IntPtr_IntPtr(nint receiver, nint selector, nint p1, nint p2);

        static unsafe nint GetNSString(string str)
        {
            fixed (char* ptrFirstChar = str)
            {
                var allocated = intptr_objc_msgSend(nsStringClass, allocSel);
                return IntPtr_objc_msgSend_IntPtr_IntPtr(allocated, initWithCharactersSel, (nint)ptrFirstChar, str.Length);
            }
        }

        [LibraryImport("/System/Library/Frameworks/CoreFoundation.framework/CoreFoundation")]
        private static partial nint CFDataGetBytePtr(nint ptr);

        [MethodImpl(MethodImplOptions.NoInlining)]
        internal static string GetIOPlatformSerialNumber()
        {
            // 禁止反射调用此函数
            var caller = new StackTrace().GetFrame(1)?.GetMethod()?.DeclaringType;
            if (typeof(MachineUniqueIdentifier).Assembly != caller?.Assembly)
                throw new InvalidOperationException("Direct invocation of this method is not allowed.");

            var value = string.Empty;
            nint matching = IOServiceMatching("IOPlatformExpertDevice");
            var platformExpert = IOServiceGetMatchingService(0, matching);
            if (platformExpert != 0)
            {
                var valueIntPtr = IORegistryEntryCreateCFProperty(platformExpert, GetNSString("IOPlatformSerialNumber"), default, default);
                if (valueIntPtr != default)
                {
                    value = Marshal.PtrToStringAuto(CFDataGetBytePtr(valueIntPtr)) ?? string.Empty;
                }
                _ = IOObjectRelease(platformExpert);
            }

            return value;
        }
    }
}
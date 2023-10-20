using MonoMac.Foundation;

namespace BD.Common8.UnitTest;

/// <summary>
/// 提供对 <see cref="MachineUniqueIdentifier"/> 的单元测试
/// </summary>
public sealed partial class MachineUniqueIdentifierTest
{
    /// <summary>
    /// 测试获取 <see cref="MachineUniqueIdentifier.MachineSecretKey"/>
    /// </summary>
    [Test]
    public void GetMachineSecretKey()
    {
        var machineSecretKey = MachineUniqueIdentifier.MachineSecretKey;
        TestContext.WriteLine("Key:");
        TestContext.WriteLine(machineSecretKey.Key.ToHexString());
        TestContext.WriteLine("IV:");
        TestContext.WriteLine(machineSecretKey.IV.ToHexString());
    }

    /// <summary>
    /// 测试获取 <see cref="MachineUniqueIdentifier.MachineId"/>
    /// </summary>
    [Test]
    public void GetMachineId()
    {
        string? machineIdString = null;
        try
        {
            var machineId = MachineUniqueIdentifier.MachineId;
            if (machineId != null)
            {
                var ptr = Marshal.SecureStringToGlobalAllocAnsi(machineId);
                try
                {
                    machineIdString = Marshal.PtrToStringAnsi(ptr);
                }
                finally
                {
                    Marshal.ZeroFreeGlobalAllocAnsi(ptr);
                }
            }

            if (OperatingSystem.IsMacOS())
            {
                // 测试与上一个版本的实现结果是否完全一致
                var serialNumber = MacCatalystPlatformServiceImpl.GetSerialNumber();
                Assert.That(machineIdString ?? string.Empty, Is.EqualTo(serialNumber));
            }
        }
        finally
        {
            machineIdString = null;
        }
        Assert.That(machineIdString, Is.EqualTo(null));
    }

#pragma warning disable SYSLIB1054 // 使用 “LibraryImportAttribute” 而不是 “DllImportAttribute” 在编译时生成 P/Invoke 封送代码
#pragma warning disable CA2101 // 指定对 P/Invoke 字符串参数进行封送处理
    static partial class MacCatalystPlatformServiceImpl
    {
        [LibraryImport("/System/Library/Frameworks/IOKit.framework/IOKit")]
        private static partial uint IOServiceGetMatchingService(uint masterPort, IntPtr matching);

        [DllImport("/System/Library/Frameworks/IOKit.framework/IOKit")]
        static extern IntPtr IOServiceMatching(string s);

        [LibraryImport("/System/Library/Frameworks/IOKit.framework/IOKit")]
        private static partial IntPtr IORegistryEntryCreateCFProperty(uint entry, IntPtr key, IntPtr allocator, uint options);

        [LibraryImport("/System/Library/Frameworks/IOKit.framework/IOKit")]
        private static partial int IOObjectRelease(uint o);

        static string GetIOPlatformExpertDevice(string keyName)
        {
            var value = string.Empty;
            var platformExpert = IOServiceGetMatchingService(0, IOServiceMatching("IOPlatformExpertDevice"));
            if (platformExpert != 0)
            {
                var key = (NSString)keyName;
                var valueIntPtr = IORegistryEntryCreateCFProperty(platformExpert, key.Handle, IntPtr.Zero, 0);
                if (valueIntPtr != IntPtr.Zero)
                {
                    value = NSString.FromHandle(valueIntPtr) ?? value;
                }
                _ = IOObjectRelease(platformExpert);
            }

            return value;
        }

        public static string GetSerialNumber() => GetIOPlatformExpertDevice("IOPlatformSerialNumber");
    }
#pragma warning restore CA2101 // 指定对 P/Invoke 字符串参数进行封送处理
#pragma warning restore SYSLIB1054 // 使用 “LibraryImportAttribute” 而不是 “DllImportAttribute” 在编译时生成 P/Invoke 封送代码
}

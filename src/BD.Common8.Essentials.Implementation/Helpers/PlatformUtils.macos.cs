#if MACOS

using CoreFoundation;
using ObjCRuntime;

namespace BD.Common8.Essentials.Helpers;

/// <summary>
/// 用于操作 macOS 下的 IOKit 服务的工具类
/// https://github.com/dotnet/maui/blob/8.0.0-rc.2.9373/src/Essentials/src/Platform/PlatformUtils.macos.cs
/// </summary>
[SupportedOSPlatform("macos")]
static partial class IOKit
{
    const string IOKitLibrary = "/System/Library/Frameworks/IOKit.framework/IOKit";
    const string IOPlatformExpertDeviceClassName = "IOPlatformExpertDevice";

    const uint kIOPMAssertionLevelOff = 0;
    const uint kIOPMAssertionLevelOn = 255;

    /// <summary>
    /// 交流电源的键名
    /// </summary>
    const string kIOPMACPowerKey = "AC Power";

    /// <summary>
    /// UPS 电源的键名
    /// </summary>
    const string kIOPMUPSPowerKey = "UPS Power";

    /// <summary>
    /// 电池电源的键名
    /// </summary>
    const string kIOPMBatteryPowerKey = "Battery Power";

    /// <summary>
    /// 当前电源容量的 CFDictionary 键
    /// </summary>
    const string kIOPSCurrentCapacityKey = "Current Capacity";

    /// <summary>
    /// 当前电源最大值或“完全充电容量的” CFDictionary 键
    /// </summary>
    const string kIOPSMaxCapacityKey = "Max Capacity";

    /// <summary>
    /// CFDictionary 键，用于电源类型
    /// </summary>
    const string kIOPSTypeKey = "Type";

    /// <summary>
    /// 表示驻留在 Mac 内部的电池
    /// </summary>
    const string kIOPSInternalBatteryType = "InternalBattery";

    /// <summary>
    /// CFDictionary 键表示当前电源的存在
    /// </summary>
    const string kIOPSIsPresentKey = "Is Present";

    /// <summary>
    /// 当前电源充电状态的 CFDictionary 键
    /// </summary>
    const string kIOPSIsChargingKey = "Is Charging";

    /// <summary>
    /// CFDictionary 键指示电池是否已充电
    /// </summary>
    const string kIOPSIsChargedKey = "Is Charged";

    /// <summary>
    /// CFDictionary 键指示电池是否正在完成充电
    /// </summary>
    const string kIOPSIsFinishingChargeKey = "Is Finishing Charge";

    /// <summary>
    /// 防止显示屏自动变暗
    /// </summary>
    static readonly CFString kIOPMAssertionTypePreventUserIdleDisplaySleep = "PreventUserIdleDisplaySleep";

    /// <summary>
    /// 在不透明的 CFTypeRef 中返回电源信息的 blob
    /// </summary>
    [LibraryImport(IOKitLibrary)]
    public static partial nint IOPSCopyPowerSourcesInfo();

    /// <summary>
    /// 指示计算机当前正在从中汲取电源
    /// </summary>
    /// <param name="snapshot">IOPSCopyPowerSourcesInfo（）返回的 CFTypeRef</param>
    /// <returns>以下之一：CFSTR（kIOPMACPowerKey）、CFSTR（kIOPMBatteryPowerKey）、CFSTR（kIOPMUPSPowerKey）</returns>
    [LibraryImport(IOKitLibrary)]
    public static partial nint IOPSGetProvidingPowerSourceType(nint snapshot);

    /// <summary>
    /// 返回电源句柄的 CFArray，每个句柄的类型为 CFTypeRef
    /// </summary>
    /// <param name="blob">获取 IOPSCopyPowerSourcesInfo（）返回的 CFTypeRef</param>
    /// <returns>如果遇到错误，则返回 NULL，否则返回 CFTypeRefs 的 CFArray。调用方必须 CFRelease（）返回的 CFArrayRef</returns>
    [LibraryImport(IOKitLibrary)]
    public static partial nint IOPSCopyPowerSourcesList(nint blob);

    /// <summary>
    /// 返回一个 CFDictionary，其中包含有关特定电源的可读信息
    /// </summary>
    /// <param name="blob">IOPSCopyPowerSourcesInfo（）返回的 CFTypeRef</param>
    /// <param name="ps">IOPSCopyPowerSourcesList（）返回的 CFArray 中的 CFTypeRefs 之一</param>
    /// <returns>如果遇到错误，则返回 NULL，否则返回 CFDictionary。调用方不应释放返回的 CFDictionary - 它将作为 IOPSCopyPowerSourcesInfo（）返回的 CFTypeRef 的一部分发布</returns>
    [LibraryImport(IOKitLibrary)]
    public static partial nint IOPSGetPowerSourceDescription(nint blob, nint ps);

    //[DllImport(IOKitLibrary)]
    //public static extern nint IOPSNotificationCreateRunLoopSource(IOPowerSourceCallback callback, nint context);

    /// <summary>
    /// 查找与匹配字典匹配的已注册 IOService 对象
    /// </summary>
    /// <param name="masterPort">从 IOMasterPort（_：_：）获取的主端口。传递 kIOMasterPortDefault 以查找默认主端口</param>
    /// <param name="matching">包含匹配信息的 CF 字典，此函数始终使用其中一个引用（请注意，在 Tiger 发布之前，如果尝试序列化字典时出错，则可能不会发布字典的可能性很小）。
    /// IOKitLib 可以使用帮助程序函数（如 IOServiceMatching、IOServiceNameMatching、IOBSDNameMatching）为常见条件构造匹配字典</param>
    /// <returns>成功后返回第一个匹配的服务。该服务必须由调用方释放</returns>
    [LibraryImport(IOKitLibrary)]
    public static partial uint IOServiceGetMatchingService(uint masterPort, nint matching);

    /// <summary>
    /// 创建一个匹配字典，用于指定 IOService 类匹配
    /// </summary>
    /// <param name="s">类名，作为 const C 字符串。类匹配在此类或任何子类的 IOService 上成功</param>
    /// <returns></returns>
    [DllImport(IOKitLibrary)]
    public static extern nint IOServiceMatching(string s);

    /// <summary>
    /// 创建注册表项属性的 CF 表示形式
    /// </summary>
    /// <param name="entry">注册表项处理要复制其属性的属性</param>
    /// <param name="key">指定属性名称的 CFString</param>
    /// <param name="allocator">创建 CF 容器时要使用的 CF 分配器</param>
    /// <param name="options">当前未定义任何选项</param>
    /// <returns>将创建一个 CF 容器，并在成功时返回调用方。调用方应使用 CFRelease 释放</returns>
    [LibraryImport(IOKitLibrary)]
    public static partial nint IORegistryEntryCreateCFProperty(uint entry, nint key, nint allocator, uint options);

    /// <summary>
    /// 从电源管理系统动态请求系统行为
    /// </summary>
    /// <param name="type">要从 PM 系统请求的 CFString 断言类型</param>
    /// <param name="level">传递 kIOPMAssertionLevelOn 或 kIOPMAssertionLevelOff</param>
    /// <param name="name">一个字符串，描述调用方的名称和此断言正在处理的活动（例如“邮件压缩邮箱”）。名称不得超过 128 个字符</param>
    /// <param name="id">成功后，此参数中将返回唯一 ID</param>
    /// <returns>成功时返回 kIOReturnSuccess，任何其他返回都指示 PM 无法成功激活指定的断言</returns>
    [LibraryImport(IOKitLibrary)]
    public static partial uint IOPMAssertionCreateWithName(nint type, uint level, nint name, out uint id);

    /// <summary>
    /// 递减断言的保留计数
    /// </summary>
    /// <param name="id">从 IOPMAssertionCreate 返回的要取消的 assertion_id</param>
    /// <returns>成功时返回 kIOReturnSuccess</returns>
    [LibraryImport(IOKitLibrary)]
    public static partial uint IOPMAssertionRelease(uint id);

    /// <summary>
    /// 释放以前由 IOKitLib 返回的对象句柄
    /// </summary>
    /// <param name="o">要释放的 IOKit 对象</param>
    /// <returns>kern_return_t 错误代码</returns>
    [LibraryImport(IOKitLibrary)]
    public static partial int IOObjectRelease(uint o);

    /// <summary>
    /// 释放 Core Foundation 对象
    /// </summary>
    /// <param name="obj">要释放的 CFType 对象，此值不能是 NULL</param>
    [LibraryImport(ObjCRuntime.Constants.CoreFoundationLibrary)]
    public static partial void CFRelease(nint obj);

    /// <summary>
    /// 尝试从 NSDictionary 中获取指定键名的值
    /// </summary>
    /// <typeparam name="T">值的类型</typeparam>
    /// <param name="dic">NSDictionary 对象</param>
    /// <param name="key">键名</param>
    /// <param name="value">获取到的值</param>
    /// <returns>是否成功获取值</returns>
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

    /// <summary>
    /// 获取平台专家属性值
    /// </summary>
    /// <typeparam name="T">属性值的类型，必须是NSObject的子类</typeparam>
    /// <param name="property">属性名称</param>
    /// <returns>属性值</returns>
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

    /// <summary>
    /// 阻止用户空闲时显示器进入睡眠模式
    /// </summary>
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

    /// <summary>
    /// 允许用户空闲时显示器进入睡眠模式
    /// </summary>
    internal static bool AllowUserIdleDisplaySleep(uint id)
    {
        var result = IOPMAssertionRelease(id);
        return result == 0;
    }
}
#endif
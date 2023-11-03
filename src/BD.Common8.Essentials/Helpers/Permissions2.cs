namespace BD.Common8.Essentials.Helpers;

/// <summary>
/// 提供了检查和授予权限
/// </summary>
public static partial class Permissions2
{
    /// <summary>
    /// 检索权限的当前状态
    /// </summary>
    /// <remarks>
    /// 如果在应用程序清单中找不到所需的条目，将抛出 <see cref="PermissionException"/>
    /// 并非所有权限都需要清单条目
    /// </remarks>
    /// <exception cref="PermissionException">如果在应用程序清单中找不到所需的条目，则引发</exception>
    /// <typeparam name="TPermission">要检查的权限类型</typeparam>
    /// <returns><see cref="PermissionStatus"/> 指示权限的当前状态的值</returns>
    public static Task<PermissionStatus> CheckStatusAsync<TPermission>()
        where TPermission : IBasePermission
    {
        var permissionsPlatformService = IPermissionsPlatformService.Instance;
        if (permissionsPlatformService != null)
        {
            return permissionsPlatformService.CheckStatusAsync<TPermission>();
        }
        return Task.FromResult(PermissionStatus.Granted);
    }

    /// <summary>
    /// 请求用户对此应用程序的权限
    /// </summary>
    /// <remarks>
    /// 如果在应用程序清单中找不到所需的条目，将抛出 <see cref="PermissionException"/>
    /// 并非所有权限都需要清单条目
    /// </remarks>
    /// <exception cref="PermissionException">如果在应用程序清单中找不到所需的条目，则引发</exception>
    /// <typeparam name="TPermission">要检查的权限类型</typeparam>
    /// <returns><see cref="PermissionStatus"/> 指示此权限请求结果的值</returns>
    public static Task<PermissionStatus> RequestAsync<TPermission>()
        where TPermission : IBasePermission
    {
        var permissionsPlatformService = IPermissionsPlatformService.Instance;
        if (permissionsPlatformService != null)
        {
            return permissionsPlatformService.RequestAsync<TPermission>();
        }
        return Task.FromResult(PermissionStatus.Granted);
    }

    /// <summary>
    /// 确定是否应显示 UI，向用户解释如何在应用程序中使用权限
    /// </summary>
    /// <remarks> 仅在 Android 上使用，其他平台将始终返回 <see langword="false"/></remarks>
    /// <typeparam name="TPermission">要检查的权限类型</typeparam>
    /// <returns><see langword="true"/> 如果用户过去曾拒绝或禁用该权限，则 <see langword="false"/></returns>
    public static bool ShouldShowRationale<TPermission>()
        where TPermission : IBasePermission
    {
        var permissionsPlatformService = IPermissionsPlatformService.Instance;
        if (permissionsPlatformService != null)
        {
            return permissionsPlatformService.ShouldShowRationale<TPermission>();
        }
        return default;
    }

    /// <summary>
    /// 确保特定权限 TPermission 被授予
    /// </summary>
    public static void EnsureDeclared<TPermission>()
        where TPermission : IBasePermission
    {
        var permissionsPlatformService = IPermissionsPlatformService.Instance;
        permissionsPlatformService?.EnsureDeclared<TPermission>();
    }

    /// <summary>
    /// 检查是否授予了指定权限，如果没有授予，则引发 <see cref="PermissionException"/> 异常
    /// </summary>
    public static async Task EnsureGrantedAsync<TPermission>()
        where TPermission : IBasePermission
    {
        var status = await RequestAsync<TPermission>();

        if (status != PermissionStatus.Granted)
            throw new PermissionException($"{typeof(TPermission).Name} permission was not granted: {status}");
    }

    /// <summary>
    /// 检查特定权限是否被授予或限制，如果没有授予或受限制，则引发 <see cref="PermissionException"/> 异常。
    /// </summary>
    public static async Task EnsureGrantedOrRestrictedAsync<TPermission>()
        where TPermission : IBasePermission
    {
        var status = await RequestAsync<TPermission>();

        if (status != PermissionStatus.Granted && status != PermissionStatus.Restricted)
            throw new PermissionException($"{typeof(TPermission).Name} permission was not granted or restricted: {status}");
    }

#pragma warning disable IDE1006 // 命名样式
#pragma warning disable SA1302 // Interface names should begin with I

    /// <summary>
    /// 表示访问电话号码的权限
    /// </summary>
    public interface PhoneNumber : IBasePermission { }

    /// <summary>
    /// 表示访问设备电池信息的权限
    /// </summary>
    public interface Battery : IBasePermission { }

    /// <summary>
    /// 表示通过蓝牙通信的权限（扫描、连接和/或广告）
    /// </summary>
    public interface Bluetooth : IBasePermission { }

    /// <summary>
    /// 表示读取设备日历信息的权限
    /// </summary>
    public interface CalendarRead : IBasePermission { }

    /// <summary>
    /// 表示写入设备日历数据的权限
    /// </summary>
    public interface CalendarWrite : IBasePermission { }

    /// <summary>
    /// 表示访问设备摄像头的权限
    /// </summary>
    public interface Camera : IBasePermission { }

    /// <summary>
    /// 表示读取设备联系人信息的权限
    /// </summary>
    public interface ContactsRead : IBasePermission { }

    /// <summary>
    /// 表示写入设备联系人数据的权限
    /// </summary>
    public interface ContactsWrite : IBasePermission { }

    /// <summary>
    /// 表示访问设备手电筒的权限
    /// </summary>
    public interface Flashlight : IBasePermission { }

    /// <summary>
    /// 表示在设备上启动其他应用程序的权限
    /// </summary>
    public interface LaunchApp : IBasePermission { }

    /// <summary>
    /// 表示仅当应用程序正在使用时访问设备位置的权限
    /// </summary>
    public interface LocationWhenInUse : IBasePermission { }

    /// <summary>
    /// 表示访问设备位置的权限，始终
    /// </summary>
    public interface LocationAlways : IBasePermission { }

    /// <summary>
    /// 表示访问设备映射应用程序的权限
    /// </summary>
    public interface Maps : IBasePermission { }

    /// <summary>
    /// 表示从设备库访问媒体的权限
    /// </summary>
    public interface Media : IBasePermission { }

    /// <summary>
    /// 表示访问设备麦克风的权限
    /// </summary>
    public interface Microphone : IBasePermission { }

    /// <summary>
    /// 表示访问附近 WiFi 设备的权限
    /// </summary>
    public interface NearbyWifiDevices : IBasePermission { }

    /// <summary>
    /// 表示访问设备网络状态信息的权限
    /// </summary>
    public interface NetworkState : IBasePermission { }

    /// <summary>
    /// 表示访问设备电话数据的权限
    /// </summary>
    public interface Phone : IBasePermission { }

    /// <summary>
    /// 表示访问设备库中照片的权限
    /// </summary>
    public interface Photos : IBasePermission { }

    /// <summary>
    /// 表示向设备库添加照片（非读取）的权限
    /// </summary>
    public interface PhotosAddOnly : IBasePermission { }

    /// <summary>
    /// 表示访问设备提醒数据的权限
    /// </summary>
    public interface Reminders : IBasePermission { }

    /// <summary>
    /// 表示访问设备传感器的权限
    /// </summary>
    public interface Sensors : IBasePermission { }

    /// <summary>
    /// 表示访问设备 SMS 数据的权限
    /// </summary>
    public interface Sms : IBasePermission { }

    /// <summary>
    /// 表示访问设备语音功能的权限
    /// </summary>
    public interface Speech : IBasePermission { }

    /// <summary>
    /// 表示读取设备存储的权限
    /// </summary>
    public interface StorageRead : IBasePermission { }

    /// <summary>
    /// 表示对设备存储的写入权限
    /// </summary>
    public interface StorageWrite : IBasePermission { }

    /// <summary>
    /// 表示访问设备振动电机的权限
    /// </summary>
    public interface Vibrate : IBasePermission { }

#pragma warning restore SA1302 // Interface names should begin with I
#pragma warning restore IDE1006 // 命名样式
}
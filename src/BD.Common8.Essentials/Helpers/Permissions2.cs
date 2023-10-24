namespace BD.Common8.Essentials.Helpers;

#pragma warning disable SA1600 // Elements should be documented

/// <summary>
/// The Permissions API provides the ability to check and request runtime permissions.
/// </summary>
public static partial class Permissions2
{
    /// <summary>
    /// Retrieves the current status of the permission.
    /// </summary>
    /// <remarks>
    /// Will throw <see cref="PermissionException"/> if a required entry was not found in the application manifest.
    /// Not all permissions require a manifest entry.
    /// </remarks>
    /// <exception cref="PermissionException">Thrown if a required entry was not found in the application manifest.</exception>
    /// <typeparam name="TPermission">The permission type to check.</typeparam>
    /// <returns>A <see cref="PermissionStatus"/> value indicating the current status of the permission.</returns>
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
    /// Requests the permission from the user for this application.
    /// </summary>
    /// <remarks>
    /// Will throw <see cref="PermissionException"/> if a required entry was not found in the application manifest.
    /// Not all permissions require a manifest entry.
    /// </remarks>
    /// <exception cref="PermissionException">Thrown if a required entry was not found in the application manifest.</exception>
    /// <typeparam name="TPermission">The permission type to check.</typeparam>
    /// <returns>A <see cref="PermissionStatus"/> value indicating the result of this permission request.</returns>
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
    /// Determines if an educational UI should be displayed explaining to the user how the permission will be used in the application.
    /// </summary>
    /// <remarks>Only used on Android, other platforms will always return <see langword="false"/>.</remarks>
    /// <typeparam name="TPermission">The permission type to check.</typeparam>
    /// <returns><see langword="true"/> if the user has denied or disabled the permission in the past, else <see langword="false"/>.</returns>
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

    public static void EnsureDeclared<TPermission>()
        where TPermission : IBasePermission
    {
        var permissionsPlatformService = IPermissionsPlatformService.Instance;
        permissionsPlatformService?.EnsureDeclared<TPermission>();
    }

    public static async Task EnsureGrantedAsync<TPermission>()
        where TPermission : IBasePermission
    {
        var status = await RequestAsync<TPermission>();

        if (status != PermissionStatus.Granted)
            throw new PermissionException($"{typeof(TPermission).Name} permission was not granted: {status}");
    }

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
    /// Represents permission to access phone number.
    /// </summary>
    public interface PhoneNumber : IBasePermission { }

    /// <summary>
    /// Represents permission to access the device battery information.
    /// </summary>
    public interface Battery : IBasePermission { }

    /// <summary>
    /// Represents permission to communicate via Bluetooth (scanning, connecting and/or advertising).
    /// </summary>
    public interface Bluetooth : IBasePermission { }

    /// <summary>
    /// Represents permission to read the device calendar information.
    /// </summary>
    public interface CalendarRead : IBasePermission { }

    /// <summary>
    /// Represents permission to write to the device calendar data.
    /// </summary>
    public interface CalendarWrite : IBasePermission { }

    /// <summary>
    /// Represents permission to access the device camera.
    /// </summary>
    public interface Camera : IBasePermission { }

    /// <summary>
    /// Represents permission to read the device contacts information.
    /// </summary>
    public interface ContactsRead : IBasePermission { }

    /// <summary>
    /// Represents permission to write to the device contacts data.
    /// </summary>
    public interface ContactsWrite : IBasePermission { }

    /// <summary>
    /// Represents permission to access the device flashlight.
    /// </summary>
    public interface Flashlight : IBasePermission { }

    /// <summary>
    /// Represents permission to launch other apps on the device.
    /// </summary>
    public interface LaunchApp : IBasePermission { }

    /// <summary>
    /// Represents permission to access the device location, only when the app is in use.
    /// </summary>
    public interface LocationWhenInUse : IBasePermission { }

    /// <summary>
    /// Represents permission to access the device location, always.
    /// </summary>
    public interface LocationAlways : IBasePermission { }

    /// <summary>
    /// Represents permission to access the device maps application.
    /// </summary>
    public interface Maps : IBasePermission { }

    /// <summary>
    /// Represents permission to access media from the device gallery.
    /// </summary>
    public interface Media : IBasePermission { }

    /// <summary>
    /// Represents permission to access the device microphone.
    /// </summary>
    public interface Microphone : IBasePermission { }

    /// <summary>
    /// Represents permission to access nearby WiFi devices.
    /// </summary>
    public interface NearbyWifiDevices : IBasePermission { }

    /// <summary>
    /// Represents permission to access the device network state information.
    /// </summary>
    public interface NetworkState : IBasePermission { }

    /// <summary>
    /// Represents permission to access the device phone data.
    /// </summary>
    public interface Phone : IBasePermission { }

    /// <summary>
    /// Represents permission to access photos from the device gallery.
    /// </summary>
    public interface Photos : IBasePermission { }

    /// <summary>
    /// Represents permission to add photos (not read) to the device gallery.
    /// </summary>
    public interface PhotosAddOnly : IBasePermission { }

    /// <summary>
    /// Represents permission to access the device reminders data.
    /// </summary>
    public interface Reminders : IBasePermission { }

    /// <summary>
    /// Represents permission to access the device sensors.
    /// </summary>
    public interface Sensors : IBasePermission { }

    /// <summary>
    /// Represents permission to access the device SMS data.
    /// </summary>
    public interface Sms : IBasePermission { }

    /// <summary>
    /// Represents permission to access the device speech capabilities.
    /// </summary>
    public interface Speech : IBasePermission { }

    /// <summary>
    /// Represents permission to read the device storage.
    /// </summary>
    public interface StorageRead : IBasePermission { }

    /// <summary>
    /// Represents permission to write to the device storage.
    /// </summary>
    public interface StorageWrite : IBasePermission { }

    /// <summary>
    /// Represents permission to access the device vibration motor.
    /// </summary>
    public interface Vibrate : IBasePermission { }

#pragma warning restore SA1302 // Interface names should begin with I
#pragma warning restore IDE1006 // 命名样式
}
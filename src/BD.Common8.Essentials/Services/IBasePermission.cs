namespace BD.Common8.Essentials.Services;

/// <summary>
/// Represents the abstract base class for all permissions.
/// </summary>
public interface IBasePermission
{
    /// <summary>
    /// Retrieves the current status of this permission.
    /// </summary>
    /// <remarks>
    /// Will throw <see cref="PermissionException"/> if a required entry was not found in the application manifest.
    /// Not all permissions require a manifest entry.
    /// </remarks>
    /// <exception cref="PermissionException">Thrown if a required entry was not found in the application manifest.</exception>
    /// <returns>A <see cref="PermissionStatus"/> value indicating the current status of this permission.</returns>
    Task<PermissionStatus> CheckStatusAsync();

    /// <summary>
    /// Requests this permission from the user for this application.
    /// </summary>
    /// <remarks>
    /// Will throw <see cref="PermissionException"/> if a required entry was not found in the application manifest.
    /// Not all permissions require a manifest entry.
    /// </remarks>
    /// <exception cref="PermissionException">Thrown if a required entry was not found in the application manifest.</exception>
    /// <returns>A <see cref="PermissionStatus"/> value indicating the result of this permission request.</returns>
    Task<PermissionStatus> RequestAsync();

    /// <summary>
    /// Ensures that a required entry matching this permission is found in the application manifest file.
    /// </summary>
    /// <remarks>
    /// Will throw <see cref="PermissionException"/> if a required entry was not found in the application manifest.
    /// Not all permissions require a manifest entry.
    /// </remarks>
    /// <exception cref="PermissionException">Thrown if a required entry was not found in the application manifest.</exception>
    void EnsureDeclared();

    /// <summary>
    /// Determines if an educational UI should be displayed explaining to the user how this permission will be used in the application.
    /// </summary>
    /// <remarks>Only used on Android, other platforms will always return <see langword="false"/>.</remarks>
    /// <returns><see langword="true"/> if the user has denied or disabled this permission in the past, else <see langword="false"/>.</returns>
    bool ShouldShowRationale();
}

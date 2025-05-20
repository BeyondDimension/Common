namespace BD.Common8.Essentials.Services.Implementation;

/// <summary>
/// https://github.com/dotnet/maui/tree/8.0.0-rc.2.9373/src/Essentials/src/DeviceInfo
/// </summary>
public partial class DeviceInfoPlatformServiceImpl : IDeviceInfoPlatformService
{
    /// <inheritdoc/>
    public virtual partial string Model { get; }

    /// <inheritdoc/>
    public virtual partial string Manufacturer { get; }

    /// <inheritdoc/>
    public virtual partial string Name { get; }

    /// <inheritdoc/>
    public virtual partial string VersionString { get; }

    /// <inheritdoc/>
    public virtual partial global::BD.Common8.Essentials.Enums.DeviceType DeviceType { get; }

    /// <inheritdoc/>
    public virtual partial global::BD.Common8.Essentials.Enums.DeviceIdiom Idiom { get; }

    /// <inheritdoc/>
    public virtual partial bool IsChromeOS { get; }
}

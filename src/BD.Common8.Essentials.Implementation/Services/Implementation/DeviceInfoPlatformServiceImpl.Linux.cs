#if LINUX
namespace BD.Common8.Essentials.Services.Implementation;

partial class DeviceInfoPlatformServiceImpl
{
    /// <inheritdoc/>
    public virtual partial string Model
    {
        get
        {
            return string.Empty;
        }
    }

    /// <inheritdoc/>
    public virtual partial string Manufacturer
    {
        get
        {
            return string.Empty;
        }
    }

    /// <inheritdoc/>
    public virtual partial string Name
    {
        get
        {
            return string.Empty;
        }
    }

    /// <inheritdoc/>
    public virtual partial string VersionString
    {
        get
        {
            var value = Environment.OSVersion.Version.ToString();
            return value ?? string.Empty;
        }
    }

    /// <inheritdoc/>
    public virtual partial global::BD.Common8.Essentials.Enums.DeviceType DeviceType
    {
        get
        {
            return global::BD.Common8.Essentials.Enums.DeviceType.Physical;
        }
    }

    /// <inheritdoc/>
    public virtual partial global::BD.Common8.Essentials.Enums.DeviceIdiom Idiom
    {
        get
        {
            return global::BD.Common8.Essentials.Enums.DeviceIdiom.Desktop;
        }
    }

    /// <inheritdoc/>
    public virtual partial bool IsChromeOS
    {
        get
        {
            return false;
        }
    }
}
#endif
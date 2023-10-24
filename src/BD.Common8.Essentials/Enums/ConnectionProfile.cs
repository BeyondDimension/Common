namespace BD.Common8.Essentials.Enums;

/// <summary>
/// Describes the type of connection the device is using.
/// </summary>
public enum ConnectionProfile
{
    /// <summary>Other unknown type of connection.</summary>
    Unknown = 0,

    /// <summary>The bluetooth data connection.</summary>
    Bluetooth = 1,

    /// <summary>The mobile/cellular data connection.</summary>
    Cellular = 2,

    /// <summary>The ethernet data connection.</summary>
    Ethernet = 3,

    /// <summary>The Wi-Fi data connection.</summary>
    WiFi = 4,
}
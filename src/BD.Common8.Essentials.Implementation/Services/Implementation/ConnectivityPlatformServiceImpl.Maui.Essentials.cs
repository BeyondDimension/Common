#if !LINUX
namespace BD.Common8.Essentials.Services.Implementation;

partial class ConnectivityPlatformServiceImpl
{
    static global::BD.Common8.Essentials.Enums.NetworkAccess Convert(global::Microsoft.Maui.Networking.NetworkAccess value)
        => unchecked((global::BD.Common8.Essentials.Enums.NetworkAccess)value);

    static global::BD.Common8.Essentials.Enums.ConnectionProfile Convert(global::Microsoft.Maui.Networking.ConnectionProfile value)
        => unchecked((global::BD.Common8.Essentials.Enums.ConnectionProfile)value);

    /// <inheritdoc/>
    global::BD.Common8.Essentials.Enums.NetworkAccess IConnectivityPlatformService.NetworkAccess
        => Convert(global::Microsoft.Maui.Networking.Connectivity.NetworkAccess);

    /// <inheritdoc/>
    IEnumerable<global::BD.Common8.Essentials.Enums.ConnectionProfile> IConnectivityPlatformService.ConnectionProfiles
        => global::Microsoft.Maui.Networking.Connectivity.ConnectionProfiles.Select(Convert);
}
#endif
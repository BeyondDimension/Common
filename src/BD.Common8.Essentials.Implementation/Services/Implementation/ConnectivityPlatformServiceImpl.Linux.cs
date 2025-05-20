#if LINUX
using BD.Common8.Essentials.Enums;

namespace BD.Common8.Essentials.Services.Implementation;

partial class ConnectivityPlatformServiceImpl
{
    /// <inheritdoc/>
    NetworkAccess IConnectivityPlatformService.NetworkAccess => NetworkAccess.Internet;

    /// <inheritdoc/>
    IEnumerable<ConnectionProfile> IConnectivityPlatformService.ConnectionProfiles => [ConnectionProfile.Ethernet];
}
#endif
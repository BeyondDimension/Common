namespace BD.Common8.Essentials.Helpers;

#pragma warning disable SA1600 // Elements should be documented

public static class Connectivity2
{
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static NetworkAccess NetworkAccess()
    {
        var connectivityPlatformService = IConnectivityPlatformService.Interface;
        if (connectivityPlatformService == null)
            return default;
        return connectivityPlatformService.NetworkAccess;
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static IEnumerable<ConnectionProfile> ConnectionProfiles()
    {
        var connectivityPlatformService = IConnectivityPlatformService.Interface;
        if (connectivityPlatformService == null)
            return [];
        return connectivityPlatformService.ConnectionProfiles;
    }
}
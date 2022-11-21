namespace BD.Common;

public static class Connectivity2
{
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static NetworkAccess NetworkAccess()
    {
        var i = IConnectivityPlatformService.Interface;
        if (i == null) return default;
        return i.NetworkAccess;
    }
}
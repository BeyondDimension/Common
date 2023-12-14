using NetworkAccess = BD.Common8.Enums.NetworkAccess;

namespace BD.Common8.Essentials.Helpers;

/// <summary>
/// 提供网络连接相关的辅助方法
/// </summary>
public static class Connectivity2
{
    /// <summary>
    /// 获取当前网络访问的状态
    /// </summary>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static NetworkAccess NetworkAccess()
    {
        var connectivityPlatformService = IConnectivityPlatformService.Interface;
        if (connectivityPlatformService == null)
            return default;
        return connectivityPlatformService.NetworkAccess;
    }

    /// <summary>
    /// 获取当前连接的配置文件列表
    /// </summary>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static IEnumerable<ConnectionProfile> ConnectionProfiles()
    {
        var connectivityPlatformService = IConnectivityPlatformService.Interface;
        if (connectivityPlatformService == null)
            return [];
        return connectivityPlatformService.ConnectionProfiles;
    }
}
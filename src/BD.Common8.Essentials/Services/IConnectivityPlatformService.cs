namespace BD.Common8.Essentials.Services;

/// <summary>
/// 提供了获取设备网络连接状态的功能
/// </summary>
public interface IConnectivityPlatformService
{
    /// <summary>
    /// 获取 <see cref="IConnectivityPlatformService"/>实例
    /// </summary>
    static IConnectivityPlatformService? Interface => Ioc.Get_Nullable<IConnectivityPlatformService>();

    /// <summary>
    /// 获取当前网络访问状态
    /// </summary>
    NetworkAccess NetworkAccess { get; }

    /// <summary>
    /// 获取当前连接配置文件的集合
    /// </summary>
    IEnumerable<ConnectionProfile> ConnectionProfiles { get; }
}

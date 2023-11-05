namespace BD.Common8.Essentials.Enums;

/// <summary>
/// 描述设备正在使用的连接类型
/// </summary>
public enum ConnectionProfile
{
    /// <summary>其他未知类型的连接</summary>
    Unknown = 0,

    /// <summary>蓝牙数据连接</summary>
    Bluetooth = 1,

    /// <summary>移动/蜂窝数据连接</summary>
    Cellular = 2,

    /// <summary>以太网数据连接</summary>
    Ethernet = 3,

    /// <summary>Wi-Fi 数据连接</summary>
    WiFi = 4,
}
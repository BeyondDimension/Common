namespace BD.Common8.Primitives.Essentials.Enums;

/// <summary>
/// 权限的可能状态
/// </summary>
public enum PermissionStatus : byte
{
    /// <summary>权限处于未知状态</summary>
    Unknown = 0,

    /// <summary>用户拒绝了权限请求</summary>
    Denied = 1,

    /// <summary>该功能在设备上被禁用</summary>
    Disabled = 2,

    /// <summary>授予权限的用户或自动授予的用户</summary>
    Granted = 3,

    /// <summary>处于受限状态</summary>
    Restricted = 4,

    /// <summary>处于受限状态（仅限 iOS）</summary>
    Limited = 5,
}
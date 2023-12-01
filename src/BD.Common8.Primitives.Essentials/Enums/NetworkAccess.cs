namespace BD.Common8.Enums;

/// <summary>
/// 与互联网连接的各种状态
/// </summary>
public enum NetworkAccess : byte
{
    /// <summary>连接状态未知</summary>
    Unknown = 0,

    /// <summary>无连接</summary>
    None = 1,

    /// <summary>仅本地网络访问</summary>
    Local = 2,

    /// <summary>有限的互联网接入</summary>
    ConstrainedInternet = 3,

    /// <summary>本地和互联网接入</summary>
    Internet = 4,
}
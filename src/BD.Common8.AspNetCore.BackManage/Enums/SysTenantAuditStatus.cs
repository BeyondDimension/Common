namespace BD.Common8.AspNetCore.Enums;

/// <summary>
/// 系统租户审核状态
/// </summary>
public enum SysTenantAuditStatus : byte
{
    /// <summary>
    /// 草稿
    /// </summary>
    Draft = 0,

    /// <summary>
    /// 待审核
    /// </summary>
    Pending = 1,

    /// <summary>
    /// 审核中
    /// </summary>
    InProgress = 2,

    /// <summary>
    /// 审核通过
    /// </summary>
    Approved = 3,

    /// <summary>
    /// 审核不通过
    /// </summary>
    Rejected = 4,
}
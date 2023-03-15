namespace BD.Common.Entities;

/// <summary>
/// 系统信息实体类
/// </summary>
[Table("BM_SystemInfos")]
public class SysInfo : INEWSEQUENTIALID, ITenant
{
    /// <summary>
    /// Id
    /// </summary>
    [Comment("Id")]
    public Guid Id { get; set; }

    /// <summary>
    /// 租户Id
    /// </summary>
    [Comment("租户Id")]
    public Guid TenantId { get; set; }

    /// <summary>
    /// 网站名称
    /// </summary>
    [Comment("网站名称")]
    [MaxLength(100)]
    public string WebsiteName { get; set; } = string.Empty;

    /// <summary>
    /// 网站域名
    /// </summary>
    [Comment("网站域名")]
    [MaxLength(150)]
    public string WebsiteDomainName { get; set; } = string.Empty;
}
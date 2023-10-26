namespace BD.Common8.AspNetCore.Entities;

/// <summary>
/// 后台系统信息实体类
/// </summary>
[Table("BM_SystemInfos")]
public sealed class SysInfo : Entity<Guid>, INEWSEQUENTIALID, ITenant
{
    [Comment("租户 Id")]
    public Guid TenantId { get; set; }

    /// <summary>
    /// 后台网站名称
    /// </summary>
    [Required]
    [Comment("网站名称")]
    [MaxLength(MaxLengths.LongName)]
    public string WebsiteName { get; set; } = "";

    /// <summary>
    /// 后台网站域名
    /// </summary>
    [Required]
    [Comment("网站域名")]
    [MaxLength(MaxLengths.Url)]
    public string WebsiteDomainName { get; set; } = "";
}
using BD.Common8.Columns;
using BD.Common8.Entities.Abstractions;
using BD.Common8.Orm.EFCore.Columns;
using Microsoft.EntityFrameworkCore;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace BD.Common8.AspNetCore.Entities;

/// <summary>
/// 后台系统信息实体类
/// </summary>
[Table("BM_SystemInfos")]
public sealed class BMInfo : Entity<Guid>, INEWSEQUENTIALID, ITenant
{
    /// <inheritdoc/>
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
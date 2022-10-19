using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BD.Common.Entities;

public class TenantInfo : TenantEntity<Guid>
{
    public const int MaxLength_Address = 150;

    /// <summary>
    /// 按钮多语言名称
    /// </summary>
    [Required]
    [MaxLength(SharedMaxLengths.NickName)]
    public string Name { get; set; }

    /// <summary>
    /// 唯一编码
    /// </summary>
    public Guid OnlyCode { get; set; }

    /// <summary>
    /// 联系人名称
    /// </summary>
    [Required]
    [MaxLength(SharedMaxLengths.NickName)]
    public string ContactName { get; set; }

    /// <summary>
    /// 联系人电话
    /// </summary>
    [Required]
    [MaxLength(SharedMaxLengths.PhoneNumber)]
    public string ContactPhone { get; set; }

    /// <summary>
    /// 地址
    /// </summary>
    [Required]
    [MaxLength(MaxLength_Address)]
    public string Address { get; set; }

    /// <summary>
    /// 按钮多语言名称
    /// </summary>
    [Required]
    [MaxLength(SharedMaxLengths.Email)]
    public string Email { get; set; }
}

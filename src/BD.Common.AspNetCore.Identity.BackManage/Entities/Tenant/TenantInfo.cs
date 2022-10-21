namespace BD.Common.Entities;

public class TenantInfo<TBMUser> :
    OperatorEntity<Guid, TBMUser>,
    ISoftDeleted
    where TBMUser : BMUser
{
    /// <summary>
    /// 是否删除
    /// </summary>
    public bool SoftDeleted { get; set; }

    /// <summary>
    /// 名称
    /// </summary>
    [Required]
    [MaxLength(MaxLengths.NickName)]
    public string Name { get; set; }

    /// <summary>
    /// 唯一编码
    /// </summary>
    public Guid OnlyCode { get; set; }

    /// <summary>
    /// 联系人名称
    /// </summary>
    [Required]
    [MaxLength(MaxLengths.NickName)]
    public string ContactName { get; set; }

    /// <summary>
    /// 联系人电话
    /// </summary>
    [Required]
    [MaxLength(PhoneNumberHelper.ChineseMainlandPhoneNumberLength)]
    public string ContactPhone { get; set; }

    /// <summary>
    /// 地址
    /// </summary>
    [Required]
    [MaxLength(MaxLengths.RealityAddress)]
    public string Address { get; set; }

    /// <summary>
    /// 按钮多语言名称
    /// </summary>
    [Required]
    [MaxLength(MaxLengths.Email)]
    public string Email { get; set; }
}

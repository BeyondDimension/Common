namespace BD.Common.Entities;

public class BMRoleEntity : TenantEntity<Guid>, IOrder
{
    [MaxLength(MaxLengths.NickName)]
    [Required]
    public string Name { get; set; }

    [MaxLength(MaxLengths.Describe)]
    public string Describe { get; set; }

    public long Order { get; set; }
}

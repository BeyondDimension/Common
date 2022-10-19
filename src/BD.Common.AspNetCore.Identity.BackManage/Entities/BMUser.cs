// ReSharper disable once CheckNamespace
namespace BD.Common.Entities;

public partial class BMUser : JWTUser
{
    /// <summary>
    ///  角色ID
    /// </summary>
    public Guid AuthorityId { get; set; }

}

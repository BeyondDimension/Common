// ReSharper disable once CheckNamespace
namespace BD.Common.Models;

#if BLAZOR
partial class UserInfoDTO : IDeleteLoading
{
    [JsonIgnore]
    public bool DeleteLoading { get; set; }

    [JsonIgnore]
    public string RolesString
    {
        get => RoleEnumHelper.ToDisplayString(Roles);
        set => Roles = RoleEnumHelper.Parse(value);
    }
}
#endif
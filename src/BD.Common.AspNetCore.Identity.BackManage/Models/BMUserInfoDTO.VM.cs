// ReSharper disable once CheckNamespace
namespace BD.Common.Models;

#if BLAZOR
partial class BMUserInfoDTO : IDeleteLoading
{
    [JsonIgnore]
    public bool DeleteLoading { get; set; }

    [JsonIgnore]
    public string RolesString
    {
        get => BMRoleEnumHelper.ToDisplayString(Roles);
        set => Roles = BMRoleEnumHelper.Parse(value);
    }
}
#endif
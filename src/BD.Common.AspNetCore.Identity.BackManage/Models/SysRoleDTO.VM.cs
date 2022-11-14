namespace BD.Common.Models;

#if BLAZOR
partial class SysRoleDTO : IDeleteLoading
{
    public bool DeleteLoading { get; set; }
}
#endif
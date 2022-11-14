// ReSharper disable once CheckNamespace
namespace BD.Common.Models;

#if BLAZOR
partial class SysTenantDTO : IDeleteLoading
{
    public bool DeleteLoading { get; set; }
}
#endif

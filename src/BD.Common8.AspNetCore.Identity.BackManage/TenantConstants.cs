namespace BD.Common8.AspNetCore;

public static class TenantConstants
{
    public const string RootTenantId = "10000000000000000000000000000000";

    public static readonly Guid RootTenantIdG = Guid.ParseExact(RootTenantId, "N");
}
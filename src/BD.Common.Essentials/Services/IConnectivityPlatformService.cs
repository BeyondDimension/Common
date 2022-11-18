namespace BD.Common.Services;

interface IConnectivityPlatformService
{
    static IConnectivityPlatformService? Interface => Ioc.Get_Nullable<IConnectivityPlatformService>();

    NetworkAccess NetworkAccess { get; }
}

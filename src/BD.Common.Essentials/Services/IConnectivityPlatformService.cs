namespace BD.Common.Services;

public interface IConnectivityPlatformService
{
    static IConnectivityPlatformService? Interface => Ioc.Get_Nullable<IConnectivityPlatformService>();

    NetworkAccess NetworkAccess { get; }
}

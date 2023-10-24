namespace BD.Common8.Essentials.Services;

#pragma warning disable SA1600 // Elements should be documented

public interface IConnectivityPlatformService
{
    static IConnectivityPlatformService? Interface => Ioc.Get_Nullable<IConnectivityPlatformService>();

    NetworkAccess NetworkAccess { get; }

    IEnumerable<ConnectionProfile> ConnectionProfiles { get; }
}

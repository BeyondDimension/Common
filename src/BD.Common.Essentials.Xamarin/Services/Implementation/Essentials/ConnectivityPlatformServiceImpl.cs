// ReSharper disable once CheckNamespace
namespace BD.Common.Services.Implementation.Essentials;

sealed class ConnectivityPlatformServiceImpl : IConnectivityPlatformService
{
    NetworkAccess IConnectivityPlatformService.NetworkAccess
        => Connectivity.NetworkAccess.Convert();
}

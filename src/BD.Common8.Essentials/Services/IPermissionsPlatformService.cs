namespace BD.Common8.Essentials.Services;

#pragma warning disable SA1600 // Elements should be documented

public interface IPermissionsPlatformService
{
    static IPermissionsPlatformService? Instance => Ioc.Get_Nullable<IPermissionsPlatformService>();

    /// <inheritdoc cref="Permissions2.CheckStatusAsync{TPermission}"/>
    Task<PermissionStatus> CheckStatusAsync<TPermission>() where TPermission : IBasePermission;

    /// <inheritdoc cref="Permissions2.RequestAsync{TPermission}"/>
    Task<PermissionStatus> RequestAsync<TPermission>() where TPermission : IBasePermission;

    /// <inheritdoc cref="Permissions2.ShouldShowRationale{TPermission}"/>
    bool ShouldShowRationale<TPermission>() where TPermission : IBasePermission;

    /// <inheritdoc cref="Permissions2.EnsureDeclared{TPermission}"/>
    void EnsureDeclared<TPermission>() where TPermission : IBasePermission;
}

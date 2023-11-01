namespace BD.Common8.Essentials.Services;

/// <summary>
/// 提供权限平台服务接口
/// </summary>
public interface IPermissionsPlatformService
{
    /// <summary>
    /// 获取 <see cref="IPermissionsPlatformService"/>  的实例
    /// </summary>
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

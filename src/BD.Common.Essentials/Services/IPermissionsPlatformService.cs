namespace BD.Common.Services;

interface IPermissionsPlatformService
{
    static IPermissionsPlatformService Instance => Ioc.Get<IPermissionsPlatformService>();

    /// <summary>
    /// 检查并申请一组权限
    /// </summary>
    /// <param name="permission"></param>
    /// <returns></returns>
    Task<PermissionStatus> CheckAndRequestAsync(object permission);

    /// <summary>
    /// 检查并申请一组权限
    /// </summary>
    /// <typeparam name="TPermission"></typeparam>
    /// <returns></returns>
    Task<PermissionStatus> CheckAndRequestAsync<TPermission>() where TPermission : notnull
    {
        TPermission permission;
        var typePermission = typeof(TPermission);
        if (typePermission.IsInterface)
        {
            permission = Ioc.Get<TPermission>();
        }
        else
        {
#pragma warning disable IL2091 // Target generic argument does not satisfy 'DynamicallyAccessedMembersAttribute' in target method or type. The generic parameter of the source method or type does not have matching annotations.
            permission = Activator.CreateInstance<TPermission>();
#pragma warning restore IL2091 // Target generic argument does not satisfy 'DynamicallyAccessedMembersAttribute' in target method or type. The generic parameter of the source method or type does not have matching annotations.
            if (permission == null) throw new ArgumentNullException(nameof(permission));
        }
        return CheckAndRequestAsync(permission);
    }
}

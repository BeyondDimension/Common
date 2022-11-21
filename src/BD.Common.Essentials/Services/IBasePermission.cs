namespace BD.Common.Services;

public interface IBasePermission
{
    Task<PermissionStatus> CheckStatusAsync();

    Task<PermissionStatus> RequestAsync();

    void EnsureDeclared();

    bool ShouldShowRationale();
}

public interface IBasePermission<TPermission> : IBasePermission where TPermission : notnull
{
    static TPermission Instance => Ioc.Get<TPermission>();
}

// ReSharper disable once CheckNamespace
namespace BD.Common;

public sealed class PermissionAuthorizationRequirement : IAuthorizationRequirement
{
    public PermissionAuthorizationRequirement(string controllerName)
    {
        ControllerName = controllerName;
    }

    /// <summary>
    /// 控制器名称
    /// </summary>
    public string ControllerName { get; set; }
}

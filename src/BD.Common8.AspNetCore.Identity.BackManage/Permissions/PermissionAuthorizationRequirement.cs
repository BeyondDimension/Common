namespace BD.Common8.AspNetCore.Permissions;

/// <summary>
/// 权限授权要求类，用于定义控制器的权限授权需求
/// </summary>
public sealed class PermissionAuthorizationRequirement(string controllerName) : IAuthorizationRequirement
{
    /// <summary>
    /// 控制器名称
    /// </summary>
    public string ControllerName { get; set; } = controllerName;
}

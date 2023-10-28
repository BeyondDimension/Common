namespace BD.Common8.AspNetCore.Permissions;

#pragma warning disable SA1600 // Elements should be documented

public sealed class PermissionAuthorizationRequirement(string controllerName) : IAuthorizationRequirement
{
    /// <summary>
    /// 控制器名称
    /// </summary>
    public string ControllerName { get; set; } = controllerName;
}

namespace BD.Common8.Essentials.Services;

/// <summary>
/// 表示所有权限的抽象基类
/// </summary>
public interface IBasePermission
{
    /// <summary>
    /// 检索此权限的当前状态
    /// </summary>
    /// <remarks>
    /// 如果在应用程序清单中找不到所需的条目，将抛出 <see cref="PermissionException"/>
    /// 并非所有权限都需要清单条目
    /// </remarks>
    /// <exception cref="PermissionException">如果在应用程序清单中找不到所需的条目，则引发</exception>
    /// <returns><see cref="PermissionStatus"/>指示此权限的当前状态的值</returns>
    Task<PermissionStatus> CheckStatusAsync();

    /// <summary>
    /// 向用户请求此应用程序的此权限
    /// </summary>
    /// <remarks>
    /// 如果在应用程序清单中找不到所需的条目，将抛出 <see cref="PermissionException"/>
    /// 并非所有权限都需要清单条目
    /// </remarks>
    /// <exception cref="PermissionException">如果在应用程序清单中找不到所需的条目，则引发</exception>
    /// <returns><see cref="PermissionStatus"/> 指示此权限请求的结果的值</returns>
    Task<PermissionStatus> RequestAsync();

    /// <summary>
    /// 确保在应用程序清单文件中找到与此权限匹配的必需条目
    /// </summary>
    /// <remarks>
    /// 如果在应用程序清单中找不到所需的条目，将抛出 <see cref="PermissionException"/>
    /// 并非所有权限都需要清单条目
    /// </remarks>
    /// <exception cref="PermissionException">如果在应用程序清单中找不到所需的条目，则引发</exception>
    void EnsureDeclared();

    /// <summary>
    /// 确定是否应显示 Educational UI，向用户解释如何在应用程序中使用此权限
    /// </summary>
    /// <remarks>仅在Android上使用，其他平台将始终返回 <see langword="false"/></remarks>
    /// <returns><see langword="true"/> 如果用户过去拒绝或禁用过此权限，则返回 <see langword="false"/></returns>
    bool ShouldShowRationale();
}

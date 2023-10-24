namespace BD.Common8.Essentials.Services;

#pragma warning disable SA1600 // Elements should be documented

/// <summary>
/// 应用程序版本号服务，必须由应用层实现
/// </summary>
public interface IApplicationVersionService
{
    static IApplicationVersionService Instance => Ioc.Get<IApplicationVersionService>();

    /// <summary>
    /// 获取应用程序的当前版本号
    /// </summary>
    string ApplicationVersion { get; }

    /// <summary>
    /// 定义程序集清单的商标自定义属性
    /// </summary>
    string AssemblyTrademark { get; }
}

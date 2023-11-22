namespace BD.Common8.Settings5.Services;

/// <summary>
/// 设置项加载单例服务，无需添加到 <see cref="Ioc"/>
/// </summary>
public interface ISettingsLoadService
{
    /// <summary>
    /// 获取当前单例服务
    /// </summary>
    static ISettingsLoadService Current { get; protected set; } = null!;

    /// <summary>
    /// 设置项模型的 <see cref="DynamicallyAccessedMembersAttribute"/> 标注值
    /// </summary>
    const DynamicallyAccessedMemberTypes DAMT_M = DynamicallyAccessedMemberTypes.PublicParameterlessConstructor;

    /// <summary>
    /// 设置项文件存放文件夹名
    /// </summary>
    const string DirName = "Settings";

    /// <summary>
    /// 加载设置项，返回是否已经加载过
    /// </summary>
    /// <typeparam name="TSettingsModel">设置项模型</typeparam>
    /// <param name="configureServices">返回配置该设置项的服务配置委托</param>
    /// <param name="options">设置项监听接口</param>
    /// <param name="settingsFileDirectoryExists">设置项保存文件所在文件夹是否存在，传递此值以防止多次计算</param>
    /// <param name="settingsFileDirectory">设置项保存文件所在文件夹，默认将取 <see cref="IOPath.AppDataDirectory"/></param>
    /// <returns></returns>
    bool Load<[DynamicallyAccessedMembers(DAMT_M)] TSettingsModel>(
        out Action<IServiceCollection>? configureServices,
        out IOptionsMonitor<TSettingsModel>? options,
        bool settingsFileDirectoryExists,
        string? settingsFileDirectory = null) where TSettingsModel : class, new();

    /// <summary>
    /// 保存配置项到文件
    /// </summary>
    /// <typeparam name="TSettingsModel"></typeparam>
    /// <param name="settingsModel">新的值</param>
    /// <param name="force">是否忽略比较相等，强制保存写入文件，默认值：<see langword="true"/></param>
    void Save<[DynamicallyAccessedMembers(DAMT_M)] TSettingsModel>(TSettingsModel settingsModel, bool force = true) where TSettingsModel : class, new();
}

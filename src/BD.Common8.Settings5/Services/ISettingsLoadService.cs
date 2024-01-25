namespace BD.Common8.Settings5.Services;

/// <summary>
/// 设置项加载单例服务，无需添加到 <see cref="Ioc"/>
/// </summary>
public interface ISettingsLoadService
{
    /// <summary>
    /// 获取当前单例服务
    /// </summary>
    static ISettingsLoadService Current => SettingsLoadServiceImpl.Current;

    /// <summary>
    /// 加载设置项，返回是否已经加载过
    /// </summary>
    /// <typeparam name="TSettingsModel">设置项模型</typeparam>
    /// <param name="configureServices">返回配置该设置项的服务配置委托</param>
    /// <param name="options">设置项监听接口</param>
    /// <param name="settingsFileDirectory">设置项保存文件所在文件夹，默认将取 <see cref="IOPath.AppDataDirectory"/></param>
    /// <returns></returns>
    (bool isInvalid, Exception? ex, string settingsFileNameWithoutExtension) Load<[DynamicallyAccessedMembers(DynamicallyAccessedMemberTypes.All)] TSettingsModel>(
        out Action<IServiceCollection>? configureServices,
        out IOptionsMonitor<TSettingsModel>? options,
        string? settingsFileDirectory = null) where TSettingsModel : class, new();

    /// <summary>
    /// 获取依赖注入服务
    /// </summary>
    /// <typeparam name="T"></typeparam>
    /// <returns></returns>
    T Get<[DynamicallyAccessedMembers(DynamicallyAccessedMemberTypes.All)] T>() where T : notnull;

    /// <summary>
    /// 保存配置项到文件
    /// </summary>
    /// <typeparam name="TSettingsModel"></typeparam>
    /// <param name="settingsModel">新的值</param>
    /// <param name="force">是否忽略比较相等，强制保存写入文件，默认值：<see langword="true"/></param>
    void Save<[DynamicallyAccessedMembers(DynamicallyAccessedMemberTypes.All)] TSettingsModel>(TSettingsModel settingsModel, bool force = true) where TSettingsModel : class, new();

    /// <summary>
    /// 强制保存配置项到文件
    /// </summary>
    /// <typeparam name="TSettingsModel"></typeparam>
    void ForceSave<[DynamicallyAccessedMembers(DynamicallyAccessedMemberTypes.All)] TSettingsModel>() where TSettingsModel : class, new();
}

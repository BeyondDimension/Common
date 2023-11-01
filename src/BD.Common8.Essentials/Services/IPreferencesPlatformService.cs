namespace BD.Common8.Essentials.Services;

#pragma warning disable SA1600 // Elements should be documented
/// <summary>
/// 提供管理应用程序的偏好设置数据服务
/// </summary>
public interface IPreferencesPlatformService
{
    /// <summary>
    /// 获取 <see cref="IPreferencesPlatformService"/>  的实例
    /// </summary>
    static IPreferencesPlatformService Instance => Ioc.Get<IPreferencesPlatformService>();

    /// <summary>
    /// 是否包含指定键值的数据
    /// </summary>
    bool PlatformContainsKey(string key, string? sharedName);

    /// <summary>
    /// 移除指定键值的数据
    /// </summary>
    void PlatformRemove(string key, string? sharedName);

    /// <summary>
    /// 清空指定共享名称下的数据
    /// </summary>
    void PlatformClear(string? sharedName);

    /// <summary>
    /// 获取指定键值的字符串数据，若不存在则返回默认值
    /// </summary>
    string? PlatformGet(string key, string? defaultValue, string? sharedName);

    /// <summary>
    /// 获取指定键值的布尔型数据，若不存在则返回默认值
    /// </summary>
    bool PlatformGet(string key, bool defaultValue, string? sharedName);

    /// <summary>
    /// 获取指定键值的日期时间数据，若不存在则返回默认值
    /// </summary>
    DateTime PlatformGet(string key, DateTime defaultValue, string? sharedName);

    /// <summary>
    /// 获取指定键值的浮点数数据，若不存在则返回默认值
    /// </summary>
    double PlatformGet(string key, double defaultValue, string? sharedName);

    /// <summary>
    /// 获取指定键值的整数数据，若不存在则返回默认值
    /// </summary>
    int PlatformGet(string key, int defaultValue, string? sharedName);

    /// <summary>
    /// 获取指定键值的长整型数据，若不存在则返回默认值
    /// </summary>
    long PlatformGet(string key, long defaultValue, string? sharedName);

    /// <summary>
    /// 获取指定键值的浮点数数据，若不存在则返回默认值
    /// </summary>
    float PlatformGet(string key, float defaultValue, string? sharedName);

    /// <summary>
    /// 设置指定键值的字符串数据
    /// </summary>
    void PlatformSet(string key, string? value, string? sharedName);

    /// <summary>
    /// 设置指定键值的布尔型数据
    /// </summary>
    void PlatformSet(string key, bool value, string? sharedName);

    /// <summary>
    /// 设置指定键值的日期时间数据
    /// </summary>
    void PlatformSet(string key, DateTime value, string? sharedName);

    /// <summary>
    /// 设置指定键值的浮点数数据
    /// </summary>
    void PlatformSet(string key, double value, string? sharedName);

    /// <summary>
    /// 设置指定键值的整数数据
    /// </summary>
    void PlatformSet(string key, int value, string? sharedName);

    /// <summary>
    /// 设置指定键值的长整型数据
    /// </summary>
    void PlatformSet(string key, long value, string? sharedName);

    /// <summary>
    /// 设置指定键值的浮点数数据
    /// </summary>
    void PlatformSet(string key, float value, string? sharedName);
}

public interface IPreferencesGenericPlatformService : IPreferencesPlatformService
{
    /// <summary>
    /// 获取平台上指定键的值，并将其转换为指定类型 T，如果键不存在则返回 defaultValue
    /// </summary>
    T? PlatformGet<T>(string key, T? defaultValue, string? sharedName) where T : notnull, IConvertible;

    /// <summary>
    /// 设置平台上指定键的值
    /// </summary>
    void PlatformSet<T>(string key, T? value, string? sharedName) where T : notnull, IConvertible;

    string? IPreferencesPlatformService.PlatformGet(string key, string? defaultValue, string? sharedName) => PlatformGet(key, defaultValue, sharedName);

    bool IPreferencesPlatformService.PlatformGet(string key, bool defaultValue, string? sharedName) => PlatformGet(key, defaultValue, sharedName);

    DateTime IPreferencesPlatformService.PlatformGet(string key, DateTime defaultValue, string? sharedName) => DateTime.FromBinary(PlatformGet(key, defaultValue.ToBinary(), sharedName));

    double IPreferencesPlatformService.PlatformGet(string key, double defaultValue, string? sharedName) => PlatformGet(key, defaultValue, sharedName);

    int IPreferencesPlatformService.PlatformGet(string key, int defaultValue, string? sharedName) => PlatformGet(key, defaultValue, sharedName);

    long IPreferencesPlatformService.PlatformGet(string key, long defaultValue, string? sharedName) => PlatformGet(key, defaultValue, sharedName);

    float IPreferencesPlatformService.PlatformGet(string key, float defaultValue, string? sharedName) => PlatformGet(key, defaultValue, sharedName);

    void IPreferencesPlatformService.PlatformSet(string key, string? value, string? sharedName) => PlatformSet(key, value, sharedName);

    void IPreferencesPlatformService.PlatformSet(string key, bool value, string? sharedName) => PlatformSet(key, value, sharedName);

    void IPreferencesPlatformService.PlatformSet(string key, DateTime value, string? sharedName) => PlatformSet(key, value.ToBinary(), sharedName);

    void IPreferencesPlatformService.PlatformSet(string key, double value, string? sharedName) => PlatformSet(key, value, sharedName);

    void IPreferencesPlatformService.PlatformSet(string key, int value, string? sharedName) => PlatformSet(key, value, sharedName);

    void IPreferencesPlatformService.PlatformSet(string key, long value, string? sharedName) => PlatformSet(key, value, sharedName);

    void IPreferencesPlatformService.PlatformSet(string key, float value, string? sharedName) => PlatformSet(key, value, sharedName);
}
namespace BD.Common8.Settings5.Infrastructure;

/// <summary>
/// 设置属性
/// </summary>
/// <param name="propertyName">属性名</param>
/// <param name="autoSave">是否在设置值时自动保存，默认值 <see cref="DefaultAutoSave"/></param>
public abstract class SettingsProperty(string propertyName,
    bool autoSave = SettingsProperty.DefaultAutoSave)
{
    /// <summary>
    /// 属性名
    /// </summary>
    public string PropertyName { get; } = propertyName;

    /// <summary>
    /// 是否在设置值时自动保存
    /// </summary>
    public bool AutoSave { get; set; } = autoSave;

    /// <summary>
    /// 是否在设置值时自动保存的默认值
    /// </summary>
    protected const bool DefaultAutoSave = true;

    /// <summary>
    /// 触发属性通知
    /// </summary>
    /// <param name="notSave"></param>
    public abstract void RaiseValueChanged(bool notSave = false);

    /// <summary>
    /// 重置值
    /// </summary>
    public abstract void Reset();
}
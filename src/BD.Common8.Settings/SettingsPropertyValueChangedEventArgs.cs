namespace BD.Common8.Settings;

/// <summary>
/// 设置项属性之更改事件参数
/// </summary>
/// <typeparam name="TValue"></typeparam>
public sealed class SettingsPropertyValueChangedEventArgs<TValue>(TValue? oldValue, TValue? newValue) : EventArgs
{
    /// <summary>
    /// 旧的值
    /// </summary>
    public TValue? OldValue { get; } = oldValue;

    /// <summary>
    /// 新的值
    /// </summary>
    public TValue? NewValue { get; } = newValue;
}
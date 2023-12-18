namespace BD.Common8.Settings5.Infrastructure;

/// <summary>
/// 表示设置属性值更改事件参数的泛型类
/// </summary>
/// <typeparam name="TValue">属性值的类型</typeparam>
public sealed class SettingsPropertyValueChangedEventArgs<TValue>(TValue? oldValue, TValue? newValue) : EventArgs
{
    /// <summary>
    /// 属性的旧值
    /// </summary>
    public TValue? OldValue { get; } = oldValue;

    /// <summary>
    /// 属性的新值
    /// </summary>
    public TValue? NewValue { get; } = newValue;
}
namespace BD.Common8.Settings.Abstractions;

/// <summary>
/// 设置属性基类
/// </summary>
public abstract class SettingsPropertyBase
{
    /// <summary>
    /// 属性名
    /// </summary>
    public abstract string PropertyName { get; }

    /// <summary>
    /// 自动保存
    /// </summary>
    public bool AutoSave { get; set; }

    /// <summary>
    /// 引发值变化事件
    /// </summary>
    /// <param name="notSave">是否不保存</param>
    public abstract void RaiseValueChanged(bool notSave = false);

    /// <summary>
    /// 重置属性值
    /// </summary>
    public abstract void Reset();
}

/// <summary>
/// 设置属性泛型基类，继承自 <see cref="SettingsPropertyBase"/>
/// 并实现了 <see cref="INotifyPropertyChanged"/> 接口
/// </summary>
/// <typeparam name="TValue">属性值的类型</typeparam>
[DebuggerDisplay("Value={ActualValue}, PropertyName={PropertyName}, AutoSave={AutoSave}")]
public abstract class SettingsPropertyBase<TValue> : SettingsPropertyBase, INotifyPropertyChanged
{
    /// <summary>
    /// 实际值
    /// </summary>
    protected abstract TValue? ActualValue { get; set; }

    /// <summary>
    /// 默认值
    /// </summary>
    public abstract TValue? Default { get; }

    /// <summary>
    /// 判断两个值是否相等
    /// </summary>
    protected virtual bool Equals(TValue? left, TValue? right)
    {
        return EqualityComparer<TValue>.Default.Equals(left, right);
    }

    /// <summary>
    /// 订阅属性值变化事件
    /// </summary>
    /// <param name="listener">监听器</param>
    /// <param name="notifyOnInitialValue">是否在初始值上通知监听器</param>
    /// <returns>用于取消订阅的对象</returns>
    public IDisposable Subscribe(Action<TValue?> listener, bool notifyOnInitialValue = true)
    {
        if (notifyOnInitialValue)
            listener(ActualValue);
        return new ValueChangedEventListener(this, listener);
    }

    /// <summary>
    /// 属性值变化事件监听器
    /// </summary>
    protected sealed class ValueChangedEventListener : IDisposable
    {
        readonly Action<TValue?> _listener;
        readonly SettingsPropertyBase<TValue> _source;

        /// <summary>
        /// 实例化属性值变化事件监听器
        /// </summary>
        /// <param name="property">属性</param>
        /// <param name="listener">监听器</param>
        public ValueChangedEventListener(SettingsPropertyBase<TValue> property, Action<TValue?> listener)
        {
            _listener = listener;
            _source = property;
            _source.ValueChanged += HandleValueChanged;
        }

        /// <summary>
        /// 处理属性值变化事件
        /// </summary>
        /// <param name="sender">事件发送者</param>
        /// <param name="args">事件参数</param>
        void HandleValueChanged(object? sender, SettingsPropertyValueChangedEventArgs<TValue> args)
        {
            _listener(args.NewValue);
        }

        /// <inheritdoc cref="IDisposable.Dispose"/>
        public void Dispose()
        {
            _source.ValueChanged -= HandleValueChanged;
        }
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static implicit operator TValue?(SettingsPropertyBase<TValue> property)
        => property.ActualValue;

    #region events

    /// <summary>
    /// 属性值变化事件
    /// </summary>
    public event EventHandler<SettingsPropertyValueChangedEventArgs<TValue>>? ValueChanged;

    /// <summary>
    /// 引发属性值变化事件
    /// </summary>
    /// <param name="oldValue">旧的属性值</param>
    /// <param name="newValue">新的属性值</param>
    protected async void OnValueChanged(TValue? oldValue, TValue? newValue)
    {
        await MainThread2.InvokeOnMainThreadAsync(() =>
        {
            ValueChanged?.Invoke(this, new SettingsPropertyValueChangedEventArgs<TValue>(oldValue, newValue));
        });
    }

    readonly Dictionary<PropertyChangedEventHandler, EventHandler<SettingsPropertyValueChangedEventArgs<TValue>>>
        _handlers = [];

    /// <summary>
    /// 属性值变化事件，实现 <see cref="INotifyPropertyChanged"/> 接口
    /// </summary>
    event PropertyChangedEventHandler? INotifyPropertyChanged.PropertyChanged
    {
        add
        {
            if (value == null) return;
            ValueChanged += _handlers[value] = (sender, args) => value(sender, new PropertyChangedEventArgs("Value"));
        }

        remove
        {
            if (value == null) return;
            if (_handlers.TryGetValue(value, out var handler))
            {
                ValueChanged -= handler;
                _handlers.Remove(value);
            }
        }
    }

    #endregion
}
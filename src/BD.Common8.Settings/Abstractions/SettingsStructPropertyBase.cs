namespace BD.Common8.Settings.Abstractions;

/// <summary>
/// 结构类型的设置属性
/// </summary>
/// <typeparam name="TValue"></typeparam>
/// <typeparam name="TSettings"></typeparam>
public class SettingsStructPropertyBase<TValue, [DynamicallyAccessedMembers(DynamicallyAccessedMemberTypes.All)] TSettings> : SettingsPropertyBase<TValue?>, IDisposable, INotifyPropertyChanged
    where TValue : struct
{
    readonly Action<TSettings, TValue?> setter;
    readonly Func<TSettings, TValue?> getter;
    IDisposable? disposable;
    readonly IOptionsMonitor<TSettings> monitor;
    TValue? value;
    bool disposedValue;

    /// <summary>
    /// 初始化结构类型的设置属性新实例
    /// </summary>
    /// <param name="default"></param>
    /// <param name="autoSave"></param>
    /// <param name="propertyName"></param>
    public SettingsStructPropertyBase(TValue? @default = default, bool autoSave = true, [CallerMemberName] string? propertyName = null)
    {
        PropertyName = propertyName.ThrowIsNull();

        AutoSave = autoSave;
        Default = @default;
        var parameter = Expression.Parameter(typeof(TSettings), "obj");
        var property = Expression.Property(parameter, PropertyName);
        var value = Expression.Parameter(typeof(TValue?), "value");
        var assign = Expression.Assign(property, value);
        setter = Expression.Lambda<Action<TSettings, TValue?>>(assign, parameter, value).Compile();
        getter = Expression.Lambda<Func<TSettings, TValue?>>(property, parameter).Compile();
        monitor = Ioc.Get<IOptionsMonitor<TSettings>>();
        this.value = getter(monitor.CurrentValue);
        disposable = monitor.OnChange(OnChange);
    }

    /// <summary>
    /// 当设置发生变化时触发的事件
    /// </summary>
    /// <param name="settings">更改后的设置</param>
    void OnChange(TSettings settings)
    {
        if (ISettings.settingsTypeCaches[typeof(TSettings)].IsSaveing)
            return;

        SetValue(getter(settings), false);
    }

    /// <summary>
    /// 属性名称
    /// </summary>
    public override string PropertyName { get; }

    /// <summary>
    /// 实际值
    /// </summary>
    protected override TValue? ActualValue
    {
        get => value ?? Default;
        set
        {
            SetValue(value);
        }
    }

    /// <summary>
    /// 默认值
    /// </summary>
    public override TValue? Default { get; }

    /// <summary>
    /// 设置字段的值，并触发相应事件
    /// </summary>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    void SetValue(TValue? value, bool save = true)
    {
        if (Equals(value, this.value))
            return; // 值相同无变化

        var oldValue = this.value;
        this.value = value; // 赋值当前字段

        var setter_value = value;
        if (EqualityComparer<TValue?>.Default.Equals(value, Default))
            setter_value = default;
        setter(monitor.CurrentValue, setter_value); // 赋值模型类属性

        OnValueChanged(oldValue, value); // 调用变更事件

        if (save && AutoSave) // 自动保存
            Save();
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    void Save() => ISettings.TrySave(typeof(TSettings), monitor, true);

    /// <summary>
    /// 触发值变化事件
    /// </summary>
    /// <param name="notSave">是否不保存</param>
    public override void RaiseValueChanged(bool notSave = false)
    {
        setter(monitor.CurrentValue, value); // 赋值模型类属性
        OnValueChanged(value, value); // 调用变更事件
        if (!notSave && AutoSave) // 自动保存
            Save();
    }

    /// <summary>
    /// 重置设置为默认值，并触发相应事件
    /// </summary>
    public override void Reset()
    {
        var oldValue = value;
        value = Default; // 赋值当前字段
        setter(monitor.CurrentValue, default); // 赋值模型类属性

        OnValueChanged(oldValue, value); // 调用变更事件

        if (AutoSave) // 自动保存
            Save();
    }

    /// <summary>
    /// 获取表示该对象的字符串
    /// </summary>
    public override string ToString() => value?.ToString() ?? string.Empty;

    /// <summary>
    /// 订阅设置值变化事件
    /// </summary>
    /// <param name="listener">设置值变化事件的回调函数</param>
    /// <param name="notifyOnInitialValue">是否在初始化时触发回调函数</param>
    /// <returns>可用于取消订阅的对象</returns>
    public IDisposable Subscribe(Action<TValue> listener, bool notifyOnInitialValue = true)
    {
        void listener_(TValue? value)
        {
            TValue value_;
            if (value.HasValue)
                value_ = value.Value;
            else if (Default.HasValue)
                value_ = Default.Value;
            else
                value_ = default;
            listener?.Invoke(value_);
        }
        return Subscribe((Action<TValue?>)listener_, notifyOnInitialValue);
    }

    /// <summary>
    /// 释放资源
    /// </summary>
    /// <param name="disposing">是否正在主动释放资源</param>
    protected virtual void Dispose(bool disposing)
    {
        if (!disposedValue)
        {
            if (disposing)
                // 释放托管状态(托管对象)
                disposable?.Dispose();

            // 释放未托管的资源(未托管的对象)并重写终结器
            // 将大型字段设置为 null
            disposable = null;
            disposedValue = true;
        }
    }

    /// <inheritdoc cref="IDisposable.Dispose"/>
    public void Dispose()
    {
        // 不要更改此代码。请将清理代码放入“Dispose(bool disposing)”方法中
        Dispose(disposing: true);
        GC.SuppressFinalize(this);
    }
}
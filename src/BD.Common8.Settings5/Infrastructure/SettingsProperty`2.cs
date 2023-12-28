namespace BD.Common8.Settings5.Infrastructure;

#pragma warning disable SA1600 // Elements should be documented

/// <summary>
/// 设置属性
/// </summary>
/// <typeparam name="TValue"></typeparam>
/// <typeparam name="TSettingsModel"></typeparam>
[DebuggerDisplay("Value={value}, ModelValue={ModelValue}, ModelValueIsNull={ModelValueIsNull}, Default={Default}, PropertyName={PropertyName}, AutoSave={AutoSave}")]
public class SettingsProperty<TValue,
    [DynamicallyAccessedMembers(DynamicallyAccessedMemberTypes.All)] TSettingsModel>
    : SettingsProperty, INotifyPropertyChanged, IDisposable
    where TSettingsModel : class, new()
{
    protected readonly Action<TSettingsModel, TValue?> setter;
    protected readonly Func<TSettingsModel, TValue?> getter;
    protected IDisposable? disposable;
    protected readonly IOptionsMonitor<TSettingsModel> monitor;
    protected bool disposedValue;

    /// <summary>
    /// 当前设置的值
    /// </summary>
    protected TValue? value;

    /// <summary>
    /// 获取模型上的值，在 OnChange 时，模型上的值为新值，字段上的为旧值
    /// </summary>
    protected virtual TValue? ModelValue
    {
        get => getter(monitor.CurrentValue);
        set
        {
            if (typeof(TValue) == typeof(string))
            {
                var valueString = value?.ToString();
                if (string.IsNullOrWhiteSpace(valueString))
                    value = default;
            }
            setter(monitor.CurrentValue, value); // 赋值模型类属性
        }
    }

    /// <summary>
    /// 当前属性值
    /// </summary>
    public virtual TValue? Value
    {
        get
        {
            if (value is null)
            {
                return Default;
            }
            else if (typeof(TValue) == typeof(string))
            {
                if (string.IsNullOrWhiteSpace(value?.ToString()))
                    return Default;
                return value;
            }
            else if (default(TValue?) is null && Equals(value, default))
            {
                return Default;
            }
            return value;
        }
        set => SetValue(value);
    }

    /// <summary>
    /// 当前属性的默认值
    /// </summary>
    public TValue? Default { get; set; }

    /// <summary>
    /// Initializes a new instance of the <see cref="SettingsProperty{TValue, TSettingsModel}"/> class.
    /// </summary>
    /// <param name="default"></param>
    /// <param name="autoSave"></param>
    /// <param name="propertyName"></param>
    [RequiresUnreferencedCode("Creating Expressions requires unreferenced code because the members being referenced by the Expression may be trimmed.")]
    public SettingsProperty(TValue? @default = default,
         bool autoSave = DefaultAutoSave,
         [CallerMemberName] string? propertyName = null) : base(propertyName.ThrowIsNull(), autoSave)
    {
        var settingsType = typeof(TSettingsModel);
        Default = @default;
        ParameterExpression parameter = Expression.Parameter(settingsType, "obj");
        MemberExpression property = Expression.Property(parameter, PropertyName);
        ParameterExpression value = Expression.Parameter(typeof(TValue), "value");
        BinaryExpression assign = Expression.Assign(property, value);
        setter = Expression.Lambda<Action<TSettingsModel, TValue?>>(assign, parameter, value).Compile();
        getter = Expression.Lambda<Func<TSettingsModel, TValue?>>(property, parameter).Compile();
        monitor = ISettingsLoadService.Current.Get<IOptionsMonitor<TSettingsModel>>();
        this.value = ModelValue;
#if DEBUG && SETTINGS_TRACK
        Console.WriteLine($"已初始化设置项属性：{settingsType}.{propertyName}");
#endif
        disposable = monitor.OnChange(OnChange);
    }

    void OnChange(TSettingsModel settings)
    {
        SetValue(getter(settings), false);
    }

    protected virtual bool Equals(TValue? left, TValue? right)
        => EqualityComparer<TValue>.Default.Equals(left, right);

    public virtual IDisposable Subscribe(Action<TValue?> listener, bool notifyOnInitialValue = true)
    {
        if (notifyOnInitialValue)
            listener(Value);
        return new ValueChangedEventListener(this, listener);
    }

    protected sealed class ValueChangedEventListener : IDisposable
    {
        readonly Action<TValue?> _listener;
        readonly SettingsProperty<TValue?, TSettingsModel> _source;
        bool disposedValue;

        public ValueChangedEventListener(SettingsProperty<TValue?, TSettingsModel> property, Action<TValue?> listener)
        {
            _listener = listener;
            _source = property;
            _source.ValueChanged += HandleValueChanged;
        }

        void HandleValueChanged(object? sender, SettingsPropertyValueChangedEventArgs<TValue?> args)
        {
            _listener(args.NewValue);
        }

        void Dispose(bool disposing)
        {
            if (!disposedValue)
            {
                if (disposing)
                {
                    // 释放托管状态(托管对象)
                    _source.ValueChanged -= HandleValueChanged;
                }

                // 释放未托管的资源(未托管的对象)并重写终结器
                // 将大型字段设置为 null
                disposedValue = true;
            }
        }

        public void Dispose()
        {
            // 不要更改此代码。请将清理代码放入“Dispose(bool disposing)”方法中
            Dispose(disposing: true);
            GC.SuppressFinalize(this);
        }
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static implicit operator TValue?(SettingsProperty<TValue?, TSettingsModel> property)
        => property.Value;

    #region events

    public event EventHandler<SettingsPropertyValueChangedEventArgs<TValue>>? ValueChanged;

    protected async virtual void OnValueChanged(TValue? oldValue, TValue? newValue)
    {
        await MainThread2.InvokeOnMainThreadAsync(() =>
        {
            ValueChanged?.Invoke(this, new(oldValue, newValue));
        });
    }

    readonly Dictionary<PropertyChangedEventHandler, EventHandler<SettingsPropertyValueChangedEventArgs<TValue>>>
        _handlers = [];

    event PropertyChangedEventHandler? INotifyPropertyChanged.PropertyChanged
    {
        add
        {
            if (value == null) return;
            ValueChanged += _handlers[value] = (sender, args) => value(sender, new PropertyChangedEventArgs(nameof(Value)));
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

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    protected virtual void SetValue(TValue? value, bool save = true)
    {
        if (Equals(value, this.value))
            return; // 值相同无变化

        var oldValue = this.value;
        this.value = value; // 赋值当前字段

        var setter_value = value;
        ModelValue = setter_value; // 赋值模型类属性

        OnValueChanged(oldValue, value); // 调用变更事件

        if (save && AutoSave) // 自动保存
        {
            Save();
        }
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    void Save()
    {
        SettingsLoadServiceImpl.Current.ForceSave(monitor);
    }

    public override void RaiseValueChanged(bool notSave = false)
    {
        ModelValue = value;
        OnValueChanged(value, value); // 调用变更事件
        if (!notSave && AutoSave) // 自动保存
        {
            Save();
        }
    }

    public override void Reset()
    {
        var oldValue = value;
        value = Default; // 赋值当前字段
        ModelValue = default; // 赋值模型类属性

        OnValueChanged(oldValue, value); // 调用变更事件

        if (AutoSave) // 自动保存
        {
            Save();
        }
    }

    public override string ToString() => value?.ToString() ?? string.Empty;

    protected virtual void Dispose(bool disposing)
    {
        if (!disposedValue)
        {
            if (disposing)
            {
                // 释放托管状态(托管对象)
                disposable?.Dispose();
            }

            // 释放未托管的资源(未托管的对象)并重写终结器
            // 将大型字段设置为 null
            disposable = null;
            disposedValue = true;
        }
    }

    public void Dispose()
    {
        // 不要更改此代码。请将清理代码放入“Dispose(bool disposing)”方法中
        Dispose(disposing: true);
        GC.SuppressFinalize(this);
    }
}
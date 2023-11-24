namespace BD.Common8.Settings;

/// <summary>
/// 引用类型的设置项属性
/// </summary>
/// <typeparam name="TValue"></typeparam>
/// <typeparam name="TSettings"></typeparam>
public class SettingsProperty<TValue,
    [DynamicallyAccessedMembers(DynamicallyAccessedMemberTypes.All)] TSettings> :
    SettingsPropertyBase<TValue>, IDisposable, INotifyPropertyChanged
    where TValue : class
{
    /// <summary>
    /// 用于设置属性值的委托
    /// </summary>
    readonly Action<TSettings, TValue?> setter;

    /// <summary>
    /// 用于获取属性值的委托
    /// </summary>
    readonly Func<TSettings, TValue?> getter;

    /// <summary>
    /// 表示一个可释放的对象，用于管理资源的释放
    /// </summary>
    IDisposable? disposable;

    /// <summary>
    /// 用于监视设置项的变化
    /// </summary>
    readonly IOptionsMonitor<TSettings> monitor;

    /// <summary>
    /// 表示设置项的当前值
    /// </summary>
    protected TValue? value;

    /// <summary>
    /// 表示是否已经调用了 Dispose 方法的标识
    /// </summary>
    bool disposedValue;

    /// <summary>
    /// 初始化引用类型的设置项新实例
    /// </summary>
    /// <param name="default"></param>
    /// <param name="autoSave"></param>
    /// <param name="propertyName"></param>
    public SettingsProperty(TValue? @default = default, bool autoSave = true, [CallerMemberName] string? propertyName = null)
    {
        PropertyName = propertyName.ThrowIsNull();
        AutoSave = autoSave;
        Default = @default;
        var parameter = Expression.Parameter(typeof(TSettings), "obj");
        var property = Expression.Property(parameter, PropertyName);
        var value = Expression.Parameter(typeof(TValue), "value");
        var assign = Expression.Assign(property, value);
        setter = Expression.Lambda<Action<TSettings, TValue?>>(assign, parameter, value).Compile();
        getter = Expression.Lambda<Func<TSettings, TValue?>>(property, parameter).Compile();
        monitor = Ioc.Get<IOptionsMonitor<TSettings>>();
        this.value = getter(monitor.CurrentValue);
        disposable = monitor.OnChange(OnChange);
    }

    /// <summary>
    /// 当 <see langword="TSettings"/> 类型的设置发生变化时调用，根据新的设置值更新属性值
    /// </summary>
    /// <param name="settings"></param>
    void OnChange(TSettings settings)
    {
        if (ISettings.settingsTypeCaches[typeof(TSettings)].IsSaveing)
            return;

        SetValue(getter(settings), false);
    }

    /// <summary>
    /// 获取或设置属性的名称
    /// </summary>
    public override string PropertyName { get; }

    /// <summary>
    /// 获取或设置属性的实际值
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
    /// 获取或设置属性的值
    /// </summary>
    public TValue? Value
    {
        get => ActualValue;
        set => ActualValue = value;
    }

    /// <summary>
    /// 获取属性的默认值
    /// </summary>
    public override TValue? Default { get; }

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
    /// 触发属性值变更事件
    /// </summary>
    /// <param name="notSave">是否自动保存</param>
    public override void RaiseValueChanged(bool notSave = false)
    {
        var setter_value = value;
        if (EqualityComparer<TValue?>.Default.Equals(value, Default))
            setter_value = default;
        setter(monitor.CurrentValue, setter_value); // 赋值模型类属性
        OnValueChanged(value, value); // 调用变更事件
        if (!notSave && AutoSave) // 自动保存
            Save();
    }

    /// <summary>
    /// 重置属性的值为默认值
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
    /// 返回属性值的字符串表示形式
    /// </summary>
    /// <returns>属性值的字符串表示形式</returns>
    public override string ToString() => value?.ToString() ?? string.Empty;

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

/// <summary>
/// 数组或集合等可遍历集合的设置属性
/// </summary>
/// <typeparam name="TValue"></typeparam>
/// <typeparam name="TEnumerable"></typeparam>
/// <typeparam name="TSettings"></typeparam>
/// <param name="default"></param>
/// <param name="autoSave"></param>
/// <param name="propertyName"></param>
public class SettingsProperty<TValue,
    [DynamicallyAccessedMembers(DynamicallyAccessedMemberTypes.All)] TEnumerable,
    [DynamicallyAccessedMembers(DynamicallyAccessedMemberTypes.All)] TSettings>(
    TEnumerable? @default = default,
    bool autoSave = true,
    [CallerMemberName] string? propertyName = null) :
    SettingsProperty<TEnumerable, TSettings>(
        @default, autoSave, propertyName), ICollection<TValue>
    where TEnumerable : class, ICollection<TValue>, new()
{
    /// <summary>
    /// 获取集合中元素的数量抛出异常 <see cref="NotImplementedException"/>
    /// </summary>
    int ICollection<TValue>.Count => throw new NotImplementedException();

    /// <summary>
    /// 获取一个值该值指示集合是否为只读，抛出异常 <see cref="NotImplementedException"/>
    /// </summary>
    bool ICollection<TValue>.IsReadOnly => throw new NotImplementedException();

    /// <summary>
    /// 比较两个可遍历集合是否相等
    /// </summary>
    protected override bool Equals(TEnumerable? left, TEnumerable? right)
    {
        if (left == null)
            return right == null;
        else if (right == null)
            return left == null;

        if (EqualityComparer<TEnumerable>.Default.Equals(left, right))
            return true;

        return left.SequenceEqual(right);
    }

    /// <summary>
    /// 将指定的项添加到可遍历集合中
    /// </summary>
    static void AddRange(TEnumerable source, IEnumerable<TValue>? items)
    {
        if (items == null)
            return;

        if (source is List<TValue> list)
        {
            list.AddRange(items);
            return;
        }

        try
        {
            if (source is DynamicData.IExtendedList<TValue> extendedList)
            {
                extendedList.AddRange(items);
                return;
            }
        }
        catch
        {
            // 引入相关包使用 PrivateAssets="All" 避免过多依赖
            // 缺少依赖项时必定抛出类型找不到异常，忽略
        }

        items.ForEach(source.Add);
    }

    /// <summary>
    /// 向可遍历集合中添加指定的项
    /// </summary>
    /// <param name="item">要添加的项</param>
    /// <param name="raiseValueChanged">是否触发值改变事件，默认为 <see langword="true"/></param>
    /// <param name="notSave">是否不保存，默认为 <see langword="false"/></param>
    public virtual void Add(TValue item, bool raiseValueChanged = true, bool notSave = false)
    {
        if (value == null)
        {
            value = Activator.CreateInstance<TEnumerable>();
            AddRange(value, Default);
        }
        value.Add(item);
        if (raiseValueChanged)
            RaiseValueChanged(notSave);
    }

    /// <summary>
    /// 向可遍历集合中添加指定的项集合
    /// </summary>
    /// <param name="items">要添加的项的集合</param>
    /// <param name="raiseValueChanged">是否触发值改变事件，<see langword="true"/></param>
    /// <param name="notSave">是否不保存，默认为 <see langword="false"/></param>
    public virtual void AddRange(IEnumerable<TValue> items, bool raiseValueChanged = true, bool notSave = false)
    {
        if (value == null)
        {
            value = Activator.CreateInstance<TEnumerable>();
            AddRange(value, Default);
        }
        AddRange(value, items);
        if (raiseValueChanged)
            RaiseValueChanged(notSave);
    }

    /// <summary>
    /// 从可遍历集合中移除指定的项
    /// </summary>
    /// <param name="item">要移除的项</param>
    /// <param name="raiseValueChanged">是否触发值改变事件，默认为 <see langword="true"/></param>
    /// <param name="notSave">是否不保存，默认为 <see langword="false"/></param>
    /// <returns>如果成功移除了项，则为 <see langword="true"/>；否则为 <see langword="false"/></returns>
    public virtual bool Remove(TValue item, bool raiseValueChanged = true, bool notSave = false)
    {
        bool result;
        if (value == null)
        {
            if (Default.Any_Nullable())
            {
                value = Activator.CreateInstance<TEnumerable>();
                AddRange(value, Default);

                result = value.Remove(item);
                if (raiseValueChanged)
                    RaiseValueChanged(notSave);

                return result;
            }
            return false;
        }

        result = value.Remove(item);
        if (raiseValueChanged)
            RaiseValueChanged(notSave);

        return result;
    }

    /// <summary>
    /// 判断可遍历集合是否包含指定的项
    /// </summary>
    /// <param name="item">要判断的项</param>
    /// <returns>如果可遍历集合包含指定的项，则为 <see langword="true"/>；否则为 <see langword="false"/></returns>
    public virtual bool Contains(TValue item)
    {
        if (value == null)
        {
            if (Default.Any_Nullable())
                return Default.Contains(item);
            return false;
        }

        return value.Contains(item);
    }

    /// <summary>
    /// 清空可遍历集合
    /// </summary>
    /// <param name="raiseValueChanged">是否触发值改变事件，默认为 <see langword="true"/></param>
    /// <param name="notSave">是否不保存，默认为 <see langword="false"/></param>
    public virtual void Clear(bool raiseValueChanged = true, bool notSave = false)
    {
        if (value == null)
        {
            value = Activator.CreateInstance<TEnumerable>();
            if (raiseValueChanged)
                RaiseValueChanged(notSave);
            return;
        }

        value = null;
        if (raiseValueChanged)
            RaiseValueChanged(notSave);
    }

    /// <inheritdoc cref="Add"/>
    void ICollection<TValue>.Add(TValue item) => Add(item);

    /// <inheritdoc cref="Clear"/>
    void ICollection<TValue>.Clear() => Clear();

    /// <summary>
    /// 将集合中的项复制到指定数组的指定索引处
    /// </summary>
    /// <param name="array">目标数组</param>
    /// <param name="arrayIndex">目标数组的起始索引</param>
    void ICollection<TValue>.CopyTo(TValue[] array, int arrayIndex)
    {
        if (value == null) return;

        value.CopyTo(array, arrayIndex);
    }

    /// <inheritdoc cref="Remove"/>
    bool ICollection<TValue>.Remove(TValue item) => Remove(item);

    /// <inheritdoc/>
    IEnumerator<TValue> IEnumerable<TValue>.GetEnumerator()
    {
        if (value == null)
            return (IEnumerator<TValue>)Array.Empty<TValue>().GetEnumerator();
        return value.GetEnumerator();
    }

    /// <inheritdoc/>
    IEnumerator IEnumerable.GetEnumerator()
    {
        if (value == null)
            return Array.Empty<TValue>().GetEnumerator();
        return value.GetEnumerator();
    }
}

/// <summary>
/// 字典的设置属性
/// </summary>
/// <typeparam name="TKey"></typeparam>
/// <typeparam name="TValue"></typeparam>
/// <typeparam name="TDictionary"></typeparam>
/// <typeparam name="TSettings"></typeparam>
/// <param name="default"></param>
/// <param name="autoSave"></param>
/// <param name="propertyName"></param>
public class SettingsProperty<TKey, TValue,
    [DynamicallyAccessedMembers(DynamicallyAccessedMemberTypes.All)] TDictionary,
    [DynamicallyAccessedMembers(DynamicallyAccessedMemberTypes.All)] TSettings>(
    TDictionary? @default = default,
    bool autoSave = true,
    [CallerMemberName] string? propertyName = null) :
    SettingsProperty<KeyValuePair<TKey, TValue>, TDictionary, TSettings>(
        @default, autoSave, propertyName), IDictionary<TKey, TValue>
     where TDictionary : class, IDictionary<TKey, TValue>, new()
{
    /// <summary>
    /// 索引器，用于获取或设置指定键所对应的值
    /// </summary>
    /// <param name="key">键</param>
    /// <returns>与指定键关联的值</returns>
    TValue IDictionary<TKey, TValue>.this[TKey key]
    {
        get
        {
            if (value == null)
            {
                if (Default != null)
                    if (Default.TryGetValue(key, out var value))
                        return value;
                return default!;
            }
            return value[key];
        }

        set
        {
            if (this.value == null)
            {
                this.value = Activator.CreateInstance<TDictionary>();
                if (Default != null)
                    foreach (var item in Default)
                        this.value.Add(item.Key, item.Value);
            }
            this.value.Add(key, value);
        }
    }

    /// <summary>
    /// 获取包含字典中的键的集合
    /// </summary>
    ICollection<TKey> IDictionary<TKey, TValue>.Keys
    {
        get
        {
            if (value == null)
                if (Default != null)
                    return Default.Keys;
                else
                    return Array.Empty<TKey>();
            return value.Keys;
        }
    }

    /// <summary>
    /// 获取包含字典中的值的集合
    /// </summary>
    ICollection<TValue> IDictionary<TKey, TValue>.Values
    {
        get
        {
            if (value == null)
                if (Default != null)
                    return Default.Values;
                else
                    return Array.Empty<TValue>();
            return value.Values;
        }
    }

    /// <summary>
    /// 向字典中添加具有指定键和值的元素
    /// </summary>
    /// <param name="key">要添加的元素的键</param>
    /// <param name="value">要添加的元素的值</param>
    /// <param name="raiseValueChanged">是否触发值更改事件，默认为 <see langword="true"/></param>
    /// <param name="notSave">是否不保存，默认为 <see langword="false"/></param>
    public virtual void Add(TKey key, TValue value, bool raiseValueChanged = true, bool notSave = false)
    {
        if (this.value == null)
        {
            this.value = Activator.CreateInstance<TDictionary>();
            if (Default != null)
                foreach (var item in Default)
                    this.value.Add(item.Key, item.Value);
        }
        if (!this.value.TryAdd(key, value))
            this.value[key] = value;

        if (raiseValueChanged)
            RaiseValueChanged(notSave);
    }

    /// <summary>
    /// 向字典中添加指定键和值的键值对
    /// </summary>
    /// <param name="pair">要添加的键值对</param>
    /// <param name="raiseValueChanged">是否触发值更改事件，默认为 <see langword="true"/></param>
    /// <param name="notSave">是否不保存，默认为 <see langword="false"/></param>
    public override void Add(KeyValuePair<TKey, TValue> pair, bool raiseValueChanged = true, bool notSave = false)
    {
        Add(pair.Key, pair.Value, raiseValueChanged, notSave);
    }

    /// <summary>
    /// 向字典中添加指定键值对集合的元素
    /// </summary>
    /// <param name="items">要添加的键值对集合</param>
    /// <param name="raiseValueChanged">是否触发值更改事件，默认为 <see langword="true"/></param>
    /// <param name="notSave">是否不保存，默认为 <see langword="false"/></param>
    public override void AddRange(IEnumerable<KeyValuePair<TKey, TValue>> items, bool raiseValueChanged = true, bool notSave = false)
    {
        if (value == null)
        {
            value = Activator.CreateInstance<TDictionary>();
            if (Default != null)
                foreach (var item in Default)
                    value.Add(item.Key, item.Value);
        }

        foreach (var item in items)
            if (!value.TryAdd(item.Key, item.Value))
                value[item.Key] = item.Value;

        if (raiseValueChanged)
            RaiseValueChanged(notSave);
    }

    /// <summary>
    /// 确定字典是否包含指定的键
    /// </summary>
    /// <param name="key">要检查的键</param>
    /// <returns>如果字典包含具有指定键的元素，则为 <see langword="true"/>；否则为 <see langword="false"/></returns>
    public virtual bool ContainsKey(TKey key)
    {
        if (value == null)
        {
            if (Default != null)
                return Default.ContainsKey(key);
            return false;
        }
        return value.ContainsKey(key);
    }

    /// <summary>
    /// 从字典中移除具有指定键的元素
    /// </summary>
    /// <param name="key">要移除的键</param>
    /// <param name="raiseValueChanged">是否引发值更改事件，默认为 <see langword="true"/></param>
    /// <param name="notSave">是否不保存更改，默认为 <see langword="false"/></param>
    /// <returns>如果成功移除了元素，则为 <see langword="true"/>；否则为 <see langword="false"/></returns>
    public virtual bool Remove(TKey key, bool raiseValueChanged = true, bool notSave = false)
    {
        bool result;
        if (value == null)
        {
            if (Default.Any_Nullable())
            {
                value = Activator.CreateInstance<TDictionary>();
                foreach (var item in Default)
                    value.Add(item.Key, item.Value);

                result = value.Remove(key);
                if (raiseValueChanged)
                    RaiseValueChanged(notSave);

                return result;
            }
            return false;
        }

        result = value.Remove(key);
        if (raiseValueChanged)
            RaiseValueChanged(notSave);

        return result;
    }

    /// <summary>
    /// 获取具有指定键的值
    /// </summary>
    /// <param name="key">要获取值的键</param>
    /// <param name="value">当此方法返回时，如果找到具有指定键的元素，则包含该元素的值；否则为该类型的默认值</param>
    public virtual bool TryGetValue(TKey key, out TValue value)
    {
        if (this.value == null)
        {
            if (Default != null)
                return Default.TryGetValue(key, out value!);
            value = default!;
            return false;
        }
        return this.value.TryGetValue(key, out value!);
    }

    /// <summary>
    /// 向字典中添加指定的键值对
    /// </summary>
    void IDictionary<TKey, TValue>.Add(TKey key, TValue value) => Add(key, value);

    /// <summary>
    /// 从字典中移除指定键的键值对
    /// </summary>
    bool IDictionary<TKey, TValue>.Remove(TKey key) => Remove(key);
}

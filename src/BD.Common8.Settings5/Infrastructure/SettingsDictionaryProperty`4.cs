namespace BD.Common8.Settings5.Infrastructure;

/// <summary>
/// 设置属性，<see cref="IDictionary{TKey, TValue}"/>
/// </summary>
/// <typeparam name="TKey"></typeparam>
/// <typeparam name="TValue"></typeparam>
/// <typeparam name="TDictionary"></typeparam>
/// <typeparam name="TSettingsModel"></typeparam>
/// <remarks>
/// Initializes a new instance of the <see cref="SettingsDictionaryProperty{TKey, TValue, TDictionary, TSettingsModel}"/> class.
/// </remarks>
/// <param name="default"></param>
/// <param name="autoSave"></param>
/// <param name="propertyName"></param>
[method: RequiresUnreferencedCode("Creating Expressions requires unreferenced code because the members being referenced by the Expression may be trimmed.")]
public class SettingsDictionaryProperty<TKey,
    TValue,
    [DynamicallyAccessedMembers(DynamicallyAccessedMemberTypes.PublicConstructors)] TDictionary,
    [DynamicallyAccessedMembers(DynamicallyAccessedMemberTypes.All)] TSettingsModel>(TDictionary? @default = default, bool autoSave = true, [CallerMemberName] string? propertyName = null)
    : SettingsCollectionProperty<KeyValuePair<TKey, TValue>, TDictionary, TSettingsModel>(@default, autoSave, propertyName),
    IDictionary<TKey, TValue>
    where TDictionary : class, IDictionary<TKey, TValue>, new()
    where TSettingsModel : class, new()
{
    /// <inheritdoc/>
    TValue IDictionary<TKey, TValue>.this[TKey key]
    {
        get
        {
            if (value == null)
            {
                var @default = Default;
                if (@default != null)
                {
                    if (@default.TryGetValue(key, out var value))
                    {
                        return value;
                    }
                }
                return default!;
            }
            try
            {
                return value[key];
            }
            catch
            {
                return default!;
            }
        }

        set
        {
            if (this.value == null)
            {
                this.value = Activator.CreateInstance<TDictionary>();
                var @default = Default;
                if (@default != null)
                {
                    foreach (var item in @default)
                    {
                        this.value.Add(item.Key, item.Value);
                    }
                }
            }
            this.value.Add(key, value);
        }
    }

    /// <inheritdoc/>
    ICollection<TKey> IDictionary<TKey, TValue>.Keys
    {
        get
        {
            if (value == null)
            {
                var @default = Default;
                if (@default != null)
                {
                    return @default.Keys;
                }
                else
                {
                    return Array.Empty<TKey>();
                }
            }
            return value.Keys;
        }
    }

    /// <inheritdoc/>
    ICollection<TValue> IDictionary<TKey, TValue>.Values
    {
        get
        {
            if (value == null)
            {
                var @default = Default;
                if (@default != null)
                {
                    return @default.Values;
                }
                else
                {
                    return Array.Empty<TValue>();
                }
            }
            return value.Values;
        }
    }

    /// <inheritdoc cref="IDictionary{TKey, TValue}.Add(TKey, TValue)"/>
    public virtual void Add(TKey key, TValue value, bool raiseValueChanged = true, bool notSave = false)
    {
        if (this.value == null)
        {
            this.value = new();
            var @default = Default;
            if (@default != null)
            {
                foreach (var item in @default)
                {
                    this.value.Add(item.Key, item.Value);
                }
            }
        }
        if (!this.value.TryAdd(key, value))
        {
            this.value[key] = value;
        }

        if (raiseValueChanged)
            RaiseValueChanged(notSave);
    }

    /// <inheritdoc/>
    public override void Add(KeyValuePair<TKey, TValue> pair, bool raiseValueChanged = true, bool notSave = false)
    {
        Add(pair.Key, pair.Value, raiseValueChanged, notSave);
    }

    static bool TryAdd(IDictionary<TKey, TValue> dictionary, TKey key, TValue value)
    {
        if (dictionary is ConcurrentDictionary<TKey, TValue> cdict)
        {
            // https://github.com/dotnet/runtime/issues/30451
            return cdict.TryAdd(key, value);
        }
        return dictionary.TryAdd(key, value);
    }

    /// <inheritdoc/>
    public override void AddRange(IEnumerable<KeyValuePair<TKey, TValue>> items, bool raiseValueChanged = true, bool notSave = false)
    {
        if (value == null)
        {
            value = new();
            var @default = Default;
            if (@default != null)
            {
                foreach (var item in @default)
                {
                    value.Add(item.Key, item.Value);
                }
            }
        }

        foreach (var item in items)
        {
            if (!TryAdd(value, item.Key, item.Value))
            {
                value[item.Key] = item.Value;
            }
        }

        if (raiseValueChanged)
            RaiseValueChanged(notSave);
    }

    /// <inheritdoc cref="IDictionary{TKey, TValue}.ContainsKey(TKey)"/>
    public virtual bool ContainsKey(TKey key)
    {
        if (value == null)
        {
            var @default = Default;
            if (@default != null)
            {
                return @default.ContainsKey(key);
            }
            return false;
        }
        return value.ContainsKey(key);
    }

    /// <inheritdoc cref="IDictionary{TKey, TValue}.Remove(TKey)"/>
    public virtual bool Remove(TKey key, bool raiseValueChanged = true, bool notSave = false)
    {
        bool result;
        if (value == null)
        {
            var @default = Default;
            if (@default != null && @default.Count != 0)
            {
                value = new();
                foreach (var item in @default)
                {
                    value.Add(item.Key, item.Value);
                }

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

    /// <inheritdoc cref="IDictionary{TKey, TValue}.TryGetValue(TKey, out TValue)"/>
    public virtual bool TryGetValue(TKey key, out TValue value)
    {
        if (this.value == null)
        {
            var @default = Default;
            if (@default != null)
            {
                return @default.TryGetValue(key, out value!);
            }
            value = default!;
            return false;
        }
        return this.value.TryGetValue(key, out value!);
    }

    /// <inheritdoc/>
    void IDictionary<TKey, TValue>.Add(TKey key, TValue value) => Add(key, value);

    /// <inheritdoc/>
    bool IDictionary<TKey, TValue>.Remove(TKey key) => Remove(key);

    /// <summary>
    /// 添加或替换值并返回值
    /// </summary>
    /// <param name="key"></param>
    /// <param name="value"></param>
    /// <returns></returns>
    public virtual TValue GetOrAdd(TKey key, TValue value)
    {
        Add(key, value);
        return value;
    }
}

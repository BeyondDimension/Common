namespace BD.Common8.Settings5.Infrastructure;

/// <summary>
/// 设置属性，<see cref="ICollection{T}"/>
/// </summary>
/// <typeparam name="TValue"></typeparam>
/// <typeparam name="TEnumerable"></typeparam>
/// <typeparam name="TSettingsModel"></typeparam>
/// <remarks>
/// Initializes a new instance of the <see cref="SettingsCollectionProperty{TValue, TEnumerable, TSettingsModel}"/> class.
/// </remarks>
/// <param name="default"></param>
/// <param name="autoSave"></param>
/// <param name="propertyName"></param>
[method: RequiresUnreferencedCode("Creating Expressions requires unreferenced code because the members being referenced by the Expression may be trimmed.")]
public class SettingsCollectionProperty<TValue,
    [DynamicallyAccessedMembers(DynamicallyAccessedMemberTypes.PublicConstructors)] TEnumerable,
    [DynamicallyAccessedMembers(DynamicallyAccessedMemberTypes.All)] TSettingsModel>(TEnumerable? @default = default,
     bool autoSave = SettingsProperty.DefaultAutoSave,
     [CallerMemberName] string? propertyName = null)
    : SettingsProperty<TEnumerable, TSettingsModel>(@default, autoSave, propertyName),
    ICollection<TValue>
    where TEnumerable : class, ICollection<TValue>, new()
    where TSettingsModel : class, new()
{
    /// <inheritdoc/>
    int ICollection<TValue>.Count => value?.Count ?? 0;

    /// <inheritdoc/>
    bool ICollection<TValue>.IsReadOnly => value?.IsReadOnly ?? false;

    /// <inheritdoc/>
    protected override bool Equals(TEnumerable? left, TEnumerable? right)
    {
        if (left == null)
        {
            return right == null;
        }
        else if (right == null)
        {
            return left == null;
        }

        if (EqualityComparer<TEnumerable>.Default.Equals(left, right))
        {
            return true;
        }

        return left.SequenceEqual(right);
    }

    /// <inheritdoc cref="List{T}.AddRange(IEnumerable{T})"/>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    static void AddRange(TEnumerable source, IEnumerable<TValue>? items)
    {
        if (items == null) return;

        switch (source)
        {
            case List<TValue> list:
                list.AddRange(items);
                break;
            case IExtendedList<TValue> extendedList:
                extendedList.AddRange(items);
                break;
            default:
                items.ForEach(source.Add);
                break;
        }
    }

    /// <inheritdoc cref="ICollection{T}.Add(T)"/>
    public virtual void Add(TValue item, bool raiseValueChanged = true, bool notSave = false)
    {
        if (value == null)
        {
            value = new();
            AddRange(value, Default);
        }
        value.Add(item);
        if (raiseValueChanged)
            RaiseValueChanged(notSave);
    }

    /// <inheritdoc cref="List{T}.AddRange(IEnumerable{T})"/>
    public virtual void AddRange(IEnumerable<TValue> items, bool raiseValueChanged = true, bool notSave = false)
    {
        if (value == null)
        {
            value = new();
            AddRange(value, Default);
        }
        AddRange(value, items);
        if (raiseValueChanged)
            RaiseValueChanged(notSave);
    }

    /// <inheritdoc cref="ICollection{T}.Remove(T)"/>
    public virtual bool Remove(TValue item, bool raiseValueChanged = true, bool notSave = false)
    {
        bool result;
        if (value == null)
        {
            var @default = Default;
            if (@default != null && @default.Count != 0)
            {
                value = new();
                AddRange(value, @default);

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

    /// <inheritdoc cref="ICollection{T}.Contains(T)"/>
    public virtual bool Contains(TValue item)
    {
        if (value == null)
        {
            var @default = Default;
            if (@default != null && @default.Count != 0)
            {
                return @default.Contains(item);
            }
            return false;
        }

        return value.Contains(item);
    }

    /// <inheritdoc cref="ICollection{T}.Clear"/>
    public virtual void Clear(bool raiseValueChanged = true, bool notSave = false)
    {
        if (value == null)
        {
            value = new();
            if (raiseValueChanged)
                RaiseValueChanged(notSave);
            return;
        }

        value = null;
        if (raiseValueChanged)
            RaiseValueChanged(notSave);
    }

    /// <inheritdoc/>
    void ICollection<TValue>.Add(TValue item) => Add(item);

    /// <inheritdoc/>
    void ICollection<TValue>.Clear() => Clear();

    /// <inheritdoc/>
    void ICollection<TValue>.CopyTo(TValue[] array, int arrayIndex)
    {
        if (value == null)
            return;
        value.CopyTo(array, arrayIndex);
    }

    /// <inheritdoc/>
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

    /// <inheritdoc/>
    public override string ToString() => value == null ? "[]" : $"[{string.Join(", ", value)}]";
}

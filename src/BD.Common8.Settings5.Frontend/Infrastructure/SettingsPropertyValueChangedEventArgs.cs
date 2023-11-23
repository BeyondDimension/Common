namespace BD.Common8.Settings5.Infrastructure;

#pragma warning disable SA1600 // Elements should be documented

public sealed class SettingsPropertyValueChangedEventArgs<TValue>(TValue? oldValue, TValue? newValue) : EventArgs
{
    public TValue? OldValue { get; } = oldValue;

    public TValue? NewValue { get; } = newValue;
}
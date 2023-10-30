namespace BD.Common8.Settings;

#pragma warning disable SA1600 // Elements should be documented

/// <summary>
/// 值类型的设置项属性
/// </summary>
/// <typeparam name="TValue"></typeparam>
/// <typeparam name="TSettings"></typeparam>
/// <param name="default"></param>
/// <param name="autoSave"></param>
/// <param name="propertyName"></param>
public class SettingsStructProperty<TValue, [DynamicallyAccessedMembers(DynamicallyAccessedMemberTypes.All)] TSettings>(
    TValue? @default = default, bool autoSave = true, [CallerMemberName] string? propertyName = null) :
    SettingsStructPropertyBase<TValue, TSettings>(@default, autoSave, propertyName)
    where TValue : struct
{
    public TValue Value
    {
        get
        {
            var value = ActualValue;
            if (value.HasValue)
                return value.Value;
            return default;
        }

        set => ActualValue = value;
    }
}